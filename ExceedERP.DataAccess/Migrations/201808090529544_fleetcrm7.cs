namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CRM.AdvancePaymentDistibution",
                c => new
                    {
                        AdvancePaymentDistibutionId = c.Int(nullable: false, identity: true),
                        AdvancePaymentLineId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvancePaymentDistibutionId)
                .ForeignKey("CRM.AdvancePaymentLine", t => t.AdvancePaymentLineId, cascadeDelete: true)
                .Index(t => t.AdvancePaymentLineId);
            
            CreateTable(
                "CRM.AdvancePaymentLog",
                c => new
                    {
                        AdvancePaymentLogId = c.Int(nullable: false, identity: true),
                        AdvancePaymentLineId = c.Int(nullable: false),
                        CashSalesInvoiceId = c.Int(nullable: false),
                        LineItemId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvancePaymentLogId)
                .ForeignKey("CRM.AdvancePaymentLine", t => t.AdvancePaymentLineId, cascadeDelete: true)
                .Index(t => t.AdvancePaymentLineId);
            
            CreateTable(
                "Budget.BudgetRequestTransferLog",
                c => new
                    {
                        BudgetRequestTransferLogId = c.Int(nullable: false, identity: true),
                        BudgetYear = c.String(),
                        PlanCategoryId = c.String(nullable: false),
                        BranchId = c.Int(nullable: false),
                        WorkUnit = c.String(),
                        FromMonth = c.String(),
                        ToMonth = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
                        RequestNo = c.Int(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetRequestTransferLogId)
                .ForeignKey("Budget.BudgetRequest", t => t.RequestNo, cascadeDelete: true)
                .Index(t => t.RequestNo);
            
            CreateTable(
                "CRM.CashSalesPartialItem",
                c => new
                    {
                        CashSalesPartialItemId = c.Int(nullable: false, identity: true),
                        CashSalesInvoiceId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsReceiptPrinted = c.Boolean(nullable: false),
                        IsPaymentProcessed = c.Boolean(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashSalesPartialItemId)
                .ForeignKey("CRM.CashSalesInvoice", t => t.CashSalesInvoiceId, cascadeDelete: true)
                .Index(t => t.CashSalesInvoiceId);
            
            CreateTable(
                "CRM.CashSalesPartialLineItem",
                c => new
                    {
                        CashSalesPartialLineItemId = c.Int(nullable: false, identity: true),
                        LineItemID = c.Int(nullable: false),
                        CashSalesPartialItemId = c.Int(nullable: false),
                        PartialPayedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashSalesPartialLineItemId)
                .ForeignKey("CRM.CashSalesPartialItem", t => t.CashSalesPartialItemId, cascadeDelete: true)
                .ForeignKey("CRM.LineItem", t => t.LineItemID, cascadeDelete: true)
                .Index(t => t.LineItemID)
                .Index(t => t.CashSalesPartialItemId);
            
            AddColumn("CRM.AdvancePaymentLine", "RecieveMethod", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetRequest", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetRequest", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("Budget.BudgetRequest", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("Budget.BudgetRequest", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetRequest", "SecondSendForApprovalBy", c => c.String());
            AddColumn("Budget.BudgetRequest", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("Budget.BudgetRequestLine", "Accepted", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "IsReceiptPrinted", c => c.Boolean(nullable: false));
            AlterColumn("CRM.AdvancePayment", "Customer", c => c.Int(nullable: false));
            AlterColumn("CRM.CreditSettlement", "Customer", c => c.Int(nullable: false));
            AlterColumn("CRM.CreditSettlement", "RecieveMethod", c => c.Int(nullable: false));
            AlterColumn("CRM.LineCreditSettlement", "InvoiceId", c => c.Int(nullable: false));
            CreateIndex("CRM.AdvancePaymentLine", "AdvancePaymentId");
            CreateIndex("CRM.AdvancePaymentLine", "RecieveMethod");
            CreateIndex("CRM.AdvancePayment", "Customer");
            CreateIndex("CRM.CreditSettlement", "Customer");
            CreateIndex("CRM.CreditSettlement", "RecieveMethod");
            AddForeignKey("CRM.AdvancePayment", "Customer", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            AddForeignKey("CRM.AdvancePaymentLine", "AdvancePaymentId", "CRM.AdvancePayment", "AdvancePaymentId", cascadeDelete: true);
            AddForeignKey("CRM.AdvancePaymentLine", "RecieveMethod", "EXCEED.PaymentMethod", "PaymentMethodID", cascadeDelete: true);
            AddForeignKey("CRM.CreditSettlement", "Customer", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            AddForeignKey("CRM.CreditSettlement", "RecieveMethod", "EXCEED.PaymentMethod", "PaymentMethodID", cascadeDelete: true);
            DropColumn("CRM.AdvancePaymentLine", "AccountCode");
        }
        
        public override void Down()
        {
            AddColumn("CRM.AdvancePaymentLine", "AccountCode", c => c.String());
            DropForeignKey("CRM.CreditSettlement", "RecieveMethod", "EXCEED.PaymentMethod");
            DropForeignKey("CRM.CreditSettlement", "Customer", "CRM.OrganizationCustomer");
            DropForeignKey("CRM.CashSalesPartialLineItem", "LineItemID", "CRM.LineItem");
            DropForeignKey("CRM.CashSalesPartialLineItem", "CashSalesPartialItemId", "CRM.CashSalesPartialItem");
            DropForeignKey("CRM.CashSalesPartialItem", "CashSalesInvoiceId", "CRM.CashSalesInvoice");
            DropForeignKey("Budget.BudgetRequestTransferLog", "RequestNo", "Budget.BudgetRequest");
            DropForeignKey("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId", "CRM.AdvancePaymentLine");
            DropForeignKey("CRM.AdvancePaymentLine", "RecieveMethod", "EXCEED.PaymentMethod");
            DropForeignKey("CRM.AdvancePaymentLog", "AdvancePaymentLineId", "CRM.AdvancePaymentLine");
            DropForeignKey("CRM.AdvancePaymentLine", "AdvancePaymentId", "CRM.AdvancePayment");
            DropForeignKey("CRM.AdvancePayment", "Customer", "CRM.OrganizationCustomer");
            DropIndex("CRM.CreditSettlement", new[] { "RecieveMethod" });
            DropIndex("CRM.CreditSettlement", new[] { "Customer" });
            DropIndex("CRM.CashSalesPartialLineItem", new[] { "CashSalesPartialItemId" });
            DropIndex("CRM.CashSalesPartialLineItem", new[] { "LineItemID" });
            DropIndex("CRM.CashSalesPartialItem", new[] { "CashSalesInvoiceId" });
            DropIndex("Budget.BudgetRequestTransferLog", new[] { "RequestNo" });
            DropIndex("CRM.AdvancePaymentLog", new[] { "AdvancePaymentLineId" });
            DropIndex("CRM.AdvancePayment", new[] { "Customer" });
            DropIndex("CRM.AdvancePaymentLine", new[] { "RecieveMethod" });
            DropIndex("CRM.AdvancePaymentLine", new[] { "AdvancePaymentId" });
            DropIndex("CRM.AdvancePaymentDistibution", new[] { "AdvancePaymentLineId" });
            AlterColumn("CRM.LineCreditSettlement", "InvoiceId", c => c.String());
            AlterColumn("CRM.CreditSettlement", "RecieveMethod", c => c.String());
            AlterColumn("CRM.CreditSettlement", "Customer", c => c.String());
            AlterColumn("CRM.AdvancePayment", "Customer", c => c.String());
            DropColumn("CRM.CashSalesInvoice", "IsReceiptPrinted");
            DropColumn("Budget.BudgetRequestLine", "Accepted");
            DropColumn("Budget.BudgetRequest", "SecondSendForApprovalTime");
            DropColumn("Budget.BudgetRequest", "SecondSendForApprovalBy");
            DropColumn("Budget.BudgetRequest", "IsSecondSendForApproval");
            DropColumn("Budget.BudgetRequest", "SecondOnlineApprovedTime");
            DropColumn("Budget.BudgetRequest", "SecondOnlineApprovedBy");
            DropColumn("Budget.BudgetRequest", "IsSecondOnlineApproved");
            DropColumn("CRM.AdvancePaymentLine", "RecieveMethod");
            DropTable("CRM.CashSalesPartialLineItem");
            DropTable("CRM.CashSalesPartialItem");
            DropTable("Budget.BudgetRequestTransferLog");
            DropTable("CRM.AdvancePaymentLog");
            DropTable("CRM.AdvancePaymentDistibution");
        }
    }
}
