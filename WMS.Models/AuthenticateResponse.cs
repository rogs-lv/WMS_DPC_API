using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;

namespace WMS.Models
{
    public class AuthenticateResponse
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public string FirstName { get; set; }
        public string WhsCode { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            IdUser = user.IdUser;
            NameUser = user.NameUser;
            FirstName = user.FirstName;
            WhsCode = user.WhsCode;
            Token = token;
        }
    }
}
