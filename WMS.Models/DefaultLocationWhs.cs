using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class DefaultLocationWhs
    {
        public int DftBinAbs { get; set; }
        public string BinCode { get; set; }
        public char DftBinEnfd { get; set; }
        public DefaultLocationWhs()
        {
            DftBinAbs = 0;
            BinCode = "";
            DftBinEnfd = 'N';
        }

    }
}
