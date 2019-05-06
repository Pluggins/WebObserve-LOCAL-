namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ApiCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ApiCode");
        }
    }
}
