using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WMS.DAO.IService;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class FolioService : IFolioService
    {
        Log lg;
        public FolioService()
        {
            lg = Log.getIntance();
        }

        public bool CheckFolioFile(string user, string file)
        {
            List<string> basePath = GetPathFile();
            try
            {
                string fullPath = @"C:\" + basePath[1] + @"\FoliosSAP\" + user + @"\" + file;
                if (File.Exists(fullPath))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        
        public Response<bool> WriteFolioFile(string user, string file, string[] batchs)
        {
            List<string> basePathFile = GetPathFile();
            string basePath = @"C:\" + basePathFile[1] + @"\";
            DirectoryInfo warehouseDirectory;
            try
            {
                //Create RecuentoInventario if not exist
                if (!Directory.Exists(basePath))
                    return new Response<bool>(false, -1, $"No se encontró ningún folder con el nombre: {basePathFile}"); ;
                //Create folder of FolioSAP if not exist
                string directoryUser = string.Empty;
                if (!Directory.Exists($"{basePath}FoliosSAP")) {
                    warehouseDirectory = Directory.CreateDirectory(basePath + "FoliosSAP");
                    directoryUser = basePath + "FoliosSAP" + @"\";
                } else
                  directoryUser = basePath + "FoliosSAP" + @"\";

                //Create folder of user if not exist
                string pathFull = string.Empty;
                if (!Directory.Exists(directoryUser + user))
                {
                    Directory.CreateDirectory(directoryUser + user);
                    pathFull = directoryUser + user + @"\" + file;
                } else
                    pathFull = directoryUser + user + @"\" + file;

                if (File.Exists(pathFull)) //If exist, delete
                    File.Delete(pathFull);

                if (!File.Exists(pathFull)) //If not exist, create
                {
                    
                        File.WriteAllLines(pathFull, batchs); 
                    
                }
                
                return new Response<bool>(true, 0, "Archivo creado correctamente"); ;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<bool>(false, -1, ex.Message); ;
            }
        }

        private List<string> GetPathFile() {
            string path = string.Empty, regexPattern = @"\\\\", replaceBy = "";
            List<string> subPaths = new List<string>();

            try
            {
                path = ConfigurationManager.AppSettings["PathFolio"];
                if (!string.IsNullOrEmpty(path))
                {
                    string result = Regex.Replace(path, regexPattern, replaceBy);
                    int lastSubsStr = result.LastIndexOf("\\");
                    if (lastSubsStr == -1)
                    {
                        string[] routes = result.Split('\\');
                        foreach (string route in routes)
                        {
                            subPaths.Add(route);
                        }
                        return subPaths;
                    }

                    string finalResult = result.Remove(lastSubsStr, "\\".Length).Insert(lastSubsStr, replaceBy);
                    string[] words = finalResult.Split('\\');
                    foreach (string word in words)
                    {
                        subPaths.Add(word);
                    }
                }
                return subPaths;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<string>();
            }
        }
    }
}
