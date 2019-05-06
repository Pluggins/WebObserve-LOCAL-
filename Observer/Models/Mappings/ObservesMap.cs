using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class ObservesMap : EntityTypeConfiguration<ObservesModel>
    {
        public ObservesMap()
        {
            HasKey(e => e.Id);

            //fk
            HasRequired(e => e.Users);

            ToTable("Observes");
        }
    }
}