using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.DAO
{
    public class DBFactory
    {
        public static IDBAdapter GetDefaultAdapater() {
            try
            {
                string defaultDBClass = ConfigurationManager.AppSettings["dbClass"];
                Type type = Type.GetType(defaultDBClass);
                return (IDBAdapter)Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
