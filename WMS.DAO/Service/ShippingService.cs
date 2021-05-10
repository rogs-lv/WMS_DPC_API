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
    public class ShippingService : IShippingService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public ShippingService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public Response<List<Partner>> GetListPartner()
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listPartner = connection.Query<Partner>($"{schema}.\"WMS_GetBusinessPartner\"();").ToList();
                return new Response<List<Partner>>(listPartner, 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<List<Partner>>(null, -1, ex.Message);
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
        public List<Batch> ReadCode(string codebars, string warehouse)
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
    }
}
