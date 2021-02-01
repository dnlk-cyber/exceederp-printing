namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priceindex3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemPriceIndex", "SupplierName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemPriceIndex", "SupplierName");
        }
    }
}
