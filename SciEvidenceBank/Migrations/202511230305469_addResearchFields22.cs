namespace SciEvidenceBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addResearchFields22 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInterests", "ResearchFieldId", "dbo.ResearchFields");
            DropIndex("dbo.UserInterests", new[] { "ResearchFieldId" });
            RenameColumn(table: "dbo.UserInterests", name: "ResearchFieldId", newName: "ResearchField_Id");
            DropPrimaryKey("dbo.UserInterests");
            AddColumn("dbo.UserInterests", "CategoryId", c => c.Int(nullable: false));
            AlterColumn("dbo.UserInterests", "ResearchField_Id", c => c.Int());
            AddPrimaryKey("dbo.UserInterests", new[] { "UserId", "CategoryId" });
            CreateIndex("dbo.UserInterests", "CategoryId");
            CreateIndex("dbo.UserInterests", "ResearchField_Id");
            AddForeignKey("dbo.UserInterests", "CategoryId", "dbo.Categories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInterests", "ResearchField_Id", "dbo.ResearchFields", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInterests", "ResearchField_Id", "dbo.ResearchFields");
            DropForeignKey("dbo.UserInterests", "CategoryId", "dbo.Categories");
            DropIndex("dbo.UserInterests", new[] { "ResearchField_Id" });
            DropIndex("dbo.UserInterests", new[] { "CategoryId" });
            DropPrimaryKey("dbo.UserInterests");
            AlterColumn("dbo.UserInterests", "ResearchField_Id", c => c.Int(nullable: false));
            DropColumn("dbo.UserInterests", "CategoryId");
            AddPrimaryKey("dbo.UserInterests", new[] { "UserId", "ResearchFieldId" });
            RenameColumn(table: "dbo.UserInterests", name: "ResearchField_Id", newName: "ResearchFieldId");
            CreateIndex("dbo.UserInterests", "ResearchFieldId");
            AddForeignKey("dbo.UserInterests", "ResearchFieldId", "dbo.ResearchFields", "Id", cascadeDelete: true);
        }
    }
}
