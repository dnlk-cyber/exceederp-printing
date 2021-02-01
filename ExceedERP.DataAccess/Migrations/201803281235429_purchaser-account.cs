namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchaseraccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "PurchaseReference", c => c.String());
            AddColumn("EXCEED.Purchaser", "AccountCode", c => c.String(nullable: false));
            DropColumn("EXCEED.ApPayment", "PurechaseReference");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApPayment", "PurechaseReference", c => c.String());
            DropColumn("EXCEED.Purchaser", "AccountCode");
            DropColumn("EXCEED.ApPayment", "PurchaseReference");
        }
    }
}
