using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class ServerStatusOutputModel
    {
        public int Status { get; set; }
        public int LastCheckedMin { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public string SuccessRate { get; set; }
    }
}