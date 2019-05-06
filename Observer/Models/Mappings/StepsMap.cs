using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class StepsMap : EntityTypeConfiguration<StepsModel>
    {
        public StepsMap()
        {
            HasKey(e => e.Id);

            //Fk
            HasRequired(e => e.Observes);

            //Required
            Property(e => e.Order).IsRequired();
            Property(e => e.Method).IsRequired();
            Property(e => e.SetHeader).IsRequired();

            ToTable("Steps");
        }
    }
}