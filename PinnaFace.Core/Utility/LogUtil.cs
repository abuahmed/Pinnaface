using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace PinnaFace.Core
{
    public static class LogUtil
    {
        public static void LogError(ErrorSeverity severity,
            string methodName,
            string developerMessage,
            string userId,
            string customerId)
        {
            try
            {
                var today = DateTime.Now.Date.ToString("dd-MM-yy");

                var path = PathUtil.GetLogPath();
                var pathfile = Path.Combine(path, today + "_Log.txt");

                var w = new StreamWriter(pathfile, true); //ConfigManager.Get("LogFile")
                w.WriteLine("********************************************************************");
                w.WriteLine(DateTime.Now.ToString("f"));
                w.WriteLine("Method: " + methodName);
                w.WriteLine("Message: " + developerMessage);
                w.WriteLine("User/Customer: [" + userId + "][" + customerId + "]");
                w.Close();
                w.Dispose();
            }
            catch
            {
            }
        }

        public static void AddUpdateExistance()
        {
            var path = PathUtil.GetLogPath();
            var pathfile = Path.Combine(path, "updateExists.txt");

            var w = new StreamWriter(pathfile,false); 
            w.WriteLine("********************************************************************");
            w.WriteLine(DateTime.Now.ToString("f"));
            w.Close();
            w.Dispose();
        }

        public static bool CheckUpdateExistance()
        {
            var path = PathUtil.GetLogPath();
            var fname = Path.Combine(path, "updateExists.txt");
            bool fileExists = false;
            if (File.Exists(fname))
            {
                fileExists = true;
                File.Delete(fname);
            }
            return fileExists;
        }
    }

    public class ConfigManager
    {
        public static string Get(string name)
        {
            //If it's in AppSettings great return it
            if (null != ConfigurationManager.AppSettings.Get(name))
                return ConfigurationManager.AppSettings.Get(name);

            // not found so we need to dig a little
            //string settingsDir = Path.GetDirectoryName(System.Environment.CurrentDirectory);

            var di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] fi = di.GetFiles("*.config");
            foreach (FileInfo f in fi)
            {
                if (f.Name.IndexOf("vshost") > -1)
                    continue; //Ignore these, they're dups of the originals

                var fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = f.FullName;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                    ConfigurationUserLevel.None);

                if (null != config.AppSettings.Settings[name] && null != config.AppSettings.Settings[name].Value)
                    return config.AppSettings.Settings[name].Value;
            }
            return null;
        }

        public static bool GetBoolean(string name)
        {
            //If it's in AppSettings great return it
            if (null != ConfigurationManager.AppSettings.Get(name))
            {
                try
                {
                    return Boolean.Parse(ConfigurationManager.AppSettings.Get(name));
                }
                catch
                {
                    return false;
                }
            }

            //not found so we need to dig a little
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] fi = di.GetFiles("*.config");
            foreach (FileInfo f in fi)
            {
                if (f.Name.IndexOf("vshost") > -1)
                    continue; //Ignore these, they're dups of the originals

                var fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = f.FullName;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                    ConfigurationUserLevel.None);

                if (null != config.AppSettings.Settings[name] && null != config.AppSettings.Settings[name].Value)
                {
                    try
                    {
                        return Boolean.Parse(config.AppSettings.Settings[name].Value);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static string GetCurrentPath(string name)
        {
            string result = "Results\r\n";
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] fi = di.GetFiles("*.config");
            foreach (FileInfo f in fi)
                result += f.FullName + "\r\n";

            return result;
        }
    }

    public enum ErrorSeverity
    {
        /// <summary>An informational notification has occurred.</summary>
        Info = 0,

        /// <summary>A warning was occurred.</summary>
        Warning = 1,

        /// <summary>A general error has occurred.</summary>
        General = 2,

        /// <summary>A critical error has occurred.</summary>
        Critical = 3,

        /// <summary>A fatal error has occurred.</summary>
        Fatal = 4
    }
}