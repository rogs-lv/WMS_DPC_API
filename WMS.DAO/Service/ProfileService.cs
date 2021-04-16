using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.DAO.Utility;
using WMS.Entities;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class ProfileService : IProfileService
    {
        IDBAdapter dBAdapter;
        Log lg;
        Encrypt encry;
        private readonly string schema;
        public ProfileService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
            encry = new Encrypt();
        }
        public List<Profile> GetProfiles()
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listProfiles = connection.Query<Profile>($"{schema}.\"WMS_CreateProfiles_Modules\"();").ToList();
                return listProfiles;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Profile>();
            }
        }

        public Response<string> CreateNewProfile<T>(Profile profile, List<ModuleResponse> modules, List<AdditionalSettings> configurations)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (ValidateIdUser(profile.IdUser))
                {
                    return new Response<string>("", -1, "El Id de usuario ya existe");
                }

                string passEncrypt = encry.EncryptPassword(profile.Password);
                int result = connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{2}, idUser=>'{profile.IdUser}', nameUser=>'{profile.NameUser}', firstName=>'', lastName=>'', status=>'{(profile.Status ? 'A' : 'I')}', pass=>'{passEncrypt}', warehouse=>'{profile.WhsCode}');");
                if (result > 0)
                {
                    foreach (ModuleResponse mod in modules)
                    {
                        int resultModulos = connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{3}, idUser=>'{profile.IdUser}', idModule=>'{mod.IdModule}');");
                    }
                    foreach (AdditionalSettings sett in configurations) { 
                        int additionalConfRow = connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{4}, idUser=>'{profile.IdUser}', idConfig=>'{sett.IdConfig}');");
                    }
                }
                return new Response<string>($"Usuario {profile.IdUser} creado", 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<string>($"", -1, ex.Message);
            }
        }

        public bool UpdateProfile(Profile profile, List<ModuleResponse> modules, AdditionalSettings configurations)
        {
            throw new NotImplementedException();
        }
        
        public List<ModuleUser> GetModulesUser(string userId)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var module = connection.Query<ModuleUser>($"{schema}.\"WMS_AdminModule\"( user_ => '{userId}');").ToList();
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

        public List<Warehouse> GetWarehouse()
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var listWarehouse = connection.Query<Warehouse>($"{schema}.\"WMS_Warehouse\"();").ToList();
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

        private bool ValidateIdUser(string leadIdUser)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var validId = connection.Query<bool>($"{schema}.\"WMS_ValidIduser\"(leadId=>{leadIdUser});").First();
                return validId;
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
    }
}
