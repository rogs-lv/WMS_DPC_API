using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WMS.DAO.IService;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class InventoryService : IInventoryService, IReadCodebars
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public InventoryService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public int NumbersOfRecount(string warehouseUser)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var numbers = connection.Query<int>($"{schema}.\"WMS_NumbersOfRecount\"( warehouse => '{warehouseUser}');").First();
                return numbers;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return 1;
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

        public WarehouseLocation LocationsWhsInventory(string warehouseCount)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var location = connection.Query<WarehouseLocation>($"{schema}.\"WMS_LocationWhsInventory\"( warehouse => '{warehouseCount}');").First();
                return location;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new WarehouseLocation();
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
        
        public List<BatchsInLocation> SeeLocation(string location, string warehouseInventory) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listLocation = connection.Query<BatchsInLocation>($"{schema}.\"WMS_BatchsInLocation\"(location => '{location}', warehouse => '{warehouseInventory}');").ToList();
                return listLocation;
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
        
        public List<Warehouse> WarehouseInventory(string warehouse, string typeQuery)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listWarehouse = connection.Query<Warehouse>($"{schema}.\"WMS_WarehouseInventory\"(warehouse => '{warehouse}', typeQuery => '{typeQuery}');").ToList();
                return listWarehouse;
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
        
        public Location AbsEntryFromBinCode(string warehouse, string bincode, string type) {
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
        
        public bool AccessQuantitySAP(string userId)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var status = connection.Query<bool>($"{schema}.\"WMS_QuantitySAP\"(userId => '{userId}');").FirstOrDefault();
                return status;
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

        public bool CheckInventoryFile(string warehouse, string user, string file)
        {
            List<string> basePath = PathFile();
            try
            {
                string path = @"C:\" + basePath[1] + @"\" + warehouse + @"\" + user + @"\" + file;
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }

        public Response<bool> WriteInventoryFile(string warehouse, string user, string fileName, string[] stringValues)
        {            
            try
            {
                List<string> basePath = PathFile();
                string raiz = @"C:\" + basePath[1] + @"\", dirAlmacen = string.Empty, dirUser = string.Empty, finalRoute = string.Empty;
                DirectoryInfo dirWarehouse;

                //Create RecuentoInventario if not exist
                if (!Directory.Exists(raiz)) 
                    return new Response<bool>(false, -1, $"No se encontró ningún folder con el nombre: {basePath[1]}");
                
                //Create folder of warehous if not exist
                if (!Directory.Exists(raiz + warehouse)) {
                    dirWarehouse = Directory.CreateDirectory(raiz + warehouse);
                    dirUser = raiz + warehouse + @"\";
                }
                else 
                    dirUser = raiz + warehouse + @"\";

                //Create folder of user if not exist
                if (!Directory.Exists(dirUser + user)) {
                    Directory.CreateDirectory(dirUser + user);
                    finalRoute = dirUser + user + @"\" + fileName;
                }
                else
                    finalRoute = dirUser + user + @"\" + fileName;

                if (File.Exists(finalRoute)) {//if exist, delete
                    File.Delete(finalRoute);
                }
                if (!File.Exists(finalRoute)) {
                    File.WriteAllLines(finalRoute, stringValues);
                }
                return new Response<bool>(true, 0, "Se generó correctamente el archivo.");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<bool>(false, -1, ex.Message);
            }
        }

        private List<string> PathFile()
        {
            List<string> subPaths = new List<string>();
            try
            {
                string path = "", regexPattern = @"\\\\", replaceBy = "";
                path = ConfigurationManager.AppSettings["PathFolio"];
                if (!string.IsNullOrEmpty(path)) {
                    string result = Regex.Replace(path, regexPattern, replaceBy);
                    int lastSubsStr = result.LastIndexOf("\\");
                    if (lastSubsStr == -1) {
                        string[] routes = result.Split('\\');
                        foreach (string route in routes)
                        {
                            subPaths.Add(route);
                        }
                        return subPaths;
                    }

                    string finalResult = result.Remove(lastSubsStr, "\\".Length).Insert(lastSubsStr, replaceBy);
                    string[] words = finalResult.Split('\\');
                    foreach (string word in words)
                    {
                        subPaths.Add(word);
                    }
                }
                return subPaths;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<string>();
            }
        }
    }
}
