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
using WMS.Token;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class AuthenticateService : IAuthenticateService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema = string.Empty;
        Encrypt utilityEncrypt;
        public AuthenticateService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
            utilityEncrypt = new Encrypt();
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest credential)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            string token = string.Empty;
            try
            {
                var user = connection.Query<User>($"{schema}.\"WMS_Login\"( user_ => '{credential.Username}', password_ => '{credential.Password}'); ").FirstOrDefault();
                if (user == null) return null;

                string passDecrypt = utilityEncrypt.DescryPassword(user.Password);
                if (credential.Password == passDecrypt)
                {
                    token = TokenGenerator.GenerateTokenJwt(user);
                    return new AuthenticateResponse(user, token);
                }
                else {
                    return null;
                }
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
