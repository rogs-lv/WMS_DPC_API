using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class QualityService : ValidationService, IQualityService, IReadCodebars
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;

        public QualityService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }

        public List<Warehouse> WarehouseQuality(string warehouse)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listWhsQuality = connection.Query<Warehouse>($"{schema}.\"WMS_WhsQuality\"( warehouse => '{warehouse}');").ToList();
                return listWhsQuality;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Warehouse>();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public bool CheckSameStatus(string codebars, int status) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                return validSameStatusBatch(connection, schema, status, codebars);                
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public List<Batch> ReadCode(string codebars, string warehouse, int status)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                List<Batch> listBatchs = connection.Query<Batch>($"{schema}.\"WMS_BatcNumber\"(batch=>'{codebars}', warehouse=> '{warehouse}');").ToList();
                return listBatchs;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Batch>();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public DefaultLocationWhs DefaulLocationWarehouse(string warehouse)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                DefaultLocationWhs defaultLocation = connection.Query<DefaultLocationWhs>($"{schema}.\"WMS_BinDeftWarehouse\"(warehouse=>'{warehouse}');").FirstOrDefault();
                if (defaultLocation == null)
                    return new DefaultLocationWhs();
                else
                    return defaultLocation;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new DefaultLocationWhs();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
