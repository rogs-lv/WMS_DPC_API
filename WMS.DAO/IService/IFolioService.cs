using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IFolioService
    {
        bool CheckFolioFile(string user, string file);
        Response<bool> WriteFolioFile(string user, string file, string[] batchs);
    }
}
