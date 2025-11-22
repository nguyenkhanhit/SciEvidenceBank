namespace SciEvidenceBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Evidences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 350),
                        Authors = c.String(maxLength: 500),
                        Source = c.String(maxLength: 500),
                        Year = c.Int(),
                        AbstractText = c.String(),
                        Url = c.String(maxLength: 500),
                        FilePath = c.String(maxLength: 255),
                        IsPublished = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        ApprovedById = c.String(maxLength: 128),
                        ApprovedByName = c.String(maxLength: 200),
                        ApprovedAt = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        CreatedByName = c.String(maxLength: 200),
                        LikesCount = c.Int(nullable: false),
                        BookmarksCount = c.Int(nullable: false),
                        ViewsCount = c.Int(nullable: false),
                        CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.EvidenceTags",
                c => new
                    {
                        EvidenceId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EvidenceId, t.TagId })
                .ForeignKey("dbo.Evidences", t => t.EvidenceId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.EvidenceId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EvidenceTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.EvidenceTags", "EvidenceId", "dbo.Evidences");
            DropForeignKey("dbo.Evidences", "CategoryId", "dbo.Categories");
            DropIndex("dbo.EvidenceTags", new[] { "TagId" });
            DropIndex("dbo.EvidenceTags", new[] { "EvidenceId" });
            DropIndex("dbo.Evidences", new[] { "CategoryId" });
            DropTable("dbo.Tags");
            DropTable("dbo.EvidenceTags");
            DropTable("dbo.Evidences");
            DropTable("dbo.Categories");
        }
    }
}
