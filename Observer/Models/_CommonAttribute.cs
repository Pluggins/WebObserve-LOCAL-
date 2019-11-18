using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class _CommonAttribute
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool Deleted { get; set; }

        public _CommonAttribute()
        {
            DateCreated = DateTime.UtcNow.AddHours(8);
            CreatedBy = "SYSTEM";
            Deleted = false;
        }
    }
}