namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction13 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "UnitOfMeasurementId" });
            AddColumn("Construction.ProjectEstimnWorkItem", "IsSubCategory", c => c.Boolean(nullable: false));
            AddColumn("Construction.PriceAdjustmentWeightItem", "ItemCode", c => c.String(maxLength: 128));
            AlterColumn("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId", c => c.Int());
            CreateIndex("Construction.PriceAdjustmentWeightItem", "ItemCode");
            AddForeignKey("Construction.PriceAdjustmentWeightItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.PriceAdjustmentWeightItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Construction.PriceAdjustmentWeightItem", new[] { "ItemCode" });
            AlterColumn("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId", c => c.Int(nullable: false));
            DropColumn("Construction.PriceAdjustmentWeightItem", "ItemCode");
            DropColumn("Construction.ProjectEstimnWorkItem", "IsSubCategory");
            CreateIndex("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId");
            AddForeignKey("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId", "Construction.UnitOfMeasurement", "UnitOfMeasurementId", cascadeDelete: true);
        }
    }
}
