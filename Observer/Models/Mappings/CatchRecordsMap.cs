using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class CatchRecordsMap : EntityTypeConfiguration<CatchRecordsModel>
    {
        public CatchRecordsMap()
        {
            HasKey(e => e.Id);

            HasRequired(e => e.Catches);
            HasRequired(e => e.Steps);

            ToTable("CatchRecords");
        }
    }
}