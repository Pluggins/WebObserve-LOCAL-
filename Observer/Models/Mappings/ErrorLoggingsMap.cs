using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class ErrorLoggingsMap : EntityTypeConfiguration<ErrorLoggingsModel>
    {
        public ErrorLoggingsMap()
        {
            HasKey(e => e.Id);

            ToTable("ErrorLoggings");
        }
    }
}