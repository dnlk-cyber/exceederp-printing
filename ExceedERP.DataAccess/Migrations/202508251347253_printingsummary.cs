namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingsummary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingSummaryComment",
                c => new
                    {
                        PrintingSummaryCommentId = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        PrintingEstimationSummaryId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingSummaryCommentId);
            
        }
        
        public override void Down()
        {
            DropTable("Printing.PrintingSummaryComment");
        }
    }
}
