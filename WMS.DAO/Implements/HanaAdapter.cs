using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Utilities;

namespace WMS.DAO.Implements
{
    public class HanaAdapter : IDBAdapter
    {
        Log lg;
        public HanaAdapter() {
            lg = Log.getIntance();
        }
        public IDbConnection GetConnection()
        {
            try
            {
                string connectionString = CreateConnectionString();
                HanaConnection connection = new HanaConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
            }
        }

        private string CreateConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HanaServer"].ConnectionString;
            return connectionString;
        }
    }
}
