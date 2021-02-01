namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory13 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.StoreRequisitionItem", "PurchaseQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.StoreRequisitionItem", "PurchaseQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
