using Observer.Models;
using Observer.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Observer.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace Observer.Controllers
{
    public class IdentityController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public IdentityController()
        {
        }

        public IdentityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [Route("Register")]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                ViewBag.Url = Request.Headers["Referer"].ToString();
            } catch (NullReferenceException e)
            {
                e.ToString();
                ViewBag.Url = Request.Url.Host;
            }
            return View("Register");
        }

        [Route("Login")]
        public async Task<ActionResult> Login(string errorMessage = null, string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { status = 2 });
            }


            ObserverDbContext _db = new ObserverDbContext();
            try
            {
                string sessionId = Request.Cookies.Get("SessionId").Value;
                string sessionKey = Request.Cookies.Get("SessionKey").Value;
                string myUserAgent = Request.UserAgent;
                var session = _db.LoginSessions
                    .Where(e => e.Id == sessionId)
                    .FirstOrDefault();
                

                if (session.UserAgent == myUserAgent && session.Key == sessionKey && (session.Status == 3 || session.Status == 5) && session.Status != 6 && session.SessionDate.AddMonths(2) > DateTime.UtcNow)
                {
                    ApplicationUser user = UserManager.FindById(session.Users.Id);
                    await SignInManager.SignInAsync(user, true, true);
                    string newId = Guid.NewGuid().ToString();
                    string newKey = HashingAlgorithmServiceManager.GenerateSHA256(Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()), Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()));
                    LoginSessionsModel newSession = new LoginSessionsModel()
                    {
                        Id = newId,
                        Key = newKey,
                        Status = 3,
                        Users = session.Users,
                        Ip = Request.UserHostAddress,
                        UserAgent = Request.UserAgent,
                        DateCreation = DateTime.UtcNow,
                        SessionDate = session.SessionDate
                    };
                    session.Status = 6;

                    HttpCookie SessionCookie = new HttpCookie("SessionId");
                    SessionCookie.Value = newId;
                    SessionCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionCookie);

                    HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
                    SessionKeyCookie.Value = newKey;
                    SessionKeyCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionKeyCookie);

                    _db.LoginSessions.Add(newSession);
                    _db.SaveChanges();

                    if (String.IsNullOrEmpty(ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home", new { status = 13, name = session.Users.Name });
                    } else
                    {
                        if (ReturnUrl.Contains("?"))
                        {
                            ReturnUrl = ReturnUrl + "&status=13&name=" + session.Users.Name;
                        } else
                        {
                            ReturnUrl = ReturnUrl + "?status=13&name=" + session.Users.Name;
                        }
                        return Redirect(ReturnUrl);
                    }
                } else
                {
                    string loginSessionId = Guid.NewGuid().ToString();
                    string key = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString();
                    Uri url = new Uri(Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/QRLogin?Id=" + loginSessionId + "&Key=" + key);
                    string image = QRCodeServiceManager.GenerateLoginQRCode(url.ToString());
                    LoginSessionsModel loginSession = new LoginSessionsModel()
                    {
                        Id = loginSessionId,
                        Status = 1,
                        Key = key,
                        Ip = Request.UserHostAddress,
                        UserAgent = Request.UserAgent,
                        DateCreation = DateTime.UtcNow,
                        SessionDate = DateTime.UtcNow,
                        Path = ReturnUrl
                    };
                    session.Status = 6;
                    _db.LoginSessions.Add(loginSession);
                    _db.SaveChanges();
                    ViewBag.LoginSessionId = loginSessionId;
                    ViewBag.Key = key;
                    ViewBag.QRImage = image;

                    HttpCookie SessionCookie = new HttpCookie("SessionId");
                    SessionCookie.Value = null;
                    SessionCookie.Expires = DateTime.Now.AddYears(-10);
                    Response.SetCookie(SessionCookie);

                    HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
                    SessionKeyCookie.Value = null;
                    SessionKeyCookie.Expires = DateTime.Now.AddYears(-10);
                    Response.SetCookie(SessionKeyCookie);

                    return View("Login");
                }
            } catch (NullReferenceException e)
            {
                e.ToString();
                string loginSessionId = Guid.NewGuid().ToString();
                string key = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString();
                Uri url = new Uri(Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/QRLogin?Id=" + loginSessionId + "&Key=" + key);
                string image = QRCodeServiceManager.GenerateLoginQRCode(url.ToString());
                LoginSessionsModel loginSession = new LoginSessionsModel()
                {
                    Id = loginSessionId,
                    Status = 1,
                    Key = key,
                    Ip = Request.UserHostAddress,
                    UserAgent = Request.UserAgent,
                    DateCreation = DateTime.UtcNow,
                    SessionDate = DateTime.UtcNow,
                    Path = ReturnUrl
                };
                _db.LoginSessions.Add(loginSession);
                _db.SaveChanges();
                ViewBag.LoginSessionId = loginSessionId;
                ViewBag.Key = key;
                ViewBag.QRImage = image;

                HttpCookie SessionCookie = new HttpCookie("SessionId");
                SessionCookie.Value = null;
                SessionCookie.Expires = DateTime.Now.AddYears(-10);
                Response.SetCookie(SessionCookie);

                HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
                SessionKeyCookie.Value = null;
                SessionKeyCookie.Expires = DateTime.Now.AddYears(-10);
                Response.SetCookie(SessionKeyCookie);

                return View("Login");
            }
        }
        
        [Route("VerifyEmail")]
        public async Task<ActionResult> VerifyEmail(string Id, string Key)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query = _db.PendingUsers
                .Where(e => e.Id == Id && e.Key == Key)
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            if (query.Count > 0)
            {
                var query2 = query.FirstOrDefault();
                if (query2.DateCreated.AddHours(1) < DateTime.UtcNow)
                {
                    return RedirectToAction("Index", "Home", new { status = 8 });
                }
                else if (query2.Id == Id && query2.Key == Key)
                {
                    query2.Status = 1;
                    var user = new ApplicationUser { UserName = query2.Email, Email = query2.Email, LockoutEndDateUtc = System.DateTime.UtcNow };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded) {
                        UsersModel newUser = new UsersModel()
                        {
                            Id = user.Id,
                            Name = query2.DisplayName,
                            Email = query2.Email,
                            Status = 1,
                            DateCreated = DateTime.UtcNow,
                            ApiCode = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()
                        };
                        _db.Users.Add(newUser);
                        _db.SaveChanges();

                        await SignInManager.SignInAsync(user, true, true);
                    }

                    return RedirectToAction("Index", "Home", new { status = 9 });
                } else
                {
                    return RedirectToAction("Index", "Home", new { status = 8 });
                }
            } else
            {
                return RedirectToAction("Index", "Home", new { status = 8 });
            }
        }

        [Route("LoginEmail")]
        public ActionResult EmailLogin(string Id, string Key)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var loginSession = _db.LoginSessions
                .Where(e => e.Id == Id)
                .FirstOrDefault();

            if (loginSession.EmailKey == Key && loginSession.Status == 4)
            {
                loginSession.Status = 5;
                _db.SaveChanges();
                return RedirectToAction("Index", "Home", new { status = 10 });
            } else
            {
                return RedirectToAction("Index", "Home", new { status = 11 });
            }
        }
        
        [Route("QRLogin")]
        public async Task<ActionResult> QRLogin(string Id, string Key)
        {
            ObserverDbContext _db = new ObserverDbContext();
            string userId = null;
            
            try
            {
                string sessionId = Request.Cookies.Get("SessionId").Value;
                string sessionKey = Request.Cookies.Get("SessionKey").Value;

                var session = _db.LoginSessions
                    .Where(e => e.Id == sessionId)
                    .FirstOrDefault();

                if (session.Key == sessionKey && (session.Status == 3 || session.Status == 5) && session.Status != 6 && session.SessionDate.AddMonths(2) > DateTime.UtcNow)
                {
                    ApplicationUser user = UserManager.FindById(session.Users.Id);
                    userId = user.Id;
                    await SignInManager.SignInAsync(user, true, true);
                    string newId = Guid.NewGuid().ToString();
                    string newKey = HashingAlgorithmServiceManager.GenerateSHA256(Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()), Encoding.ASCII.GetBytes(EncryptionAlgorithmServiceManager.GetRNGGuid().ToString()));
                    LoginSessionsModel newSession = new LoginSessionsModel()
                    {
                        Id = newId,
                        Key = newKey,
                        Status = 3,
                        Users = session.Users,
                        DateCreation = DateTime.UtcNow,
                        SessionDate = session.SessionDate
                    };
                    session.Status = 6;

                    HttpCookie SessionCookie = new HttpCookie("SessionId");
                    SessionCookie.Value = newId;
                    SessionCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionCookie);

                    HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
                    SessionKeyCookie.Value = newKey;
                    SessionKeyCookie.Expires = DateTime.Now.AddMonths(2);
                    Response.SetCookie(SessionKeyCookie);

                    _db.LoginSessions.Add(newSession);
                    _db.SaveChanges();
                }
                else
                {
                    RedirectToAction("Login", "Identity");
                }
            }
            catch (NullReferenceException e)
            {
                RedirectToAction("Login", "Identity");
            }
            
            var loginSession = _db.LoginSessions
                .Where(e => e.Id == Id)
                .FirstOrDefault();
            if (loginSession.SessionDate.AddMinutes(30) < DateTime.UtcNow)
            {
                loginSession.Status = 6;
                _db.SaveChanges();
                return RedirectToAction("Index", "Home", new { status = 12 });
            } else
            {
                var user = _db.Users
                .Where(e => e.Id == userId)
                .FirstOrDefault();

                string proceedKey = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString();
                Random random = new Random();
                int randomInt = random.Next(999);
                while (randomInt < 100)
                {
                    randomInt = random.Next(999);
                }
                string sessionCode = randomInt.ToString();

                if (loginSession.Status == 1)
                {
                    loginSession.Status = 2;
                    loginSession.Users = user;
                    loginSession.QRKey = proceedKey;
                    loginSession.SessionCode = sessionCode;
                    loginSession.SessionDate = DateTime.UtcNow.AddSeconds(30);
                    _db.SaveChanges();
                }

                ViewBag.SessionId = Id;
                ViewBag.ProceedKey = proceedKey;
                ViewBag.SessionCode = sessionCode;
                return View();
            }
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Register(RegisterInputModel Input)
        {
            ObserverDbContext _db = new ObserverDbContext();

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var pendingUsers = _db.PendingUsers
                .Where(e => e.Email == Input.Email)
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            var user = _db.Users
                .Where(e => e.Email == Input.Email)
                .ToList();

            if (user.Count > 0)
            {
                ViewBag.ErrorMessage = "Error: The email has been used.";
                return View();
            }
            else if (pendingUsers.Count >= 1)
            {
                var pendingUser = pendingUsers.FirstOrDefault();
                if (pendingUser.Status == 1)
                {
                    ViewBag.ErrorMessage = "Error: The email has been used.";
                    return View();
                } else if (pendingUser.Status == 0 || pendingUser.Status == 2)
                {
                    if (pendingUser.DateCreated.AddHours(1) < DateTime.UtcNow)
                    {
                        pendingUser.Status = 2;
                        
                        string id = Guid.NewGuid().ToString();
                        string key = EncryptionAlgorithmServiceManager.GetRNGGuid().ToString();
                        PendingUsersModel newUser = new PendingUsersModel()
                        {
                            Id = id,
                            DisplayName = Input.Name,
                            Email = Input.Email,
                            Key = key,
                            Status = 0,
                            DateCreated = DateTime.UtcNow
                        };

                        _db.PendingUsers.Add(newUser);
                        _db.SaveChanges();
                        EmailServiceManager.AccountPendingVerification(Input.Name, Request.Url.Scheme+"://"+Request.Url.Host+":"+Request.Url.Port+"/VerifyEmail?Id="+id+"&Key="+key, Input.Email);
                        return RedirectToAction("Index", "Home", new { Status = 7, email = Input.Email });

                    } else
                    {
                        int minute = DateTime.UtcNow.Subtract(pendingUser.DateCreated).Minutes;
                        return RedirectToAction("Index", "Home", new { Status = 6, min = minute });
                    }
                }
                return View();
            } else
            {
                string key = Guid.NewGuid().ToString();
                string id = Guid.NewGuid().ToString();
                PendingUsersModel newUser = new PendingUsersModel()
                {
                    Id = id,
                    DisplayName = Input.Name,
                    Email = Input.Email,
                    Key = key,
                    Status = 0,
                    DateCreated = DateTime.UtcNow
                };

                _db.PendingUsers.Add(newUser);
                _db.SaveChanges();
                EmailServiceManager.AccountPendingVerification(Input.Name, Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/VerifyEmail?Id=" + id + "&Key=" + key, Input.Email);
                return RedirectToAction("Index", "Home", new { Status = 7, email = Input.Email });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginInputModel Input)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            UsersModel user = UserAccountServiceManager.getUserByEmail(Input.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Error: Username/Password is incorrect.";
                return View();
            }
            var result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, true, true);
            if (result != SignInStatus.Success)
            {
                ViewBag.ErrorMessage = "Error: Username/Password is incorrect.";
                return View();
            } else
            {
                if (Request.UrlReferrer.ToString().Contains("?"))
                {
                    if (Request.UrlReferrer.ToString().Contains("status="))
                    {
                        int match = Regex.Match(Request.UrlReferrer.ToString(), "status=").Index;

                        if (Request.UrlReferrer.ToString().Remove(0, match + 8).Contains("?")) {
                            return Redirect(Request.UrlReferrer.ToString().Remove(0, match + 8) + "&status=2");
                        } else
                        {
                            return Redirect(Request.UrlReferrer.ToString().Remove(0, match + 8) + "?status=2");
                        }
                    } else
                    {
                        return Redirect(Request.UrlReferrer.ToString() + "&status=2");
                    }
                } else
                {
                    return Redirect(Request.UrlReferrer.ToString() + "?status=2");
                }
                
            }
        }

        [Route("Logout")]
        public ActionResult Logout(string from = null)
        {
            if (from == "fb")
            {
                //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { status = 4 });
            } else if (from == "fbexpired")
            {
                return RedirectToAction("Index", "Home", new { status = 5 });
            }
            ObserverDbContext _db = new ObserverDbContext();
            
            HttpCookie SessionCookie = new HttpCookie("SessionId");
            SessionCookie.Value = null;
            SessionCookie.Expires = DateTime.Now.AddYears(-10);
            Response.SetCookie(SessionCookie);

            HttpCookie SessionKeyCookie = new HttpCookie("SessionKey");
            SessionKeyCookie.Value = null;
            SessionKeyCookie.Expires = DateTime.Now.AddYears(-10);
            Response.SetCookie(SessionKeyCookie);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { status = 3 });
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}