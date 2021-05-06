using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Movement
    {
        public string ItemCode { get; set; }
        public string From_ { get; set; }
        public string To_ { get; set; }
        public string Batch { get; set; }
        public string WhsBatch { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public decimal QToLocation { get; set; }
        public string IdGUID { get; set; }
    }
    public class ManualToWhsCode {
        public string toWhsCode { get; set; }
        public string toBinLocation { get; set; }
    }
}
