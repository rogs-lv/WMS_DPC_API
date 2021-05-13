using System;
using System.Collections.Generic;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IShippingService
    {
        Response<List<Partner>> GetListPartner();
        List<Batch> ReadCode(string codebars, string warehouse);
    }
}
