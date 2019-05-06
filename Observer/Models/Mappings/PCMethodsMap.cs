using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class PCMethodsMap : EntityTypeConfiguration<PCMethodsModel>
    {
        public PCMethodsMap()
        {
            HasKey(e => e.Id);

            ToTable("PCMethods");
        }
    }
}