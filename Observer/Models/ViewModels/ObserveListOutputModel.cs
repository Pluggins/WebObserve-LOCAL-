using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models.ViewModels
{
    public class ObserveListOutputModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string DateCreated { get; set; }
    }
}