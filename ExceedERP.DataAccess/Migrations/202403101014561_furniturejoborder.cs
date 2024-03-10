namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furniturejoborder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.FurnitureJobOrderItem",
                c => new
                    {
                        FurnitureJobOrderItemId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        UnitOfMeasurment = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceWithVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureJobOrderFormId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderItemId)
                .ForeignKey("EXCEED.FurnitureJobOrderForm", t => t.FurnitureJobOrderFormId, cascadeDelete: true)
                .Index(t => t.FurnitureJobOrderFormId);
            
            AddColumn("EXCEED.FurnitureJobOrderForm", "JobNo", c => c.String());
            AddColumn("EXCEED.FurnitureJobOrderForm", "PaymentType", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureProformaInvoice", "AdvancePaymentInPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.FurnitureJobOrderForm", "JobTypeId");
            DropColumn("EXCEED.FurnitureJobOrderForm", "UnitOfMeasurment");
            DropColumn("EXCEED.FurnitureJobOrderForm", "Quantity");
            DropColumn("EXCEED.FurnitureJobOrderForm", "UnitPriceBeforeVAT");
            DropColumn("EXCEED.FurnitureJobOrderForm", "UnitPriceWithVAT");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FurnitureJobOrderForm", "UnitPriceWithVAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FurnitureJobOrderForm", "UnitPriceBeforeVAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FurnitureJobOrderForm", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FurnitureJobOrderForm", "UnitOfMeasurment", c => c.String());
            AddColumn("EXCEED.FurnitureJobOrderForm", "JobTypeId", c => c.Int(nullable: false));
            DropForeignKey("Manufacturing.FurnitureJobOrderItem", "FurnitureJobOrderFormId", "EXCEED.FurnitureJobOrderForm");
            DropIndex("Manufacturing.FurnitureJobOrderItem", new[] { "FurnitureJobOrderFormId" });
            DropColumn("Manufacturing.FurnitureProformaInvoice", "AdvancePaymentInPercent");
            DropColumn("EXCEED.FurnitureJobOrderForm", "PaymentType");
            DropColumn("EXCEED.FurnitureJobOrderForm", "JobNo");
            DropTable("Manufacturing.FurnitureJobOrderItem");
        }
    }
}
