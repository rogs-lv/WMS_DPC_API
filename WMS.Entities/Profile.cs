using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class Profile
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public bool Status { get; set; }
        public string WhsCode { get; set; }
        public string Password { get; set; }
    }
}
