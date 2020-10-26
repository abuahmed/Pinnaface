using System;
using System.IO;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core
{
    public static class PathUtil
    {
        public static string GetFolderPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\PinnaFace\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetDatabasePath()
        {
            string path = GetFolderPath();
            return Path.Combine(path, "PinnaFaceDbProd.sdf");
        }

        public static string GetPhotoPath()
        {
            string photoPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                               "\\PinnaFace\\Photo\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetLocalPhotoPath()
        {
            string photoPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                               "\\PinnaFace\\Photo\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetAgreementPath()
        {
            string photoPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                               "\\PinnaFace\\Agreements\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetLogPath()
        {
            string photoPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                               "\\PinnaFace\\Logs\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetLocalServerBackupPath()
        {
            string photoPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                               "\\PinnaFace\\DbBackups\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetDestinationPhotoPath()
        {
            string photoPath = "";

            
            if (Singleton.BuildType == BuildType.LocalDev)
            {
                photoPath = "E:\\OneDrive\\Dev\\PinnaFace\\PinnaFace.Web\\Content\\Photos\\";
                if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            }
            else if (Singleton.BuildType == BuildType.Dev)
            {
                photoPath = "ftp://198.38.83.167/dev.pinnaface.com/wwwroot/Content/Photos";
            }
            else
            {
                photoPath = "ftp://198.38.83.167/pinnaface.com/wwwroot/Content/Images";
            }
            return photoPath;
        }

        public static string GetDestinationAgreementsPath()
        {
            string agreementsPath = "";
            if (Singleton.BuildType == BuildType.LocalDev)
            {
                agreementsPath = "E:\\Dev\\PinnaFace\\PinnaFace.Web\\Content\\Agreements";
                if (!Directory.Exists(agreementsPath))
                    Directory.CreateDirectory(agreementsPath);
            }
            else if (Singleton.BuildType == BuildType.Dev)
            {
                agreementsPath = "ftp://198.38.83.167/dev.pinnaface.com/wwwroot/Content/Agreements";
            }
            else
            {
                    
                agreementsPath = "ftp://198.38.83.167/pinnaface.com/wwwroot/Content/Agreements";
            }

            return agreementsPath;
        }

        public static string GetServerLogPath()
        {
            const string logPath = "ftp://198.38.83.167/pinnaface.com/logs";
            return logPath;
        }

        public static string GetServerBackupPath()
        {
            const string dbbackupPath = "ftp://198.38.83.167/pinnaface.com/dbbackups";
            return dbbackupPath;
        }
    }
}