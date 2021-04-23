using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IReadCodebars
    {
        List<Batch> ReadCode(string codebars, string warehouse, int status = 0);
    }
}
