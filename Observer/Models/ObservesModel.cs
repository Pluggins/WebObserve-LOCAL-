using Observer.Models.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class ObservesModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public virtual UsersModel Users { get; set; }
        public int? Interval { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime NextRun { get; set; }
        public bool? Deleted { get; set; }
        public virtual ICollection<StepsModel> List_Steps { get; set; }
        public virtual ICollection<RecipientsModel> List_Recipients { get; set; }
        public virtual ICollection<ObserveSessionDatasModel> List_ObserveSessionDatas { get; set; }
    }
}