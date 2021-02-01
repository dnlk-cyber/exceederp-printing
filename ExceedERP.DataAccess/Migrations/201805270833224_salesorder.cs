namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderPartial",
                c => new
                    {
                        ArSalesOrderPartialId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        ItemName = c.String(nullable: false),
                        SubItem = c.String(),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        UoM = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalesPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsReceiptPrinted = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderPartialId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            AddColumn("EXCEED.ArSalesOrder", "IsReceiptPrinted", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "JobOrderNo", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArSalesOrderPartial", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderPartial", new[] { "ArSalesOrderId" });
            DropColumn("EXCEED.StoreRequisition", "JobOrderNo");
            DropColumn("EXCEED.ArSalesOrder", "IsReceiptPrinted");
            DropTable("EXCEED.ArSalesOrderPartial");
        }
    }
}
