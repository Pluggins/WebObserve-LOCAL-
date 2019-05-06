using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Observer.API.ViewModels;
using Observer.Models;
using Observer.Services;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Observer.API.Controllers
{
    [Authorize]
    public class StepApiController : Controller
    {
        [HttpPost]
        [Route("Api/Step/GetStepCount")]
        public int CountStepByObserveId(string ObserveId)
        {
            return StepServiceManager.GetStepListByObserveId(ObserveId).Where(e => e.Deleted == false).Count();
        }

        [HttpPost]
        [Route("Api/Step/CreateNewStep")]
        public int CreateNewStep(StepCreationInputModel model) 
        {
            if (model.Url == null)
            {
                return 1;
            } else if (model.Method != 1 && model.Method != 2)
            {
                return 2;
            } else if (model.Priority != 1 && model.Priority != 2)
            {
                return 3;
            } else
            {
                ObserverDbContext _db = new ObserverDbContext();

                ObservesModel observesModel = _db.Observes
                    .Where(e => e.Id == model.ObserveId)
                    .FirstOrDefault();

                PCMethodsModel pcMethodsModel = _db.PCMethods
                    .Where(e => e.Id == model.ContentMethod.ToString())
                    .FirstOrDefault();

                byte[] encrypt = null;
                byte[] encryptSecret = null;
                string encrypted = null;
                string encryptedSecret = null;
                if (model.PC2 != null)
                {
                    byte[] secretKey = Convert.FromBase64String(_db._AESSecretKeyS);
                    using (AesManaged myAes = new AesManaged())
                    {
                        encrypt = EncryptionAlgorithmServiceManager.EncryptStringToBytes_Aes(model.PC2, secretKey, myAes.IV);
                        encryptSecret = myAes.IV;
                    }
                }

                if (encrypt != null)
                {
                    encrypted = Convert.ToBase64String(encrypt);
                    encryptedSecret = Convert.ToBase64String(encryptSecret);
                }

                StepsModel stepsModel = new StepsModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Url = model.Url,
                    Method = model.Method,
                    SetHeader = model.Header,
                    Deleted = false,
                    PredefinedHeader = model.PredefinedHeader,
                    PC_Method = pcMethodsModel,
                    PC1 = model.PC1,
                    PC2 = encrypted,
                    PC2Secret = encryptedSecret
                };

                int numStep = StepServiceManager.GetStepListByObserveId(model.ObserveId).Count();
                if (numStep == 0)
                {
                    if (StepServiceManager.CreateStep(User.Identity.GetUserId(), stepsModel, 1, model.ObserveId))
                    {
                        return 4;
                    } else
                    {
                        return 5;
                    }
                }
                else
                {
                    if (model.Priority == 1)
                    {
                        if (StepServiceManager.CreateStep(User.Identity.GetUserId(), stepsModel, 2, model.ObserveId))
                        {
                            return 4;
                        } else
                        {
                            return 5;
                        }
                    }
                    if (model.Priority == 2)
                    {
                        if (StepServiceManager.CreateStep(User.Identity.GetUserId(), stepsModel, 3, model.ObserveId))
                        {
                            return 4;
                        } else
                        {
                            return 5;
                        }
                    }
                    return 4;
                }
                
            }
        }

        [HttpPost]
        [Route("Api/Step/ResubmitStep")]
        public int ResubmitStep(StepResubmitInputModel model) 
        {
            if (model.Url == null)
            {
                return 1;
            } else if (model.Method != 1 && model.Method != 2)
            {
                return 2;
            } else
            {
                ObserverDbContext _db = new ObserverDbContext();

                byte[] encrypt = null;
                byte[] encryptSecret = null;
                string encrypted = null;
                string encryptedSecret = null;
                if (model.PC2 != null)
                {
                    byte[] secretKey = Convert.FromBase64String(_db._AESSecretKeyS);
                    using (AesManaged myAes = new AesManaged())
                    {
                        encrypt = EncryptionAlgorithmServiceManager.EncryptStringToBytes_Aes(model.PC2, secretKey, myAes.IV);
                        encryptSecret = myAes.IV;
                    }
                }

                if (encrypt != null)
                {
                    encrypted = Convert.ToBase64String(encrypt);
                    encryptedSecret = Convert.ToBase64String(encryptSecret);
                }

                PCMethodsModel pcMethodsModel = PCMethodServiceManager.GetPCMethodById(model.PC_Method.ToString());
                StepsModel stepsModel = new StepsModel()
                {
                    Id = model.StepId,
                    Url = model.Url,
                    Method = model.Method,
                    SetHeader = model.Header,
                    Deleted = false,
                    PredefinedHeader = model.PredefinedHeader,
                    PC1 = model.PC1,
                    PC2 = encrypted,
                    PC2Secret = encryptedSecret
                };

                StepServiceManager.UpdateStep(stepsModel, model.PC_Method.ToString());
                return 4;
            }
        }

        [HttpPost]
        [Route("Api/Step/ReorderStep")]
        public int ReorderStep(ReorderStepInputModel model)
        {
            ObservesModel observesModel = ObserveServiceManager.GetObserveById(model.ObserveId);
            if (observesModel.Users.Id == User.Identity.GetUserId())
            {
                if (model.initOrder == model.minOrder)
                {
                    StepServiceManager.ReorderStepDesc(model.ObserveId, model.minOrder, model.maxOrder);
                    return 1;
                }
                else
                {
                    StepServiceManager.ReorderStepAsc(model.ObserveId, model.minOrder, model.maxOrder);
                    return 1;
                }
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Step/GetList")]
        public JsonResult GetStepList(String ObserveId)
        {
            ObservesModel observesModel = ObserveServiceManager.GetObserveById(ObserveId);

            if (observesModel.Users.Id == User.Identity.GetUserId())
            {
                List<StepsModel> modelList = StepServiceManager.GetStepListByObserveId(ObserveId);
                List<StepListOutputModel> newList = new List<StepListOutputModel>();

                foreach (StepsModel item in modelList)
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

                return Json(newList);
            } else
            {
                return Json(JsonConvert.DeserializeObject("{ 'Status' = 'Unauthorized Access' }"));
            }
        }

        [HttpPost]
        [Route("Api/Step/DeleteStep")]
        public int DeleteStep(string StepId)
        {
            ObservesModel model = ObserveServiceManager.GetObserveByStepId(StepId);

            if (model.Users.Id == User.Identity.GetUserId())
            {
                StepServiceManager.DeleteStepById(StepId);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Step/GetStepDetail")]
        public JsonResult GetStepDetail(string StepId)
        {
            if (ObserveServiceManager.GetObserveByStepId(StepId).Users.Id == User.Identity.GetUserId())
            {
                int numPC = 0;
                string PC1Label = null;
                string PC2Label = null;
                string decrypted = null;

                ObserverDbContext _db = new ObserverDbContext();

                StepsModel model = _db.Steps.Where(e => e.Id == StepId).FirstOrDefault();

                if (model.PC_Method != null)
                {
                    PCMethodsModel models = model.PC_Method;
                    numPC = models.Type;
                    PC1Label = models.PC1;
                    PC2Label = models.PC2;
                }

                int pcmethod = 0;
                if (model.PC_Method != null)
                {
                    pcmethod = int.Parse(model.PC_Method.Id);
                }

                if (model.PC2 != null)
                {
                    decrypted = EncryptionAlgorithmServiceManager.DecryptStringFromBytes_Aes(Convert.FromBase64String(model.PC2), Convert.FromBase64String(_db._AESSecretKeyS), Convert.FromBase64String(model.PC2Secret));
                }

                StepDetailOutputModel outputModel = new StepDetailOutputModel()
                {
                    StepId = model.Id,
                    Url = model.Url,
                    Method = model.Method.GetValueOrDefault(),
                    Header = model.SetHeader.GetValueOrDefault(),
                    Priority = model.Order.GetValueOrDefault(),
                    PredefinedHeader = model.PredefinedHeader,
                    Status = "Authorized",
                    PC_Method = pcmethod,
                    PC1 = model.PC1,
                    PC2 = decrypted,
                    PC1Label = PC1Label,
                    PC2Label = PC2Label,
                    numPC = numPC
                };

                return Json(outputModel);
            } else
            {
                return Json(JsonConvert.DeserializeObject("{ 'Status': 'Unauthorized' }"));
            }
        }

        [HttpPost]
        [Route("Api/Step/ExecuteStep")]
        public async Task<JsonResult> ExecuteStep(string StepId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            StepsModel stepsModel = _db.Steps
                .Where(e => e.Id == StepId)
                .FirstOrDefault();

            ObservesModel observesModel = stepsModel.Observes;

            if (observesModel.Users.Id == User.Identity.GetUserId())
            {
                string pcMethodsModelId = null;
                string LabelPC1 = null;
                string LabelPC2 = null;
                int pcMethodsModelIdInt = 0;
                if (stepsModel.PC_Method != null)
                {
                    pcMethodsModelId = stepsModel.PC_Method.Id;
                    LabelPC1 = stepsModel.PC_Method.PC1;
                    LabelPC2 = stepsModel.PC_Method.PC2;
                    pcMethodsModelIdInt = int.Parse(pcMethodsModelId);
                }

                StepExecutionInputModel input = new StepExecutionInputModel()
                {
                    Order = stepsModel.Order.GetValueOrDefault(),
                    Uri = stepsModel.Url,
                    Method = stepsModel.Method.GetValueOrDefault(),
                    PC_Method = pcMethodsModelIdInt,
                    PC1 = stepsModel.PC1,
                    PC2 = stepsModel.PC2,
                    CustomHeader = stepsModel.PredefinedHeader,
                    PredefinedHeader = stepsModel.SetHeader.GetValueOrDefault(),
                    LabelPC1 = LabelPC1,
                    LabelPC2 = LabelPC2,
                    PC2Secret = stepsModel.PC2Secret
                };
                
                return Json(await StepServiceManager.ExecuteStepReturningResult(input));
            } else
            {
                StepExecutionOutputModel input = new StepExecutionOutputModel()
                {
                    Status = "Unauthorized"
                };

                return Json(input);
            }
        }

        [HttpPost]
        [Route("Api/Step/ExecuteBatchStep")]
        public async Task<JsonResult> ExecuteBatchStepByObserveId(string ObserveId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            ObservesModel model = _db.Observes
                .Where(e => e.Id == ObserveId)
                .FirstOrDefault();

            if (model.Users.Id == User.Identity.GetUserId())
            {
                List<StepsModel> listModel = _db.Observes.Where(e => e.Id == ObserveId).FirstOrDefault().List_Steps.Where(e => e.Deleted == false).OrderBy(e => e.Order).ToList();
                List<StepExecutionInputModel> inputList = new List<StepExecutionInputModel>();
                PCMethodsModel pcMethodsModel = null;

                foreach (StepsModel item in listModel)
                {
                    string PC1 = null;
                    string PC2 = null;
                    int PCMethod = 0;
                    if (item.PC_Method != null)
                    {
                        pcMethodsModel = PCMethodServiceManager.GetPCMethodById(item.PC_Method.Id);
                        PC1 = pcMethodsModel.PC1;
                        PC2 = pcMethodsModel.PC2;
                        PCMethod = int.Parse(item.PC_Method.Id);
                    }

                    StepExecutionInputModel input = new StepExecutionInputModel()
                    {
                        Order = item.Order.GetValueOrDefault(),
                        Uri = item.Url,
                        Method = item.Method.GetValueOrDefault(),
                        PC_Method = PCMethod,
                        PC1 = item.PC1,
                        PC2 = item.PC2,
                        PC2Secret = item.PC2Secret,
                        CustomHeader = item.PredefinedHeader,
                        UseHeader = item.SetHeader.GetValueOrDefault(),
                        LabelPC1 = PC1,
                        LabelPC2 = PC2
                    };

                    inputList.Add(input);
                }

                return Json(await StepServiceManager.ExecuteBatchStepReturningResult(inputList));
            } else
            {
                StepExecutionOutputModel input = new StepExecutionOutputModel()
                {
                    Status = "Unauthorized"
                };

                return Json(input);
            }
        }

        [HttpPost]
        [Route("Api/Step/SetCatch")]
        public int SetCatchByStepId(string StepId, string CatchId)
        {
            ObservesModel model = ObserveServiceManager.GetObserveByStepId(StepId);
            if (model.Users.Id == User.Identity.GetUserId())
            {
                StepServiceManager.SetCatchByStepId(StepId, CatchId);
                return 1;
            } else
            {
                return 2;
            }
        }

        [HttpPost]
        [Route("Api/Step/GetPCMethod")]
        public JsonResult GetPCMethod()
        {
            List<PCMethodsModel> model = PCMethodServiceManager.GetPCMethodList().OrderBy(e => e.Name).ToList();
            List<PCMethodOutputModel> output = new List<PCMethodOutputModel>();

            foreach (PCMethodsModel item in model)
            {
                PCMethodOutputModel newModel = new PCMethodOutputModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };

                output.Add(newModel);
            }

            return Json(output);
        }

        [HttpPost]
        [Route("Api/Step/GetPCMethodType")]
        public JsonResult GetPCMethodType()
        {
            List<PCMethodsModel> model = PCMethodServiceManager.GetPCMethodList().OrderBy(e => e.Name).ToList();
            List<PCMethodOutputModel> output = new List<PCMethodOutputModel>();

            foreach (PCMethodsModel item in model)
            {
                PCMethodOutputModel newModel = new PCMethodOutputModel()
                {
                    Id = item.Id,
                    Type = item.Type,
                    PC1 = item.PC1,
                    PC2 = item.PC2
                };

                output.Add(newModel);
            }

            return Json(output);
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("Api/Step/SetHeader")]
        public JsonResult SetPredefinedHeader(string StepId, string Header, string Code)
        {
            ObserverDbContext _db = new ObserverDbContext();
            Header = Regex.Replace(Header, @"\s+", "");

            var query = _db.Steps
                .Where(e => e.Id == StepId)
                .FirstOrDefault();

            if (query.Observes.Users.ApiCode == Code)
            {
                query.PredefinedHeader = Header;
                ObservesModel observesModel = _db.Steps.Where(e => e.Id == StepId).FirstOrDefault().Observes;
                observesModel.Status = 1;
                _db.SaveChanges();

                return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Success' }"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Unauthorized' }"), JsonRequestBehavior.AllowGet);
            }
        }
    }
}