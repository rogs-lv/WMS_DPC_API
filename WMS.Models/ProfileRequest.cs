using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;

namespace WMS.Models
{
    public class ProfileRequest
    {
        public Profile UserProfile { get; set; }
        public List<ModuleResponse> UserModules { get; set; }
        public List<AdditionalSettings> UserAdditionalSettings { get; set; }
    }
}
