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
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class LoadUserService : ILoadUserService
    {
        IDBAdapter dBAdapter;
        Log lg;
        Encrypt encry;
        private readonly string schema;
        public LoadUserService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
            encry = new Encrypt();
        }
        public Response<int> SaveUsers(List<LoadUser> data)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            int rowsInserts = 0;
            try
            {
                foreach (LoadUser row in data) {
                    string passEncrypt = encry.EncryptPassword(row.U_PassWMS.Trim());
                    rowsInserts += connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{2}, idUser=>'{row.jobTitle.Trim()}', nameUser=>'{row.firstName.Trim()}', firstName=>'{row.firstName.Trim()}', lastName=>'{row.lastName.Trim()}', status=>'{row.Status.Trim()}', pass=>'{passEncrypt}', warehouse=>'{row.WhsCode.Trim()}');");
                }
                return new Response<int>(rowsInserts, 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<int>(-1, -1, ex.Message); ;
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
        /// <summary>
        /// Insert modules and sub modules
        /// </summary>
        /// <param name="data">rows or data of query</param>
        /// <param name="option">3 for insert module and submodules</param>
        /// <returns></returns>
        public Response<int> SaveModule(List<LoadModule> data, int option)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            int rowsInserts = 0;
            try
            {
                foreach (LoadModule row in data)
                {
                    rowsInserts += connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{option}, idUser=>'{row.jobTitle.Trim()}', idModule=>'{row.IdModule.Trim()}', statusMod=>'{row.StatusModule.Trim()}');");
                }
                return new Response<int>(rowsInserts, 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<int>(-1, -1, ex.Message); ;
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
        /// <summary>
        /// Insert additional settings
        /// </summary>
        /// <param name="data">rows or data of query</param>
        /// <param name="option">option 4 to insert additional setting</param>
        /// <returns></returns>
        public Response<int> SaveOtherConfiguration(List<LoadModule> data, int option)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            int rowsInserts = 0;
            try
            {
                foreach (LoadModule row in data)
                {
                    rowsInserts += connection.Execute($"{schema}.\"WMS_CreateProfiles_Modules\"(opt=>{option}, idUser=>'{row.jobTitle.Trim()}', idConfig=>'{row.IdModule.Trim()}', statusConfig=>'{row.StatusModule.Trim()}');");
                }
                return new Response<int>(rowsInserts, 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<int>(-1, -1, ex.Message); ;
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
