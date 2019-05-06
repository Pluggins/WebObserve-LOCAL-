
using Observer.API.ViewModels;
using Observer.Models;
using Observer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Observer.API.Controllers
{
    [Authorize]
    public class CatchApiController : Controller
    {
        [HttpPost]
        [Route("Api/Catch/GetList")]
        public JsonResult GetCatchList(string StepId)
        {
            List<CatchesModel> list = CatchServiceManager.GetCatchesList().OrderBy(e => e.Name).ToList();
            CatchListOutputModel output = new CatchListOutputModel();
            List<CatchesModel> newList = new List<CatchesModel>();
            StepsModel step = StepServiceManager.GetStepById(StepId);
            string currentCatchId = "0";

            foreach (CatchesModel item in list)
            {
                CatchesModel catchesModel = new CatchesModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };
                newList.Add(catchesModel);
            }

            if (step.Catches != null)
            {
                currentCatchId = step.Catches.Id;
            }

            output.Catches = newList;
            output.SelectedId = currentCatchId;

            return Json(output);
        }
    }
}