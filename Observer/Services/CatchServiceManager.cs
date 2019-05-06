using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Observer.Models;

namespace Observer.Services
{
    public static class CatchServiceManager
    {
        public static List<CatchesModel> GetCatchesList()
        {
            ObserverDbContext _db = new ObserverDbContext();

            return _db.Catches.ToList();
        }

        public static void Catch(string StepId, string Html)
        {
            bool newEntry = false;
            ObserverDbContext _db = new ObserverDbContext();
            string ObserveId = ObserveServiceManager.GetObserveByStepId(StepId).Id;
            string ObserveName = ObserveServiceManager.GetObserveByStepId(StepId).Name;
            List<RecipientsModel> recipientsList = ObserveServiceManager.GetObserveByStepId(StepId).List_Recipients.Where(e => e.Deleted == false).ToList();
            
            string data = null;
            try
            {
                data = _db.Steps
                    .Where(e => e.Id == StepId)
                    .FirstOrDefault()
                    .List_CatchRecordsModel
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault()
                    .Data;
            } catch (NullReferenceException e)
            {
                e.ToString();
                newEntry = true;
            }

            string catches = StepServiceManager.GetStepById(StepId).Catches.Id;
            CatchesModel catchesModel = _db.Catches
                .Where(e => e.Id == catches)
                .FirstOrDefault();

            // TaQBin Tracking
            if (catches.Equals("E0779B1E-3288-403C-85A4-C07E697C9A5D"))
            {
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//div").ElementAt(6).InnerHtml;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();

                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            // HongLeong Bank Stm
            else if (catches.Equals("A08BB1EE-CC58-48A0-9B9F-ACD5FFC07DA4"))
            {
                string HTML = null;
                string HTML1 = null;
                string HTML2 = null;
                var htmlDoc = new HtmlDocument();

                try
                {
                    htmlDoc.LoadHtml(Html);
                    HTML1 = htmlDoc.DocumentNode
                        .SelectNodes("//table").ElementAt(0).InnerHtml;
                    htmlDoc.LoadHtml(HTML1);
                    var Html2 = htmlDoc.DocumentNode
                        .SelectNodes("//tr").ElementAt(3).InnerHtml;
                    var Html3 = htmlDoc.DocumentNode
                        .SelectNodes("//tr").ElementAt(4).InnerHtml;
                    htmlDoc.LoadHtml(Html);
                    HTML2 = htmlDoc.DocumentNode
                        .SelectNodes("//table").ElementAt(4).InnerHtml;
                    HTML = Html2 + Html3 + HTML2;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 120);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    if (!HTML.Contains("No records found."))
                    {
                        string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                        StepsModel stepsModel = _db.Steps
                            .Where(e => e.Id == StepId)
                            .FirstOrDefault();

                        CatchRecordsModel model = new CatchRecordsModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Data = HTML,
                            DateCreated = DateTime.UtcNow,
                            Catches = catchesModel,
                            Steps = stepsModel,
                            PDF = fileUrl
                        };

                        foreach (RecipientsModel item in recipientsList)
                        {
                            EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                        }

                        stepsModel.CheckedTime = DateTime.UtcNow;

                        _db.CatchRecords.Add(model);
                        _db.SaveChanges();
                    }
                }
            }
            // UCSI Academic Result
            else if (catches.Equals("CFAA4AB2-7F9D-4B3A-BA03-E4A490419C2C"))
            {
                string HTML = null;
                var htmlDoc = new HtmlDocument();

                try
                {
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//body").ElementAt(0).InnerHtml;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 180);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {

                    if (!HTML.Contains("&nbsp;&nbsp;&nbsp;Please provide your User ID and Password to proceed."))
                    {
                        string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                        StepsModel stepsModel = _db.Steps
                            .Where(e => e.Id == StepId)
                            .FirstOrDefault();

                        CatchRecordsModel model = new CatchRecordsModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Data = HTML,
                            DateCreated = DateTime.UtcNow,
                            Catches = catchesModel,
                            Steps = stepsModel,
                            PDF = fileUrl
                        };

                        foreach (RecipientsModel item in recipientsList)
                        {
                            EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                        }

                        stepsModel.CheckedTime = DateTime.UtcNow;

                        _db.CatchRecords.Add(model);
                        _db.SaveChanges();
                    }
                }
            }
            // Poslaju
            else if (catches.Equals("8DA4B8C8-1FFA-4336-958B-42678418B79C"))
            {
                string HTML = null;
                int count = 0;
                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//script").ElementAt(0).InnerHtml;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    try
                    {
                        while (!HTML.Contains("function cleanTrackingIds"))
                        {
                            HTML = htmlDoc.DocumentNode.SelectNodes("//script").ElementAt(count).InnerHtml;
                            count++;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        e.ToString();
                    }

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            // UCSI ELE
            else if (catches.Equals("E9356BC5-E5ED-470A-A97F-80BAF3CF21AF"))
            {
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//table").ElementAt(1).InnerHtml;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
                // Unitea Home
            }
            else if (catches.Equals("D469A7A5-76A6-4C03-BD23-C6B0B7F6E19F"))
            {
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//input").ElementAt(0).Attributes["value"].Value;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            // * Catch All Content *
            }
            else if (catches.Equals("4E0E2AAD-8389-4466-8C99-C4879A2ADEB2"))
            {
                string HTML = null;

                try
                {
                    HTML = Html;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            /* HongLeong Bank Balance (OLD)
            else if (catches.Equals("28CF45D5-0C68-4EC0-9F1F-CDA2EC23BB1C"))
            {
                string HTML1 = null;
                string HTML2 = null;
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML1 = htmlDoc.DocumentNode
                        .SelectNodes("//td").ElementAt(7).InnerHtml;
                    HTML2 = htmlDoc.DocumentNode
                        .SelectNodes("//td").ElementAt(9).InnerHtml;
                    HTML = HTML1 + HTML2;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }*/
            // HongLeong Bank Balance
            else if (catches.Equals("28CF45D5-0C68-4EC0-9F1F-CDA2EC23BB1C"))
            {
                string HTML1 = null;
                string HTML2 = null;
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML1 = htmlDoc.DocumentNode
                        .SelectNodes("//td").ElementAt(7).InnerHtml;
                    HTML2 = htmlDoc.DocumentNode
                        .SelectNodes("//td").ElementAt(9).InnerHtml;
                    HTML = HTML1 + HTML2;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            // UCSI ELE Viewstate
            else if (catches.Equals("37A8B073-AC68-4AD2-A793-3AD664305856"))
            {
                string HTML1 = null;
                string HTML2 = null;
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML1 = htmlDoc.DocumentNode.SelectNodes("//input").ElementAt(0).Attributes["value"].Value;
                    HTML2 = htmlDoc.DocumentNode.SelectNodes("//input").ElementAt(3).Attributes["value"].Value;
                    HTML = HTML1 + HTML2;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                ObserveSessionDatasModel newModel = new ObserveSessionDatasModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Observes = _db.Steps.Where(e => e.Id == StepId).FirstOrDefault().Observes,
                    Key = "ViewState"
                };
            }
            // Wiwaa Bank Submit
            else if (catches.Equals("6B5DE6DE-84D4-4CBB-A8B8-812E45CDB136"))
            {
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//table").ElementAt(0).InnerHtml;
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            // A2Hosting Issue Log
            else if (catches.Equals("EE3A2DE8-EBE6-4126-A07A-5F3B535532CE"))
            {
                string HTML = null;

                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Html);
                    HTML = htmlDoc.DocumentNode
                        .SelectNodes("//div").ElementAt(8).InnerHtml;
                    HTML = HTML.Substring(HTML.Length - 200);
                    HTML = HashingAlgorithmServiceManager.GenerateSHA1(Encoding.UTF8.GetBytes(HTML));

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    stepsModel.CheckedTime = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }
                catch (NullReferenceException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    HTML = "1";
                    data = "1";
                }

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = HTML,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
            /* GDex Tracking (INCOMPLETE)
            else if (catches.Equals("AB46347E-6745-4ADD-BE34-E58EC49CBDFD"))
            {
                StepsModel stepsModel = _db.Steps
                    .Where(e => e.Id == StepId)
                    .FirstOrDefault();

                try
                {
                    bool retry = true;
                    int i = 0;
                    while (retry) {
                        int seq = int.Parse(JObject.Parse(Html).SelectToken("result[0].listPodData["+i+"].seq").ToString());
                        if (seq > int.Parse(data) || newEntry)
                        {
                            string date = JObject.Parse(Html).SelectToken("result[0].listPodData[" + i + "].dtScan").ToString();
                            string origin = JObject.Parse(Html).SelectToken("result[0].listPodData[" + i + "].origin").ToString();
                            string type = JObject.Parse(Html).SelectToken("result[0].listPodData[" + i + "].type").ToString();
                            string newType;

                            if (type.Equals("i_pod"))
                            {
                                newType = "Under Claim";
                            } else if (type.Equals("undl"))
                                retry = false;

                            CatchRecordsModel model = new CatchRecordsModel()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Data = seq.ToString(),
                                DateCreated = DateTime.UtcNow,
                                Catches = catchesModel,
                                Steps = stepsModel,
                                PDF = fileUrl
                            };
                        }
                        i++;
                    }
                } catch (NullReferenceException e)
                {
                    e.ToString();
                } catch (ArgumentNullException e)
                {
                    e.ToString();
                    DetectResponseIrregularity(StepId, 60);
                    data = "1";
                }

                stepsModel.CheckedTime = DateTime.UtcNow;
                _db.SaveChanges();

                if (HTML != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    

                    foreach (RecipientsModel item in recipientsList)
                    {
                        EmailServiceManager.CatchNotification(fileUrl, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }*/
            // GDex Tracking
            else if (catches.Equals("AB46347E-6745-4ADD-BE34-E58EC49CBDFD"))
            {
                if (Html != data || newEntry)
                {
                    string fileUrl = DocumentRenderServiceManager.A4Html2Pdf(Html);

                    StepsModel stepsModel = _db.Steps
                        .Where(e => e.Id == StepId)
                        .FirstOrDefault();

                    CatchRecordsModel model = new CatchRecordsModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Data = Html,
                        DateCreated = DateTime.UtcNow,
                        Catches = catchesModel,
                        Steps = stepsModel,
                        PDF = fileUrl
                    };

                    foreach (RecipientsModel item in recipientsList)
                    {
                        string shippingId = stepsModel.PC1;
                        string message = "The system has detected changes of shipping status.<br>Please visit <a href='https://web3.gdexpress.com/official/iframe/etracking_v4.php?input=" + shippingId + "&choice=cnGdex'>https://web3.gdexpress.com/official/iframe/etracking_v4.php?input=" + shippingId + "&choice=cnGdex</a>";
                        EmailServiceManager.CatchNotificationWithoutScreenshot(message, ObserveName, item.Email);
                    }

                    stepsModel.CheckedTime = DateTime.UtcNow;

                    _db.CatchRecords.Add(model);
                    _db.SaveChanges();
                }
            }
        }

        public static string GetDataByStepId(string StepId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            try
            {
                return _db.Steps
                    .Where(e => e.Id == StepId)
                    .FirstOrDefault()
                    .Catches
                    .List_CatchRecords
                    .OrderByDescending(e => e.DateCreated)
                    .FirstOrDefault()
                    .Data;

            } catch (NullReferenceException e)
            {
                e.ToString();
                return null;
            }
        }

        public static void DetectResponseIrregularity(string StepId, int Minutes)
        {
            ObserverDbContext _db = new ObserverDbContext();

            StepsModel stepsModel = _db.Steps
                .Where(e => e.Id == StepId)
                .FirstOrDefault();

            int minute = (int) DateTime.UtcNow.Subtract(stepsModel.CheckedTime).TotalMinutes;

            try
            {
                if (minute > Minutes)
                {
                    string ObservesId = ObserveServiceManager.GetObserveByStepId(StepId).Id;
                    ObservesModel model = _db.Observes.Where(r => r.Id == ObservesId).FirstOrDefault();
                    model.Status = 0;
                    _db.SaveChanges();
                    foreach (RecipientsModel item in model.List_Recipients.Where(e => e.Deleted == false).ToList())
                    {
                        EmailServiceManager.CatchErrorNotification(model.Name, item.Email);
                    }
                }
            } catch (NullReferenceException e)
            {
                e.ToString();
            }
        }
    }
}