using System.Data.Entity.Infrastructure;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.DAL
{
    public class DbContextFactory : IDbContextFactory<PinnaFaceDbContext>
    {
        public PinnaFaceDbContext Create()
        {
            switch (Singleton.Edition)
            {
                case PinnaFaceEdition.CompactEdition:

                    var sqlCeConString = DbCommandUtil.GetConnectionString();

                    Singleton.ConnectionStringName = sqlCeConString;
                    Singleton.ProviderName = "System.Data.SqlServerCe.4.0";
                    var sqlce = new SqlCeConnectionFactory(Singleton.ProviderName);
                    return new PinnaFaceDbContext(sqlce.CreateConnection(sqlCeConString), true);

                case PinnaFaceEdition.ServerEdition:

                    var sQlServConString = DbCommandUtil.GetConnectionString();

                    Singleton.ConnectionStringName = sQlServConString;
                    Singleton.ProviderName = "System.Data.SqlClient";
                    var sql = new SqlConnectionFactory(sQlServConString);
                    return new PinnaFaceDbContext(sql.CreateConnection(sQlServConString), true);
            }
            return null;
        }
    }
}