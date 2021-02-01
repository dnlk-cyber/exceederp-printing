namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class service : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.ServiceReceiveItem", "ServiceItemCode", "EXCEED.ServiceItem");
            DropIndex("EXCEED.ServiceReceiveItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.ServiceReceiveItem", new[] { "ServiceItemCode" });
            AddColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryName", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "ServiceItemName", c => c.String());
            AlterColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", c => c.String());
            AlterColumn("EXCEED.ServiceReceiveItem", "ServiceItemCode", c => c.String());
            DropColumn("EXCEED.ServiceReceive", "PurchaseRequisitionID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ServiceReceive", "PurchaseRequisitionID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ServiceReceiveItem", "ServiceItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.ServiceReceiveItem", "ServiceItemName");
            DropColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryName");
            CreateIndex("EXCEED.ServiceReceiveItem", "ServiceItemCode");
            CreateIndex("EXCEED.ServiceReceiveItem", "ServiceCategoryCode");
            AddForeignKey("EXCEED.ServiceReceiveItem", "ServiceItemCode", "EXCEED.ServiceItem", "ServiceItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", "EXCEED.ServiceCategory", "ServiceCategoryCode", cascadeDelete: true);
        }
    }
}
