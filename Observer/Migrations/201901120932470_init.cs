namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Catches",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CatchRecords",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Data = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        PDF = c.String(),
                        Catches_Id = c.String(nullable: false, maxLength: 128),
                        Steps_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Catches", t => t.Catches_Id, cascadeDelete: true)
                .ForeignKey("dbo.Steps", t => t.Steps_Id, cascadeDelete: true)
                .Index(t => t.Catches_Id)
                .Index(t => t.Steps_Id);
            
            CreateTable(
                "dbo.Steps",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Url = c.String(),
                        Method = c.Int(nullable: false),
                        SetHeader = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        PredefinedHeader = c.String(),
                        PC1 = c.String(),
                        PC2 = c.String(),
                        PC2Secret = c.String(),
                        Deleted = c.Boolean(),
                        Catches_Id = c.String(maxLength: 128),
                        Observes_Id = c.String(nullable: false, maxLength: 128),
                        PC_Method_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Catches", t => t.Catches_Id)
                .ForeignKey("dbo.Observes", t => t.Observes_Id, cascadeDelete: true)
                .ForeignKey("dbo.PCMethods", t => t.PC_Method_Id)
                .Index(t => t.Catches_Id)
                .Index(t => t.Observes_Id)
                .Index(t => t.PC_Method_Id);
            
            CreateTable(
                "dbo.Observes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Status = c.Int(),
                        Interval = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        NextRun = c.DateTime(nullable: false),
                        Deleted = c.Boolean(),
                        Users_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Users_Id, cascadeDelete: true)
                .Index(t => t.Users_Id);
            
            CreateTable(
                "dbo.Recipients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 128),
                        Deleted = c.Boolean(),
                        Observes_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Observes", t => t.Observes_Id, cascadeDelete: true)
                .Index(t => t.Observes_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.Email);
            
            CreateTable(
                "dbo.PCMethods",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        PC1 = c.String(),
                        PC2 = c.String(),
                        ContentType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PCMethodDatas",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PCMethod = c.String(nullable: false, maxLength: 128),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PCMethods", t => t.PCMethod, cascadeDelete: true)
                .Index(t => t.PCMethod);
            
            CreateTable(
                "dbo.LoginSessions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Key = c.String(),
                        EmailKey = c.String(),
                        QRKey = c.String(),
                        Status = c.Int(nullable: false),
                        SessionCode = c.String(),
                        DateCreation = c.DateTime(nullable: false),
                        Users_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Users_Id)
                .Index(t => t.Users_Id);
            
            CreateTable(
                "dbo.PendingUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                        Email = c.String(),
                        Key = c.String(),
                        Status = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.WebConfigs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.LoginSessions", "Users_Id", "dbo.Users");
            DropForeignKey("dbo.CatchRecords", "Steps_Id", "dbo.Steps");
            DropForeignKey("dbo.Steps", "PC_Method_Id", "dbo.PCMethods");
            DropForeignKey("dbo.PCMethodDatas", "PCMethod", "dbo.PCMethods");
            DropForeignKey("dbo.Steps", "Observes_Id", "dbo.Observes");
            DropForeignKey("dbo.Observes", "Users_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recipients", "Observes_Id", "dbo.Observes");
            DropForeignKey("dbo.Steps", "Catches_Id", "dbo.Catches");
            DropForeignKey("dbo.CatchRecords", "Catches_Id", "dbo.Catches");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LoginSessions", new[] { "Users_Id" });
            DropIndex("dbo.PCMethodDatas", new[] { "PCMethod" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "Id" });
            DropIndex("dbo.Recipients", new[] { "Observes_Id" });
            DropIndex("dbo.Observes", new[] { "Users_Id" });
            DropIndex("dbo.Steps", new[] { "PC_Method_Id" });
            DropIndex("dbo.Steps", new[] { "Observes_Id" });
            DropIndex("dbo.Steps", new[] { "Catches_Id" });
            DropIndex("dbo.CatchRecords", new[] { "Steps_Id" });
            DropIndex("dbo.CatchRecords", new[] { "Catches_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropTable("dbo.WebConfigs");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PendingUsers");
            DropTable("dbo.LoginSessions");
            DropTable("dbo.PCMethodDatas");
            DropTable("dbo.PCMethods");
            DropTable("dbo.Users");
            DropTable("dbo.Recipients");
            DropTable("dbo.Observes");
            DropTable("dbo.Steps");
            DropTable("dbo.CatchRecords");
            DropTable("dbo.Catches");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
        }
    }
}
