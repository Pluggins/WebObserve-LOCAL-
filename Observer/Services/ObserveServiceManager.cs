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
using System.Collections;

namespace Observer.Services
{
    public static class ObserveServiceManager
    {
        public static List<ObservesModel> GetObserveListByAspId(string AspId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            List<ObservesModel> query = _db.Observes
                .Where(e => e.Users.Id == AspId && e.Deleted == false)
                .ToList();

            return query;
        }

        public static void CreateObserve(string AspId, string Name)
        {
            ObserverDbContext _db = new ObserverDbContext();

            UsersModel user = _db.Users.Where(e => e.Id == AspId).FirstOrDefault();

            ObservesModel model = new ObservesModel()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Status = 0,
                Users = user,
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                Interval = 0,
                NextRun = DateTime.UtcNow
            };

            _db.Observes.Add(model);
            _db.SaveChanges();
        }

        public static void DeleteObserveById(string Id)
        {
            ObserverDbContext _db = new ObserverDbContext();
            var query = _db.Observes
                .Where(e => e.Id == Id)
                .FirstOrDefault();

            query.Deleted = true;
            _db.SaveChanges();
        }

        public static ObservesModel GetObserveById(string Id)
        {
            ObserverDbContext _db = new ObserverDbContext();

            ObservesModel query = _db.Observes
                .Where(e => e.Id == Id)
                .FirstOrDefault();

            return query;
        }

        public static ObservesModel GetObserveByStepId(string StepId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            string observeId = _db.Steps
                .Where(e => e.Id == StepId)
                .FirstOrDefault()
                .Observes.Id;

            ObservesModel query = _db.Observes
                .Where(e => e.Id == observeId)
                .FirstOrDefault();

            return query;
        }

        public static ObservesModel GetObservesByRecipientId(string RecipientId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            string observeId = _db.Recipients
                .Where(e => e.Id == RecipientId)
                .FirstOrDefault()
                .Observes
                .Id;

            ObservesModel query = _db.Observes
                .Where(e => e.Id == observeId)
                .FirstOrDefault();

            return query;
        }

        public static void UpdateObserve(ObservesModel model)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query = _db.Observes
                .Where(e => e.Id == model.Id)
                .FirstOrDefault();

            query.Status = model.Status;
            query.Name = model.Name;

            _db.SaveChanges();
        }

        public static List<RecipientsModel> GetRecipientList(string ObserveId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            return _db.Recipients
                .Where(e => e.Observes.Id == ObserveId && e.Deleted == false)
                .ToList();
        }

        public static void AddRecipient(RecipientsModel model)
        {
            ObserverDbContext _db = new ObserverDbContext();
            ObservesModel observesModel = _db.Observes
                .Where(e => e.Id == model.Observes.Id)
                .FirstOrDefault();

            model.Observes = observesModel;

            _db.Recipients.Add(model);
            _db.SaveChanges();
        }

        public static void DeleteRecipientById(string RecipientId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            _db.Recipients.Where(e => e.Id == RecipientId).FirstOrDefault().Deleted = true;
            _db.SaveChanges();
        }

        public static void SetIntervalByObserveId(string ObserveId, int Interval)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query = _db.Observes.Where(e => e.Id == ObserveId).FirstOrDefault();
            query.Interval = Interval;
            query.NextRun = DateTime.UtcNow;

            _db.SaveChanges();
        }

        public static void ExecuteInterval(string ObserveId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query = _db.Observes.Where(e => e.Id == ObserveId).FirstOrDefault();
            int intervalType = query.Interval.GetValueOrDefault();

            if (intervalType == 1)
            {
                query.NextRun = DateTime.UtcNow.AddMinutes(1);
            } else if (intervalType == 2)
            {
                query.NextRun = DateTime.UtcNow.AddMinutes(5);
            } else if (intervalType == 3)
            {
                query.NextRun = DateTime.UtcNow.AddMinutes(30);
            } else if (intervalType == 4)
            {
                query.NextRun = DateTime.UtcNow.AddHours(1);
            } else if (intervalType == 5)
            {
                query.NextRun = DateTime.UtcNow.AddHours(6);
            } else if (intervalType == 6)
            {
                query.NextRun = DateTime.UtcNow.AddHours(12);
            } else if (intervalType == 7)
            {
                query.NextRun = DateTime.UtcNow.AddDays(1);
            } else if (intervalType == 8)
            {
                query.NextRun = DateTime.UtcNow.AddDays(2);
            } else if (intervalType == 9)
            {
                query.NextRun = DateTime.UtcNow.AddDays(7);
            }

            _db.SaveChanges();
        }

        public static async Task<int> RunExecution()
        {
            ObserverDbContext _db = new ObserverDbContext();

            int count = 0;

            var query1 = _db.Observes
                .Where(e => e.Status == 1 && e.Deleted == false)
                .ToList();

            var query2 = _db.Steps;

            foreach (var item in query1)
            {
                try
                {
                    if (item.NextRun <= DateTime.UtcNow && item.Interval != 0)
                    {
                        var queryTemp = query2.Where(e => e.Observes.Id == item.Id && e.Deleted == false).OrderBy(e => e.Order).ToList();
                        Uri uri = null;
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.CookieContainer = new CookieContainer();
                        StringContent queryString = null;
                        HttpClient client = new HttpClient(handler);
                        string respond = null;
                        HttpResponseMessage response = null;

                        foreach (var item2 in queryTemp)
                        {
                            string decrypt = null;

                            if (item2.SetHeader == 0)
                            {
                                handler = new HttpClientHandler();
                                client = new HttpClient(handler);
                            }

                            if (item2.PC2 != null)
                            {
                                decrypt = EncryptionAlgorithmServiceManager.DecryptStringFromBytes_Aes(Convert.FromBase64String(item2.PC2), Convert.FromBase64String(_db._AESSecretKeyS), Convert.FromBase64String(item2.PC2Secret));
                            }

                            uri = new Uri(item2.Url);

                            if (item2.SetHeader == 2)
                            {
                                handler.CookieContainer.SetCookies(uri, item2.PredefinedHeader);
                            }
                            if (item2.Method == 2)
                            {
                                if (int.Parse(item2.PC_Method.Id) == 1)
                                {
                                    email_password model = new email_password()
                                    {
                                        email = item2.PC1,
                                        password = decrypt
                                    };
                                    queryString = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                                }
                                else if (int.Parse(item2.PC_Method.Id) >= 3)
                                {
                                    var query = _db.PCMethods
                                        .Where(e => e.Id == item2.PC_Method.Id)
                                        .FirstOrDefault();

                                    var list = query.List_PCMethodDatas;

                                    if (query.ContentType == 1)
                                    {
                                        string s = "{ ";
                                        int counts = 0;

                                        foreach (var items in list)
                                        {
                                            if (counts == 0)
                                            {
                                                s = s + "\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                            }
                                            else
                                            {
                                                s = s + ",\"" + items.Key + "\": " + "\"" + items.Value + "\" ";
                                            }

                                            counts = counts + 1;
                                        }

                                        s = s + "}";
                                        queryString = new StringContent(s, Encoding.UTF8, "application/json");
                                    }
                                    else if (query.ContentType == 2)
                                    {
                                        string content = null;
                                        foreach (var items in list)
                                        {
                                            content = content + items.Value;
                                        }
                                        if (item2.PC1 != null)
                                        {
                                            content = content + "&" + query.PC1 + "=" + item2.PC1;
                                        }
                                        if (item2.PC2 != null)
                                        {
                                            content = content + "&" + query.PC2 + "=" + decrypt;
                                        }

                                        queryString = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
                                    }
                                    else if (query.ContentType == 3)
                                    {
                                        string content = "-----------------------------114782935826962Content-Disposition: form-data; name='__EVENTTARGET'-----------------------------114782935826962Content-Disposition: form-data; name='__EVENTARGUMENT'-----------------------------114782935826962Content-Disposition: form-data; name='__VIEWSTATEGENERATOR'CF6B32F0-----------------------------114782935826962Content-Disposition: form-data; name='__SCROLLPOSITIONX'0-----------------------------114782935826962Content-Disposition: form-data; name='__SCROLLPOSITIONY'226.39999389648438-----------------------------114782935826962Content-Disposition: form-data; name='__VIEWSTATEENCRYPTED'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$hflApplicationID'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$hflMode'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$ImgEvents.x'12-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$ImgEvents.y'6-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$GeneralAttachments$txtDescriptions'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$GeneralAttachments$fupAttachment'; filename=''Content-Type: application/octet-stream-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$GeneralAttachments$hflApplicationID'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$GeneralAttachments$hflHDNID'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$GeneralAttachments$hflArea'-----------------------------114782935826962Content-Disposition: form-data; name='ctl00$ContentPlaceHolder1$Submission$hflApplicationID'";
                                        foreach (var items in list)
                                        {
                                            content = content + items.Value;
                                        }
                                        if (item2.PC1 != null)
                                        {
                                            content = content + "&" + query.PC1 + "=" + item2.PC1;
                                        }
                                        if (item2.PC2 != null)
                                        {
                                            content = content + "&" + query.PC2 + "=" + decrypt;
                                        }

                                        queryString = new StringContent(content, Encoding.UTF8, "multipart/form-data");
                                        queryString.Headers.Remove("Content-Type");
                                        queryString.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=---------------------------114782935826962");
                                    }
                                }
                                try
                                {
                                    response = await client.PostAsync(uri, queryString).ConfigureAwait(false);
                                    respond = await response.Content.ReadAsStringAsync();
                                } catch (HttpRequestException e)
                                {
                                    StepsModel step = _db.Steps.Where(r => r.Id == item2.Id).FirstOrDefault();

                                    string innerException = null;
                                    if (e.InnerException != null)
                                    {
                                        if (e.InnerException.InnerException != null)
                                        {
                                            innerException = e.InnerException.InnerException.Message;
                                        }
                                    }
                                    StepSocketExceptionsModel model = new StepSocketExceptionsModel()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Steps = step,
                                        Url = item2.Url,
                                        Message = innerException,
                                        Date = DateTime.UtcNow
                                    };

                                    _db.StepSocketExceptions.Add(model);
                                    _db.SaveChanges();
                                } catch (Exception e)
                                {
                                    string innerException = null;

                                    if (e.InnerException != null)
                                    {
                                        innerException = e.InnerException.Message;
                                    }
                                    ErrorLoggingsModel model = new ErrorLoggingsModel()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Source = "Observe RunExecution",
                                        Message = e.Message,
                                        InnerException = innerException,
                                        Date = DateTime.UtcNow
                                    };

                                    _db.ErrorLoggings.Add(model);
                                    _db.SaveChanges();
                                }
                            }
                            else if (item2.Method == 1)
                            {
                                try
                                {
                                    response = await client.GetAsync(uri).ConfigureAwait(false);
                                    respond = await response.Content.ReadAsStringAsync();
                                }
                                catch (HttpRequestException e)
                                {
                                    StepsModel step = _db.Steps.Where(r => r.Id == item2.Id).FirstOrDefault();
                                    string innerException = null;
                                    if (e.InnerException != null)
                                    {
                                        if (e.InnerException.InnerException != null)
                                        {
                                            innerException = e.InnerException.InnerException.Message;
                                        }
                                    }
                                    StepSocketExceptionsModel model = new StepSocketExceptionsModel()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Steps = step,
                                        Url = item2.Url,
                                        Message = innerException,
                                        Date = DateTime.UtcNow
                                    };

                                    _db.StepSocketExceptions.Add(model);
                                    _db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    string innerException = null;

                                    if (e.InnerException != null)
                                    {
                                        innerException = e.InnerException.Message;
                                    }
                                    ErrorLoggingsModel model = new ErrorLoggingsModel()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Source = "Observe RunExecution",
                                        Message = e.Message,
                                        InnerException = innerException,
                                        Date = DateTime.UtcNow
                                    };

                                    _db.ErrorLoggings.Add(model);
                                    _db.SaveChanges();
                                }
                            }

                            if (item2.Catches != null)
                            {
                                CatchServiceManager.Catch(item2.Id, respond);
                            }
                        }
                        ExecuteInterval(item.Id);
                        count = count + 1;
                    }
                } catch (UriFormatException e)
                {
                    e.ToString();
                    item.Status = 0;
                    _db.SaveChanges();
                }
            }
            var webConfig = _db.WebConfigs.Where(e => e.Id == "E7461932-0A62-4AC0-9D34-23C8C2B839B2").FirstOrDefault();
            webConfig.Timestamp = DateTime.UtcNow;

            var webConfigSuccess = _db.WebConfigs.Where(e => e.Id == "9F0EF51A-E5D0-486E-A0C9-D1E003D3C0D8").FirstOrDefault();

            if (webConfigSuccess.Timestamp.AddHours(1) < DateTime.UtcNow)
            {
                webConfigSuccess.Value = count.ToString();
                webConfigSuccess.Timestamp = DateTime.UtcNow;
            } else
            {
                webConfigSuccess.Value = (int.Parse(webConfigSuccess.Value) + count).ToString();
            }

            _db.SaveChanges();
            return count;
        }
    }
}