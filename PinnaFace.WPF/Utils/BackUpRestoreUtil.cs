using System;
using System.Windows;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using PinnaFace.Core;

namespace PinnaFace.WPF
{
    public static class BackUpRestoreUtil
    {
        public static Server GetServer()
        {
            try
            {
                var serveruspas = DbCommandUtil.GetBackUpUserNameAndPassword().Split('_');

                var serverConnection = new ServerConnection(serveruspas[0])
                {
                    LoginSecure = false,
                    Login = serveruspas[1],
                    Password = serveruspas[2]
                };
                return new Server(serverConnection);
            }
            catch
            {
                return null;
            }
        }

        public static void AutoBackUp()
        {
            try
            {
                var path = PathUtil.GetLocalServerBackupPath();
                var status=BackUpServerDatabase(GetServer(), path);
            }
            catch
            {

            }
        }

        public static string BackUpServerDatabase(Server server, string path)
        {
            try
            {
                var agencyName = "NoAgencyName";
                var biosSn = "00000";
                try
                {
                    agencyName = Singleton.Agency.AgencyName;
                    biosSn = Singleton.ProductActivation.BiosSn;
                    agencyName = agencyName.Substring(0, agencyName.IndexOf(' '));
                }
                catch
                {
                }

                var bkpDatabase = new Backup {Action = BackupActionType.Database, Database = "PinnaFaceDbProd"}; //
                var bkpDevice =
                    new BackupDeviceItem(
                        path + "\\" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + agencyName + "_" + biosSn + ".bak",
                        DeviceType.File);
                bkpDatabase.Devices.Add(bkpDevice);
                bkpDatabase.SqlBackup(server);
                return "";
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }
    }
}