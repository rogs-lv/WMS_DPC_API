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
        private readonly string filterModule;
        public ConfigurationService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
            filterModule = "TRAN";
        }
        public List<ModuleResponse> GetModulesUser(string userId)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            List<ModuleResponse> listModules = new List<ModuleResponse>();
            try
            {
                var module = connection.Query<ModuleUser>($"{schema}.\"WMS_Module\"( user_ => '{userId}');").ToList();
                var moduleLevel1 = module.Where(x => x.Principal == "").ToList();
                foreach (ModuleUser m in moduleLevel1)
                {
                    listModules.Add(new ModuleResponse
                    {
                        IdModule = m.IdModule,
                        Path = m.Path,
                        Icon = m.Icon,
                        Title = m.Title,
                        Status = m.Status
                    });
                }
                var moduleLevel2 = module.Where(z => z.Principal != "" && z.Icon == "");
                var findPrincipal = listModules.Find(p => p.IdModule == filterModule);
                if (findPrincipal != null)
                {
                    foreach (var sub in moduleLevel2)
                    {
                        var element = listModules.Find(f => f.IdModule == sub.Principal);
                        element.Submodules.Add(new SubmoduleResponse
                        {
                            Icon = sub.Icon,
                            Path = sub.Path,
                            Title = sub.Title
                        });
                    }
                }
                return listModules;
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
    }
}
