using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class StepsModel
    {
        public string Id { get; set; }
        public virtual ObservesModel Observes { get; set; }
        public string Url { get; set; }
        public int? Method { get; set; }
        public int? SetHeader { get; set; }
        public int? Order { get; set; }
        public string PredefinedHeader { get; set; }
        public virtual PCMethodsModel PC_Method { get; set; }
        public string PC1 { get; set; }
        public string PC2 { get; set; }
        public string PC2Secret { get; set; }
        public bool? Deleted { get; set; }
        public DateTime CheckedTime { get; set; }
        public virtual CatchesModel Catches { get; set; }
        public virtual ICollection<CatchRecordsModel> List_CatchRecordsModel { get; set; }
    }
}