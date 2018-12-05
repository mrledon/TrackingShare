using EmployeeTracking.API.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmployeeTracking.API
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_End()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://103.48.194.70/employeetracking_api");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                }
            }
            catch { }
        }
    }
}
