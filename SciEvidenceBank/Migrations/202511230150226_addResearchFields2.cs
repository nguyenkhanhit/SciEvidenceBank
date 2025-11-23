namespace SciEvidenceBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addResearchFields2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInterests",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ResearchFieldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ResearchFieldId })
                .ForeignKey("dbo.ResearchFields", t => t.ResearchFieldId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ResearchFieldId);
            
            AlterColumn("dbo.ResearchFields", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInterests", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInterests", "ResearchFieldId", "dbo.ResearchFields");
            DropIndex("dbo.UserInterests", new[] { "ResearchFieldId" });
            DropIndex("dbo.UserInterests", new[] { "UserId" });
            AlterColumn("dbo.ResearchFields", "Name", c => c.String(nullable: false, maxLength: 200));
            DropTable("dbo.UserInterests");
        }
    }
}
