namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ErrorLoggings", "InnerException", c => c.String());
            AddColumn("dbo.StepSocketExceptions", "Message", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StepSocketExceptions", "Message");
            DropColumn("dbo.ErrorLoggings", "InnerException");
        }
    }
}
