using Observer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Observer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            ObserverDbContext _db = new ObserverDbContext();
            string referrer = null;

            if (Request.UrlReferrer != null)
            {
                referrer = Request.UrlReferrer.ToString();
            }
            ErrorLoggingsModel model = new ErrorLoggingsModel()
            {
                Id = Guid.NewGuid().ToString(),
                Source = exception.Source,
                Message = exception.Message,
                UserAgent = Request.UserAgent,
                Ip = Request.UserHostAddress,
                Referrer = referrer,
                Date = DateTime.UtcNow
            };

            _db.ErrorLoggings.Add(model);
            _db.SaveChanges();
        }
    }
}
