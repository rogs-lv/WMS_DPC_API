using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class ModuleResponse
    {
        public string IdModule { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool Status { get; set; }
        public List<SubmoduleResponse> Submodules { get; set; }
        public ModuleResponse()
        {
            Submodules = new List<SubmoduleResponse>();
        }
    }
    public class SubmoduleResponse
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
    }
    public class ModulesHome {
        public string IdModule { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Principal { get; set; }
        public bool Status { get; set; }
    }
}
