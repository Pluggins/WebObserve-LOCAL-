namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ObserveSessionDatas",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Key = c.String(),
                        Value = c.String(),
                        Date = c.DateTime(nullable: false),
                        Observes_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Observes", t => t.Observes_Id)
                .Index(t => t.Observes_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ObserveSessionDatas", "Observes_Id", "dbo.Observes");
            DropIndex("dbo.ObserveSessionDatas", new[] { "Observes_Id" });
            DropTable("dbo.ObserveSessionDatas");
        }
    }
}
