#region Using

using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

#endregion

namespace SmartAdminMvc
{
    public class MvcApplication : HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            IdentityConfig.RegisterIdentities();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }

        protected void Application_Error()
        {
            var MachineName = Server.MachineName;
            var Exception = Server.GetLastError();
            //log the error!
            logger.Error(Exception, "Error Occured in " + MachineName + " :- ");
        }
    }
}