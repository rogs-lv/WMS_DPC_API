using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Partner
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int U_GrpItems { get; set; }
    }
    public class PartnerADSL
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int ListNum { get; set; }
        public string ListName { get; set; }
    }
}
