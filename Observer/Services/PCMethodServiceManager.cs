using Observer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Services
{
    public static class PCMethodServiceManager
    {
        public static PCMethodsModel GetPCMethodById(string Id)
        {
            ObserverDbContext _db = new ObserverDbContext();

            return _db.PCMethods
                .Where(e => e.Id == Id)
                .FirstOrDefault();
        }

        public static List<PCMethodsModel> GetPCMethodList()
        {
            ObserverDbContext _db = new ObserverDbContext();

            return _db.PCMethods
                .ToList();
        }
    }
}