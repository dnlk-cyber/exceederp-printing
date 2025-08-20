namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureproforma1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.FurnitureProformaInvoice", "TransporCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureProformaInvoice", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureProformaInvoice", "TransportCostTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "TransportCost");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "TransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureProformaInvoice", "TransportCostTotal");
            DropColumn("Manufacturing.FurnitureProformaInvoice", "GLTaxRateID");
            DropColumn("Manufacturing.FurnitureProformaInvoice", "TransporCost");
        }
    }
}
