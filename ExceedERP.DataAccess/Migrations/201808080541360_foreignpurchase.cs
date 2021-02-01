namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignpurchase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ForeignPurchaseItemCost",
                c => new
                    {
                        ForeignPurchaseItemCostId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseFollowUpItemsID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        CostType = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ForexAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Paid = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseItemCostId)
                .ForeignKey("EXCEED.ForeignPurchaseFollowUpItems", t => t.ForeignPurchaseFollowUpItemsID, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseFollowUpItemsID);
            
            AddColumn("EXCEED.ForeignPurchaseCost", "ForexAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseCost", "Paid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "AdditionalUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "FinalUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ForeignPurchaseItemCost", "ForeignPurchaseFollowUpItemsID", "EXCEED.ForeignPurchaseFollowUpItems");
            DropIndex("EXCEED.ForeignPurchaseItemCost", new[] { "ForeignPurchaseFollowUpItemsID" });
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "Total");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "FinalUnitPrice");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "AdditionalUnitCost");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "SubTotal");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "TaxAmount");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "GLTaxRateID");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "UnitPrice");
            DropColumn("EXCEED.ForeignPurchaseCost", "Paid");
            DropColumn("EXCEED.ForeignPurchaseCost", "ForexAmount");
            DropTable("EXCEED.ForeignPurchaseItemCost");
        }
    }
}
