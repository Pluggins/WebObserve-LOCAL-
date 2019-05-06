using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class ReorderStepInputModel
    {
        public string ObserveId { get; set; }
        public int initOrder { get; set; }
        public int minOrder { get; set; }
        public int maxOrder { get; set; }
    }
}