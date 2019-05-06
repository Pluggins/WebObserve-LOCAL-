using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Observer.Models;
using Observer.Models.ViewModels;
using Observer.Models.ViewModels.PCInput;

namespace Observer.Services
{
    public static class StepServiceManager
    {
        public static List<StepsModel> GetStepListByObserveId(String ObserveId)
        {
            ObserverDbContext _db = new ObserverDbContext();

                return _db.Steps
                    .Where(e=>e.Observes.Id == ObserveId && e.Deleted == false)
                    .OrderBy(e=>e.Order)
                    .ToList();
        }

        /*
         * Method
         * 1 - No previous step
         * 2 - Insert new step as level 1 priority
         * 3 - Insert new step as last level priority
         */

        public static bool CreateStep(string AspId, StepsModel model, int Method, string ObserveId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            ObservesModel queryModel = _db.Observes
                .Where(e => e.Id == ObserveId)
                .FirstOrDefault();

            try
            {
                if (model.PC_Method.Id != null)
                {
                    PCMethodsModel pcMethodsModel = _db.PCMethods.Where(e => e.Id == model.PC_Method.Id).FirstOrDefault();
                    model.PC_Method = pcMethodsModel;
                }
            } catch (NullReferenceException e)
            {
                e.ToString();
            }

            if (AspId == queryModel.Users.Id)
            {
                model.Observes = queryModel;
                model.CheckedTime = DateTime.UtcNow;

                if (Method == 1)
                {
                    model.Order = 1;
                    _db.Steps.Add(model);
                    _db.SaveChanges();
                }
                else if (Method == 2)
                {
                    var modelList = _db.Steps
                        .Where(e => e.Observes.Id == ObserveId)
                        .OrderByDescending(e => e.Order)
                        .ToList();

                    foreach (StepsModel item in modelList)
                    {
                        item.Order = item.Order + 1;
                    }

                    model.Order = 1;
                    _db.Steps.Add(model);
                    _db.SaveChanges();
                }
                else if (Method == 3)
                {
                    int countStep = _db.Steps
                        .Where(e => e.Observes.Id == model.Observes.Id && e.Deleted == false)
                        .Count();

                    model.Order = countStep + 1;
                    _db.Steps.Add(model);
                    _db.SaveChanges();
                }
                return true;
            } else
            {
                return false;
            }
        }

        public static void ReorderStepAsc(string ObserveId, int initOrder, int finalOrder)
        {
            ObserverDbContext _db = new ObserverDbContext();

                List<StepsModel> listModel = _db.Steps
                    .Where(e => e.Observes.Id == ObserveId && e.Deleted == false)
                    .OrderBy(e => e.Order)
                    .ToList();
                int diff = finalOrder - initOrder;
                listModel.Where(e => e.Order == initOrder).FirstOrDefault().Order = 0;
                for (int i = 1; i <= diff; i++)
                {
                    listModel.Where(e => e.Order == initOrder + i).FirstOrDefault().Order = listModel.Where(e => e.Order == initOrder + i).FirstOrDefault().Order - 1;
                }
                listModel.Where(e => e.Order == 0).FirstOrDefault().Order = finalOrder;

                _db.SaveChanges();
        }

        public static void ReorderStepDesc(string ObserveId, int initOrder, int finalOrder)
        {
            ObserverDbContext _db = new ObserverDbContext();

            List<StepsModel> listModel = _db.Steps
                    .Where(e => e.Observes.Id == ObserveId && e.Deleted == false)
                    .OrderBy(e => e.Order)
                    .ToList();
                int diff = finalOrder - initOrder;
                listModel.Where(e => e.Order == finalOrder).FirstOrDefault().Order = 0;
                for (int i = finalOrder; i > initOrder; i--)
                {
                    listModel.Where(e => e.Order == i - 1).FirstOrDefault().Order = listModel.Where(e => e.Order == i - 1).FirstOrDefault().Order + 1;
                }
                listModel.Where(e => e.Order == 0).FirstOrDefault().Order = initOrder;

                _db.SaveChanges();
        }

        public static void DeleteStepById(string StepId)
        {
            using (ObserverDbContext _db = new ObserverDbContext())
            {
                StepsModel model = _db.Steps
                    .Where(e => e.Id == StepId)
                    .FirstOrDefault();

                int numOrder = model.Order.GetValueOrDefault();
                string observeId = model.Observes.Id;

                model.Deleted = true;
                model.Order = 0;

                List<StepsModel> listModel = _db.Steps
                    .Where(e => e.Observes.Id == observeId)
                    .ToList();

                foreach (StepsModel item in listModel)
                {
                    if (item.Order > numOrder)
                    {
                        item.Order = item.Order - 1;
                    }
                }

                _db.SaveChanges();
            }
        }

        public static StepsModel GetStepById(string Id)
        {
            ObserverDbContext _db = new ObserverDbContext();

            return _db.Steps.Where(e => e.Id == Id).FirstOrDefault();
        }

        public static void UpdateStep(StepsModel model, string PCMethodId)
        {
            ObserverDbContext _db = new ObserverDbContext();
            PCMethodsModel pcMethodsModel = _db.PCMethods.Where(e => e.Id == PCMethodId).FirstOrDefault();

            StepsModel oriModel = _db.Steps
                .Where(e => e.Id == model.Id)
                .FirstOrDefault();

            oriModel.Url = model.Url;
            oriModel.Method = model.Method;
            oriModel.SetHeader = model.SetHeader;
            oriModel.PredefinedHeader = model.PredefinedHeader;
            oriModel.PC_Method = pcMethodsModel;
            oriModel.PC1 = model.PC1;
            oriModel.PC2 = model.PC2;
            oriModel.PC2Secret = model.PC2Secret;

            _db.SaveChanges();
        }

        public static async Task<StepExecutionOutputModel> ExecuteStepReturningResult(Observer.API.ViewModels.StepExecutionInputModel Input)
        {
            ObserverDbContext _db = new ObserverDbContext();

            try
            {
                Uri uri = new Uri(Input.Uri);
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = new CookieContainer();
                StringContent queryString = null;
                HttpClient client = new HttpClient(handler);
                string respond = null;
                string decrypt = null;

                if (Input.PredefinedHeader == 1)
                {
                    handler.CookieContainer.SetCookies(uri, Input.CustomHeader);
                }

                if (Input.PC2 != null)
                {
                    decrypt = EncryptionAlgorithmServiceManager.DecryptStringFromBytes_Aes(Convert.FromBase64String(Input.PC2), Convert.FromBase64String(_db._AESSecretKeyS), Convert.FromBase64String(Input.PC2Secret));
                }

                if (Input.Method == 2)
                {
                    if (Input.PC_Method == 1)
                    {
                        email_password model = new email_password()
                        {
                            email = Input.PC1,
                            password = decrypt
                        };
                        queryString = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    }
                    else if (Input.PC_Method >= 3)
                    {
                        var query = _db.PCMethods
                            .Where(e => e.Id == Input.PC_Method.ToString())
                            .FirstOrDefault();

                        var list = query.List_PCMethodDatas;

                        if (query.ContentType == 1)
                        {
                            string s = "{ ";
                            int count = 0;

                            foreach (var items in list)
                            {
                                if (count == 0)
                                {
                                    s = s + "\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                }
                                else
                                {
                                    s = s + ",\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                }

                                count = count + 1;
                            }

                            s = s + "}";
                        }
                        else if (query.ContentType == 2)
                        {
                            string content = null;
                            foreach (var items in list)
                            {
                                content = content + items.Value;
                            }
                            if (Input.PC1 != null)
                            {
                                content = content + "&" + Input.LabelPC1 + "=" + Input.PC1;
                            }
                            if (Input.PC2 != null)
                            {
                                content = content + "&" + Input.LabelPC2 + "=" + decrypt;
                            }

                            queryString = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
                        }
                        else if (query.ContentType == 3)
                        {
                            string content = null;
                            foreach (var items in list)
                            {
                                content = content + items.Value;
                            }
                            if (Input.PC1 != null)
                            {
                                content = content + "&" + Input.LabelPC1 + "=" + Input.PC1;
                            }
                            if (Input.PC2 != null)
                            {
                                content = content + "&" + Input.LabelPC2 + "=" + decrypt;
                            }

                            queryString = new StringContent(content, Encoding.UTF8, "multipart/form-data");
                            queryString.Headers.Remove("Content-Type");
                            queryString.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=---------------------------114782935826962");
                        }
                    }
                    HttpResponseMessage response = await client.PostAsync(uri, queryString);
                    respond = await response.Content.ReadAsStringAsync();
                }
                else if (Input.Method == 1)
                {
                    HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
                    respond = await response.Content.ReadAsStringAsync();
                }


                StepExecutionOutputModel output = new StepExecutionOutputModel()
                {
                    ResponseContent = respond,
                    Status = "Authorized"
                };

                return output;
            } catch (UriFormatException e)
            {
                e.ToString();
                return null;
            }
        }

        public static async Task<StepExecutionOutputModel> ExecuteBatchStepReturningResult(List<Observer.API.ViewModels.StepExecutionInputModel> Input)
        {
            ObserverDbContext _db = new ObserverDbContext();

            try
            {
                Uri uri = null;
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = new CookieContainer();
                StringContent queryString = null;
                HttpClient client = new HttpClient(handler);
                string respond = null;
                HttpResponseMessage response = null;
                string decrypt = null;

                foreach (Observer.API.ViewModels.StepExecutionInputModel item in Input)
                {
                    if (item.PC2 != null)
                    {
                        decrypt = EncryptionAlgorithmServiceManager.DecryptStringFromBytes_Aes(Convert.FromBase64String(item.PC2), Convert.FromBase64String(_db._AESSecretKeyS), Convert.FromBase64String(item.PC2Secret));
                    }

                    if (item.UseHeader == 0)
                    {
                        handler = new HttpClientHandler();
                        client = new HttpClient(handler);
                    }

                    uri = new Uri(item.Uri);

                    if (item.UseHeader == 2)
                    {
                        handler.CookieContainer.SetCookies(uri, item.CustomHeader);
                    }
                    if (item.Method == 2)
                    {
                        if (item.PC_Method == 1)
                        {
                            email_password model = new email_password()
                            {
                                email = item.PC1,
                                password = decrypt
                            };
                            queryString = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        }
                        else if (item.PC_Method >= 3)
                        {
                            var query = _db.PCMethods
                                .Where(e => e.Id == item.PC_Method.ToString())
                                .FirstOrDefault();

                            var list = query.List_PCMethodDatas;

                            if (query.ContentType == 1)
                            {
                                string s = "{ ";
                                int count = 0;

                                foreach (var items in list)
                                {
                                    if (count == 0)
                                    {
                                        s = s + "\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                    }
                                    else
                                    {
                                        s = s + ",\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                    }

                                    count = count + 1;
                                }

                                s = s + "}";
                            }
                            else if (query.ContentType == 2)
                            {
                                string content = null;
                                foreach (var items in list)
                                {
                                    content = content + items.Value;
                                }
                                if (item.PC1 != null)
                                {
                                    content = content + "&" + item.LabelPC1 + "=" + item.PC1;
                                }
                                if (item.PC2 != null)
                                {
                                    content = content + "&" + item.LabelPC2 + "=" + decrypt;
                                }

                                queryString = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
                            }
                            //FOR UCSI ELE
                            else if (query.ContentType == 3)
                            {
                                string content = null;
                                foreach (var items in list)
                                {
                                    content = content + items.Value;
                                }
                                if (item.PC1 != null)
                                {
                                    content = content + "&" + item.LabelPC1 + "=" + item.PC1;
                                }
                                if (item.PC2 != null)
                                {
                                    content = content + "&" + item.LabelPC2 + "=" + decrypt;
                                }
                                queryString = new StringContent(content, Encoding.UTF8, "multipart/form-data");
                                queryString.Headers.Remove("Content-Type");
                                queryString.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=---------------------------114782935826962");
                            }
                        }
                        response = await client.PostAsync(uri, queryString);
                        respond = await response.Content.ReadAsStringAsync();
                    }
                    else if (item.Method == 1)
                    {
                        response = await client.GetAsync(uri).ConfigureAwait(false);
                        respond = await response.Content.ReadAsStringAsync();
                    }

                }

                StepExecutionOutputModel output = new StepExecutionOutputModel()
                {
                    ResponseContent = respond,
                    Status = "Authorized"
                };

                return output;
            } catch (UriFormatException e)
            {
                e.ToString();
                return null;
            }
        }

        public static void SetCatchByStepId(string StepId, string CatchId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            CatchesModel catchesModel = _db.Catches.Where(e => e.Id == CatchId).FirstOrDefault();

            _db.Steps.Where(e => e.Id == StepId).FirstOrDefault().Catches = catchesModel;
            _db.SaveChanges();
        }
    }
}