using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
//using PinnaFace.Reports;
//using PinnaFace.Reports.Labour;
using PinnaFace.Service;
using WebMatrix.WebData;

namespace PinnaFace.Web.Controllers
{
    public class ReportController : Controller
    {
        [Authorize]
        public ActionResult AgreementDocument(string empId)
        {
            try
            {
                int employeeId = EncryptionUtility.Hash64Decode(empId);
                var cri = new SearchCriteria<EmployeeDTO>
                {
                    CurrentUserId = WebSecurity.CurrentUserId
                };
                cri.FiList.Add(v => v.Id == employeeId);

                EmployeeDTO employeeDTO = new EmployeeService(true, true)
                    .GetAll(cri).ToList().FirstOrDefault();


                if (employeeDTO != null)
                {
                    int? labourProcessId = employeeDTO.LabourProcessId;


                    var criLabour = new SearchCriteria<LabourProcessDTO>();
                    criLabour.FiList.Add(v => v.Id == labourProcessId);
                    LabourProcessDTO labourDTO =
                        new LabourProcessService(true).GetAll(criLabour).ToList().FirstOrDefault();
                    if (labourDTO != null)
                    {
                        var fileName = labourDTO.AgreementFileName;

                        string serverpath = Server.MapPath("../Content/Agreements/");

                        //string path = Path.Combine(serverpath, fileName);
                        //return File(path, "application/pdf");

                        var fileStream = new FileStream(serverpath + fileName,
                                   FileMode.Open,
                                   FileAccess.Read
                                 );
                        var fsResult = new FileStreamResult(fileStream, "application/pdf");
                        //fsResult.FileDownloadName = fileName;
                        return fsResult;
                    }
                }
            }
            catch //(Exception ex)
            {
                //return View("Error",
                //    new HandleErrorInfo(new Exception("Error Processing Agreeemnt Document"+ex.Message+ex.InnerException), "Report",
                //        "AgreementDocument"));
            }
            return View("Error",
                    new HandleErrorInfo(new Exception("Can't get Agreeemnt document "), "Report",
                        "AgreementDocument"));
        }

        //[Authorize]
        //public ActionResult GetPdfDocument(string empId, int reportTypee)
        //{
        //    try
        //    {
        //        int employeeId = EncryptionUtility.Hash64Decode(empId);
        //        var cri = new SearchCriteria<EmployeeDTO>();
        //        cri.FiList.Add(v => v.Id == employeeId);

        //        EmployeeDTO employeeDTO = new EmployeeService(true, true)
        //            .GetAll(cri).ToList().FirstOrDefault();

        //        var reportType = (ReportDocumentTypes) reportTypee;
        //        var myReport = new ReportClass();

        //        switch (reportType)
        //        {
        //            case ReportDocumentTypes.LabourLetter:
        //                myReport = new LabourSingleLetterCustom4();
        //                myReport.SetDataSource(GenerateReportDatasets.GetLetterDataSet(employeeDTO));
        //                break;
        //            case ReportDocumentTypes.AggreementFront:
        //                //myReport = new AgreementFront();
        //                //myReport.SetDataSource(GenerateReportDatasets.GetAgreementFrontDataSet(employeeDTO));
        //                myReport = new AgreementFull();
        //                ReportsDataSet2 myDataSetAgreementFront =
        //                    GenerateReportDatasets.GetAgreementFrontDataSet(employeeDTO);
        //                ReportsDataSet2 myDataSetAgreementBack =
        //                    GenerateReportDatasets.GetAgreementBackDataSet(employeeDTO);
        //                myReport.Subreports["AgreementFront.rpt"].SetDataSource(myDataSetAgreementFront);
        //                myReport.Subreports["AgreementBack.rpt"].SetDataSource(myDataSetAgreementBack);
        //                break;
        //        }

        //        //MemoryStream oStream = new MemoryStream(); // using System.IO
        //        //oStream = (MemoryStream)
        //        //myReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        ////crystalReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
        //        //Response.ClearContent(); //Clear the content
        //        //Response.ClearHeaders(); //Clear the Headers
        //        //Response.Buffer = true;
        //        //Response.ContentType = "application/pdf";
        //        //string strPath = Server.MapPath("..");
        //        //Response.AddHeader("Content-Disposition", "inline; filename=" + strPath + "Report.pdf");
        //        //Response.Clear();
        //        //Response.Buffer = true;
        //        //Response.ContentType = "application/pdf";
        //        //Response.BinaryWrite(oStream.ToArray());
        //        //Response.End();
        //        ////MemoryStream oStream = new MemoryStream(); // using System.IO
        //        ////oStream = (MemoryStream)
        //        ////myReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        ////Response.Clear();
        //        ////Response.Buffer = true;
        //        ////Response.ContentType = "application/pdf";
        //        ////Response.BinaryWrite(oStream.ToArray());
        //        ////Response.End();

        //        //myReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat,Response, true, "Crystal");
        //        //Response.End();

        //        Stream oStream = myReport.ExportToStream(ExportFormatType.PortableDocFormat);

        //        var fsResult = new FileStreamResult(oStream, "application/pdf");

        //        //fsResult.FileDownloadName
        //        return fsResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error",
        //            new HandleErrorInfo(new Exception("Invalid Request/url" + ex.Message + ex.InnerException), "Report",
        //                "GetPdfDocument"));
        //    }
        //}
    }
}