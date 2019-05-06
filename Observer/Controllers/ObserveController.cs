using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Observer.Models;
using Observer.Models.ViewModels;
using Observer.Services;

namespace Observer.Controllers
{
    [Authorize]
    public class ObserveController : Controller
    {
        // GET: Observe
        public ActionResult Index()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("ObserveM");
            }
            string UserId = User.Identity.GetUserId();

            ObserverDbContext _db = new ObserverDbContext();

            List<ObservesModel> oriList = _db.Observes
                .Where(e => e.Users.Id == UserId && e.Deleted == false)
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            List<ObserveListOutputModel> newList = new List<ObserveListOutputModel>();

            foreach (ObservesModel item in oriList)
            {
                ObserveListOutputModel model = new ObserveListOutputModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Status = item.Status.GetValueOrDefault(),
                    DateCreated = item.DateCreated.AddHours(8).ToString("dd/MM/yy")
                };

                newList.Add(model);
            }

            
            return View(newList);
        }

        public ActionResult ObserveM()
        {
            if (!Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Index");
            }

            string UserId = User.Identity.GetUserId();
            ObserverDbContext _db = new ObserverDbContext();

            List<ObservesModel> oriList = _db.Observes
                .Where(e => e.Users.Id == UserId && e.Deleted == false)
                .OrderByDescending(e => e.DateCreated)
                .ToList();
            List<ObserveListOutputModel> newList = new List<ObserveListOutputModel>();

            foreach (ObservesModel item in oriList)
            {
                ObserveListOutputModel model = new ObserveListOutputModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Status = item.Status.GetValueOrDefault(),
                    DateCreated = item.DateCreated.AddHours(8).ToString("dd/MM/yy")
                };

                newList.Add(model);
            }


            return View(newList);
        }

        public ActionResult Create()
        {
            return View();
        }

        [Route("Observe/Modify")]
        public ActionResult ModifyRedirect()
        {
            return RedirectToAction("Index", "Observe");
        }
        
        [Route("Observe/Modify/{id}")]
        public ActionResult Modify(string id)
        {
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("ModifyM");
            }

            ObservesModel model = ObserveServiceManager.GetObserveById(id);
            List<StepsModel> oriList = StepServiceManager.GetStepListByObserveId(id);
            List<PCMethodsModel> methodList = PCMethodServiceManager.GetPCMethodList().OrderBy(e => e.Name).ToList();
            List<StepListOutputModel> newList = new List<StepListOutputModel>();
            List<PCMethodOutputModel> PCMethodOutput = new List<PCMethodOutputModel>();

            foreach (StepsModel item in oriList)
            {
                string StringMethod = null;
                string StringSetHeader = null;

                if (item.Method == 1)
                {
                    StringMethod = "GET";
                } else if (item.Method >= 2)
                {
                    StringMethod = "POST";
                }

                if (item.SetHeader == 1)
                {
                    StringSetHeader = "Yes";
                } else if (item.SetHeader == 0)
                {
                    StringSetHeader = "No";
                } else if (item.SetHeader == 2)
                {
                    StringSetHeader = "Pre";
                }
                StepListOutputModel newModel = new StepListOutputModel()
                {
                    Id = item.Id,
                    Priority = item.Order.GetValueOrDefault(),
                    Url = item.Url,
                    Method = StringMethod,
                    SetHeader = StringSetHeader

                };
                newList.Add(newModel);
            }

            foreach (PCMethodsModel item in methodList)
            {
                PCMethodOutputModel newModel = new PCMethodOutputModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };

                PCMethodOutput.Add(newModel);
            }

            try
            {
                ObserveOutputModel output = new ObserveOutputModel()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Status = model.Status.GetValueOrDefault(),
                    Interval = model.Interval.GetValueOrDefault(),
                    StepList = newList,
                    PCMethod = PCMethodOutput
                };

                return View(output);
            } catch (NullReferenceException e)
            {
                return RedirectToAction("Index", "Observe");
            }
        }

        [Route("Observe/ModifyM/{id}")]
        public ActionResult ModifyM(string id)
        {
            if (!Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("ModifyM");
            }

            ObservesModel model = ObserveServiceManager.GetObserveById(id);
            List<StepsModel> oriList = StepServiceManager.GetStepListByObserveId(id);
            List<PCMethodsModel> methodList = PCMethodServiceManager.GetPCMethodList().OrderBy(e => e.Name).ToList();
            List<StepListOutputModel> newList = new List<StepListOutputModel>();
            List<PCMethodOutputModel> PCMethodOutput = new List<PCMethodOutputModel>();

            foreach (StepsModel item in oriList)
            {
                string StringMethod = null;
                string StringSetHeader = null;

                if (item.Method == 1)
                {
                    StringMethod = "GET";
                }
                else if (item.Method >= 2)
                {
                    StringMethod = "POST";
                }

                if (item.SetHeader == 1)
                {
                    StringSetHeader = "Yes";
                }
                else if (item.SetHeader == 0)
                {
                    StringSetHeader = "No";
                }
                else if (item.SetHeader == 2)
                {
                    StringSetHeader = "Pre";
                }
                StepListOutputModel newModel = new StepListOutputModel()
                {
                    Id = item.Id,
                    Priority = item.Order.GetValueOrDefault(),
                    Url = item.Url,
                    Method = StringMethod,
                    SetHeader = StringSetHeader

                };
                newList.Add(newModel);
            }

            foreach (PCMethodsModel item in methodList)
            {
                PCMethodOutputModel newModel = new PCMethodOutputModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };

                PCMethodOutput.Add(newModel);
            }

            try
            {
                ObserveOutputModel output = new ObserveOutputModel()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Status = model.Status.GetValueOrDefault(),
                    Interval = model.Interval.GetValueOrDefault(),
                    StepList = newList,
                    PCMethod = PCMethodOutput
                };

                return View(output);
            }
            catch (NullReferenceException e)
            {
                e.ToString();
                return RedirectToAction("Index", "Observe");
            }
        }
    }
}