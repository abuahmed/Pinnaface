using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using PinnaFace.Core;

namespace PinnaFace.SyncEngine.WPF.Common
{
    public class FileUploader
    {
        private readonly string _destination;
        private readonly string _sourceFile;

        public FileUploader()
        {
            _destination = PathUtil.GetDestinationPhotoPath();
            _sourceFile = PathUtil.GetLocalPhotoPath();
        }

        public List<string> GetServerFileNames()
        {
            var listRequest = WebRequest.Create(PathUtil.GetDestinationPhotoPath());
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = DbCommandUtil.GetNetworkCredential();
            var lines = new List<string>();
            var lines2 = new List<string>();

            using (var listResponse = listRequest.GetResponse())
            using (var listStream = listResponse.GetResponseStream())
            using (var listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    var line = listReader.ReadLine();
                    lines.Add(line);
                }
            }

            foreach (var line in lines)
            {
                var tokens =
                    line.Split(new[] {' '}, 9, StringSplitOptions.RemoveEmptyEntries);

                var name = tokens[3];
                lines2.Add(name);
            }


            return lines2;
        }

        public void UploadFiles()
        {
            var serverFileNames = GetServerFileNames();

            var dir = new DirectoryInfo(_sourceFile);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.jpg", SearchOption.AllDirectories);
            //IList<string> fileNames = fileList.Select(file => file.Name).ToList();
            IList<string> fileNames = (from file in fileList where !serverFileNames.Contains(file.Name) select file.Name).ToList();


            if (fileNames.Count > 0)
                UploadFiles(fileNames);
        }

        private void UploadFiles(IEnumerable<string> fileNames)
        {
            using (var client = new WebClient())
            {
                client.Credentials = DbCommandUtil.GetNetworkCredential();

                foreach (var fileName in fileNames)
                {
                    var destpa = new Uri(Path.Combine(_destination, fileName));
                    var sourceLogFile = Path.Combine(_sourceFile, fileName);
                    try
                    {
                        client.UploadFile(destpa, WebRequestMethods.Ftp.UploadFile, sourceLogFile);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "FileUploader.UploadFile problem",
                            ex.Message + Environment.NewLine + ex.InnerException, "", "");
                    }
                }
            }
        }

        //https://stackoverflow.com/questions/44606028/upload-and-download-a-binary-file-to-from-ftp-server-in-c-net


        //foreach (var file in fileList)
        //{
        //    var fiName = PathUtil.GetDestinationPhotoPath() + "/" + file.Name;
        //    var request = (FtpWebRequest)WebRequest.Create(fiName);
        //    request.Credentials = DbCommandUtil.GetNetworkCredential();
        //    request.Method = WebRequestMethods.Ftp.GetDateTimestamp;// WebRequestMethods.Ftp.GetFileSize;
        //    request.UsePassive = true;
        //    request.UseBinary = true;
        //    request.KeepAlive = false;

        //    try
        //    {
        //        var response = (FtpWebResponse)request.GetResponse();
        //        response.Close();
        //    }
        //    catch //(WebException ex)
        //    {
        //        fileNames.Add(file.Name);
        //        //var response = (FtpWebResponse)ex.Response;
        //        //if (response.StatusCode ==
        //        //    FtpStatusCode.ActionNotTakenFileUnavailable)
        //        //{
        //        //    //Does not exist
        //        //    fileNames.Add(file.Name);
        //        //}
        //    }

        //}

        //foreach (FileInfo file in fileList)
        //{
        //    const string fiDest = "http://pinnaface.com/Content/Images";

        //    try
        //    {
        //        var fiName = fiDest + "/" + file.Name;

        //        HttpResponseMessage httpResponseMessage;
        //        using (var client = new HttpClient())
        //        {
        //            var msg = new HttpRequestMessage(HttpMethod.Head, fiName);
        //            httpResponseMessage = client.SendAsync(msg).Result;
        //        }
        //        if (httpResponseMessage != null)
        //        {
        //            var resp = httpResponseMessage;

        //            if (resp.StatusCode == HttpStatusCode.OK)
        //            {
        //                var lastMod = resp.Content.Headers.LastModified;

        //                if (lastMod != null)
        //                {
        //                    var lastModD = lastMod.Value.DateTime;
        //                    var sourceLastModD = file.LastWriteTimeUtc;
        //                    var serverFileDate = new DateTime(lastModD.Year, lastModD.Month, lastModD.Day,
        //                        lastModD.Hour,
        //                        lastModD.Minute, lastModD.Second);
        //                    var sourceFileDate = new DateTime(sourceLastModD.Year, sourceLastModD.Month,
        //                        sourceLastModD.Day, sourceLastModD.Hour,
        //                        sourceLastModD.Minute, sourceLastModD.Second);

        //                    if (serverFileDate < sourceFileDate)
        //                        fileNames.Add(file.Name); //UploadFile(file.Name);
        //                }
        //                else //(file.LastWriteTime > lastMod) //destFile.LastWriteTime
        //                {
        //                    fileNames.Add(file.Name); //UploadFile(file.Name);
        //                }
        //            }
        //            else
        //            {
        //                fileNames.Add(file.Name); //UploadFile(file.Name);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "FileUploader.UploadFiles problem",
        //            ex.Message + Environment.NewLine + ex.InnerException, "", "");
        //    }
        //}

        /*
     
Upload
The most trivial way to upload a binary file to an FTP server using .NET framework is using WebClient.UploadFile:

WebClient client = new WebClient();
client.Credentials = new NetworkCredential("username", "password");
client.UploadFile("ftp://ftp.example.com/remote/path/file.zip", @"C:\local\path\file.zip");

         * If you need a greater control, that WebClient does not offer (like TLS/SSL encryption, etc), use FtpWebRequest. Easy way is to just copy a FileStream to FTP stream using Stream.CopyTo:

FtpWebRequest request =
    (FtpWebRequest)WebRequest.Create("ftp://ftp.example.com/remote/path/file.zip");
request.Credentials = new NetworkCredential("username", "password");
request.Method = WebRequestMethods.Ftp.UploadFile;  

using (Stream fileStream = File.OpenRead(@"C:\local\path\file.zip"))
using (Stream ftpStream = request.GetRequestStream())
{
    fileStream.CopyTo(ftpStream);
}
If you need to monitor an upload progress, you have to copy the contents by chunks yourself:

FtpWebRequest request =
    (FtpWebRequest)WebRequest.Create("ftp://ftp.example.com/remote/path/file.zip");
request.Credentials = new NetworkCredential("username", "password");
request.Method = WebRequestMethods.Ftp.UploadFile;  

using (Stream fileStream = File.OpenRead(@"C:\local\path\file.zip"))
using (Stream ftpStream = request.GetRequestStream())
{
    byte[] buffer = new byte[10240];
    int read;
    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
    {
        ftpStream.Write(buffer, 0, read);
        Console.WriteLine("Uploaded {0} bytes", fileStream.Position);
    } 
}     
         
         
         */


        //private void UploadFile(string fileName)
        //{
        //    try
        //    {
        //        //var tcs = new TaskCompletionSource<bool>();
        //        using (var client = new WebClient())
        //        {
        //            var destpa = new Uri(Path.Combine(_destination, fileName));
        //            var sourceLogFile = Path.Combine(_sourceFile, fileName);

        //            client.Credentials = DbCommandUtil.GetNetworkCredential();
        //            client.UploadFile(destpa, WebRequestMethods.Ftp.UploadFile, sourceLogFile);

        //            //client.UploadProgressChanged += UploadProgressChangedHandler;
        //            //client.UploadFileCompleted += (sender, args) => UploadCompletedHandler(fileName, tcs, args);
        //            //client.UploadFileAsync(new Uri(Path.Combine(_destination, fileName)),
        //            //                    Path.Combine(_sourceFile, fileName));
        //            //tcs.Task.Wait();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "FileUploader.UploadFile problem",
        //            ex.Message + Environment.NewLine + ex.InnerException, "", "");
        //    }
        //}

        //private void UploadCompletedHandler(string fileName, TaskCompletionSource<bool> tcs,
        //    UploadFileCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        tcs.TrySetCanceled();
        //    }
        //    else if (e.Error != null)
        //    {
        //        tcs.TrySetException(e.Error);
        //    }
        //    else
        //    {
        //        tcs.TrySetResult(true);
        //    }
        //}

        //private void UploadProgressChangedHandler(object sender, UploadProgressChangedEventArgs e)
        //{
        //    // Handle progress, e.g.
        //    //System.Diagnostics.Debug.WriteLine(e.ProgressPercentage);
        //    //LogUtil.LogError(ErrorSeverity.Info, "Upload Progress",
        //        //e.ProgressPercentage.ToString(), "Progress", "Progress");
        //}
    }
}

//foreach (var fileName in fileNames)
//{
//    FileInfo file = new FileInfo(Path.Combine(_sourceFile, fileName));
//    FileInfo destFile=new FileInfo(Path.Combine(_destination, fileName));


//    if (destFile.Exists)
//    {
//        if (file.LastWriteTime > destFile.LastWriteTime)
//        {
//            UploadFile(fileName);
//            //file.CopyTo(destFile.FullName, true);
//        }
//    }
//    else
//    {
//        UploadFile(fileName);
//    }
//    ////UploadFile(fileName);
//}


//private void UploadFile(string destinationFileName, string sourceFileName)
//{
//    var tcs = new TaskCompletionSource<bool>();
//    using (var client = new WebClient())
//    {
//        client.Credentials = DbCommandUtil.GetNetworkCredential();

//        client.UploadProgressChanged += UploadProgressChangedHandler;
//        client.UploadFileCompleted += (sender, args) => UploadCompletedHandler(sourceFileName, tcs, args);
//        client.UploadFileAsync(new Uri(destinationFileName), sourceFileName);
//        tcs.Task.Wait();
//    }
//}