namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "PurchaseQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisitionItem", "PurchaseQuantity");
        }
    }
}
