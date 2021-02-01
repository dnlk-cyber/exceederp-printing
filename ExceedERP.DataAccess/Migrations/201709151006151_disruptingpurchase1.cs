namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disruptingpurchase1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ParticipatingSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.ParticipatingSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "ItemCode" });
            AddColumn("EXCEED.ParticipatingSupplier", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "ItemType", c => c.Int(nullable: false));
            DropColumn("EXCEED.ParticipatingSupplier", "ItemCategoryCode");
            DropColumn("EXCEED.ParticipatingSupplier", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ParticipatingSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.ParticipatingSupplier", "ItemType");
            DropColumn("EXCEED.ParticipatingSupplier", "UoM");
            DropColumn("EXCEED.ParticipatingSupplier", "ItemSpecification");
            DropColumn("EXCEED.ParticipatingSupplier", "IsApproved");
            DropColumn("EXCEED.ParticipatingSupplier", "PartNo");
            DropColumn("EXCEED.ParticipatingSupplier", "ItemName");
            DropColumn("EXCEED.ParticipatingSupplier", "ItmCode");
            DropColumn("EXCEED.ParticipatingSupplier", "CategoryName");
            DropColumn("EXCEED.ParticipatingSupplier", "CatCode");
            CreateIndex("EXCEED.ParticipatingSupplier", "ItemCode");
            CreateIndex("EXCEED.ParticipatingSupplier", "ItemCategoryCode");
            AddForeignKey("EXCEED.ParticipatingSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ParticipatingSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
