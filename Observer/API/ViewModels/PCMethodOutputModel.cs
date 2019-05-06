using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.API.ViewModels
{
    public class PCMethodOutputModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
    }
}