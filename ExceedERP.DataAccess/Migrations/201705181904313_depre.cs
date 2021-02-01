namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class depre : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.DepreciationForCost",
                c => new
                    {
                        DepreciationForCostId = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLPeriodId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationForCostId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.DepreciationForCost", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.DepreciationForCost", new[] { "AssetRegistrationId" });
            DropTable("EXCEED.DepreciationForCost");
        }
    }
}
