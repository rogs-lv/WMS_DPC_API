using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.Entities;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class ConfigurationService : IConfigurationService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        //private readonly string filterModule;
        public ConfigurationService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
            //filterModule = "TRAN";
        }
        public List<ModulesHome> GetModulesUser(string userId)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            List<ModuleResponse> listModules = new List<ModuleResponse>();
            try
            {
                var module = connection.Query<ModulesHome>($"{schema}.\"WMS_Module\"( user_ => '{userId}');").ToList();
                return module;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
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
        public List<AdditionalSettings> GetAdditionalSettings(string userId)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var aditionalModule = connection.Query<AdditionalSettings>($"{schema}.\"WMS_ConfigurationAditional\"( user_ => '{userId}');").ToList();
                return aditionalModule;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
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
        public ModulesHome VerifyPath(string userId, string path) {
            IDbConnection connection = dBAdapter.GetConnection();
            List<ModuleResponse> listModules = new List<ModuleResponse>();
            try
            {
                var module = connection.Query<ModulesHome>($"{schema}.\"WMS_VerifyPath\"(path => '{path}', user_ => '{userId}');").FirstOrDefault();
                return module;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
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
        public int Serie(string warehouse) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var numberSerie = connection.Query<int>($"{schema}.\"WMS_Series\"(warehouse => '{warehouse}');").First();
                return numberSerie;
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
