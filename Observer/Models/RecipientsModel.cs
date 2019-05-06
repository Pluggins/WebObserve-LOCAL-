using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class RecipientsModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool? Deleted { get; set; }
        public virtual ObservesModel Observes { get; set; }
    }
}