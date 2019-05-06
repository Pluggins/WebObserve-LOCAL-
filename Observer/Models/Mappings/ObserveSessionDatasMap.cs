using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class ObserveSessionDatasMap : EntityTypeConfiguration<ObserveSessionDatasModel>
    {
        public ObserveSessionDatasMap()
        {
            HasKey(e => e.Id);

            ToTable("ObserveSessionDatas");
        }
    }
}