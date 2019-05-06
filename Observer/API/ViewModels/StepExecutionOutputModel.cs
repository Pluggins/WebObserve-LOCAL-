using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class StepExecutionOutputModel
    {
        public string ResponseHeader { get; set; }
        public string ResponseContent { get; set; }
        public string Status { get; set; }
    }
}