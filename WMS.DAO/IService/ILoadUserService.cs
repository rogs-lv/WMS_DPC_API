using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface ILoadUserService
    {
        Response<int> SaveUsers(List<LoadUser> data);
        Response<int> SaveModule(List<LoadModule> data, int option);
        Response<int> SaveOtherConfiguration(List<LoadModule> data, int option);
    }
}
