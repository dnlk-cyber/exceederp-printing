namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tendercancellation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.TenderCancellation",
                c => new
                    {
                        TenderCancellationId = c.Int(nullable: false, identity: true),
                        TenderID = c.Int(nullable: false),
                        FileName = c.String(),
                        Description = c.String(nullable: false),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderCancellationId)
                .ForeignKey("EXCEED.Tender", t => t.TenderID, cascadeDelete: true)
                .Index(t => t.TenderID);
            
            AddColumn("EXCEED.Tender", "IsCancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.TenderCancellation", "TenderID", "EXCEED.Tender");
            DropIndex("EXCEED.TenderCancellation", new[] { "TenderID" });
            DropColumn("EXCEED.Tender", "IsCancelled");
            DropTable("EXCEED.TenderCancellation");
        }
    }
}
