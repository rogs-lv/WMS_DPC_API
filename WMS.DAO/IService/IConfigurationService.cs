using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IConfigurationService
    {
        List<ModulesHome> GetModulesUser(string userId);
        List<AdditionalSettings> GetAdditionalSettings(string userId);
        ModulesHome VerifyPath(string userId, string path);
    }
}
