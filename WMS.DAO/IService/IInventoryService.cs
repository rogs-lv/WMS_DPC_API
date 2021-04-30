using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IInventoryService
    {
        int NumbersOfRecount(string warehouseUser);
        WarehouseLocation LocationsWhsInventory(string warehouseCount);
        List<BatchsInLocation> SeeLocation(string location, string warehouseInventory);
        List<Warehouse> WarehouseInventory(string warehouse, string typeQuery);
        Location AbsEntryFromBinCode(string warehouse, string bincode, string type);
        bool AccessQuantitySAP(string userId);
        bool CheckInventoryFile(string warehouse, string user, string file);
        Response<bool> WriteInventoryFile(string warehouse, string user, string fileName, string[] stringValues);
    }
}
