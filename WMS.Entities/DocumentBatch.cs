using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class DocumentBatch
    {
        public int BaseLine { get; set; }
        public int LineNumBatch { get; set; }
        public string Batch { get; set; }
        public decimal Quantity { get; set; }
    }
}
