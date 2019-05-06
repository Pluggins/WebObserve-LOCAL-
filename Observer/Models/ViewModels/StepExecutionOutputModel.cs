using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class StepExecutionOutputModel
    {
        public string ResponseHeader { get; set; }
        public string ResponseContent { get; set; }
        public string Status { get; set; }
    }
}