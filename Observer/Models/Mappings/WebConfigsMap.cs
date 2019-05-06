using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class WebConfigsMap : EntityTypeConfiguration<WebConfigsModel>
    {
        public WebConfigsMap()
        {
            HasKey(e => e.Id);

            ToTable("WebConfigs");
        }
    }
}