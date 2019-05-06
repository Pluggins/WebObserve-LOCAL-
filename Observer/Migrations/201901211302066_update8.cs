namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ErrorLoggings", "Ip", c => c.String());
            AddColumn("dbo.ErrorLoggings", "UserAgent", c => c.String());
            AddColumn("dbo.ErrorLoggings", "Referrer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ErrorLoggings", "Referrer");
            DropColumn("dbo.ErrorLoggings", "UserAgent");
            DropColumn("dbo.ErrorLoggings", "Ip");
        }
    }
}
