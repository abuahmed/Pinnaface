using System;
using System.Collections.Generic;
//using System.Globalization;
using System.IO;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Reports.BioData;
using PinnaFace.Reports.Complain;
using PinnaFace.Reports.CV;
using PinnaFace.Reports.Labour;
using PinnaFace.Reports.Summary;
using PinnaFace.Service;
using PinnaFace.Reports;
using PinnaFace.Reports.Embassy;
using PinnaFace.WPF.Views;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using MessageBox = System.Windows.MessageBox;
using ReportUtility = PinnaFace.WPF.Reports.ReportUtility;

namespace PinnaFace.WPF
{
    public static class GenerateReports
    {
        #region BioDataAndCV

        public static void LoadBioData(EmployeeDTO selectedEmployee)
        {
            try
            {
                var myDataSet = new ReportsDataSet2();

                var localAgency = new LocalAgencyService(true).GetLocalAgency();


                var employee = selectedEmployee;

                myDataSet.LetterHeads.Rows.Add("1", localAgency.Header.AttachedFile,
                    localAgency.Footer.AttachedFile, null, "", "");

                if (employee != null)
                {
                    var edu = employee.Education;
                    var expr = employee.Experience;
                    var cont = employee.ContactPerson;

                    double age = DateTime.Now.Subtract(employee.DateOfBirth).Days;
                    age = age/365.25;
                    string agee = "";
                    try
                    {
                        agee = age.ToString().Substring(0, 4);
                    }
                    catch
                    {
                        agee = age.ToString();
                    }

                    #region Language Conditions

                    string poor1 = "NO", poor2 = "NO", fair1 = "NO", fair2 = "NO", fluent1 = "NO", fluent2 = "NO";
                    if (edu.EnglishLanguage == LanguageExperience.Poor)
                    {
                        poor1 = "YES";
                    }
                    else if (edu.EnglishLanguage == LanguageExperience.Fair)
                    {
                        fair1 = "YES";
                    }
                    else if (edu.EnglishLanguage == LanguageExperience.Fluent)
                    {
                        fluent1 = "YES";
                    }

                    if (edu.ArabicLanguage == LanguageExperience.Poor)
                    {
                        poor2 = "YES";
                    }
                    else if (edu.ArabicLanguage == LanguageExperience.Fair)
                    {
                        fair2 = "YES";
                    }
                    else if (edu.ArabicLanguage == LanguageExperience.Fluent)
                    {
                        fluent2 = "YES";
                    }

                    #endregion

                    #region Skill Conditions

                    string baby = "NO",
                        clean = "NO",
                        cook = "NO",
                        wash = "NO",
                        decor = "NO",
                        drive = "NO",
                        sew = "NO";
                    if (expr.BabySitting)
                        baby = "YES";
                    if (expr.Cleaning)
                        clean = "YES";
                    if (expr.Cooking)
                        cook = "YES";
                    if (expr.Washing)
                        wash = "YES";
                    if (expr.Decorating)
                        decor = "YES";
                    if (expr.Driving)
                        drive = "YES";
                    if (expr.Sewing)
                        sew = "YES";

                    #endregion

                    myDataSet.KuwaitApp.Rows.Add("1", employee.CodeNumber,
                        employee.Address.HouseNumber,
                        employee.Address.Mobile,
                        employee.Salary + " " + EnumUtil.GetEnumDesc(employee.CurrencyType),
                        EnumUtil.GetEnumDesc(employee.ContratPeriod),
                        employee.FullName, employee.PassportNumber,
                        employee.PassportIssueDateString, "ADDIS ABABA",
                        employee.PassportExpiryDateString, cont.FullName,
                        "Subcity/Zone:- " + cont.Address.SubCity, "Kebele:- " + cont.Address.Kebele,
                        "House No:- " + cont.Address.HouseNumber, "Tele:- " + cont.Address.Mobile,
                        employee.Address.City, employee.Address.SubCity, employee.Address.Kebele);

                    myDataSet.KuwaitAppDetail.Rows.Add("1",
                        employee.Sex, employee.Religion,
                        employee.DateOfBirthString, agee + " YEARS",
                        employee.PlaceOfBirth,
                        employee.Address.City, employee.MaritalStatus,
                        EnumUtil.GetEnumDesc(employee.NumberOfChildren), employee.Weight, employee.Height,
                        employee.Complexion, edu.EducateQG,
                        poor1, fair1, fluent1, poor2, fair2, fluent2,
                        EnumUtil.GetEnumDesc(expr.ExperienceCountry), EnumUtil.GetEnumDesc(expr.ExperiencePeriod),
                        decor, drive, sew, baby, clean, cook, wash, "", "", "");

                    var myReport = new BioData();
                    myReport.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch
            {
            }
        }

        public static void LoadCv(EmployeeDTO selectedEmployee)
        {
            try
            {
                var employee = selectedEmployee;
                var myDataSet = new ReportsDataSet2();
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();

                if (setting.CvHeaderFormat == CvHeaderFormats.Agent)
                {
                    var agents = new ForeignAgentService(true, true).GetAll().ToList();
                    var agent = agents.FirstOrDefault();
                    if (agent != null && (selectedEmployee.AgentId != null && selectedEmployee.AgentId != agent.Id))
                        agent = agents.FirstOrDefault(a => a.Id == selectedEmployee.AgentId);
                    if (agent == null)
                    {
                        MessageBox.Show("Can't get Agent Data");
                        return;
                    }

                    myDataSet.LetterHeads.Rows.Add("1", agent.Header.AttachedFile, employee.Photo.AttachedFile,
                        employee.StandPhoto.AttachedFile, "", "");
                }
                else
                {
                    myDataSet.LetterHeads.Rows.Add("1", localAgency.Header.AttachedFile, employee.Photo.AttachedFile,
                        employee.StandPhoto.AttachedFile, "", "");
                }

                if (employee != null)
                {
                    var edu = employee.Education;
                    var expr = employee.Experience;
                    var cont = employee.ContactPerson;

                    if (string.IsNullOrEmpty(employee.PlaceOfIssue))
                        employee.PlaceOfIssue = "Addis Ababa";
                    var expCountry = "First Timer";
                    var expPeriod = "";
                    if (expr.HaveWorkExperience)
                    {
                        expCountry = EnumUtil.GetEnumDesc(expr.ExperienceCountry);
                        expPeriod = EnumUtil.GetEnumDesc(expr.ExperiencePeriod);
                    }

                    #region Age Calculation

                    double age = DateTime.Now.Subtract(employee.DateOfBirth).Days;
                    age = age/365.25;
                    string agee = "";
                    try
                    {
                        agee = age.ToString().Substring(0, 4);
                    }
                    catch
                    {
                        agee = age.ToString();
                    }

                    #endregion

                    #region Language Conditions

                    string poor1 = "NO", poor2 = "NO", fair1 = "NO", fair2 = "NO", fluent1 = "NO", fluent2 = "NO";
                    if (edu.EnglishLanguage == LanguageExperience.Poor)
                    {
                        poor1 = "YES";
                    }
                    else if (edu.EnglishLanguage == LanguageExperience.Fair)
                    {
                        fair1 = "YES";
                    }
                    else if (edu.EnglishLanguage == LanguageExperience.Fluent)
                    {
                        fluent1 = "YES";
                    }

                    if (edu.ArabicLanguage == LanguageExperience.Poor)
                    {
                        poor2 = "YES";
                    }
                    else if (edu.ArabicLanguage == LanguageExperience.Fair)
                    {
                        fair2 = "YES";
                    }
                    else if (edu.ArabicLanguage == LanguageExperience.Fluent)
                    {
                        fluent2 = "YES";
                    }

                    #endregion

                    #region Skill Conditions

                    string baby = "NO",
                        clean = "NO",
                        cook = "NO",
                        wash = "NO",
                        decor = "NO",
                        drive = "NO",
                        sew = "NO";
                    if (expr.BabySitting)
                        baby = "YES";
                    if (expr.Cleaning)
                        clean = "YES";
                    if (expr.Cooking)
                        cook = "YES";
                    if (expr.Washing)
                        wash = "YES";
                    if (expr.Decorating)
                        decor = "YES";
                    if (expr.Driving)
                        drive = "YES";
                    if (expr.Sewing)
                        sew = "YES";

                    #endregion

                    myDataSet.KuwaitApp.Rows.Add("1",
                        employee.CodeNumber,
                        DateTime.Now.ToShortDateString(),
                        EnumUtil.GetEnumDesc(employee.AppliedProfession),
                        employee.Salary + " " + EnumUtil.GetEnumDesc(employee.CurrencyType),
                        EnumUtil.GetEnumDesc(employee.ContratPeriod),
                        employee.FullName, employee.PassportNumber,
                        employee.PassportIssueDateString, employee.PlaceOfIssue,
                        employee.PassportExpiryDateString, cont.FullName,
                        "" + cont.Address.SubCity, "" + cont.Address.Kebele,
                        "" + cont.Address.HouseNumber, "" + cont.Address.Mobile,
                        employee.Address.City, employee.Address.SubCity, employee.Address.Kebele);

                    myDataSet.KuwaitAppDetail.Rows.Add("1",
                        "ETHIOPIAN", employee.Religion,
                        employee.DateOfBirthString, agee + " YEARS",
                        employee.PlaceOfBirth,
                        employee.Address.City, employee.MaritalStatus,
                        EnumUtil.GetEnumDesc(employee.NumberOfChildren), employee.Weight, employee.Height,
                        employee.Complexion, edu.EducateQG,
                        poor1, fair1, fluent1, poor2, fair2, fluent2,
                        expCountry, expPeriod,
                        decor, drive, sew, baby, clean, cook, wash, "", "", "");

                    var myReport = new CV();
                    myReport.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Can't get CV Document");
            }
        }

        public static void LoadSummary(IEnumerable<EmployeeDTO> employeeList,
            ProcessStatusTypesForDisplay selectedStatus,
            AgentDTO selectedAgent)
        {
            try
            {
                var myDataSet = new ReportsDataSet();

                var myReport = new SummaryList();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();

                var serNo = 1;
                //bool found = false;

                foreach (var employ in employeeList)
                {
                    //found = true;
                    if (employ == null)
                        continue;
                    if (employ.Visa == null)
                        continue;
                    if (employ.Visa.Sponsor == null)
                        continue;
                    if (employ.Visa.Sponsor.Address == null)
                        continue;

                    var title = "List Of " + EnumUtil.GetEnumDesc(selectedStatus) + " Employees ";
                    var agents = selectedAgent != null ? selectedAgent.AgentName : " For All Agents";
                    title = title + agents;
                    var city = employ.Visa.Sponsor.Address.City;

                    myDataSet.FlightTicket.Rows.Add(title,
                        serNo.ToString(),
                        employ.FullName,
                        employ.PassportNumber,
                        city, 0.0,
                        employ.Visa.Sponsor.Address.City,
                        employ.Visa.VisaNumber,
                        employ.Visa.SponsorId,
                        employ.Visa.Sponsor.FullName, 0.0);

                    serNo++;
                }
                serNo--;
                myDataSet.LetterHeads.Rows.Add(serNo.ToString(), localAgency.Header.AttachedFile,
                    localAgency.Footer.AttachedFile, null, "", "");

                myReport.SetDataSource(myDataSet);
                var report = new ReportViewerCommon(myReport);
                report.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        #endregion

        public static void PrintForensicList(IEnumerable<EmployeeDTO> empList, bool directPrint)
        {
            try
            {
                ReportClass myReport = new ForensicList();
                var listDate = DateTime.Now;
                var calConv = new Calendar(DateTime.Now);
                calConv.ShowDialog();
                var dialogueResult = calConv.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    if (calConv.DtSelectedDate.SelectedDate != null)
                      listDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
                }

                var myDataSet = GenerateReportDatasets.GetForensicListDataSet(empList,listDate);

                myReport.SetDataSource(myDataSet);

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        #region Labour Reports

        public static void PrintLabourtList(IEnumerable<EmployeeDTO> empList, bool directPrint)
        {
            try
            {
                ReportClass myReport;
                var listDate = DateTime.Now;
                var calConv = new Calendar(DateTime.Now);
                calConv.ShowDialog();
                var dialogueResult = calConv.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    if (calConv.DtSelectedDate.SelectedDate != null)
                        listDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
                }

                var setting = new SettingService(true).GetSetting();

                if (setting != null && setting.LabourListType == LabourListTypes.WithNoIntroduction)
                    myReport = new LabourListAmh();
                else
                    myReport = new LabourListNew();

                var myDataSet = GenerateReportDatasets.GetLabourtListDataSet(empList,listDate);

                myReport.SetDataSource(myDataSet);

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        public static void PrintCoverLetter(EmployeeDTO employee, bool directPrint)
        {
            try
            {
                ReportClass myReport4;
                var setting = new SettingService(true).GetSetting();

                if (setting != null && setting.CoverLetterType == CoverLetterTypes.Format1)
                    myReport4 = new LabourSingleLetterCustom4();
                else
                    myReport4 = new LabourSingleLetterCustom1();
                //Lette is AgentName no CC, 1 is AgentNameWith CC, 2&3 are with CC No AgentName, 4 is no AgnetName No CC
                myReport4.SetDataSource(GenerateReportDatasets.GetLetterDataSet(employee));

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport4);
                else
                {
                    var report = new ReportViewerCommon(myReport4);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        public static void PrintTestimonialLetter(EmployeeDTO employee, bool directPrint)
        {
            try
            {
                ReportClass myReport4 = null;
                var setting = new SettingService(true).GetSetting();

                if (setting != null)
                {
                    if (setting.TestimonialFormat == TestimonialFormats.NoLetter)
                    {
                        myReport4 = new TestimonialLetterFor2NoLetter();
                    }
                    else
                    {
                        if ((int) setting.NumberOfTestimonials == 1)
                            myReport4 = new TestimonialLetter();
                        else
                            myReport4 = new TestimonialLetterFor2();
                    }
                }

                myReport4.SetDataSource(GenerateReportDatasets.GetTestimonialDataSet(employee));

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport4);
                else
                {
                    var report = new ReportViewerCommon(myReport4);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        public static void PrintLabourApplication(EmployeeDTO employee, bool directPrint)
        {
            try
            {
                var myReport = new LabourDetailNew();
                var dtSet = GenerateReportDatasets.GetLabourApplicationDataSet(employee);
                myReport.SetDataSource(dtSet);

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        //public static void PrintVisaTranslation2(EmployeeDTO employee, bool directPrint)
        //{
        //    var myReport = new VisaTranslation2();
        //    myReport.SetDataSource(GenerateReportDatasets.GetVisaTranslationDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintVisaTranslationEnglish2(EmployeeDTO employee,bool directPrint)
        //{
        //    var myReport = new VisaTranslationEnglish2();
        //    myReport.SetDataSource(GenerateReportDatasets.GetVisaTranslationEnglishDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintWekalaTranslation2(EmployeeDTO employee,bool directPrint)
        //{
        //    var myReport = new WekalaTranslation2();
        //    myReport.SetDataSource(GenerateReportDatasets.GetWekalaDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintNormalWekala(VisaDTO visa,bool directPrint)
        //{
        //    var myReport = new LabourWekala();
        //    myReport.SetDataSource(GenerateReportDatasets.GetNormalWekalaDataSet(visa));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintConditionArabic(VisaDTO visa,bool directPrint)
        //{
        //    var myReport = new LabourConditionsArabic1();
        //    myReport.SetDataSource(GenerateReportDatasets.GetConditionArabicDataSet(visa));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintConditionTranslation(VisaDTO visa,bool directPrint)
        //{
        //    var myReport = new ConditionTranslation();
        //    myReport.SetDataSource(GenerateReportDatasets.GetConditionTranslationDataSet(visa));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintAgreementFront(EmployeeDTO employee,bool directPrint)
        //{
        //    var myReport = new AgreementFront();
        //    myReport.SetDataSource(GenerateReportDatasets.GetAgreementFrontDataSet(employee));

        //    ExportOptions CrExportOptions;
        //    DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
        //    PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
        //    CrDiskFileDestinationOptions.DiskFileName = "E:\\SampleReport.pdf";
        //    CrExportOptions = myReport.ExportOptions;
        //    {
        //        CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
        //        CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
        //        CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
        //        CrExportOptions.FormatOptions = CrFormatTypeOptions;
        //    }
        //    myReport.Export();

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintAgreementBack(EmployeeDTO employee,bool directPrint)
        //{
        //    var employeeTestimony = new EmployeeTestimony(employee);
        //    employeeTestimony.ShowDialog(); 

        //    var cri = new SearchCriteria<EmployeeRelativeDTO>();
        //    cri.FiList.Add(e => e.EmployeeId == employee.Id && e.Type == RelativeTypes.Testimony);
        //    var testimonies = new EmployeeRelativeService(true).GetAll(cri).ToList();

        //    if (testimonies.Count < 3)
        //    {
        //        MessageBox.Show("Problem on getting Testimony Persons, at Least 3 is required!");
        //        return;
        //    }

        //    //var myReport = new AgreementBack();
        //    var myReport = new Testimonies();
        //    myReport.SetDataSource(GenerateReportDatasets.GetAgreementBackDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }

        //}

        //public static void PrintAgreementFull(EmployeeDTO employee, bool directPrint)
        //{
        //    var myReport = new AgreementFull();

        //    var myDataSetAgreementFront = GenerateReportDatasets.GetAgreementFrontDataSet(employee);
        //    var myDataSetAgreementBack = GenerateReportDatasets.GetAgreementBackDataSet(employee);

        //    myReport.Subreports["AgreementFront.rpt"].SetDataSource(myDataSetAgreementFront);
        //    myReport.Subreports["AgreementBack.rpt"].SetDataSource(myDataSetAgreementBack);

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        public static void PrintLabourAllInOne(EmployeeDTO employee, bool directPrint)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myReport = new LabourAllInOneCommon2();
            var rSet = new ReportsDataSet();
            const int serNo = 1;

            #region Data Sets

            var myDataSetLetterSingle = new ReportsDataSet();
            var myDataSetEmbassyLetter = new ReportsDataSet();
            var myDataSetEmbassyIqrar = new ReportsDataSet();

            var myDataSetLetterSingle1 = new ReportsDataSet();
            var myDataSetLetterSingle2 = new ReportsDataSet();
            var myDataSetLetterSingle3 = new ReportsDataSet();

            myDataSetLetterSingle.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile,
                localAgency.Footer.AttachedFile, null, "aa", "aa");
            myDataSetLetterSingle1.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile,
                localAgency.Footer.AttachedFile, null, "aa", "aa");
            myDataSetLetterSingle2.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile,
                localAgency.Footer.AttachedFile, null, "aa", "aa");
            myDataSetLetterSingle3.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile,
                localAgency.Footer.AttachedFile, null, "aa", "aa");
            myDataSetEmbassyLetter.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile,
                localAgency.Footer.AttachedFile, null, "aa", "aa");

            ReportsDataSet myDataSetLetterSingle4 = GenerateReportDatasets.GetLetterDataSet(employee);
            ReportsDataSet myDataSetApplication = GenerateReportDatasets.GetLabourApplicationDataSet(employee);
            ReportsDataSet myDataSetVisaTranslation = GenerateReportDatasets.GetVisaTranslationDataSet(employee);
            ReportsDataSet myDataSetWekalaTranslation = GenerateReportDatasets.GetWekalaDataSet(employee);
            ReportsDataSet myDataSetWekalaNormal = GenerateReportDatasets.GetNormalWekalaDataSet(employee.Visa);
            ReportsDataSet myDataSetConditionsCommon = GenerateReportDatasets.GetConditionArabicDataSet(employee.Visa);
            ReportsDataSet myDataSetConditionsTranslation =
                GenerateReportDatasets.GetConditionTranslationDataSet(employee.Visa);

            #endregion

            rSet.LetterHeads.Rows.Add(serNo.ToString(), null, null, null, "aa", "aa");
            myReport.SetDataSource(rSet);

            #region Sub Reports

            myReport.Subreports["LabourDetailNew.rpt"].SetDataSource(myDataSetApplication);
            myReport.Subreports["LabourSingleLetteCustom.rpt"].SetDataSource(myDataSetLetterSingle);
            myReport.Subreports["VisaTranslation2.rpt"].SetDataSource(myDataSetVisaTranslation);
            myReport.Subreports["WekalaTranslation2.rpt"].SetDataSource(myDataSetWekalaTranslation);
            myReport.Subreports["LabourWekala.rpt"].SetDataSource(myDataSetWekalaNormal);
            myReport.Subreports["LabourConditionsArabic1.rpt"].SetDataSource(myDataSetConditionsCommon);
            myReport.Subreports["EmbassyLetter.rpt"].SetDataSource(myDataSetEmbassyLetter);
            myReport.Subreports["EmbassyIqrar.rpt"].SetDataSource(myDataSetEmbassyIqrar);
            myReport.Subreports["LabourSingleLetterCustom1.rpt"].SetDataSource(myDataSetLetterSingle1);
            myReport.Subreports["LabourSingleLetterCustom2.rpt"].SetDataSource(myDataSetLetterSingle2);
            myReport.Subreports["LabourSingleLetterCustom3.rpt"].SetDataSource(myDataSetLetterSingle3);
            myReport.Subreports["LabourSingleLetterCustom4.rpt"].SetDataSource(myDataSetLetterSingle4);
            myReport.Subreports["ConditionTranslation.rpt"].SetDataSource(myDataSetConditionsTranslation);

            #endregion

            if (directPrint)
                new ReportUtility().DirectPrinter(myReport);
            else
            {
                var report = new ReportViewerCommon(myReport);
                report.ShowDialog();
            }
        }

        #endregion

        #region Embassy Reports

        public static void PrintList(IEnumerable<EmployeeDTO> empList, bool directPrint)
        {
            try
            {
                var myReport = new EmbassyListPortrait();
                var listDate = DateTime.Now;
                var calConv = new Calendar(DateTime.Now);
                calConv.ShowDialog();
                var dialogueResult = calConv.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    if (calConv.DtSelectedDate.SelectedDate != null)
                        listDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
                }
                var myDataSet = GenerateReportDatasets.PrintList(empList,listDate);
                myReport.SetDataSource(myDataSet);

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        public static void PrintEmbassyApplication(EmployeeDTO employee, bool directPrint)
        {
            try
            {
                var setting = new SettingService(true).GetSetting();

                ReportClass myReport;
                if (setting != null && setting.EmbassyApplicationFormat == EmbassyApplicationFormats.Format1)
                    myReport = new ConsularSectionNewUpdate();
                else
                    myReport = new ConsularSectionNewUpdate2();

                var dataSet = GenerateReportDatasets.GetEmbassyApplicationDataSet(employee);
                if (dataSet == null)
                {
                    NotifyUtility.ShowCustomBalloon("Application Error",
                        "Make sure you entered EnjazNumber starting with E", 8000);
                    return;
                }
                myReport.SetDataSource(dataSet);

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        public static void PrintRecruitingOrder(EmployeeDTO employee, bool directPrint)
        {
            try
            {
                var myReport = new RecruitingOrder();
                myReport.SetDataSource(GenerateReportDatasets.GetRecruitingOrderDataSet(employee));

                if (directPrint)
                    new ReportUtility().DirectPrinter(myReport);
                else
                {
                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException);
            }
        }

        //public static void PrintPledge(EmployeeDTO employee, bool directPrint)
        //{
        //    var myReport = new EmbassyLetter();
        //    myReport.SetDataSource(GenerateReportDatasets.GetPledgeDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        //public static void PrintConfirmation(EmployeeDTO employee,bool directPrint)
        //{

        //    var myReport = new EmbassyIqrar();
        //    myReport.SetDataSource(GenerateReportDatasets.GetConfirmationDataSet(employee));

        //    if (directPrint)
        //        new ReportUtility().DirectPrinter(myReport);
        //    else
        //    {
        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }
        //}

        public static void PrintEmbassySelection(EmployeeDTO employee, bool directPrint)
        {
            var myReport = new EmployeeSelection();
            myReport.SetDataSource(GenerateReportDatasets.GetEmbassySelectionDataSet(employee));

            if (directPrint)
                new ReportUtility().DirectPrinter(myReport);
            else
            {
                var report = new ReportViewerCommon(myReport);
                report.ShowDialog();
            }
        }

        public static void PrintAllInOne(EmployeeDTO employee, bool directPrint)
        {
            var myReport = new EmbassyAllInOne();

            var myDataSetApplication = GenerateReportDatasets.GetEmbassyApplicationDataSet(employee);
            var myDataSetRecruitingOrder = GenerateReportDatasets.GetRecruitingOrderDataSet(employee);
            var myDataSetEmbassyLetter = GenerateReportDatasets.GetPledgeDataSet(employee);
            var myDataSetEmployeeSelection = GenerateReportDatasets.GetEmbassySelectionDataSet(employee);

            var rSet = new ReportsDataSet();
            rSet.LetterHeads.Rows.Add("aa", null, null, null, "aa", "aa");

            myReport.SetDataSource(rSet);
            myReport.Subreports["ConsularSectionNewUpdate.rpt"].SetDataSource(myDataSetApplication);
            myReport.Subreports["RecruitingOrder.rpt"].SetDataSource(myDataSetRecruitingOrder);
            myReport.Subreports["EmbassyLetter.rpt"].SetDataSource(myDataSetEmbassyLetter);
            myReport.Subreports["EmployeeSelection.rpt"].SetDataSource(myDataSetEmployeeSelection);


            if (directPrint)
                new ReportUtility().DirectPrinter(myReport);
            else
            {
                var report = new ReportViewerCommon(myReport);
                report.ShowDialog();
            }
        }

        #endregion

        #region Summary Reports

        public static void PrintMonthlyList(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet2();
            string[] alphabet =
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
                "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "Z1", "Z2", "Z3", "Z4", "Z5", "Z6", "Z7", "Z8", "Z9", "Z91", "Z92", "Z93", "Z94", "Z95", "Z96", "Z97",
                "Z98", "Z99"
            };

            var myReport = new LabourMonthlyLetter();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.LabourMonthly
            };
            cri.FiList.Add(s => s.FlightProcess != null && s.FlightProcess.Departured);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }

            var serNo = 1;
            //bool found = false;

            int dif = filterEndDate.Subtract(filterStartDate).Days;
            //MessageBox.Show(dif.ToString());
            //int dif = (toDate.DayOfYear - frDate.DayOfYear) + (toDate.Year-frDate.Year) * 365;
            dif = (dif - 1)/30;

            var fromdateAmh = CalendarUtil.GetEthCalendar(filterStartDate, true);
            var todateAmh = CalendarUtil.GetEthCalendar(filterEndDate, true);

            foreach (var employ in employeeList)
            {
                //found = true;
                var vi = employ.Visa;
                if (vi == null) continue;

                int x = (serNo - 1)/15;

                myDataSet.LabourMonthly.Rows.Add(
                    serNo.ToString(),
                    employ.FullNameAmharic,
                    employ.PassportNumber,
                    CalendarUtil.GetEthCalendarFormated(employ.FlightProcess.SubmitDate, "/"),
                    employ.SponsorFullNameShort,
                    vi.Sponsor.Address.Mobile,
                    vi.Sponsor.Address.CountryAmharic,
                    vi.Sponsor.Address.CityAmharic,
                    "በጥሩ ሁኔታ",
                    alphabet[x],
                    localAgency.AgencyNameAmharic,
                    fromdateAmh + " -  " + todateAmh + "  የ " + Monthsreturn(dif) + " ");

                myDataSet.LetterHeads.Rows.Add(serNo.ToString(), null, null, null,
                    CalendarUtil.GetEthCalendarFormated(employ.FlightProcess.SubmitDate.AddYears(2), "/"), "");

                serNo++;
            }

            myReport.SetDataSource(myDataSet);
            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintLost(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();
            var myReport = new LabourMonthlyReports();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.LabourLost
            };
            cri.FiList.Add(
                s =>
                    s.FlightProcess != null && s.FlightProcess.Departured &&
                    s.AfterFlightStatus == AfterFlightStatusTypes.Lost);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }

            var serNo = 1;
            //bool found = false;

            var fromdateAmh = CalendarUtil.GetEthCalendar(filterStartDate, true);
            var todateAmh = CalendarUtil.GetEthCalendar(filterEndDate, true);

            foreach (var employ in employeeList)
            {
                //found = true;
                myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                    employ.FullNameAmharic,
                    employ.PassportNumber, "", "",
                    null, "");

                serNo++;
            }
            myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", localAgency.AgencyNameAmharic,
                " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " የጠፉ");
            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "");

            myReport.SetDataSource(myDataSet);

            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintReturned(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            var myReport = new LabourMonthlyReports();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.LabourReturned
            };
            cri.FiList.Add(
                s =>
                    s.FlightProcess != null && s.FlightProcess.Departured &&
                    s.AfterFlightStatus == AfterFlightStatusTypes.Returned);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }

            var serNo = 1;
            //bool found = false;

            var fromdateAmh = CalendarUtil.GetEthCalendar(filterStartDate, true);
            var todateAmh = CalendarUtil.GetEthCalendar(filterEndDate, true);

            foreach (var employ in employeeList)
            {
                //found = true;
                myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                    employ.FullNameAmharic,
                    employ.PassportNumber, "", "",
                    null, "");

                serNo++;
            }
            myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", localAgency.AgencyNameAmharic,
                " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " የተመለሱ");
            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "");

            myReport.SetDataSource(myDataSet);

            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintContratCompleted(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            var myReport = new LabourMonthlyReports();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.LabourContractEnd
            };
            cri.FiList.Add(
                s =>
                    s.FlightProcess != null && s.FlightProcess.Departured &&
                    s.AfterFlightStatus == AfterFlightStatusTypes.OnGoodCondition);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }

            var serNo = 1;
            //bool found = false;

            var fromdateAmh = CalendarUtil.GetEthCalendar(filterStartDate, true);
            var todateAmh = CalendarUtil.GetEthCalendar(filterEndDate, true);

            foreach (var employ in employeeList)
            {
                //found = true;
                myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                    employ.FullNameAmharic,
                    employ.PassportNumber, "", "",
                    null, "");

                serNo++;
            }

            if (filterEndDate < DateTime.Now)
            {
                myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", localAgency.AgencyNameAmharic,
                    " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ኮንትራታቸው የተጠናቀቀ");
            }
            else
            {
                myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", localAgency.AgencyNameAmharic,
                    " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ኮንትራታቸው የሚጠናቀቅ");
            }
            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "");

            myReport.SetDataSource(myDataSet);
            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintDiscontinued(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            var myReport = new LabourMonthlyReports();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.LabourDiscontinued
            };
            cri.FiList.Add(
                s => s.LabourProcess != null && s.Discontinued && s.CurrentStatus == ProcessStatusTypes.Discontinued);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();

            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }
            var serNo = 1;
            //bool found = false;

            var fromdateAmh = CalendarUtil.GetEthCalendar(filterStartDate, true);
            var todateAmh = CalendarUtil.GetEthCalendar(filterEndDate, true);

            foreach (var employ in employeeList)
            {
                //found = true;
                myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                    employ.FullNameAmharic,
                    employ.PassportNumber, "", "",
                    null, "");

                serNo++;
            }

            myDataSet.EmployeeListForEmbassyHeader.Rows.Add("",
                localAgency.AgencyNameAmharic,
                " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ማህበራዊ ከተማሩ በኋላ ያቋረጡ");

            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "");

            myReport.SetDataSource(myDataSet);
            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintEmbassyMonthly(DateTime filterStartDate, DateTime filterEndDate)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            var myReport = new EmbassyListForEmail();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = ReportTypes.EmbassyMonthly
            };
            cri.FiList.Add(s => s.EmbassyProcess != null);
                // && s.LabourProcess.Discontinued && s.CurrentStatus == ProcessStatusTypes.OnProcess);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();

            if (employeeList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }
            var serNo = 1;
            //bool found = false;

            foreach (var employ in employeeList)
            {
                myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                    employ.FullName,
                    employ.PassportNumber,
                    "ETHIOPIA", employ.EmbassyProcess.EnjazNumber, null, "");
                myDataSet.EmployeeListForEmbassyHeader.Rows.Add(serNo.ToString(), localAgency.AgencyName,
                    DateTime.Now.ToShortDateString());
                myDataSet.EmployeeListForEmbassyHeader2.Rows.Add(serNo.ToString(), filterStartDate.ToShortDateString(),
                    filterEndDate.ToShortDateString(), "", "", "", "");
                myDataSet.LetterHeads.Rows.Add(serNo.ToString(), localAgency.Header.AttachedFile, null, null, "", "");
                serNo++;

                serNo++;
            }

            myReport.SetDataSource(myDataSet);
            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        public static void PrintFlightList(DateTime filterStartDate, DateTime filterEndDate, ReportDocument myReport,
            ReportTypes reportType)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            //var myReport = new TicketAmountList();
            //var myReport = new GetTicketList();

            var cri = new SearchCriteria<EmployeeDTO>
            {
                BeginingDate = filterStartDate,
                EndingDate = filterEndDate,
                ReportType = reportType
            };
            cri.FiList.Add(s => s.FlightProcess != null && !s.FlightProcess.Departured);

            ////If Filtered By Country or City
            //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

            var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
            if (employeeList.Count == 0)
            {
                MessageBox.Show("No Data is found on the given Duration!!");
                return;
            }

            var serNo = 1;
            //bool found = false;

            foreach (var employ in employeeList)
            {
                //found = true;
                var flight = employ.FlightProcess;

                myDataSet.FlightTicket.Rows.Add(flight.SubmitDateString,
                    serNo.ToString(),
                    employ.FullName,
                    employ.PassportNumber,
                    flight.TicketNumber,
                    flight.TicketAmount,
                    flight.TicketCity.Trim().ToUpper(), "", "", "", 0.0);

                serNo++;
            }

            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "");

            myReport.SetDataSource(myDataSet);
            var report = new ReportViewerCommon(myReport);
            report.ShowDialog();
        }

        #endregion

        #region Complains

        public static void PrintComplain(EmployeeDTO selectedEmployee)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();
            var myReport = new ComplainLetter();

            try
            {
                var visa = selectedEmployee.Visa;
                var selectedComplain = selectedEmployee.CurrentComplain;
                string contactName = "", contactTel = "";
                try
                {
                    contactName = selectedComplain.Employee.ContactPerson.FullName;
                    contactTel = selectedComplain.Employee.ContactPerson.Address.MobileWithCountryCode;
                }
                catch
                {
                }

                myDataSet.LabourLetterSingleCustom.Rows.Add(
                    selectedComplain.ComplainDateAmharicString,
                    selectedComplain.ComplainNumber,
                    selectedComplain.Complain,
                    selectedEmployee.FullName,
                    visa.Sponsor.FullName,
                    visa.Sponsor.Address.Mobile + "  " + visa.Sponsor.Address.Telephone,
                    visa.Sponsor.Address.City,
                    "", contactName,
                    contactTel,
                    "",
                    selectedComplain.RaisedByName,
                    selectedComplain.RaisedByRelationship,
                    selectedComplain.RaisedByTelephone,
                    "", "");

                myDataSet.LetterHeads.Rows.Add("",
                    localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile,
                    null, "", "");

                myReport.SetDataSource(myDataSet);

                var report = new ReportViewerCommon(myReport);
                report.ShowDialog();
            }
            catch
            {
            }
        }

        #endregion

        #region Common Properties for Reports

        public static string Monthsreturn(int ind)
        {
            if (ind < 0) ind = 0;
            if (ind > 2) ind = 2;
            string[] months = {"አንድ", "ሁለት", "ሦስት"};
            return months[ind];
        }

        public static string ParseAmhCal(string amhCal)
        {
            return amhCal.Substring(0, 2) + "/" + amhCal.Substring(2, 2) + "/" + amhCal.Substring(4, 4);
        }

        public static string Translatehijri(string hijr)
        {
            try
            {
                DateTime greg;

                if (Convert.ToInt32(hijr.Substring(0, 4)) > 2000)
                {
                    greg = Convert.ToDateTime(hijr);
                }
                else
                {
                    const string format = "MM/dd/yyyy";
                    var dat = new Dates();
                    greg = Convert.ToDateTime(dat.HijriToGreg(hijr, format));
                }

                var amhcal = CalendarUtil.GetEthCalendar(greg, false);
                return amhcal.Substring(0, 2) + "/" + amhcal.Substring(2, 2) + "/" + amhcal.Substring(4, 4);
            }
            catch
            {
                return hijr;
            }
        }

        public static string TranslatehijritoGreg(string hijr)
        {
            try
            {
                //if (Convert.ToInt32(hijr.Substring(0, 4)) > 2000)
                //{
                //    greg = Convert.ToDateTime(hijr);
                //}
                //else
                //{
                const string format = "MM/dd/yyyy";
                var dat = new Dates();
                var greg = Convert.ToDateTime(dat.HijriToGreg(hijr, format));
                return greg.ToShortDateString();
                //}

                //string amhcal = new Reports.ReportUtility().getEthCalendar(greg, false);
                //return amhcal.Substring(0, 2) + "/" + amhcal.Substring(2, 2) + "/" + amhcal.Substring(4, 4);
            }
            catch
            {
                return hijr;
            }
        }

        public static byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        #endregion
    }
}