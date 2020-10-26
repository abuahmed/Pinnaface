using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json;


namespace PinnaFace.Web.Models
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success
        {
            get;
            set;
        }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage
        {
            get;
            set;
        }
    }  
    //public class DbUtil : IDisposable
    //{
    //    #region Fields
    //    private SqlConnection _objConnection;
    //    private SqlCommand _objCommand;
    //    private bool _disposed;
    //    internal NumberFormatInfo NumberProvider;
    //    public SqlDataReader Row = null; 
    //    #endregion
        
    //    #region Properties
    //    public SqlCommand Command
    //    {
    //        get
    //        {
    //            return _objCommand;
    //        }
    //    } 
    //    #endregion
        
    //    #region Constructors
        
        
    //    public DbUtil(string connectionString)
    //    {
    //        int timeout = 30;
    //        try
    //        {
    //            InitializeMaster(true, connectionString,timeout);
    //            _objCommand.CommandType = CommandType.StoredProcedure;
    //        }
    //        catch (Exception) { }
            
    //    }

    //    /// <summary>Cleanup resources</summary>
    //    private void CleanUp()
    //    {
    //        if (null != _objCommand)
    //            _objCommand.Dispose();
    //        if (null != _objConnection)
    //        {
    //            _objConnection.Close();
    //            _objConnection.Dispose();
    //        }
    //    }
    //    #endregion
        

    //    #region Methods
    //    public void InitializeMaster(bool useDatabase, string connectionString, int connectionTimeout)
    //    {
    //        SetupProviders();

    //        if (useDatabase)
    //        {
    //            if (null == _objConnection)
    //                _objConnection = new SqlConnection(connectionString);

    //            if (null == _objCommand)
    //                _objCommand = new SqlCommand("", _objConnection);

    //            _objCommand.CommandTimeout = Convert.ToInt32(connectionTimeout, NumberProvider);
    //            if (_objConnection.State != ConnectionState.Open)
    //                _objConnection.Open();
    //        }
    //    }

    //    private void SetupProviders()
    //    {
    //        //Initalize the number provider
    //        NumberProvider = new NumberFormatInfo();
    //        // These properties affect the conversion.
    //        NumberProvider.NegativeSign = "neg ";
    //        NumberProvider.PositiveSign = "pos ";

    //        // These properties do not affect the conversion.
    //        // The input string cannot have decimal and group separators.
    //        NumberProvider.NumberDecimalSeparator = ".";
    //        NumberProvider.NumberGroupSeparator = ",";
    //        NumberProvider.NumberGroupSizes = new int[] { 3 };
    //        NumberProvider.NumberNegativePattern = 0;
    //    }

    //    #region Transaction Methods

    //    /// <summary>Begin transaction.</summary>
    //    public void BeginTransaction()
    //    {
    //        if (_objConnection.State == System.Data.ConnectionState.Closed)
    //        {
    //            _objConnection.Open();
    //        }
    //        _objCommand.Transaction = _objConnection.BeginTransaction();
    //    }

    //    /// <summary>Commit transaction.</summary>
    //    public void CommitTransaction()
    //    {
    //        _objCommand.Transaction.Commit();
    //        _objConnection.Close();
    //    }

    //    /// <summary>Rollback transaction.</summary>
    //    public void RollbackTransaction()
    //    {
    //        _objCommand.Transaction.Rollback();
    //        _objConnection.Close();
    //    }

    //    #endregion

    //    #region ExecuteNonQuery

    //    /// <summary>ExecuteNonQuery</summary>
    //    public int ExecuteNonQuery(string query)
    //    {
    //        return ExecuteNonQuery(query, CommandType.StoredProcedure, false);
    //    }

    //    /// <summary>ExecuteNonQuery</summary>
    //    public int ExecuteNonQuery(string query, CommandType cmdType)
    //    {
    //        return ExecuteNonQuery(query, cmdType, false);
    //    }

    //    /// <summary>ExecuteNonQuery</summary>
    //    public int ExecuteNonQuery(string query, bool keepAlive)
    //    {
    //        return ExecuteNonQuery(query, CommandType.StoredProcedure, keepAlive);
    //    }

    //    /// <summary>ExecuteNonQuery</summary>
    //    public int ExecuteNonQuery(string query, CommandType cmdType, bool keepAlive)
    //    {
    //        _objCommand.CommandText = query;
    //        _objCommand.CommandType = cmdType;

    //        //if (ParameterExists(objCommand, "AuthorizationId"))
    //        _objCommand.Parameters.AddWithValue("@AuthorizationId", "this.AuthorizationId.ToString()");

    //        int i = -1;
    //        try
    //        {
    //            if (_objConnection.State == System.Data.ConnectionState.Closed)
    //            {
    //                _objConnection.Open();
    //            }
    //            i = _objCommand.ExecuteNonQuery();

    //            //I don't know if I like this throwing an exception
    //            //                if (i < 1)
    //            //                    throw new G6Exception("DBToolsLib:DbUtil:ExecuteNonQuery", DBMessages.DatabaseError_NoRowsAffected);
    //        }
    //        catch 
    //        {
    //            throw;
    //        }
          
    //        finally
    //        {
    //            _objCommand.Parameters.Clear();
    //            if (!keepAlive)
    //                _objConnection.Close();
    //        }
    //        return i;
    //    }

        

    //    #endregion

    //    #region ExecuteScalar

    //    /// <summary>ExecuteScaler</summary>
    //    public object ExecuteScalar(string query)
    //    {
    //        return ExecuteScalar(query, CommandType.StoredProcedure, false);
    //    }

    //    /// <summary>ExecuteScaler</summary>
    //    public object ExecuteScalar(string query, CommandType cmdType)
    //    {
    //        return ExecuteScalar(query, cmdType, false);
    //    }

    //    /// <summary>ExecuteScaler</summary>
    //    public object ExecuteScalar(string query, bool keepAlive)
    //    {
    //        return ExecuteScalar(query, CommandType.StoredProcedure, keepAlive);
    //    }

    //    /// <summary>ExecuteScaler</summary>
    //    public object ExecuteScalar(string query, CommandType cmdType, bool keepAlive)
    //    {
    //        _objCommand.CommandText = query;
    //        _objCommand.CommandType = cmdType;

    //        //if (ParameterExists(objCommand, "AuthorizationId"))
    //        _objCommand.Parameters.AddWithValue("@AuthorizationId", "this.AuthorizationId.ToString()");

    //        object o = null;
    //        try
    //        {
    //            if (_objConnection.State == System.Data.ConnectionState.Closed)
    //                _objConnection.Open();

    //            o = _objCommand.ExecuteScalar();
    //        }
    //        catch 
    //        {
    //            throw;
    //        }
     
    //        finally
    //        {
    //            _objCommand.Parameters.Clear();
    //            if (!keepAlive)
    //            {
    //                _objConnection.Close();
    //            }
    //        }
    //        return o;
    //    }

       

    //    #endregion
        
    //    #region Dispose

    //    /// <summary>Clean up.</summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    /// <summary>Private dispose method.</summary>
    //    private void Dispose(bool disposing)
    //    {
    //        if (!this._disposed)
    //        {
    //            if (disposing)
    //            {
    //                if (null != Row && !Row.IsClosed)
    //                    Row.Close();

    //            CleanUp();
                    
    //            }
    //        }
    //        _disposed = true;
    //    }
       
    //    #endregion

    //    #endregion

    //    #region Public Static DataReader Helpers

    //    public static string GetStringValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToString(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return "";
    //    }

    //    public static string GetStringValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToString(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return "";
    //    }

    //    public static string GetXmlValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToString(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return "";
    //    }

    //    public static string GetXmlValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToString(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return "";
    //    }


    //    public static int GetIntegerValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToInt32(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return -1;
    //    }

    //    public static int GetIntegerValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToInt32(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return -1;
    //    }

    //    public static double GetDoubleValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToDouble(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return -1;
    //    }

    //    public static decimal GetDecimalValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToDecimal(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return Convert.ToDecimal(0);
    //    }

    //    public static long GetLongValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToInt64(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return -1;
    //    }

    //    public static byte[] GetImageValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return (byte[])row[fieldName];
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return null;
    //    }

    //    public static byte[] GetByteArray(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return (byte[])row[fieldName];
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return null;
    //    }

    //    public static double GetDoubleValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToDouble(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return -1;
    //    }

    //    public static decimal GetDecimalValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToDecimal(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return Convert.ToDecimal(0);
    //    }

    //    public static bool GetBooleanValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToBoolean(row[fieldName]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return false;
    //    }

    //    public static bool GetBooleanValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToBoolean(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return false;
    //    }

    //    public static DateTime GetDateTimeValue(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(row.GetOrdinal(fieldName)))
    //                return Convert.ToDateTime(row[fieldName]);
    //            else
    //                return new DateTime(1800, 1, 1);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return new DateTime(1800, 1, 1);
    //    }

    //    public static DateTime GetDateTimeValue(ref SqlDataReader row, int index)
    //    {
    //        try
    //        {
    //            if (!row.IsDBNull(index))
    //                return Convert.ToDateTime(row[index]);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return new DateTime(1800, 1, 1);
    //    }

    //    public static bool FieldExists(ref SqlDataReader row, string fieldName)
    //    {
    //        try
    //        {
    //            return row.GetOrdinal(fieldName) >= 0;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }

    //    #endregion

    //    public bool GetObjectExists()
    //    {
    //        if (null == this.Row)
    //            return false;

    //        if (this.Row.Read())
    //            return DbUtil.GetBooleanValue(ref this.Row, "ObjectExists");
    //        else
    //            return false;
    //    }
    //}
}
