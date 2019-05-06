using Observer.Models.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class CatchRecordsModel
    {
        public string Id { get; set; }
        public virtual CatchesModel Catches { get; set; }
        public virtual StepsModel Steps { get; set; }
        public string Data { get; set; }
        public DateTime DateCreated { get; set; }
        public string PDF { get; set; }
    }
}