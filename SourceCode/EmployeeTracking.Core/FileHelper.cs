using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace EmployeeTracking.Core
{
    public class FileHelper
    {
        public static bool RemoveFileFromServer(string path)
        {
            try //Maybe error could happen like Access denied or Presses Already User used
            {
                //var fullPath = HostingEnvironment.MapPath(path);
                if (!File.Exists(path))
                    return false;

                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                //Debug.WriteLine(e.Message);
            }
            return false;
        }
    }
}
