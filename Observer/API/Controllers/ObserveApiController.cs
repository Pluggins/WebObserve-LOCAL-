using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Observer.API.ViewModels;
using Observer.Models;
using Observer.Services;

namespace Observer.API.Controllers
{
    [Authorize]
    public class ObserveApiController : Controller
    {
        // GET: ObserveAPI
        [HttpPost]
        [Route("Api/Observe/GetList")]
        public JsonResult GetObserveListingsByAspId()
        {
            List<ObservesModel> oriList = ObserveServiceManager.GetObserveListByAspId(User.Identity.GetUserId()).OrderByDescending(e => e.DateCreated).ToList();
            List<ObserveListOutputModel> newList = new List<ObserveListOutputModel>();

            foreach (ObservesModel a in oriList)
            {
                ObserveListOutputModel model = new ObserveListOutputModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Status = a.Status.GetValueOrDefault(),
                    DateCreated = a.DateCreated.AddHours(8).ToString("dd/MM/yy")
                };
                newList.Add(model);
            }

            return Json(newList);
        }

        [HttpPost]
        [Route("Api/Observe/Create")]
        public int CreateObserve(string Name)
        {
            ObserveServiceManager.CreateObserve(User.Identity.GetUserId(), Name);
            return 1;
        }

        [HttpPost]
        [Route("Api/Observe/Delete")]
        public int DeleteObserveById(string Id)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(Id);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                ObserveServiceManager.DeleteObserveById(Id);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Observe/SetStatus")]
        public int SetObserveStatus(string Id, int Status)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(Id);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                model.Status = Status;
                ObserveServiceManager.UpdateObserve(model);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Observe/SetName")]
        public int SetObserveName(string Id, string Name)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(Id);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                model.Name = Name;
                ObserveServiceManager.UpdateObserve(model);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Observe/GetRecipientList")]
        public JsonResult GetRecipientList(string ObserveId)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(ObserveId);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                List<RecipientsModel> listModel = ObserveServiceManager.GetRecipientList(ObserveId);
                List<RecipientListOutputModel> output = new List<RecipientListOutputModel>();

                foreach (RecipientsModel item in listModel)
                {
                    RecipientListOutputModel outputModel = new RecipientListOutputModel()
                    {
                        RecipientId = item.Id,
                        Email = item.Email
                    };

                    output.Add(outputModel);
                }

                return Json(output);
            } else
            {
                return Json(JsonConvert.DeserializeObject("{ 'Status': 'Unauthorized' }"));
            }
        }

        [HttpPost]
        [Route("Api/Observe/AddRecipient")]
        public int AddRecipient(string ObserveId, string Email)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(ObserveId);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                List<RecipientsModel> list = ObserveServiceManager.GetRecipientList(ObserveId);
                if (list.Where(e => e.Email == Email).Count() == 0)
                {
                    RecipientsModel newModel = new RecipientsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Observes = model,
                        Email = Email,
                        Deleted = false
                    };

                    ObserveServiceManager.AddRecipient(newModel);
                    return 1;
                } else
                {
                    return 3;
                }
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Observe/DeleteRecipient")]
        public int DeleteRecipientByRecipientId(string RecipientId)
        {
            ObservesModel model = ObserveServiceManager.GetObservesByRecipientId(RecipientId);

            if (model.Users.Id == User.Identity.GetUserId())
            {
                ObserveServiceManager.DeleteRecipientById(RecipientId);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Observe/SetInterval")]
        public int SetIntervalByObserveId(string ObserveId, int Interval)
        {
            ObservesModel model = ObserveServiceManager.GetObserveById(ObserveId);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                ObserveServiceManager.SetIntervalByObserveId(ObserveId, Interval);
                return 1;
            } else
            {
                return 2;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Observe/ExecuteMain")]
        public int ExecuteMain()
        {
            return ObserveServiceManager.RunExecution().Result;
        }
    }
}