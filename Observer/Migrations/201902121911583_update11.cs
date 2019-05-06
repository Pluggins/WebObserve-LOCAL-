namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginSessions", "Path", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoginSessions", "Path");
        }
    }
}
