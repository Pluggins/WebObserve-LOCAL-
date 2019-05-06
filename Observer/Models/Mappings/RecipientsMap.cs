using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class RecipientsMap : EntityTypeConfiguration<RecipientsModel>
    {
        public RecipientsMap()
        {
            HasKey(e => e.Id);

            HasRequired(e => e.Observes);

            Property(e => e.Email)
                .HasMaxLength(128);

            ToTable("Recipients");
        }
    }
}