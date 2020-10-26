using System.Data.Entity.Infrastructure;
using PinnaFace.Core;

namespace PinnaFace.DAL
{
    public class ServerDbContextFactory : IDbContextFactory<PinnaFaceServerDBContext>
    {
        public PinnaFaceServerDBContext Create()
        {
            #region For Debug
            string sQlServConString = DbCommandUtil.GetWebConnectionString();
            Singleton.ConnectionStringName = sQlServConString;
            Singleton.ProviderName = "System.Data.SqlClient";
            var sql = new SqlConnectionFactory(sQlServConString);
            return new PinnaFaceServerDBContext(sql.CreateConnection(sQlServConString), true);
            #endregion

            #region For Release
            //const string serverIp = "198.38.83.33";
            //const string serverInitialCatalog = "ibrahim11_amstock1";
            //var sQlServerConString = "Data Source=" + serverIp + ";Initial Catalog=" + serverInitialCatalog + ";"+
            //                          "User ID=ibrahim11_armsdev;Password=@rmsd3v;"+
            //                          "encrypt=true;trustServerCertificate=true";

            //var sql = new SqlConnectionFactory(sQlServerConString);
            //return new PinnaFaceServerDBContext(sql.CreateConnection(sQlServerConString), true); 
            #endregion
        }
    }
}