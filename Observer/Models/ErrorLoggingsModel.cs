using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class ErrorLoggingsModel
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public DateTime Date { get; set; }
    }
}