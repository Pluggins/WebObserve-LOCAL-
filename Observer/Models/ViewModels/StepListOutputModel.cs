using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class StepListOutputModel
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string SetHeader { get; set; }
    }
}