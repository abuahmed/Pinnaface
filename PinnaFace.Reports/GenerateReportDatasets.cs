using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Reports.Embassy;
using PinnaFace.Service;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace PinnaFace.Reports
{
    public static class GenerateReportDatasets
    {
        public static void ViewReportInWebBrowser(ReportDocument cReport)
        {
            var crReportDocument = cReport;
            var oStream = (MemoryStream) crReportDocument.ExportToStream(ExportFormatType.WordForWindows);
            //var oStream=crReportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, null, true, "");
            ////var at = new Attachment(oStream, EmailDetail.AttachmentFileName + ".doc", "application/doc");
            //return oStream;
        }

        public static ReportsDataSet GetForensicListDataSet(IEnumerable<EmployeeDTO> empList,DateTime listDate)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();
                //var selectedLabourProcess = employee.LabourProcess;
                var letterRef = "";
                if (setting != null && setting.ShowLetterReferenceNumber)
                    letterRef = setting.CurrentLetterReferenceNumberString;


                var serNo = 1;
                //var cri = new SearchCriteria<EmployeeDTO>();

                var employeeList = empList;

                foreach (var employ in employeeList)
                {
                    myDataSet.EmployeeListForLabour.Rows.Add(
                        serNo.ToString(CultureInfo.InvariantCulture),
                        employ.FullNameAmharic,
                        employ.PassportNumber,
                        "");
                    serNo++;
                }

                myDataSet.LetterHeads.Rows.Add(letterRef,
                    localAgency.Header.AttachedFile,
                    localAgency.Footer.AttachedFile, null,
                    localAgency.AgencyNameAmharic, ParseAmhCal(CalendarUtil.GetEthCalendar(listDate, false)));
                myDataSet.LabourLetterSingleCustom.Rows.Add(
                    ParseAmhCal(CalendarUtil.GetEthCalendar(listDate, false)),
                    letterRef,
                    localAgency.AgencyNameAmharic,
                    setting.AwajNumber,
                    "", //Agent Name
                    "",
                    "",
                    "",
                    "",
                    localAgency.ManagerNameAmharic,
                    localAgency.Managertype,
                    "", "", "1");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetForensicListDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }

        #region Labour Reports

        public static ReportsDataSet GetLabourtListDataSet(IEnumerable<EmployeeDTO> empList, DateTime listDate)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();
                var letterRef = "";
                if (setting != null && setting.ShowLetterReferenceNumber)
                {
                    letterRef = setting.CurrentLetterReferenceNumberString;
                }

                var serNo = 1;

                var employeeList = empList;


                foreach (var employ in employeeList)
                {
                    if (employ.Visa == null)
                        continue;
                    myDataSet.EmployeeListForLabour.Rows.Add(
                        serNo.ToString(CultureInfo.InvariantCulture),
                        employ.FullNameAmharic,
                        employ.PassportNumber,
                        "ሳውዲ አረቢያ"); //EnumUtil.GetEnumDesc(employ.Visa.Sponsor.Address.CountryAmharic)
                    serNo++;
                }

                myDataSet.LetterHeads.Rows.Add(letterRef,
                    localAgency.Header.AttachedFile,
                    localAgency.Footer.AttachedFile, null,
                    localAgency.AgencyNameAmharic,
                    ParseAmhCal(CalendarUtil.GetEthCalendar(listDate, false)));

                myDataSet.LabourLetterSingleCustom.Rows.Add(
                    ParseAmhCal(CalendarUtil.GetEthCalendar(listDate, false)),
                    letterRef,
                    localAgency.AgencyNameAmharic,
                    setting.AwajNumber,
                    "Agent Name",
                    "",
                    "ሳውዲ አረቢያ",
                    "",
                    "",
                    localAgency.ManagerNameAmharic,
                    localAgency.Managertype,
                    "", "", "1");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetLabourtListDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }
            
        public static ReportsDataSet GetLetterDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();
                var selectedLabourProcess = employee.LabourProcess;
                var employeeVisa = employee.Visa;
                var letterRef = "";
                if (setting != null && setting.ShowLetterReferenceNumber)
                {
                    letterRef = setting.CurrentLetterReferenceNumberString;
                }

                if (localAgency == null) return myDataSet;
                myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile,
                    null,
                    "aa", "aa");

                if (setting != null)
                    myDataSet.LabourLetterSingleCustom.Rows.Add(
                        ParseAmhCal(CalendarUtil.GetEthCalendar(selectedLabourProcess.SubmitDate, false)),
                        letterRef,
                        localAgency.AgencyNameAmharic,
                        setting.AwajNumber,
                        employee.FullNameAmharic,
                        employee.PassportNumber,
                        EnumUtil.GetEnumDesc(employeeVisa.Sponsor.Address.CountryAmharic),
                        employeeVisa.AgentName,
                        EnumUtil.GetEnumDesc(employeeVisa.Sponsor.Address.CountryAmharic),
                        localAgency.ManagerNameAmharic,
                        localAgency.Managertype,
                        "", "", "1");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetLetterDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetTestimonialDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();
                var selectedLabourProcess = employee.LabourProcess;
                var employeeVisa = employee.Visa;

                var letterRef = "";
                if (setting != null && setting.ShowLetterReferenceNumber)
                {
                    letterRef = setting.CurrentLetterReferenceNumberString;
                }

                if (localAgency == null) return myDataSet;
                myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile,
                    null, "aa", "aa");
                var submitDate = selectedLabourProcess != null ? selectedLabourProcess.SubmitDate : DateTime.Now;
                if (setting != null)
                    myDataSet.LabourLetterSingleCustom.Rows.Add(
                        ParseAmhCal(CalendarUtil.GetEthCalendar(submitDate, false)),
                        letterRef,
                        localAgency.AgencyNameAmharic,
                        setting.AwajNumber,
                        employee.FullNameAmharic,
                        employee.PassportNumber,
                        EnumUtil.GetEnumDesc(employeeVisa.Sponsor.Address.CountryAmharic),
                        employeeVisa.AgentName,
                        EnumUtil.GetEnumDesc(employeeVisa.Sponsor.Address.CountryAmharic),
                        localAgency.ManagerNameAmharic,
                        localAgency.Managertype,
                        "", "", "1");

                var cri = new SearchCriteria<EmployeeRelativeDTO>();
                cri.FiList.Add(e => e.EmployeeId == employee.Id && e.Type == RelativeTypes.Testimony);
                var testimonies = new EmployeeRelativeService(true)
                    .GetAll(cri)
                    .ToList();

                //var line = 0;
                //string[] testy = { "", "", "", "", "", "", "", "", "" };
                //foreach (var employeeTestimony in testimonies)
                //{
                //    testy[line + 0] = employeeTestimony.FullName;
                //    testy[line + 1] = employeeTestimony.Address.City;
                //    testy[line + 2] = employeeTestimony.Address.Mobile;
                //    line = line + 3;
                //}
                var firstTestimony = testimonies[0];
                var secondTestimony = testimonies[1];

                myDataSet.AgreementTestimonials2.Rows.Add("",
                    firstTestimony.FullName, firstTestimony.Address.SubCity, firstTestimony.Address.Mobile,
                    secondTestimony.FullName, secondTestimony.Address.SubCity, secondTestimony.Address.Mobile,
                    firstTestimony.Address.Woreda, firstTestimony.Address.Kebele, firstTestimony.Address.HouseNumber,
                    secondTestimony.Address.Woreda, secondTestimony.Address.Kebele, secondTestimony.Address.HouseNumber,
                    "", "");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetTestimonialDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetLabourApplicationDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                const int serNo = 1;
                var basename = Path.GetDirectoryName(Application.ExecutablePath);
                var maletick = Image.FromFile(basename + "\\Resources\\checkbox_yes2.png");
                var educationDto = employee.Education;


                byte[] kuwait = null,
                    female = null,
                    male = null,
                    single = null,
                    married = null,
                    divorced = null,
                    widowed = null,
                    separated = null,
                    emergfemale = null,
                    emergmale = null;

                #region Visa Detail

                var visa = employee.Visa;
                string fname = visa.Sponsor.FullName, familiyName = "", sname = "";
                //sname = fname.Substring(0, fname.LastIndexOf(' '));
                //familiyName = fname.Substring(fname.LastIndexOf(' ') + 1);

                myDataSet.EmployerDetail.Rows.Add(
                    serNo.ToString(),
                    fname,
                    sname,
                    visa.Sponsor.Address.Country.ToString().ToUpper(),
                    visa.Sponsor.Address.City.ToUpper(),
                    visa.Sponsor.Address.MobileWithCountryCode,
                    visa.Sponsor.Address.AlternateMobile,
                    visa.Sponsor.Address.Fax,
                    visa.Sponsor.Address.PoBox,
                    visa.Sponsor.Address.PrimaryEmail,
                    visa.Sponsor.PassportNumber,
                    "", "");

                #endregion

                #region Education

                if (educationDto != null)
                {
                    #region Qualification Types

                    byte[] cert = null, diploma1 = null, degree = null, others1 = null;

                    switch (educationDto.QualificationType)
                    {
                        case QualificationTypes.High:
                            cert = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Preparatory:
                            cert = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case
                            QualificationTypes.Primary:
                            cert = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Diploma:
                            diploma1 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Bachelor:
                            degree = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Illiterate:
                            others1 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Master:
                            others1 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Driving:
                            others1 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case QualificationTypes.Doctrate:
                            others1 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                    }

                    #endregion

                    #region Level of Qualification

                    byte[] elem = null,
                        junsec = null,
                        second = null,
                        secondcomplet = null,
                        voclevel = null,
                        voccomplete = null,
                        collegelevel = null,
                        collegecomplete = null,
                        postgradlevel = null,
                        postgrad = null,
                        nunformal = null,
                        others = null;

                    switch (educationDto.LevelOfQualification)
                    {
                        case LevelOfQualificationTypes.Elementary:
                            elem = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.JuniorSecondary:
                            junsec = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.SecondaryLevel:
                            second = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.SecondaryComplete:
                            secondcomplet = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.VocationalLevel:
                            voclevel = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.VocationalComplete:
                            voccomplete = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.CollegeLevel:
                            collegelevel = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.CollegeComplete:
                            collegecomplete = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.PostGraduateLevel:
                            postgradlevel = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.PostGraduate:
                            postgrad = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.NonFormalEducation:
                            nunformal = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case LevelOfQualificationTypes.Others:
                            others = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        default:
                            elem = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                    }

                    #endregion

                    #region Awards

                    byte[] cert2 = null, diploma2 = null, ba = null, ma = null, phd = null, others3 = null;
                    switch (educationDto.Award)
                    {
                        case AwardTypes.Certificate:
                            cert2 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case AwardTypes.Diploma:
                            diploma2 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case AwardTypes.BABSC:
                            ba = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case AwardTypes.MAMSC:
                            ma = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case AwardTypes.Phd:
                            phd = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                        case AwardTypes.Others:
                            others3 = ImageToByteArray(maletick, ImageFormat.Png);
                            break;
                    }

                    #endregion

                    myDataSet.EducationDetail.Rows.Add(
                        serNo.ToString(),
                        cert, diploma1, degree, others1,
                        elem, junsec, second, secondcomplet, voclevel, voccomplete, collegelevel, collegecomplete,
                        postgradlevel, postgrad, nunformal, others,
                        educationDto.YearCompleted, educationDto.FieldOfStudy,
                        cert2, diploma2, ba, ma, phd, others3,
                        educationDto.ProffesionalSkill,
                        EnumUtil.GetEnumDesc(employee.AppliedProfession),
                        employee.SalaryString, "", "");
                }

                #endregion

                #region EmployeeDetail

                #region Sex

                if (employee.Sex == Sex.Male)
                {
                    male = ImageToByteArray(maletick, ImageFormat.Png);
                }
                else
                {
                    female = ImageToByteArray(maletick, ImageFormat.Png);
                }

                #endregion

                #region Marital Status

                switch (employee.MaritalStatus)
                {
                    case MaritalStatusTypes.Single:
                        single = ImageToByteArray(maletick, ImageFormat.Png);
                        break;
                    case MaritalStatusTypes.Married:
                        married = ImageToByteArray(maletick, ImageFormat.Png);
                        break;
                    case MaritalStatusTypes.Divorced:
                        divorced = ImageToByteArray(maletick, ImageFormat.Png);
                        break;
                    case MaritalStatusTypes.Widow:
                        widowed = ImageToByteArray(maletick, ImageFormat.Png);
                        break;
                    case MaritalStatusTypes.Separated:
                        separated = ImageToByteArray(maletick, ImageFormat.Png);
                        break;
                }
                var Selectedemployee = employee;

                #endregion

                myDataSet.EmployeeDetailForLabour.Rows.Add(
                    serNo.ToString(),
                    Selectedemployee.FullName.ToUpper(),
                    female,
                    male,
                    Selectedemployee.DateOfBirth.ToString("dd MMM yyyy").ToUpper(),
                    Selectedemployee.PlaceOfBirth,
                    single, married, divorced, widowed, separated,
                    EnumUtil.GetEnumDesc(Selectedemployee.Religion).ToUpper(),
                    Selectedemployee.Address.Region,
                    Selectedemployee.Address.City,
                    Selectedemployee.Address.SubCity,
                    Selectedemployee.Address.Kebele,
                    Selectedemployee.Address.HouseNumber,
                    Selectedemployee.Address.MobileWithCountryCode,
                    Selectedemployee.Address.Fax,
                    Selectedemployee.Address.PoBox,
                    Selectedemployee.Address.PrimaryEmail, Selectedemployee.Address.Woreda, "", "", "");

                #endregion

                var contactPersonDto = employee.ContactPerson;

                #region Contact Person

                var bdate = Convert.ToDateTime(contactPersonDto.DateOfBirth).Year.ToString(CultureInfo.InvariantCulture);
                if (contactPersonDto.AgeOrBirthDate != null && !contactPersonDto.AgeOrBirthDate.Contains(":"))
                {
                    bdate = contactPersonDto.AgeOrBirthDate;
                }
                if (contactPersonDto.Sex == Sex.Male)
                {
                    emergmale = ImageToByteArray(maletick, ImageFormat.Png);
                }
                else
                {
                    emergfemale = ImageToByteArray(maletick, ImageFormat.Png);
                }

                myDataSet.EmergencyDetail.Rows.Add(
                    serNo.ToString(),
                    contactPersonDto.FullName.ToUpper(),
                    emergfemale, emergmale, bdate,
                    contactPersonDto.Address.Region,
                    contactPersonDto.Address.City,
                    contactPersonDto.Address.SubCity,
                    contactPersonDto.Address.Kebele,
                    contactPersonDto.Address.HouseNumber,
                    contactPersonDto.Address.MobileWithCountryCode,
                    contactPersonDto.Address.Fax,
                    contactPersonDto.Address.PoBox,
                    contactPersonDto.Address.PrimaryEmail, contactPersonDto.Address.Woreda, "");

                #endregion

                var localAgency = new LocalAgencyService(true).GetLocalAgency();

                #region Agency Detail

                if (localAgency.KuwaitOperation)
                    kuwait = ImageToByteArray(maletick, ImageFormat.Png);

                myDataSet.AgencyDetail.Rows.Add(
                    serNo.ToString(),
                    localAgency.AgencyName,
                    localAgency.Address.Region,
                    localAgency.Address.City,
                    localAgency.Address.SubCity,
                    localAgency.Address.Kebele,
                    localAgency.Address.MobileWithCountryCode,
                    localAgency.Address.Fax,
                    localAgency.Address.PoBox,
                    localAgency.Address.PrimaryEmail,
                    "",
                    localAgency.ManagerName,
                    localAgency.DepositAmount, null, kuwait, null, null,
                    ImageToByteArray(maletick, ImageFormat.Png), null, null, null, null,
                    employee.Photo.AttachedFile,
                    localAgency.LicenceNumber, localAgency.Address.Woreda, "");

                #endregion

                #region Agent Detail

                var agent = visa.Agent;
                myDataSet.AgentDetail.Rows.Add(
                    serNo.ToString(),
                    agent.AgentName.ToUpper(),
                    agent.Address.Country.ToString().ToUpper(),
                    agent.Address.City.ToUpper(),
                    agent.Address.MobileWithCountryCode,
                    agent.Address.AlternateTelephone,
                    agent.Address.Fax,
                    agent.Address.PoBox,
                    agent.LicenseNumber,
                    agent.Address.PrimaryEmail,
                    agent.PassportNumber, agent.ContactPerson, "");

                #endregion

                var selectedLabourProcess = employee.LabourProcess;

                #region Office Detail

                myDataSet.OfficeUseDetail.Rows.Add(
                    serNo.ToString(), "", "", "",
                    Selectedemployee.PassportNumber,
                    visa.VisaNumber, "", "",
                    visa.Sponsor.PassportNumber,
                    employee.ContractNumber, EnumUtil.GetEnumDesc(employee.AppliedProfession),
                    selectedLabourProcess.ContratBeginDate.Value.Year + " G.C.",
                    selectedLabourProcess.ContratEndDate.Value.Year + " G.C.",
                    employee.SalaryString, "",
                    "",
                    "", "");

                #endregion
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetLabourApplicationDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }

            return myDataSet;
        }

        public static ReportsDataSet GetVisaTranslationDataSet(EmployeeDTO employee)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var visa = employee.Visa;
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "aa", "aa");

            try
            {
                //var visa = EmployeeVisa;
                var count = "ኢትዮጵያዊት";

                if (employee.Sex == Sex.Male)
                {
                    count = "ኢትዮጵያዊ";
                }

                var professionAmharic = visa.Condition.ProfessionAmharic.ToString();
                if (visa.Condition.Profession.ToString().Contains("MAID"))
                {
                    professionAmharic = "የቤት ውስጥ ሠራተኛ";
                }

                var visaDate = visa.VisaDateArabic + " (" + Translatehijri(visa.VisaDateArabic) + ")";

                myDataSet.VisaTranslation.Rows.Add("",
                    visa.VisaNumber,
                    visaDate,
                    visa.BankNumber, "",
                    visa.Sponsor.FullNameAmharic,
                    visa.Sponsor.PassportNumber,
                    visa.FileNumber,
                    visa.VisaQuantity,
                    professionAmharic,
                    count,
                    "አዲስ አበባ",
                    visa.Sponsor.Address.CityAmharic,
                    "", "1", null);
            }
            catch
            {
                //MessageBox.Show("Problem Getting Visa Translation DataSet");
            }

            return myDataSet;
        }

        public static ReportsDataSet GetVisaTranslationEnglishDataSet(EmployeeDTO employee)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var visa = employee.Visa;
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "", "");

            try
            {
                //var visa = EmployeeVisa;
                var count = "Ethiopian";

                if (employee.Sex == Sex.Male)
                {
                    count = "Ethiopian";
                }

                var professionAmharic = visa.Condition.ProfessionAmharic.ToString();
                if (visa.Condition.Profession.ToString().Contains("MAID"))
                {
                    professionAmharic = "HOUSE MAID";
                }

                myDataSet.VisaTranslation.Rows.Add("",
                    visa.VisaNumber,
                    visa.VisaDateArabic + " (" + TranslatehijritoGreg(visa.VisaDateArabic) + ")",
                    visa.BankNumber, "",
                    visa.Sponsor.FullName,
                    visa.Sponsor.PassportNumber,
                    visa.FileNumber,
                    visa.VisaQuantity,
                    professionAmharic,
                    count,
                    "Addis Ababa",
                    visa.Sponsor.Address.City,
                    "", "", null);
            }
            catch
            {
                //MessageBox.Show("Problem Getting Visa Translation English DataSet");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetWekalaDataSet(EmployeeDTO employee)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var visa = employee.Visa;
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "aa", "aa");

            try
            {
                var professionAmharic = visa.Condition.ProfessionAmharic.ToString();
                if (visa.Condition.Profession.ToString().Contains("MAID"))
                {
                    professionAmharic = "የቤት ውስጥ ሠራተኛ";
                }
                var wekalaDate = visa.VisaDateArabic + " (" + Translatehijri(visa.VisaDateArabic) + ")";

                myDataSet.WekalaTranslation.Rows.Add("",
                    visa.WekalaNumber,
                    wekalaDate,
                    visa.Agent.AgentNameAmharic,
                    visa.Sponsor.FullNameAmharic,
                    visa.Sponsor.PassportNumber,
                    LocalAgency.AgencyNameAmharic,
                    LocalAgency.LicenceNumber,
                    professionAmharic,
                    visa.VisaNumber,
                    visa.VisaQuantity,
                    "አዲስ አበባ",
                    visa.Agent.AgentNameAmharic,
                    "", "1", null);
            }
            catch
            {
                //MessageBox.Show("Problem Getting Wekala DataSet");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetNormalWekalaDataSet(VisaDTO visa)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "", "");

            var licNo = LocalAgency.LicenceNumber;
            if (licNo.Contains("/"))
                licNo = licNo.Substring(0, licNo.IndexOf("/", StringComparison.Ordinal));
            try
            {
                //var visa = EmployeeVisa;
                var agent = visa.Agent;


                myDataSet.LabourWekala.Rows.Add(
                    visa.WekalaDate,
                    visa.WekalaNumber,
                    agent.AgentName,
                    visa.Sponsor.FullName,
                    visa.Sponsor.PassportNumber,
                    LocalAgency.AgencyNameShort,
                    "LIC No. " + licNo,
                    visa.VisaNumber, "1");
            }
            catch
            {
                //MessageBox.Show("Problem Getting Normal Wekala DataSet");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetConditionArabicDataSet(VisaDTO visa)
        {
            var myDataSet = new ReportsDataSet();

            try
            {
                var agent = visa.Agent;

                var ftime = "YES";
                if (visa.Condition.FirstTime)
                {
                    ftime = "NO";
                }
                if (visa.Condition.WriteRead)
                {
                }

                //var condate = DateTime.Now.AddDays(-10);
                string visadate;
                try
                {
                    visadate = visa.VisaDateArabic.Split('/')[2] + "/" + visa.VisaDateArabic.Split('/')[1] + "/" +
                               visa.VisaDateArabic.Split('/')[0];
                }
                catch
                {
                    visadate = visa.VisaDateArabic;
                }
                const string agentstamp = "";

                const int serNo = 1;
                //var educ = Selectedemployee.EmployeeEducations.FirstOrDefault();
                const string educa = "Elementary";
                //if (_educationDto != null) educa = _educationDto.LevelOfQualification.ToString();

                myDataSet.LabourConditions.Rows.Add(
                    visadate,
                    visa.Sponsor.FullName,
                    visa.Sponsor.Address.Telephone,
                    visa.VisaNumber,
                    visa.Condition.AgeFrom + " - " + visa.Condition.AgeTo,
                    visa.Condition.Salary,
                    visa.Condition.Religion.ToString().ToUpper(),
                    "Good Looking & Acceptable Appearance",
                    visa.Sponsor.Address.Country.ToString().ToUpper(),
                    visa.Sponsor.Address.City,
                    serNo.ToString(CultureInfo.InvariantCulture));

                myDataSet.LabourConditions2.Rows.Add(
                    visadate,
                    agent.Header.AttachedFile,
                    agentstamp, "", "VILLA",
                    visa.Condition.Profession,
                    ftime,
                    educa.ToUpper(),
                    serNo.ToString(CultureInfo.InvariantCulture));

                myDataSet.Countryname.Rows.Add(
                    visadate,
                    agent.Footer.AttachedFile, null,
                    serNo.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
                //MessageBox.Show("Problem Getting Condition Arabic DataSet");
            }
            return myDataSet;
        }

        public static ReportsDataSet GetConditionTranslationDataSet(VisaDTO visa)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "aa", "aa");

            try
            {
                var count = "ሙስሊም";
                if (visa.Condition.Religion.ToString().ToLower().Contains("chr"))
                {
                    count = "ክርሰቲያን";
                }
                else if (visa.Condition.Religion.ToString().ToLower().Contains("oth"))
                {
                    count = "ሌላ";
                }
                var muya = visa.Condition.ProfessionAmharic.ToString();
                if (visa.Condition.Profession.ToString().ToLower().Contains("maid"))
                {
                    muya = "የቤት ውስጥ ሠራተኛ";
                }

                string salary;
                try
                {
                    salary = visa.Condition.Salary.ToString();
                }
                catch
                {
                    salary = visa.Condition.Salary.ToString();
                }

                myDataSet.VisaTranslation.Rows.Add("1",
                    visa.VisaNumber,
                    visa.VisaDateArabic + " (" + Translatehijri(visa.VisaDateArabic) + ")",
                    visa.Agent.AgentName,
                    salary,
                    visa.Sponsor.FullNameAmharic,
                    visa.ContratNumber,
                    visa.Agent.LicenseNumber,
                    visa.Sponsor.Address.Telephone,
                    muya,
                    visa.Condition.AgeFrom.ToString(CultureInfo.InvariantCulture) + "-" +
                    visa.Condition.AgeTo.ToString(CultureInfo.InvariantCulture),
                    count,
                    visa.Agent.Address.Telephone,
                    "አያስፈልግም",
                    LocalAgency.AgencyNameAmharic,
                    null);
            }
            catch
            {
                //MessageBox.Show("Problem Getting Condition Translation DataSet");
            }
            return myDataSet;
        }

        public static ReportsDataSet2 GetAgreementFrontDataSet(EmployeeDTO employee)
        {
            var localAgency = new LocalAgencyService().GetLocalAgency();
            // new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet2();

            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "aa");

            try
            {
                var visa = employee.Visa;
                var emrg = employee.ContactPerson;

                const string educa = "Elementary";

                if (emrg != null)
                    myDataSet.ContractAgreement.Rows.Add("",
                        visa.Sponsor.FullName,
                        "",
                        visa.Sponsor.Address.Telephone,
                        "", "",
                        localAgency.AgencyName, "",
                        localAgency.Address.Telephone,
                        localAgency.Address.Fax,
                        localAgency.Address.PoBox,
                        employee.FullName,
                        (DateTime.Now.Year - employee.DateOfBirth.Year).ToString(CultureInfo.InvariantCulture),
                        employee.Sex.ToString().ToUpper(),
                        employee.MaritalStatus.ToString().ToUpper(),
                        educa.ToUpper(),
                        employee.PassportNumber,
                        employee.PassportIssueDate.ToShortDateString() + " ADDIS ABABA ",
                        emrg.FullName,
                        emrg.Address.City,
                        emrg.Address.Telephone,
                        "", "",
                        visa.Condition.Profession,
                        visa.Condition.Salary,
                        visa.Sponsor.Address.City,
                        "", "", "", "");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetAgreementFrontDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }

        public static ReportsDataSet2 GetAgreementBackDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet2();
            try
            {
                var cri = new SearchCriteria<EmployeeRelativeDTO>();
                cri.FiList.Add(e => e.EmployeeId == employee.Id && e.Type == RelativeTypes.Testimony);
                var testimonies = new EmployeeRelativeService(true)
                    .GetAll(cri)
                    .ToList();

                var line = 0;
                string[] testy = {"", "", "", "", "", "", "", "", ""};
                foreach (var employeeTestimony in testimonies)
                {
                    testy[line + 0] = employeeTestimony.FullName;
                    testy[line + 1] = employeeTestimony.Address.City;
                    testy[line + 2] = employeeTestimony.Address.Mobile;
                    line = line + 3;
                }

                myDataSet.AgreementTestimonials.Rows.Add("",
                    testy[0], testy[1], testy[2],
                    testy[3], testy[4], testy[5],
                    testy[6], testy[7], testy[8],
                    "", "", "", "", "");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.GetAgreementBackDataSet",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }

        #endregion

        #region Embassy Reports

        public static ReportsDataSet PrintList(IEnumerable<EmployeeDTO> empList, DateTime listDate)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var LocalAgency = new LocalAgencyService(true).GetLocalAgency();

                var myReport = new EmbassyListPortrait();

                var serNo = 1;

                var employeeDtos = empList;
                foreach (var employ in employeeDtos)
                {
                    //var employ = employee;
                    var embassyProcess = employ.EmbassyProcess;
                    var brCode = new BarcodeProcess();

                    myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
                        employ.FullName,
                        employ.PassportNumber, EnumUtil.GetEnumDesc(CountryList.Ethiopia),
                        embassyProcess.EnjazNumber,
                        ImageToByteArray(brCode.GetBarcode(embassyProcess.EnjazNumber, 150, 20, false), ImageFormat.Bmp),
                        "");

                    myDataSet.EmployeeListForEmbassyHeader.Rows.Add(serNo.ToString(),
                        LocalAgency.AgencyName,
                        listDate.ToShortDateString());

                    serNo++;
                }

                myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile,
                    null,
                    "", "aa");
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.PrintEmbassyList",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
            return myDataSet;
        }
            
        public static ReportsDataSet GetEmbassyApplicationDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet();
            try
            {
                var setting = new SettingService(true).GetSetting();
                var localAgency = new LocalAgencyService(true).GetLocalAgency();
                var employeeVisa = employee.Visa;
                var selectedEmbassyProcess = employee.EmbassyProcess;
                var passIssuePlace = string.IsNullOrEmpty(employee.PlaceOfIssue) ? EnumUtil.GetEnumDesc(CityList.AddisAbeba) : employee.PlaceOfIssue;
                //var EmployeePhoto = employee.Photo;

                #region Fields

                var basename = Path.GetDirectoryName(Application.ExecutablePath);
                var maletick = Image.FromFile(basename + "\\Resources\\maletick.png");
                var femaletick = Image.FromFile(basename + "\\Resources\\femaletick.png");
                var maleticknot = Image.FromFile(basename + "\\Resources\\maleticknot.png");
                var femaleticknot = Image.FromFile(basename + "\\Resources\\femaleticknot.png");
                var purpose = Image.FromFile(basename + "\\Resources\\purposeofwork.png");


                var visa = employeeVisa;
                var brCode = new BarcodeProcess();

                #endregion

                #region Variable Values

                var employeeeeName = employee.FullName;
                //.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                var enjazbarcode =
                    ImageToByteArray(brCode.GetBarcode(selectedEmbassyProcess.EnjazNumber, 320, 40, false),
                        ImageFormat.Bmp);
                var visaNumbarcode = ImageToByteArray(brCode.GetBarcode(visa.VisaNumber, 320, 40, false),
                    ImageFormat.Bmp);
                byte[] female, male;
                var purposeofW = ImageToByteArray(purpose, ImageFormat.Png);

                if (employee.Sex == Sex.Male)
                {
                    male = ImageToByteArray(maletick, ImageFormat.Png);
                    female = ImageToByteArray(femaleticknot, ImageFormat.Png);
                }
                else
                {
                    female = ImageToByteArray(femaletick, ImageFormat.Png);
                    male = ImageToByteArray(maleticknot, ImageFormat.Png);
                }

                var relg = "Muslim";
                if (employee.Religion.ToString().Contains("Non"))
                {
                    relg = "Non-Muslim";
                }
                var spname = visa.Sponsor.FullName;
                if (setting.EmbassyApplicationType == EmbassyApplicationTypes.SponsorNameOnTopArabic)
                {
                    spname = visa.Sponsor.FullNameArabic;
                }

                #endregion

                myDataSet.EmployeeDetailForConsular.Rows.Add(
                    enjazbarcode, visaNumbarcode,
                    employeeeeName, employee.DateOfBirth.ToString("dd-MMM-yyyy"),
                    employee.PlaceOfBirth.ToUpper(),
                    EnumUtil.GetEnumDesc(CountryList.Ethiopia),
                    EnumUtil.GetEnumDesc(CountryList.Ethiopia),
                    female, male,
                    employee.MaritalStatus.ToString().ToUpper(), relg.ToUpper(),
                    passIssuePlace,
                    EnumUtil.GetEnumDesc(employee.AppliedProfession), employee.Address.MobileWithCountryCode,
                    localAgency.AgencyName, purposeofW,
                    passIssuePlace,
                    employee.PassportIssueDate.ToString("dd-MMM-yyyy"),
                    employee.PassportExpiryDate.ToString("dd-MMM-yyyy"),
                    employee.PassportNumber, visa.Sponsor.FullName,
                    visa.Sponsor.Address.MobileWithCountryCode, visa.Sponsor.Address.City, employeeeeName,
                    visa.VisaNumber, visa.VisaDateArabic,
                    visa.Sponsor.FullName,
                    selectedEmbassyProcess.EnjazNumber, visa.VisaNumber,
                    spname, selectedEmbassyProcess.SubmitDate.ToString("dd-MMM-yyyy"),
                    "aa");

                byte[] empShortPhoto;
                var shortPhotoAttachment = new AttachmentService().Find(employee.PhotoId.ToString());
                if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                {
                    var fname = "";
                    if (!string.IsNullOrWhiteSpace(shortPhotoAttachment.AttachmentUrl))
                        fname = Path.Combine(ImageUtil.GetPhotoPath(), shortPhotoAttachment.AttachmentUrl);
                    empShortPhoto = ImageUtil.ToBytes(ImageUtil.ToImageFromUrl(fname));
                }
                else
                {
                    empShortPhoto = shortPhotoAttachment.AttachedFile;
                }

                myDataSet.EmployeeDetailForConsularPhoto.Rows.Add(
                    employeeeeName, empShortPhoto,
                    localAgency.Address.MobileWithCountryCode, "", "");
                //myDataSet.EmployeeDetailForConsularPhoto.Rows.Add(employeeeeName, EmployeePhoto.AttachedFile, employee.CodeNumber, "", "");
                return myDataSet;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Reports.PrintEmbassyList",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
                return null;
            }
        }

        public static ReportsDataSet2 GetRecruitingOrderDataSet(EmployeeDTO employee)
        {
            var myDataSet = new ReportsDataSet2();
            var visa = employee.Visa;

            var sponsorName = visa.Sponsor.FullName;
            var _educationDto = employee.Education;

            //var _educationDto = Selectedemployee.EmployeeEducations.FirstOrDefault();
            var educa = "Elementary";
            if (_educationDto != null) educa = _educationDto.LevelOfQualification.ToString();


            myDataSet.RecruitingOrder.Rows.Add("",
                visa.Agent.LicenseNumber,
                visa.ContratNumber,
                visa.VisaDateArabic,
                visa.Agent.AgentName,
                sponsorName,
                visa.Sponsor.PassportNumber,
                visa.Sponsor.Address.PoBox,
                visa.Sponsor.Address.City, "-",
                visa.Sponsor.Address.AlternateTelephone,
                visa.Sponsor.Address.Fax,
                visa.Sponsor.Address.Telephone,
                visa.VisaNumber,
                visa.VisaDateArabic,
                employee.AppliedProfession,
                EnumUtil.GetEnumDesc(CountryList.Ethiopia),
                "Two Years", "-", "-",
                employee.AppliedProfession,
                visa.Condition.Salary, "Two Years", visa.Sponsor.Address.City,
                visa.Condition.Religion,
                visa.Condition.AgeFrom + " - " + visa.Condition.AgeTo,
                "Acceptable", educa.ToUpper(),
                "As Available", "", "", "", "", "", "aa");

            return myDataSet;
        }

        public static ReportsDataSet GetPledgeDataSet(EmployeeDTO employee)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();
            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "aa");

            myDataSet.EmbassyLetter.Rows.Add("",
                employee.FullName,
                employee.PassportNumber,
                employee.Religion, "", "", "aa");

            return myDataSet;
        }

        public static ReportsDataSet GetConfirmationDataSet(EmployeeDTO employee)
        {
            var localAgency = new LocalAgencyService(true).GetLocalAgency();
            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", localAgency.Header.AttachedFile, localAgency.Footer.AttachedFile, null,
                "", "aa");

            myDataSet.EmbassyLetter.Rows.Add("",
                employee.FullNameAmharic,
                employee.PassportNumber,
                employee.FullNameAmharic,
                "", "", "aa");

            return myDataSet;
        }

        public static ReportsDataSet GetEmbassySelectionDataSet(EmployeeDTO employee)
        {
            var LocalAgency = new LocalAgencyService(true).GetLocalAgency();
            var EmployeeVisa = employee.Visa;

            var myDataSet = new ReportsDataSet();

            myDataSet.LetterHeads.Rows.Add("", LocalAgency.Header.AttachedFile, LocalAgency.Footer.AttachedFile, null,
                "", "aa");

            myDataSet.LabourLetterSingleCustom.Rows.Add("",
                EmployeeVisa.VisaNumber,
                LocalAgency.AgencyName,
                EmployeeVisa.Sponsor.PassportNumber,
                employee.FullName,
                employee.PassportNumber,
                LocalAgency.AgencyName,
                EmployeeVisa.Sponsor.FullName,
                "", "", "", "", "", "", "", "aa");

            return myDataSet;
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