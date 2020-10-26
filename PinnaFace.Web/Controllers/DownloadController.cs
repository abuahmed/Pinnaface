using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using PinnaFace.Core;

namespace PinnaFace.Web.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Download(string file)
        {
            //try
            //{
            //    const string ftphost = "198.38.83.167";
            //    var ftpfilepath = "/pinnaface.com/downloads/" + file;
            //    var ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            //    using (var request = new WebClient())
            //    {
            //        request.Credentials = DbCommandUtil.GetNetworkCredential();// new NetworkCredential("munahan1", "hani1212");

            //        byte[] fileData = request.DownloadData(ftpfullpath);

            //        var response = new FileContentResult(fileData, "application/exe")
            //        {
            //            FileDownloadName = file
            //        };
            //        return response;
            //    }
            //}
            //catch (WebException e)
            //{
            //    String status = ((FtpWebResponse)e.Response).StatusDescription;
            //    return View("Error",
            //        new HandleErrorInfo(new Exception(status), "Download",
            //            "Download"));
            //}

            return View("Index");
        }


        //public ActionResult Download(string file)
        //{
        //    string ftphost = "198.38.83.167";
        //    string ftpfilepath = "/munahan1/pinnaface.com/downloads/" + file;

        //    string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

        //    if (!System.IO.File.Exists(ftpfullpath))
        //    {
        //        return HttpNotFound(ftpfullpath);
        //    }

        //    var fileBytes = System.IO.File.ReadAllBytes(ftpfullpath);
        //    var response = new FileContentResult(fileBytes, "application/octet-stream")
        //    {
        //        FileDownloadName = file // "loremIpsum.pdf"
        //    };
        //    return response;
        //}
        //private void Download(string file)
        //{
        //    string url = "ftp://198.38.83.167/munahan1/pinnaface.com/downloads/";// "ftp://ftp.example.com/directory/to/download/";
        //    try
        //    {
        //        string uri = url + "/" + file;
        //        Uri serverUri = new Uri(uri);
        //        if (serverUri.Scheme != Uri.UriSchemeFtp)
        //        {
        //            return;
        //        }
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + "/" + file);
        //        request.UseBinary = true;
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;
        //        request.Credentials = new NetworkCredential("munahan1", "hani1212");
        //        request.KeepAlive = false;
        //        request.UsePassive = false;
        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        Stream responseStream = response.GetResponseStream();
        //        FileStream writeStream = new FileStream(localDestnDir + "\\" + file, FileMode.Create);
        //        int Length = 2048;
        //        Byte[] buffer = new Byte[Length];
        //        int bytesRead = responseStream.Read(buffer, 0, Length);
        //        while (bytesRead > 0)
        //        {
        //            writeStream.Write(buffer, 0, bytesRead);
        //            bytesRead = responseStream.Read(buffer, 0, Length);
        //        }
        //        writeStream.Close();
        //        response.Close();
        //    }
        //    catch (WebException wEx)
        //    {
        //        //MessageBox.Show(wEx.Message, "Download Error");
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message, "Download Error");
        //    }
        //}

        
    }
}