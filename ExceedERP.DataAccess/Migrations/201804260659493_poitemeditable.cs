namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class poitemeditable : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrderItem", "FromPreviousPurchase", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrderItem", "FromPreviousPurchase");
        }
    }
}
