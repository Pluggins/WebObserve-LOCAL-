using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Observer.Models.Mappings
{
    public class UsersMap : EntityTypeConfiguration<UsersModel>
    {
        public UsersMap()
        {
            HasKey(e => e.Id);

            //Unique
            HasIndex(e => e.Email);

            //Fk
            HasRequired(e => e.Fk_AspUser)
                .WithRequiredDependent();

            //Required
            Property(e => e.Id).IsRequired().HasMaxLength(128);
            Property(e => e.Email).IsRequired().HasMaxLength(128);
            Property(e => e.Name).IsRequired().HasMaxLength(50);

            ToTable("Users");
        }
    }
}