using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class DocumentLocation
    {
        public int BaseLine { get; set; }
        public int BaseLineBatch { get; set; }
        public int FromLocation { get; set; }
        public int ToLocation { get; set; }
        public decimal Quantity { get; set; }  
    }
}
