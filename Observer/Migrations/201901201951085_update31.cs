namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update31 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLoggingsModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ErrorLoggingsModels");
        }
    }
}
