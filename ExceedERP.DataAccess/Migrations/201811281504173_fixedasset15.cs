namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssueDistribution", "FixedAssetIssueID", "EXCEED.FixedAssetIssue");
            DropForeignKey("EXCEED.FixedAssetReturnDistribution", "FixedAssetReturnId", "EXCEED.FixedAssetReturn");
            DropIndex("EXCEED.FixedAssetIssueDistribution", new[] { "FixedAssetIssueID" });
            DropIndex("EXCEED.FixedAssetReturnDistribution", new[] { "FixedAssetReturnId" });
            DropTable("EXCEED.FixedAssetIssueDistribution");
            DropTable("EXCEED.FixedAssetReturnDistribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.FixedAssetReturnDistribution",
                c => new
                    {
                        FixedAssetReturnDistributionId = c.Int(nullable: false, identity: true),
                        FixedAssetReturnId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReturnDistributionId);
            
            CreateTable(
                "EXCEED.FixedAssetIssueDistribution",
                c => new
                    {
                        FixedAssetIssueDistributionID = c.Int(nullable: false, identity: true),
                        FixedAssetIssueID = c.Int(nullable: false),
                        FixedAssetIssueItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FixedAssetIssueDistributionID);
            
            CreateIndex("EXCEED.FixedAssetReturnDistribution", "FixedAssetReturnId");
            CreateIndex("EXCEED.FixedAssetIssueDistribution", "FixedAssetIssueID");
            AddForeignKey("EXCEED.FixedAssetReturnDistribution", "FixedAssetReturnId", "EXCEED.FixedAssetReturn", "FixedAssetReturnId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueDistribution", "FixedAssetIssueID", "EXCEED.FixedAssetIssue", "FixedAssetIssueID", cascadeDelete: true);
        }
    }
}
