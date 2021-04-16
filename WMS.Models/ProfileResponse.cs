using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;

namespace WMS.Models
{
    public class ProfileResponse
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public bool Status { get; set; }
        public string WhsCode { get; set; }
        public ProfileResponse(Profile profile)
        {
            IdUser = profile.IdUser;
            NameUser = profile.NameUser;
            //FirstName = profile.FirstName;
            //LastName = profile.LastName;
            Status = profile.Status;
            WhsCode = profile.WhsCode;
        }
    }
}
