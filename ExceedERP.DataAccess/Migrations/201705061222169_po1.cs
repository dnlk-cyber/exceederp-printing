namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseOrder", "SupplierName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrder", "SupplierName", c => c.String(nullable: false));
        }
    }
}
