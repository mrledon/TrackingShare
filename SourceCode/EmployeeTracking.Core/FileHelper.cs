using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace EmployeeTracking.Core
{
    public class FileHelper
    {
        public static bool IsValidImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                postedFile.ContentType.ToLower() != "image/jpeg" &&
                postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            return true;

        }
        public static bool RemoveFileFromServer(string path)
        {
            try //Maybe error could happen like Access denied or Presses Already User used
            {
                var fullPath = HostingEnvironment.MapPath(path);
                if (!File.Exists(fullPath))
                    return false;

                File.Delete(fullPath);
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
