namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderFinance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderAdvanceLine",
                c => new
                    {
                        ArSalesOrderAdvanceLineId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        DepositDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reference = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderAdvanceLineId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.ArSalesOrder",
                c => new
                    {
                        ArSalesOrderId = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(),
                        Customer = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        Station = c.String(),
                        BroadcastDateTime = c.DateTime(nullable: false),
                        SalesPerson = c.String(),
                        PaymentTerms = c.Int(nullable: false),
                        OrderReference = c.String(),
                        OrderStatus = c.Int(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.ArSalesOrderDistribution",
                c => new
                    {
                        ArSalesOrderDistributionId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderDistributionId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.ArSalesOrderLine",
                c => new
                    {
                        ArSalesOrderLineId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        ItemName = c.String(nullable: false),
                        UoM = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalesPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderLineId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.ArSalesOrderValidation",
                c => new
                    {
                        ArSalesOrderValidationId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderValidationId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArSalesOrderValidation", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropForeignKey("EXCEED.ArSalesOrderLine", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropForeignKey("EXCEED.ArSalesOrderDistribution", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropForeignKey("EXCEED.ArSalesOrderAdvanceLine", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderValidation", new[] { "ArSalesOrderId" });
            DropIndex("EXCEED.ArSalesOrderLine", new[] { "ArSalesOrderId" });
            DropIndex("EXCEED.ArSalesOrderDistribution", new[] { "ArSalesOrderId" });
            DropIndex("EXCEED.ArSalesOrderAdvanceLine", new[] { "ArSalesOrderId" });
            DropTable("EXCEED.ArSalesOrderValidation");
            DropTable("EXCEED.ArSalesOrderLine");
            DropTable("EXCEED.ArSalesOrderDistribution");
            DropTable("EXCEED.ArSalesOrder");
            DropTable("EXCEED.ArSalesOrderAdvanceLine");
        }
    }
}
