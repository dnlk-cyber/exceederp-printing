namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchasestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "PurchaseStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "PurchaseStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "PurchaseStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "PurchaseStatus");
            DropColumn("EXCEED.ProformaPurchase", "PurchaseStatus");
            DropColumn("EXCEED.Tender", "PurchaseStatus");
        }
    }
}
