using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class StepCreationInputModel
    {
        public string ObserveId { get; set; }
        public string Url { get; set; }
        public int Method { get; set; }
        public int Header { get; set; }
        public int Priority { get; set; }
        public string PredefinedHeader { get; set; }
        public int ContentMethod { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
    }
}