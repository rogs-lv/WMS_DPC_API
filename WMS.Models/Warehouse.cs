using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Warehouse
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
    }

    public class WarehouseLocation
    {
        public string DftBinAbs { get; set; }
        public string BinCode { get; set; }
        public string DftBinEnfd { get; set; }
    }

    public class Whs_Binlocation
    {
        public int AbsEntry { get; set; }
        public string BinCode { get; set; }
    }
}
