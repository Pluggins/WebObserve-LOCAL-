using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class StepResubmitInputModel
    {
        public string StepId { get; set; }
        public string Url { get; set; }
        public int Method { get; set; }
        public int Header { get; set; }
        public string PredefinedHeader { get; set; }
        public int PC_Method { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
    }
}