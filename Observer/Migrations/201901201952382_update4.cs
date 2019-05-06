namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ErrorLoggingsModels", newName: "ErrorLoggings");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ErrorLoggings", newName: "ErrorLoggingsModels");
        }
    }
}
