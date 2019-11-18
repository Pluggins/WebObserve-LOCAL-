using Microsoft.AspNet.Identity.EntityFramework;
using Observer.Models.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace Observer.Models
{
    public class ObserverDbContext : IdentityDbContext
    {
        public string _AESSecretKeyS = "4tSrXC7FUqxPKSX+K3SeLTqXNev3TD6bpJEbh+NsGmg=";

        public ObserverDbContext() :base("DefaultConnection")
        {
        }
        
        public DbSet<ApplicationUser> AspNetUsers { get; set; }
        public DbSet<IdentityUserRole> AspNetUserRoles { get; set; }
        public new DbSet<UsersModel> Users { get; set; }
        public DbSet<PlayersModel> Players { get; set; }
        public DbSet<ObservesModel> Observes { get; set; }
        public DbSet<StepsModel> Steps { get; set; }
        public DbSet<PCMethodsModel> PCMethods { get; set; }
        public DbSet<PCMethodDatasModel> PCMethodDatas { get; set; }
        public DbSet<RecipientsModel> Recipients { get; set; }
        public DbSet<CatchesModel> Catches { get; set; }
        public DbSet<CatchRecordsModel> CatchRecords { get; set; }
        public DbSet<PendingUsersModel> PendingUsers { get; set; }
        public DbSet<LoginSessionsModel> LoginSessions { get; set; }
        public DbSet<WebConfigsModel> WebConfigs { get; set; }
        public DbSet<ErrorLoggingsModel> ErrorLoggings { get; set; }
        public DbSet<StepSocketExceptionsModel> StepSocketExceptions { get; set; }
        public DbSet<ObserveSessionDatasModel> ObserveSessionDatas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UsersMap());
            modelBuilder.Configurations.Add(new PlayersMap());
            modelBuilder.Configurations.Add(new ObservesMap());
            modelBuilder.Configurations.Add(new StepsMap());
            modelBuilder.Configurations.Add(new PCMethodsMap());
            modelBuilder.Configurations.Add(new PCMethodDatasMap());
            modelBuilder.Configurations.Add(new RecipientsMap());
            modelBuilder.Configurations.Add(new CatchesMap());
            modelBuilder.Configurations.Add(new CatchRecordsMap());
            modelBuilder.Configurations.Add(new PendingUsersMap());
            modelBuilder.Configurations.Add(new LoginSessionsMap());
            modelBuilder.Configurations.Add(new WebConfigsMap());
            modelBuilder.Configurations.Add(new ErrorLoggingsMap());
            modelBuilder.Configurations.Add(new StepSocketExceptionsMap());
            modelBuilder.Configurations.Add(new ObserveSessionDatasMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}