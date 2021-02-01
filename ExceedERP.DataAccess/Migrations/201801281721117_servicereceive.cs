namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicereceive : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ServiceReceive", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ServiceReceive", "ReceiveTypeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ServiceReceive", "ReceiveRef", c => c.String());
            AddColumn("EXCEED.ServiceReceive", "SupplierId", c => c.String());
            AddColumn("EXCEED.ServiceReceive", "SupplierName", c => c.String());
            AddColumn("EXCEED.ServiceReceive", "SupplierInvoiceNo", c => c.String());
            AddColumn("EXCEED.ServiceReceive", "Reference", c => c.String());
            AddColumn("EXCEED.ServiceReceive", "TenderLot", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ServiceReceiveItem", "ServiceItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ServiceReceiveItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ServiceReceiveItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ServiceReceiveItem", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ServiceReceiveItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ServiceReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("EXCEED.ServiceReceive", "ReceiveTypeId");
            CreateIndex("EXCEED.ServiceReceiveItem", "ServiceCategoryCode");
            CreateIndex("EXCEED.ServiceReceiveItem", "ServiceItemCode");
            AddForeignKey("EXCEED.ServiceReceive", "ReceiveTypeId", "EXCEED.ReceiveType", "ReceiveTypeId", cascadeDelete: true);
            AddForeignKey("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", "EXCEED.ServiceCategory", "ServiceCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ServiceReceiveItem", "ServiceItemCode", "EXCEED.ServiceItem", "ServiceItemCode", cascadeDelete: true);
            DropColumn("EXCEED.ServiceReceive", "ReferenceNo");
            DropColumn("EXCEED.ServiceReceiveItem", "CatCode");
            DropColumn("EXCEED.ServiceReceiveItem", "CategoryName");
            DropColumn("EXCEED.ServiceReceiveItem", "ItmCode");
            DropColumn("EXCEED.ServiceReceiveItem", "ItemName");
            DropColumn("EXCEED.ServiceReceiveItem", "PartNo");
            DropColumn("EXCEED.ServiceReceiveItem", "ItemSpecification");
            DropColumn("EXCEED.ServiceReceiveItem", "UoM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ServiceReceiveItem", "UoM", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "PartNo", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "ItemName", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ServiceReceiveItem", "CategoryName", c => c.String());
            AddColumn("EXCEED.ServiceReceiveItem", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ServiceReceive", "ReferenceNo", c => c.String());
            DropForeignKey("EXCEED.ServiceReceiveItem", "ServiceItemCode", "EXCEED.ServiceItem");
            DropForeignKey("EXCEED.ServiceReceiveItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.ServiceReceive", "ReceiveTypeId", "EXCEED.ReceiveType");
            DropIndex("EXCEED.ServiceReceiveItem", new[] { "ServiceItemCode" });
            DropIndex("EXCEED.ServiceReceiveItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.ServiceReceive", new[] { "ReceiveTypeId" });
            DropColumn("EXCEED.ServiceReceiveItem", "Total");
            DropColumn("EXCEED.ServiceReceiveItem", "TaxAmount");
            DropColumn("EXCEED.ServiceReceiveItem", "GLTaxRateID");
            DropColumn("EXCEED.ServiceReceiveItem", "SubTotal");
            DropColumn("EXCEED.ServiceReceiveItem", "UnitPrice");
            DropColumn("EXCEED.ServiceReceiveItem", "ServiceItemCode");
            DropColumn("EXCEED.ServiceReceiveItem", "ServiceCategoryCode");
            DropColumn("EXCEED.ServiceReceive", "TenderLot");
            DropColumn("EXCEED.ServiceReceive", "Reference");
            DropColumn("EXCEED.ServiceReceive", "SupplierInvoiceNo");
            DropColumn("EXCEED.ServiceReceive", "SupplierName");
            DropColumn("EXCEED.ServiceReceive", "SupplierId");
            DropColumn("EXCEED.ServiceReceive", "ReceiveRef");
            DropColumn("EXCEED.ServiceReceive", "ReceiveTypeId");
            DropColumn("EXCEED.ServiceReceive", "BranchId");
        }
    }
}
