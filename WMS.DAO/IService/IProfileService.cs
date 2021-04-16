using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IProfileService
    {
        List<Profile> GetProfiles();
        Response<string> CreateNewProfile<T>(Profile profile, List<ModuleResponse> modules, List<AdditionalSettings> configurations);
        Response<string> UpdateProfile<T>(Profile profile, List<ModuleResponse> modules, List<AdditionalSettings> configurations);
        List<ModuleUser> GetModulesUser(string userId);
        List<Warehouse> GetWarehouse();
    }
}
