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
    public class ShipmentService : IShipmentService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public ShipmentService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public List<Shipment> GetNumberShipment(int docnum)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                List<Shipment> listBatchs = connection.Query<Shipment>($"{schema}.\"WMS_GetRequest\"(DocNum=>{docnum});").ToList();
                return listBatchs;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Shipment>();
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
            throw new NotImplementedException();
        }

        public bool ApplyGR(string batch)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                bool apply = connection.Query<bool>($"{schema}.\"WMS_ApplyGR\"(batch=>'{batch}');").First();
                return apply;
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

        public ValidateEFEEM isValidEFEEM(string batch, int type)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var isValid = connection.Query<ValidateEFEEM>($"{schema}.\"WMS_ValidateEFEEM\"(batch=>'{batch}', event=>{type});").FirstOrDefault();
                return isValid;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new ValidateEFEEM();
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

        public int UpdateBatchs(string[] batchs, string status, int docnum, int docentry)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            string stringBatch = string.Empty;
            int updateBatch = 0;
            try
            {
                if (batchs.Length > 0)
                {
                    for (int a = 0; a < batchs.Length; a++)
                    {
                        if (stringBatch == string.Empty)
                            stringBatch = $"'{batchs[a]}'";
                        else
                            stringBatch += $",'{batchs[a]}'";
                    }
                    if (stringBatch != string.Empty)
                    {
                        updateBatch = connection.Execute($"UPDATE {schema.Replace("CALL","")}.\"OBTN\" SET \"U_stFolio\" = '{status}', \"U_Remision\" = {docnum} WHERE \"DistNumber\" IN({stringBatch});");
                    }
                }                        
                return updateBatch;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return 0;
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
