using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class StepExecutionInputModel
    {
        public int Order { get; set; }
        public string Uri { get; set; }
        public int Method { get; set; }
        public int PC_Method { get; set; }
        public int PredefinedHeader { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
    }
}