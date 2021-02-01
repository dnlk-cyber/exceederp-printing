namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dashboard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.InventoryItembalance",
                c => new
                    {
                        InventoryItembalanceId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.InventoryItembalanceId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .Index(t => t.ItemCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.InventoryItembalance", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.InventoryItembalance", new[] { "ItemCode" });
            DropTable("EXCEED.InventoryItembalance");
        }
    }
}
