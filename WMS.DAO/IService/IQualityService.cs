using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IQualityService
    {
        List<Warehouse> WarehouseQuality(string warehouse);
        bool CheckSameStatus(string codebars, int status);
    }
}
