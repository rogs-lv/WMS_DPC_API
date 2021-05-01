using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface ITransferService
    {
        List<AvailableLocation> AvailableLocations(string warehouseUser);
        List<BatchsInLocation> ViewLocation(string location, string warehouseTransfer);
        Location AbsEntryFromBinCode(string warehouse, string bincode, string type = "");
    }
}
