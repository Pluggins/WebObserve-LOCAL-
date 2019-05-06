using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class StepSocketExceptionsModel
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public virtual StepsModel Steps { get; set; }
    }
}