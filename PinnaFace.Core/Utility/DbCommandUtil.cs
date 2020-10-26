using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Net;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core
{
    public static class DbCommandUtil
    {
        public static string GetConnectionString()
        {
            string connectionString = "";

            if (Singleton.Edition == PinnaFaceEdition.ServerEdition)
            {
                const string serverName = "PinnaServer"; // admin-PCClient's Server Computer Name
                connectionString = "data source=" + serverName + ";initial catalog=" +
                                   Singleton.SqlceFileName +
                                   ";user id=sa;password=amihan";
            }
            else if (Singleton.Edition == PinnaFaceEdition.CompactEdition)
            {
                connectionString = "Data Source=" + Singleton.SqlceFileName + ";" +
                                   "Max Database Size=4091;Password=1fac3P@ssw0rd";
            }
            else if (Singleton.Edition == PinnaFaceEdition.WebEdition)
            {

            }
            return connectionString;
        }

        public static string GetActivationConnectionString()
        {
            if (Singleton.BuildType == BuildType.LocalDev)
            {
                const string serverName = ".";
                const string dbName = "PinnaKeysDb3";
                const string userId = "sa";
                const string pwd = "amihan";
                const string connectionString = @"Data Source=" + serverName + ";Initial Catalog=" + dbName + ";" +
                                                "User ID=" + userId + ";password=" + pwd + ";" +
                                                "Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
                
                return connectionString;
            }
            else
            {
                const string serverName = "198.38.83.33";
                const string dbName = "munahan_pinnakeys";
                const string userId = "munahan_sa";
                const string pwd = "@rmsd3v";
                const string connectionString = @"Data Source=" + serverName + ";Initial Catalog=" + dbName + ";" +
                                                "User ID=" + userId + ";password=" + pwd + ";" +
                                                "Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
                
                return connectionString;
            }
        }

        public static string GetWebConnectionString()
        {
            if (Singleton.BuildType == BuildType.LocalDev)
            {
                string serverName = ".";
                string dbName = "PinnaFaceDbWebTest";
                string userId = "sa";
                string pwd = "amihan";

                string connectionString = @"Data Source=" + serverName + ";Initial Catalog=" + dbName + ";" +
                                                "User ID=" + userId + ";password=" + pwd + ";" +
                                                "Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
                
                return connectionString;
            }
            else if (Singleton.BuildType == BuildType.Dev)
            {
                string serverName = "198.38.83.33";
                string dbName = "munahan_pfdbtest1";//"munahan_pfdbprod1";//"munahan_pinnafaceweb";//
                string userId = "munahan_pfdbprod1sa";//"munahan_sa";//
                string pwd = "@rmsd3v";//same

                string connectionString = @"Data Source=" + serverName + ";Initial Catalog=" + dbName + ";" +
                                                "User ID=" + userId + ";password=" + pwd + ";" +
                                                "Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
                
                return connectionString;
            }
            else
            {
                string serverName = "198.38.83.33";
                string dbName = "munahan_pfproductiondb";//"munahan_pfdbtest2";//"munahan_pfdbtest1";//"munahan_pinnafaceweb";//
                string userId = "munahan_pfdbprod1sa";//"munahan_sa";//
                string pwd = "@rmsd3v";//same

                string connectionString = @"Data Source=" + serverName + ";Initial Catalog=" + dbName + ";" +
                                                "User ID=" + userId + ";password=" + pwd + ";" +
                                                "Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
                
                return connectionString;
            }
        }

        public static string GetBackUpUserNameAndPassword()
        {
            return "PinnaServer_sa_amihan";
        }

        public static NetworkCredential GetNetworkCredential()
        {
            return new NetworkCredential("munahan1", "hani1212");
        }

        public static IList<CommandModel> QueryCommand(string query)
        {
            DateTime dt = GetCurrentSqlDate(true);
            DateTime loc = dt.ToLocalTime();

            return Singleton.Edition == PinnaFaceEdition.CompactEdition
                ? QueryCommandSqlCe(query)
                : QueryCommandSql(query);
        }

        public static IList<CommandModel> QueryCommandSql(string query)
        {
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            var commandModel = new List<CommandModel>();

            try
            {
                string sQlServConString = GetConnectionString();

                sqlConnection = new SqlConnection(sQlServConString);
                sqlCommand = new SqlCommand(query, sqlConnection);


                if (sqlConnection.State != ConnectionState.Open)
                    sqlConnection.Open();


                SqlDataReader row = sqlCommand.ExecuteReader();

                while (row.Read())
                {
                    commandModel.Add(new CommandModel
                    {
                        Id = DbUtil.GetIntegerValue(ref row, "Id")
                    });
                }
            }
            catch (Exception ex)
            {
                //_exceptions = ex;
                return null;
            }
            finally
            {
                if (null != sqlCommand)
                    sqlCommand.Dispose();
                if (null != sqlConnection)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return commandModel;
        }

        public static IList<CommandModel> QueryCommandSqlCe(string query)
        {
            SqlCeConnection sqlCeConnection = null;
            SqlCeCommand sqlCeCommand = null;

            var commandModel = new List<CommandModel>();

            try
            {
                string sqlCeConString = GetConnectionString();

                //const string query = "select * from ProductActivation";

                sqlCeConnection = new SqlCeConnection(sqlCeConString);
                sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);


                if (sqlCeConnection.State != ConnectionState.Open)
                    sqlCeConnection.Open();


                SqlCeDataReader row = sqlCeCommand.ExecuteReader();

                while (row.Read())
                {
                    commandModel.Add(new CommandModel
                    {
                        Id = DbUtil.GetIntegerValueCe(ref row, "Id")
                    });
                }
            }
            catch (Exception ex)
            {
                //_exceptions = ex;
                return null;
            }
            finally
            {
                if (null != sqlCeCommand)
                    sqlCeCommand.Dispose();
                if (null != sqlCeConnection)
                {
                    sqlCeConnection.Close();
                    sqlCeConnection.Dispose();
                }
            }
            return commandModel;
        }

        public static ActivationModel ValidateProductSql()
        {
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            ActivationModel activationModel = null;

            try
            {
                string sQlServConString = GetConnectionString();
                const string query = "select * from ProductActivation";

                sqlConnection = new SqlConnection(sQlServConString);
                sqlCommand = new SqlCommand(query, sqlConnection);


                if (sqlConnection.State != ConnectionState.Open)
                    sqlConnection.Open();


                SqlDataReader row = sqlCommand.ExecuteReader();

                if (row.Read())
                {
                    activationModel = new ActivationModel
                    {
                        DatabaseVersionDate = DbUtil.GetIntegerValue(ref row, "DatabaseVersionDate"),
                        MaximumSystemVersion = DbUtil.GetIntegerValue(ref row,"MaximumSystemVersion")
                    };
                }
            }
            catch (Exception ex)
            {
                //_exceptions = ex;
                return null;
            }
            finally
            {
                if (null != sqlCommand)
                    sqlCommand.Dispose();
                if (null != sqlConnection)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return activationModel;
        }

        public static ActivationModel ValidateProductSqlCe()
        {
            SqlCeConnection sqlCeConnection = null;
            SqlCeCommand sqlCeCommand = null;

            ActivationModel activationModel = null; 

            try
            {
                string sqlCeConString = GetConnectionString();

                const string query = "select * from ProductActivation";

                sqlCeConnection = new SqlCeConnection(sqlCeConString);
                sqlCeCommand = new SqlCeCommand(query, sqlCeConnection);


                if (sqlCeConnection.State != ConnectionState.Open)
                    sqlCeConnection.Open();


                SqlCeDataReader row = sqlCeCommand.ExecuteReader();

                if (row.Read())
                {
                    activationModel = new ActivationModel
                    {
                        DatabaseVersionDate = DbUtil.GetIntegerValueCe(ref row, "DatabaseVersionDate"),
                        MaximumSystemVersion = DbUtil.GetIntegerValueCe(ref row, "MaximumSystemVersion")
                    };
                }
            }
            catch (Exception ex)
            {
                //_exceptions = ex;
                return null;
            }
            finally
            {
                if (null != sqlCeCommand)
                    sqlCeCommand.Dispose();
                if (null != sqlCeConnection)
                {
                    sqlCeConnection.Close();
                    sqlCeConnection.Dispose();
                }
            }
            return activationModel;
        }

        public static DateTime GetCurrentSqlDate(bool getUtc)
        {
            //getUtc = true;
            string sQlServConString = GetActivationConnectionString();

            var db = new DbUtil(sQlServConString);
            DateTime sqlDate = DateTime.UtcNow; // Set the default to the server time
            //Now try and get the actual sql server time.
            try
            {
                db.Command.CommandText = getUtc ? "pinna_GetUTCDate" : "pinna_GetDate";

                SqlDataReader row = db.Command.ExecuteReader();

                if (row.Read())
                {
                    sqlDate = getUtc
                        ? DbUtil.GetDateTimeValue(ref row, "SQLUTCDateTime")
                        : DbUtil.GetDateTimeValue(ref row, "SQLDateTime");
                }

                row.Close();
            }
            catch
            {
            }

            finally
            {
                db.Dispose();
            }
            return sqlDate;
        }

        public static string GetCurrentDbVersion()
        {
            //getUtc = true;
            string sQlServConString = GetActivationConnectionString();

            var db = new DbUtil(sQlServConString);
            var currentDbVersion = "";
            //DateTime sqlDate = DateTime.UtcNow; // Set the default to the server time
            //Now try and get the actual sql server time.
            try
            {
                db.Command.CommandText = "pinna_GetDataBaseVersionDate";

                SqlDataReader row = db.Command.ExecuteReader();

                if (row.Read())
                {
                    currentDbVersion = DbUtil.GetStringValue(ref row, "DataBaseVersion");
                }

                row.Close();
            }
            catch
            {
            }

            finally
            {
                db.Dispose();
            }
            return currentDbVersion;
        }

        public static int GetCurrentDatabaseVersion()
        {
            return 20180326;
            //return 20180325;
            //return 20180220;
            //return 20180217;
            //return 20180213;
            //return 20180101;
        }

        // <summary>Cleanup resources</summary>
        //private void CleanUp()
        //{
        //    if (null != _objCommand)
        //        _objCommand.Dispose();
        //    if (null != _objConnection)
        //    {
        //        _objConnection.Close();
        //        _objConnection.Dispose();
        //    }
        //}

        //#region Dispose

        ///// <summary>Clean up.</summary>
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        ///// <summary>Private dispose method.</summary>
        //private void Dispose(bool disposing)
        //{
        //    if (!this._disposed)
        //    {
        //        if (disposing)
        //        {
        //            if (null != Row && !Row.IsClosed)
        //                Row.Close();

        //            CleanUp();

        //        }
        //    }
        //    _disposed = true;
        //}

        //#endregion
    }
}