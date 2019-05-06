using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class ObserveOutputModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int Interval { get; set; }
        public List<StepListOutputModel> StepList { get; set; }
        public List<PCMethodOutputModel> PCMethod { get; set; }
    }
}