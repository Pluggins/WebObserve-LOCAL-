using Observer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Observer.Services
{
    public class UserAccountServiceManager
    {
        public static void CreateNewUser(UsersModel users)
        {
            ObserverDbContext _db = new ObserverDbContext();
            _db.Users.Add(users);
            _db.SaveChanges();
        }

        public static UsersModel getUserByEmail(string Email)
        {
            using (ObserverDbContext _db = new ObserverDbContext())
            {
                UsersModel user = new UsersModel();
                var query = _db.Users
                    .Where(e => e.Email == Email)
                    .FirstOrDefault();
                user = query;
                return user;
            }
        }

        public static int getUserAvailability(string Email)
        {
            ObserverDbContext _db = new ObserverDbContext();

            var query1 = _db.Users
            .Where(e => e.Email == Email)
            .Count();

            /*
                * 0 = Available
                * 1 = Email taken
                */
                 
            if (query1 > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static bool IsAdmin(string AspId)
        {
            using (ObserverDbContext _db = new ObserverDbContext())
            {
                var query = _db.AspNetUserRoles
                    .Where(e => e.UserId == AspId && e.RoleId == "1001")
                    .Count();

                if (query == 1)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
    }
}