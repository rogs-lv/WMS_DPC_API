using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class TransferService : IReadCodebars, ITransferService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public TransferService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public List<AvailableLocation> AvailableLocations(string warehouseUser)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var availableLocation = connection.Query<AvailableLocation>($"{schema}.\"WMS_AvailableLocations\"(warehouse=> '{warehouseUser}')").ToList();
                return availableLocation;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<AvailableLocation>();
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
        public List<BatchsInLocation> ViewLocation(string location, string warehouseInventory)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var locations = connection.Query<BatchsInLocation>($"{schema}.\"WMS_BatchsInLocation\"(location => '{location}', warehouse => '{warehouseInventory}');").ToList();
                return locations;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<BatchsInLocation>();
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
        public Location AbsEntryFromBinCode(string warehouse, string bincode, string type)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            Regex regex = new Regex(@"([A-Z]+\-PISO)");
            try
            {
                Match match = regex.Match(bincode);
                Location locationFromBinCode;
                if (match.Success)
                    locationFromBinCode = connection.Query<Location>($"{schema}.\"WMS_AbsEntryFromBinCode\"(warehouse => '{warehouse}', bincode => '', type_ => 'piso');").FirstOrDefault();
                else
                    locationFromBinCode = connection.Query<Location>($"{schema}.\"WMS_AbsEntryFromBinCode\"(warehouse => '{warehouse}', bincode => '{bincode}', type_ => 'nopiso');").FirstOrDefault();
                return locationFromBinCode;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Location();
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
        public List<Batch> ReadCode(string codebars, string warehouse, int status = 0)
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
