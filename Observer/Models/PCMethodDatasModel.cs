using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class PCMethodDatasModel
    {
        public string Id { get; set; }
        public string PCMethod { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        [ForeignKey("PCMethod")]
        public virtual PCMethodsModel Fk_PCMethods { get; set; }
    }
}