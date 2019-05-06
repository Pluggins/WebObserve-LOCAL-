namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginSessions", "UserAgent", c => c.String());
            AddColumn("dbo.LoginSessions", "Ip", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoginSessions", "Ip");
            DropColumn("dbo.LoginSessions", "UserAgent");
        }
    }
}
