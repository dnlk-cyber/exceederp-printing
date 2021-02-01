namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class need : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.AnnualNeedAssesmentItem", new[] { "ItemCode" });
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategory", c => c.String(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesment", "PeriodName", c => c.String());
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.AnnualNeedAssesment", "PeriodName");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItemDesc");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryDesc");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItem");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategory");
            CreateIndex("EXCEED.AnnualNeedAssesmentItem", "ItemCode");
            AddForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
