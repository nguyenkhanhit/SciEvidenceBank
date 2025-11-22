namespace SciEvidenceBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class citationcount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evidences", "CitationCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evidences", "CitationCount");
        }
    }
}
