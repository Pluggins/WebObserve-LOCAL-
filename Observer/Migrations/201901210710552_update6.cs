namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StepSocketExceptions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Url = c.String(),
                        Date = c.DateTime(nullable: false),
                        Steps_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Steps", t => t.Steps_Id, cascadeDelete: true)
                .Index(t => t.Steps_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StepSocketExceptions", "Steps_Id", "dbo.Steps");
            DropIndex("dbo.StepSocketExceptions", new[] { "Steps_Id" });
            DropTable("dbo.StepSocketExceptions");
        }
    }
}
