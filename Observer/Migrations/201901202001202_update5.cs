namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ErrorLoggings", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ErrorLoggings", "Source");
        }
    }
}
