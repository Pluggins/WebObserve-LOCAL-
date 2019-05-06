using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class WebConfigsModel
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}