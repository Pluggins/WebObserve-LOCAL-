using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class StepSocketExceptionsMap : EntityTypeConfiguration<StepSocketExceptionsModel>
    {
        public StepSocketExceptionsMap()
        {
            HasKey(e => e.Id);

            HasRequired(e => e.Steps)
                .WithMany();

            ToTable("StepSocketExceptions");
        }
    }
}