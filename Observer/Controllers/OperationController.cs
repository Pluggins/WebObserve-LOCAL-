using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Observer.Services;

namespace Observer.Controllers
{
    public class OperationController : Controller
    {
        // GET: Operation
        public ActionResult Index()
        {
            int count = ObserveServiceManager.RunExecution().Result;
            ViewBag.Count = count;
            return View();
        }
    }
}