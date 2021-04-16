using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Entities
{
    public class ModuleUser
    {
        public string IdModule { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Principal { get; set; }
        public bool Status { get; set; }
    }
}
