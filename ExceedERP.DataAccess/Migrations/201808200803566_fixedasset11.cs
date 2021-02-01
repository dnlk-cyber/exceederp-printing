namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.DepreciationForCostDistribution",
                c => new
                    {
                        DepreciationForCostDistributionId = c.Int(nullable: false, identity: true),
                        DepreciationForCostId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.DepreciationForCostDistributionId)
                .ForeignKey("EXCEED.DepreciationForCost", t => t.DepreciationForCostId, cascadeDelete: true)
                .Index(t => t.DepreciationForCostId);
            
            AddColumn("EXCEED.AnnualDepreciation", "AccumulatedDepreciationAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.DepreciationForCostDistribution", "DepreciationForCostId", "EXCEED.DepreciationForCost");
            DropIndex("EXCEED.DepreciationForCostDistribution", new[] { "DepreciationForCostId" });
            DropColumn("EXCEED.AnnualDepreciation", "AccumulatedDepreciationAmount");
            DropTable("EXCEED.DepreciationForCostDistribution");
        }
    }
}
