using System.Web.Mvc;

namespace EmployeeTracking.Admin.Filters
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Flush();

            //convert the current filter context to file and get the file path
            string filePath = (filterContext.Result as FilePathResult).FileName;

            if (System.IO.File.Exists(filePath))
            {
                //delete the file after download
                System.IO.File.Delete(filePath);
            }
        }
    }
}