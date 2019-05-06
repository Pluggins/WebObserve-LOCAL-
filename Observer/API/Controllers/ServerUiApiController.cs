using Observer.API.ViewModels;
using Observer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Observer.API.Controllers
{
    public class ServerUiApiController : Controller
    {
        [HttpPost]
        [Route("Api/Server/Status")]
        public JsonResult GetServerStatus()
        {
            ObserverDbContext _db = new ObserverDbContext();
            int observeServerStatus = 0;

            var serverStatus = _db.WebConfigs
                .Where(e => e.Id == "E7461932-0A62-4AC0-9D34-23C8C2B839B2")
                .FirstOrDefault();

            var serverSuccess = _db.WebConfigs
                .Where(e => e.Id == "9F0EF51A-E5D0-486E-A0C9-D1E003D3C0D8")
                .FirstOrDefault();

            var stepSocketException = _db.StepSocketExceptions
                .ToList();

            int countFailed = stepSocketException
                .OrderByDescending(e => e.Date)
                .Where(e => e.Date > serverSuccess.Timestamp)
                .Count();

            if (serverStatus.Timestamp.AddMinutes(5) > DateTime.UtcNow)
            {
                observeServerStatus = 1;
            }

            int success = int.Parse(serverSuccess.Value);
            int failure = countFailed;

            ServerStatusOutputModel model = new ServerStatusOutputModel()
            {
                Status = observeServerStatus,
                LastCheckedMin = (int) DateTime.UtcNow.Subtract(serverStatus.Timestamp).TotalMinutes,
                TotalIn = success,
                TotalOut = failure,
                SuccessRate = ((success/(double)(success+failure))*100).ToString("0.##") + "%"
            };

            return Json(model);
        }
    }
}