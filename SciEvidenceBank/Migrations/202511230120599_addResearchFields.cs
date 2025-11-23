namespace SciEvidenceBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addResearchFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EvidenceResearchFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvidenceId = c.Int(nullable: false),
                        ResearchFieldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evidences", t => t.EvidenceId, cascadeDelete: true)
                .ForeignKey("dbo.ResearchFields", t => t.ResearchFieldId, cascadeDelete: true)
                .Index(t => new { t.EvidenceId, t.ResearchFieldId }, name: "IX_Evidence_Field");
            
            CreateTable(
                "dbo.ResearchFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EvidenceResearchFields", "ResearchFieldId", "dbo.ResearchFields");
            DropForeignKey("dbo.EvidenceResearchFields", "EvidenceId", "dbo.Evidences");
            DropIndex("dbo.EvidenceResearchFields", "IX_Evidence_Field");
            DropTable("dbo.ResearchFields");
            DropTable("dbo.EvidenceResearchFields");
        }
    }
}
