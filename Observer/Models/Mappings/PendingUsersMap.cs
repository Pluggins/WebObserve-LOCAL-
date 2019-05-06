using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class PendingUsersMap : EntityTypeConfiguration<PendingUsersModel>
    {
        public PendingUsersMap()
        {
            HasKey(e => e.Id);

            ToTable("PendingUsers");
        }
    }
}