using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class PCMethodDatasMap : EntityTypeConfiguration<PCMethodDatasModel>
    {
        public PCMethodDatasMap()
        {
            HasKey(e => e.Id);

            HasRequired(e => e.Fk_PCMethods);

            ToTable("PCMethodDatas");
        }
    }
}