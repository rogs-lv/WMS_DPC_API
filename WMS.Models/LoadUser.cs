using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class LoadUser
    {
        public string jobTitle { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Status { get; set; }
        public string U_PassWMS { get; set; }
        public string WhsCode { get; set; }
    }

    public class LoadModule
    {
        public string jobTitle { get; set; }
        public string IdModule { get; set; }
        public string StatusModule { get; set; }

    }
}
