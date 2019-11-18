namespace Observer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update131 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IGN = c.String(),
                        Status = c.Int(nullable: false),
                        PIN = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "User_Id", "dbo.Users");
            DropIndex("dbo.Players", new[] { "User_Id" });
            DropTable("dbo.Players");
        }
    }
}
