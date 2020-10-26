using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service;

namespace PinnaFace.SyncEngine
{
    public static class CleanData
    {
        public static void CleanUnUsedLocalPhoto()
        {
            try
            {
                var dir = new DirectoryInfo(PathUtil.GetLocalPhotoPath());
                List<string> attachments = new AttachmentService(true).GetAll().Select(at => at.AttachmentUrl).ToList();

                DeleteData(dir, attachments);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "CleanData.CleanUnUsedLocalPhoto",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
        }

        public static void CleanUnUsedServerPhoto()
        {
            try
            {
                var dir = new DirectoryInfo(PathUtil.GetDestinationPhotoPath());
                IUnitOfWork destinationUnitOfWork = new UnitOfWorkServer(new ServerDbContextFactory().Create());
                List<string> attachments = destinationUnitOfWork.Repository<AttachmentDTO>()
                    .Query().Get()
                    .Select(at => at.AttachmentUrl).ToList();
                destinationUnitOfWork.Dispose();

                DeleteData(dir, attachments);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "CleanData.CleanUnUsedServerPhoto",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
        }

        public static string DeleteData(DirectoryInfo dir, List<string> attachments)
        {
            try
            {
                IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    string fileName = fileInfo.Name;
                    //if (fileName.Length < 36)
                    //    continue;
                    bool fileInUse = attachments.Contains(fileName);

                    if (!fileInUse)
                        fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "CleanData.DeleteData",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }

            return "";
        }

        public static void SendLogReport()
        {
            try
            {
                string dest = PathUtil.GetServerLogPath();
                string source = PathUtil.GetLogPath();
                var dir = new DirectoryInfo(source);
                AgencyDTO agency = new LocalAgencyService(true).GetLocalAgency(); // Singleton.Agency;

                IEnumerable<FileInfo> fileList = dir.GetFiles("*.txt", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    string sourceFiName = fileInfo.Name;
                    string destFiName = agency.AgencyNameShort + "_" + fileInfo.Name;

                    File.Copy(Path.Combine(source, sourceFiName), Path.Combine(dest, destFiName), true);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "SendLogReport.SendLogReport",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
        }
        public static void DeleteLogReportFiles()
        {
            try
            {
                string source = PathUtil.GetFolderPath();
                var dir = new DirectoryInfo(source);

                var todayFile = DateTime.Now.Date.ToString("dd-MM-yy")+ "_Log.txt";

                IEnumerable<FileInfo> fileList = dir.GetFiles("*.txt", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    if (todayFile != fileInfo.Name)
                    {
                        fileInfo.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "DeleteLogReportFiles.DeleteLogReportFiles",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
        }
        public static void AutomaticDatabaseBackup()
        {
            try
            {
                string dest = PathUtil.GetServerBackupPath();
                string source = PathUtil.GetFolderPath();
                var dir = new DirectoryInfo(source);
                var localAgency = new LocalAgencyService(true).GetLocalAgency(); // Singleton.Agency;

                IEnumerable<FileInfo> fileList = dir.GetFiles("*.sdf", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    string sourceFiName = fileInfo.Name;
                    string destFiName = localAgency.AgencyNameShort + "_" + DateTime.Now.ToString("dd-MM-yy") + "_" +
                                        fileInfo.Name;

                    File.Copy(Path.Combine(source, sourceFiName), Path.Combine(dest, destFiName), true);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "AutomaticDatabaseBackup.AutomaticDatabaseBackup",
                    ex.Message + Environment.NewLine + ex.InnerException, "defaultUser1", "Agency1");
            }
        }
    }
}