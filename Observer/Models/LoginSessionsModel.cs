using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class LoginSessionsModel
    {
        public string Id { get; set; }
        public virtual UsersModel Users { get; set; }
        public string Key { get; set; }
        public string EmailKey { get; set; }
        public string QRKey { get; set; }
        /*
         * Status
         * 1 - Active
         * 2 - QRConfirmation (Terminate Email Request)
         * 3 - QRLoginReady
         * 4 - EmailActive (Terminate QR Request)
         * 5 - EmailLoginReady
         * 6 - Expired
         */
        public int Status { get; set; }
        public string SessionCode { get; set; }
        public string UserAgent { get; set; }
        public string Ip { get; set; }
        public string Path { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime SessionDate { get; set; }
    }
}