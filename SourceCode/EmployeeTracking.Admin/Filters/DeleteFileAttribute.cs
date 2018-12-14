using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Filters
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string filePath = "";
            filterContext.HttpContext.Response.Flush();
            try
            {
                //convert the current filter context to file and get the file path
                filePath = (filterContext.Result as FilePathResult).FileName;

                if (File.Exists(filePath))
                {
                    //delete the file after download
                    File.Delete(filePath);
                }
            }
            catch
            {

            }
            try
            {
                var account = (Data.Database.user)filterContext.HttpContext.Session["Account"];
                string userId = account.Id.ToString();
                filePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "temp", userId.ToString() + ".zip");
                if (File.Exists(filePath))
                {
                    //delete the file after download
                    File.Delete(filePath);
                }
            }
            catch { }

            
        }
    }
}