using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Batch
    {
        public int AbsEntryBatch { get; set; }
        public string DistNumber { get; set; }
        public string BinCode { get; set; }
        public int AbsEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string WhsCode { get; set; }
        public int Status { get; set; }
    }
}
