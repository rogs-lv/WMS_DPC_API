using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class DocumentLine
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Quantity { get; set; }
        public string Batch { get; set; }
    }
}
