using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class UsersModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string ApiCode { get; set; }
        public DateTime DateCreated { get; set; }
        [ForeignKey("Id")]
        public virtual ApplicationUser Fk_AspUser { get; set; }
        public virtual ICollection<ObservesModel> List_Observes { get; set; }
    }
}