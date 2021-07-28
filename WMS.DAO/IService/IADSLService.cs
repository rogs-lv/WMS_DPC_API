using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IADSLService
    {
        Response<List<PartnerADSL>> GetListPartner();
        List<BatchADSL> ReadCode(string codebars, string warehouse, int priceList);
        Response<Delivery> ProcessDelivery(List<BatchADSL> data, string cardCode, string usuario);
    }
}
