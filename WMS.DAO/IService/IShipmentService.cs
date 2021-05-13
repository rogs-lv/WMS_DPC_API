using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IShipmentService
    {
        List<Shipment> GetNumberShipment(int docnum);
        bool ApplyGR(string batch);
        ValidateEFEEM isValidEFEEM(string batch, int type);
        List<Batch> ReadCode(string codebars, string warehouse);
        int UpdateBatchs(string[] batchs, string status, int docnum, int docentry);
        Response<List<TransferShippment>> processShipment(List<shipmentProcess> data);
    }
}
