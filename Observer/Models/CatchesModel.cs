using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class CatchesModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CatchRecordsModel> List_CatchRecords { get; set; }
    }
}