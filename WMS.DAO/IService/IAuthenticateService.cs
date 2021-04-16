using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface IAuthenticateService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest credential);
    }
}
