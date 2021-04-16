using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class User
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public string FirstName { get; set; }
        public string WhsCode { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
