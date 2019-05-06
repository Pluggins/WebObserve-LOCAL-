using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using Observer.Models;
using Observer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Observer.API.Controllers
{
    public class LoginApiController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LoginApiController()
        {

        }

        public LoginApiController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [Route("Api/Login/CheckStatus")]
        public async Task<JsonResult> CheckLoginStatus(string SessionId, string SessionKey)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query = _db.LoginSessions
                .Where(e => e.Id == SessionId)
                .FirstOrDefault();

            if (query.Key == SessionKey)
            {
                if (query.SessionDate.AddMinutes(30) < DateTime.UtcNow)
                {
                    query.Status = 6;
                    _db.SaveChanges();
                    return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Expired' }"));
                }
                else if (query.Status == 3 || query.Status == 5)
                {
                    ApplicationUser user = UserManager.Users
                        .Where(e => e.Id == query.Users.Id)
                        .FirstOrDefault();

                    await SignInManager.SignInAsync(user, true, true);
                    string key = HashingAlgorithmServiceManager.GenerateSHA256(Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()), Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()));
                    query.Key = key;
                    _db.SaveChanges();

                    HttpCookie SessionCookie = new HttpCookie("SessionId");
                    SessionCookie.Value = query.Id;
                    SessionCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionCookie);

                    HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
                    SessionKeyCookie.Value = key;
                    SessionKeyCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionKeyCookie);

                    return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'LoginReady', 'Path': '"+query.Path+"' }"));
                }
                else if (query.Status == 1 || query.Status == 2 || query.Status == 4)
                {
                    if (query.Status == 2)
                    {
                        return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'QRPending', 'SessionCode': '"+query.SessionCode+"' }"));
                    } else
                    {
                        return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Pending' }"));
                    }
                }
                else
                {
                    return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Expired' }"));
                }
            } else
            {
                return Json(new JavaScriptSerializer().Deserialize<object>("{ 'Status': 'Invalid Session' }"));
            }
        }

        [HttpPost]
        [Route("Api/Login/EmailLogin")]
        public int EmailLoginRequest(string SessionId, string Email, string SessionKey)
        {
            ObserverDbContext _db = new ObserverDbContext();
            var session = _db.LoginSessions
                .Where(e => e.Id == SessionId)
                .FirstOrDefault();

            if (session.Status == 6)
            {
                // Session expired
                return 0;
            }

            if (session.Status != 1)
            {
                // Session type error
                return 1;
            }

            if (session.Key != SessionKey)
            {
                // Session expired
                return 0;
            }

            var users = _db.Users
                .Where(e => e.Email == Email)
                .ToList();

            if (users.Count > 0)
            {
                var user = users.FirstOrDefault();
                if (user.Status == 1)
                {
                    session.EmailKey = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString();
                    session.Status = 4;
                    session.Users = user;
                    _db.SaveChanges();
                    EmailServiceManager.AccountEmailLogin(user.Name, Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/LoginEmail?Id=" + SessionId + "&Key=" + session.EmailKey, user.Email);
                    return 4;
                } else
                {
                    // Inactive user
                    return 3;
                }
            } else
            {
                // Cannot find user
                return 2;
            }
        }

        [Authorize]
        [Route("Api/Login/QRLogin")]
        public JsonResult QRLogin(string SessionId, string SessionKey)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var session = _db.LoginSessions
                .Where(e => e.Id == SessionId)
                .FirstOrDefault();

            if (session.QRKey == SessionKey && session.SessionDate > DateTime.UtcNow)
            {
                session.Status = 3;
                session.SessionDate = DateTime.UtcNow;
                _db.SaveChanges();
                return Json(new JavaScriptSerializer().DeserializeObject("{ 'Status': 'Authorized' }"));
            } else
            {
                return Json(new JavaScriptSerializer().DeserializeObject("{ 'Status': 'Unauthorized' }"));
            }
        }

        [Route("Api/Login/QRLoginCancel")]
        public int QRLoginCancel(string SessionId)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var session = _db.LoginSessions
                .Where(e => e.Id == SessionId)
                .FirstOrDefault();

            if (session.Status == 2)
            {
                session.Status = 6;
                _db.SaveChanges();
            }

            return 1;
        }
    }
}