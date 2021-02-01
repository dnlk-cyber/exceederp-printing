namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isfullfilledflag : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsFullfilled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsFullfilled");
            DropColumn("EXCEED.PurchaseRequisition", "IsFullfilled");
            DropColumn("EXCEED.ProformaPurchase", "IsFullfilled");
            DropColumn("EXCEED.ItemTransfer", "IsFullfilled");
            DropColumn("EXCEED.StoreRequisition", "IsFullfilled");
            DropColumn("EXCEED.Tender", "IsFullfilled");
        }
    }
}
