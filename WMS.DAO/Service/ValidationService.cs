using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class ValidationService
    {
        public ValidationService()
        {
            
        }
        public bool validSameStatusBatch(IDbConnection dbConnection, string schema, int status, string batch) {
            string query = $"SELECT * FROM {schema.Replace("CALL","")}.\"ValidStatusDistNumber\"('{batch}',{status});";
            var statuBatch = dbConnection.Query<bool>($"{query}").First();
            return statuBatch;
        }
    }
}
