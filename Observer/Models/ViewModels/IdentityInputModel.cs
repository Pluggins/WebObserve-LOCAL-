using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class LoginInputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterInputModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}