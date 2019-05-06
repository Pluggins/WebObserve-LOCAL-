using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class LoginSessionsMap : EntityTypeConfiguration<LoginSessionsModel>
    {
        public LoginSessionsMap()
        {
            HasKey(e => e.Id);

            ToTable("LoginSessions");
        }
    }
}