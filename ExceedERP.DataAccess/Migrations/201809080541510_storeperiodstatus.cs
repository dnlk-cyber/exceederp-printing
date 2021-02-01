namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeperiodstatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.InventoryStorePeriodStatus",
                c => new
                    {
                        InventoryStorePeriodStatusId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false, maxLength: 128),
                        InventoryPeriodId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventoryStorePeriodStatusId)
                .ForeignKey("EXCEED.InventoryPeriod", t => t.InventoryPeriodId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode, cascadeDelete: true)
                .Index(t => t.StoreCode)
                .Index(t => t.InventoryPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.InventoryStorePeriodStatus", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.InventoryStorePeriodStatus", "InventoryPeriodId", "EXCEED.InventoryPeriod");
            DropIndex("EXCEED.InventoryStorePeriodStatus", new[] { "InventoryPeriodId" });
            DropIndex("EXCEED.InventoryStorePeriodStatus", new[] { "StoreCode" });
            DropTable("EXCEED.InventoryStorePeriodStatus");
        }
    }
}
