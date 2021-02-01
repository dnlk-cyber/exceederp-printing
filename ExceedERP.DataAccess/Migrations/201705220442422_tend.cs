namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tend : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BidPurchasingSuppliers", "Supplier", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.BidPurchasingSuppliers", "Supplier", c => c.String());
        }
    }
}
