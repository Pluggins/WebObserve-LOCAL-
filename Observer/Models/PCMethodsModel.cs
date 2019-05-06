using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class PCMethodsModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
        public int ContentType { get; set; }
        public virtual ICollection<PCMethodDatasModel> List_PCMethodDatas { get; set; }
    }
}