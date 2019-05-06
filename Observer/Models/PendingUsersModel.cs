using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class PendingUsersModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}