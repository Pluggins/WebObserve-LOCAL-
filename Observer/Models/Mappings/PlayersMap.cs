using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class PlayersMap : EntityTypeConfiguration<PlayersModel>
    {
        public PlayersMap()
        {
            HasKey(e => e.Id);

            ToTable("Players");
        }
    }
}