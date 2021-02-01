namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr4digit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseRequisitionItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseRequisitionItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseRequisitionItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.PurchaseRequisitionItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
