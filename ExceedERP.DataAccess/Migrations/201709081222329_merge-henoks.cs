namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mergehenoks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BudgetAllocation",
                c => new
                    {
                        BudgetAllocationID = c.Int(nullable: false, identity: true),
                        Branch = c.Int(nullable: false),
                        BudgetGroup = c.String(),
                        BudgetYear = c.String(),
                        PlanCategoryId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        WorkUnit = c.Int(nullable: false),
                        Month = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.BudgetAllocationID);
            
            CreateTable(
                "EXCEED.BudgetCategory",
                c => new
                    {
                        BudgetCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        FromAccountCode = c.String(),
                        ToAccountCode = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetCategoryID);
            
            CreateTable(
                "EXCEED.BudgetPlanApproval",
                c => new
                    {
                        BudgetPlanApprovalId = c.Int(nullable: false, identity: true),
                        BudgetPlanId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanApprovalId)
                .ForeignKey("EXCEED.BudgetPlan", t => t.BudgetPlanId, cascadeDelete: true)
                .Index(t => t.BudgetPlanId);
            
            CreateTable(
                "EXCEED.BudgetRequest",
                c => new
                    {
                        BudgetRequestID = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        BudgetGroup = c.String(),
                        BudgetYear = c.String(),
                        PlanCategoryId = c.Int(nullable: false),
                        Branch = c.Int(nullable: false),
                        WorkUnit = c.Int(nullable: false),
                        FromMonth = c.String(),
                        ToMonth = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
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
                .PrimaryKey(t => t.BudgetRequestID);
            
            CreateTable(
                "EXCEED.CreditSettlement",
                c => new
                    {
                        CreditSettlementId = c.Int(nullable: false, identity: true),
                        Customer = c.String(),
                        Date = c.DateTime(nullable: false),
                        RecieveMethod = c.String(),
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
                .PrimaryKey(t => t.CreditSettlementId);
            
            CreateTable(
                "EXCEED.CustomerService",
                c => new
                    {
                        CustomerServiceId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomerServiceId);
            
            CreateTable(
                "EXCEED.EquipmentAttachment",
                c => new
                    {
                        EquipmentAttachmentID = c.Int(nullable: false, identity: true),
                        EquipmentID = c.Guid(nullable: false),
                        Type = c.String(),
                        FileName = c.String(),
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
                .PrimaryKey(t => t.EquipmentAttachmentID);
            
            CreateTable(
                "EXCEED.IssueHistory",
                c => new
                    {
                        IssueHistoryId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
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
                .PrimaryKey(t => t.IssueHistoryId);
            
            CreateTable(
                "EXCEED.LineCreditSettlement",
                c => new
                    {
                        LineCreditSettlementId = c.Int(nullable: false, identity: true),
                        CreditSettlementId = c.Int(nullable: false),
                        InvoiceId = c.String(),
                        CurrentAmmount = c.String(),
                        SettelmentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.LineCreditSettlementId)
                .ForeignKey("EXCEED.CreditSettlement", t => t.CreditSettlementId, cascadeDelete: true)
                .Index(t => t.CreditSettlementId);
            
            CreateTable(
                "EXCEED.PlanCategory",
                c => new
                    {
                        PlanCategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BudgetYear = c.String(),
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
                .PrimaryKey(t => t.PlanCategoryID);
            
            CreateTable(
                "EXCEED.RequestedMonth",
                c => new
                    {
                        RequestedMonthID = c.Int(nullable: false, identity: true),
                        BudgetRequestID = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.RequestedMonthID);
            
            CreateTable(
                "EXCEED.ServiceLineItem",
                c => new
                    {
                        ServiceLineItemId = c.Int(nullable: false, identity: true),
                        ServiceUsageId = c.Int(nullable: false),
                        CustomerServiceId = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.String(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.ServiceLineItemId)
                .ForeignKey("EXCEED.CustomerService", t => t.CustomerServiceId, cascadeDelete: true)
                .Index(t => t.CustomerServiceId);
            
            CreateTable(
                "EXCEED.ServiceUsageApproval",
                c => new
                    {
                        ServiceUsageApprovalId = c.Int(nullable: false, identity: true),
                        ServiceUsageId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceUsageApprovalId)
                .ForeignKey("EXCEED.ServiceUsage", t => t.ServiceUsageId, cascadeDelete: true)
                .Index(t => t.ServiceUsageId);
            
            CreateTable(
                "EXCEED.ServiceUsage",
                c => new
                    {
                        ServiceUsageId = c.Int(nullable: false, identity: true),
                        Customer = c.String(nullable: false),
                        PaymentMethod = c.String(),
                        Date = c.DateTime(nullable: false),
                        ReceiveMethod = c.String(nullable: false),
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
                        OrganizationCustomer_OrganizationCustomerID = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceUsageId)
                .ForeignKey("CRM.OrganizationCustomer", t => t.OrganizationCustomer_OrganizationCustomerID)
                .Index(t => t.OrganizationCustomer_OrganizationCustomerID);
            
            AddColumn("EXCEED.BudgetPlan", "BudgetCategoryID", c => c.Int(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "WorkUnit", c => c.Int(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "CurrentYearlyBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BudgetPlan", "Status", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "Remark", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "VoidBy", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlan", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlan", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlan", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlan", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.BudgetPlan", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlanMonthly", "TransferedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BudgetPlanMonthly", "RevisedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BudgetPlanMonthly", "AllocatedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BudgetPlanMonthly", "CurrentBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BudgetPlanMonthly", "Status", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "Remark", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlanMonthly", "VoidBy", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlanMonthly", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlanMonthly", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlanMonthly", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlanMonthly", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetPlanMonthly", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetPlanMonthly", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.BudgetPlanMonthly", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetTransfer", "BudgetGroup", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "BudgetYear", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "PlanCategoryId", c => c.Int(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "FromAccountCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "ToAccountCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "FromWorkUnit", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "ToWorkUnit", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "Month", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "Status", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "Remark", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "VoidBy", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetTransfer", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetTransfer", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetTransfer", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetUsage", "PlanCategoryId", c => c.Int(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "WorkUnit", c => c.Int(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "Status", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "Remark", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "VoidBy", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetUsage", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetUsage", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.BudgetUsage", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BudgetUsage", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.BudgetUsage", "SendForApprovalTime", c => c.DateTime());
            AddColumn("CRM.Product", "ItemCategoryCode", c => c.String());
            AddColumn("CRM.Product", "ItemCode", c => c.String());
            AddColumn("CRM.StoreTransaction", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BudgetUsage", "BudgetYear", c => c.String());
            AlterColumn("CRM.Campaign", "AssignedTo", c => c.String(nullable: false));
            AlterColumn("CRM.Campaign", "TeamId", c => c.String(nullable: false));
            AlterColumn("CRM.Campaign", "CartID", c => c.String(nullable: false));
            AlterColumn("CRM.Campaign", "CompaignType", c => c.String(nullable: false));
            AlterColumn("CRM.CustomerTicket", "CustomerID", c => c.String(nullable: false));
            AlterColumn("EXCEED.LineItem", "Variety", c => c.String());
            AlterColumn("EXCEED.PreCustomer", "TradeName", c => c.String(nullable: false));
            AlterColumn("EXCEED.PreCustomer", "BrandName", c => c.String(nullable: false));
            AlterColumn("EXCEED.PreCustomer", "BusinessType", c => c.String(nullable: false));
            AlterColumn("EXCEED.PreCustomer", "Category", c => c.String(nullable: false));
            AlterColumn("CRM.Price", "Variety", c => c.String());
            DropColumn("EXCEED.BudgetTransfer", "FromBudgetYear");
            DropColumn("EXCEED.BudgetTransfer", "ToBudgetYear");
            DropColumn("EXCEED.BudgetTransfer", "AccountCode");
            DropColumn("CRM.CashSalesInvoice", "Status");
            DropColumn("CRM.SalesOrderVoucher", "Status");
            DropColumn("CRM.SalesQuoteVoucher", "Status");
        }
        
        public override void Down()
        {
            AddColumn("CRM.SalesQuoteVoucher", "Status", c => c.String());
            AddColumn("CRM.SalesOrderVoucher", "Status", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "Status", c => c.String());
            AddColumn("EXCEED.BudgetTransfer", "AccountCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "ToBudgetYear", c => c.String(nullable: false));
            AddColumn("EXCEED.BudgetTransfer", "FromBudgetYear", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ServiceUsageApproval", "ServiceUsageId", "EXCEED.ServiceUsage");
            DropForeignKey("EXCEED.ServiceUsage", "OrganizationCustomer_OrganizationCustomerID", "CRM.OrganizationCustomer");
            DropForeignKey("EXCEED.ServiceLineItem", "CustomerServiceId", "EXCEED.CustomerService");
            DropForeignKey("EXCEED.LineCreditSettlement", "CreditSettlementId", "EXCEED.CreditSettlement");
            DropForeignKey("EXCEED.BudgetPlanApproval", "BudgetPlanId", "EXCEED.BudgetPlan");
            DropIndex("EXCEED.ServiceUsage", new[] { "OrganizationCustomer_OrganizationCustomerID" });
            DropIndex("EXCEED.ServiceUsageApproval", new[] { "ServiceUsageId" });
            DropIndex("EXCEED.ServiceLineItem", new[] { "CustomerServiceId" });
            DropIndex("EXCEED.LineCreditSettlement", new[] { "CreditSettlementId" });
            DropIndex("EXCEED.BudgetPlanApproval", new[] { "BudgetPlanId" });
            AlterColumn("CRM.Price", "Variety", c => c.String(nullable: false));
            AlterColumn("EXCEED.PreCustomer", "Category", c => c.String());
            AlterColumn("EXCEED.PreCustomer", "BusinessType", c => c.String());
            AlterColumn("EXCEED.PreCustomer", "BrandName", c => c.String());
            AlterColumn("EXCEED.PreCustomer", "TradeName", c => c.String());
            AlterColumn("EXCEED.LineItem", "Variety", c => c.String(nullable: false));
            AlterColumn("CRM.CustomerTicket", "CustomerID", c => c.String());
            AlterColumn("CRM.Campaign", "CompaignType", c => c.String());
            AlterColumn("CRM.Campaign", "CartID", c => c.String());
            AlterColumn("CRM.Campaign", "TeamId", c => c.String());
            AlterColumn("CRM.Campaign", "AssignedTo", c => c.String());
            AlterColumn("EXCEED.BudgetUsage", "BudgetYear", c => c.String(nullable: false));
            DropColumn("CRM.StoreTransaction", "Quantity");
            DropColumn("CRM.Product", "ItemCode");
            DropColumn("CRM.Product", "ItemCategoryCode");
            DropColumn("EXCEED.BudgetUsage", "SendForApprovalTime");
            DropColumn("EXCEED.BudgetUsage", "SendForApprovalBy");
            DropColumn("EXCEED.BudgetUsage", "IsSendForApproval");
            DropColumn("EXCEED.BudgetUsage", "OnlineTransferredTime");
            DropColumn("EXCEED.BudgetUsage", "IsOnlineTransferredBy");
            DropColumn("EXCEED.BudgetUsage", "IsOnlineTransferred");
            DropColumn("EXCEED.BudgetUsage", "OnlineApprovedTime");
            DropColumn("EXCEED.BudgetUsage", "OnlineApprovedBy");
            DropColumn("EXCEED.BudgetUsage", "IsOnlineApproved");
            DropColumn("EXCEED.BudgetUsage", "VoidTime");
            DropColumn("EXCEED.BudgetUsage", "VoidBy");
            DropColumn("EXCEED.BudgetUsage", "IsVoid");
            DropColumn("EXCEED.BudgetUsage", "Remark");
            DropColumn("EXCEED.BudgetUsage", "Status");
            DropColumn("EXCEED.BudgetUsage", "WorkUnit");
            DropColumn("EXCEED.BudgetUsage", "PlanCategoryId");
            DropColumn("EXCEED.BudgetTransfer", "SendForApprovalTime");
            DropColumn("EXCEED.BudgetTransfer", "SendForApprovalBy");
            DropColumn("EXCEED.BudgetTransfer", "IsSendForApproval");
            DropColumn("EXCEED.BudgetTransfer", "OnlineTransferredTime");
            DropColumn("EXCEED.BudgetTransfer", "IsOnlineTransferredBy");
            DropColumn("EXCEED.BudgetTransfer", "IsOnlineTransferred");
            DropColumn("EXCEED.BudgetTransfer", "OnlineApprovedTime");
            DropColumn("EXCEED.BudgetTransfer", "OnlineApprovedBy");
            DropColumn("EXCEED.BudgetTransfer", "IsOnlineApproved");
            DropColumn("EXCEED.BudgetTransfer", "VoidTime");
            DropColumn("EXCEED.BudgetTransfer", "VoidBy");
            DropColumn("EXCEED.BudgetTransfer", "IsVoid");
            DropColumn("EXCEED.BudgetTransfer", "Remark");
            DropColumn("EXCEED.BudgetTransfer", "Status");
            DropColumn("EXCEED.BudgetTransfer", "Month");
            DropColumn("EXCEED.BudgetTransfer", "ToWorkUnit");
            DropColumn("EXCEED.BudgetTransfer", "FromWorkUnit");
            DropColumn("EXCEED.BudgetTransfer", "ToAccountCode");
            DropColumn("EXCEED.BudgetTransfer", "FromAccountCode");
            DropColumn("EXCEED.BudgetTransfer", "PlanCategoryId");
            DropColumn("EXCEED.BudgetTransfer", "BudgetYear");
            DropColumn("EXCEED.BudgetTransfer", "BudgetGroup");
            DropColumn("EXCEED.BudgetPlanMonthly", "SendForApprovalTime");
            DropColumn("EXCEED.BudgetPlanMonthly", "SendForApprovalBy");
            DropColumn("EXCEED.BudgetPlanMonthly", "IsSendForApproval");
            DropColumn("EXCEED.BudgetPlanMonthly", "OnlineTransferredTime");
            DropColumn("EXCEED.BudgetPlanMonthly", "IsOnlineTransferredBy");
            DropColumn("EXCEED.BudgetPlanMonthly", "IsOnlineTransferred");
            DropColumn("EXCEED.BudgetPlanMonthly", "OnlineApprovedTime");
            DropColumn("EXCEED.BudgetPlanMonthly", "OnlineApprovedBy");
            DropColumn("EXCEED.BudgetPlanMonthly", "IsOnlineApproved");
            DropColumn("EXCEED.BudgetPlanMonthly", "VoidTime");
            DropColumn("EXCEED.BudgetPlanMonthly", "VoidBy");
            DropColumn("EXCEED.BudgetPlanMonthly", "IsVoid");
            DropColumn("EXCEED.BudgetPlanMonthly", "Remark");
            DropColumn("EXCEED.BudgetPlanMonthly", "Status");
            DropColumn("EXCEED.BudgetPlanMonthly", "CurrentBudget");
            DropColumn("EXCEED.BudgetPlanMonthly", "AllocatedBudget");
            DropColumn("EXCEED.BudgetPlanMonthly", "RevisedBudget");
            DropColumn("EXCEED.BudgetPlanMonthly", "TransferedBudget");
            DropColumn("EXCEED.BudgetPlan", "SendForApprovalTime");
            DropColumn("EXCEED.BudgetPlan", "SendForApprovalBy");
            DropColumn("EXCEED.BudgetPlan", "IsSendForApproval");
            DropColumn("EXCEED.BudgetPlan", "OnlineTransferredTime");
            DropColumn("EXCEED.BudgetPlan", "IsOnlineTransferredBy");
            DropColumn("EXCEED.BudgetPlan", "IsOnlineTransferred");
            DropColumn("EXCEED.BudgetPlan", "OnlineApprovedTime");
            DropColumn("EXCEED.BudgetPlan", "OnlineApprovedBy");
            DropColumn("EXCEED.BudgetPlan", "IsOnlineApproved");
            DropColumn("EXCEED.BudgetPlan", "VoidTime");
            DropColumn("EXCEED.BudgetPlan", "VoidBy");
            DropColumn("EXCEED.BudgetPlan", "IsVoid");
            DropColumn("EXCEED.BudgetPlan", "Remark");
            DropColumn("EXCEED.BudgetPlan", "Status");
            DropColumn("EXCEED.BudgetPlan", "CurrentYearlyBudget");
            DropColumn("EXCEED.BudgetPlan", "WorkUnit");
            DropColumn("EXCEED.BudgetPlan", "BudgetCategoryID");
            DropTable("EXCEED.ServiceUsage");
            DropTable("EXCEED.ServiceUsageApproval");
            DropTable("EXCEED.ServiceLineItem");
            DropTable("EXCEED.RequestedMonth");
            DropTable("EXCEED.PlanCategory");
            DropTable("EXCEED.LineCreditSettlement");
            DropTable("EXCEED.IssueHistory");
            DropTable("EXCEED.EquipmentAttachment");
            DropTable("EXCEED.CustomerService");
            DropTable("EXCEED.CreditSettlement");
            DropTable("EXCEED.BudgetRequest");
            DropTable("EXCEED.BudgetPlanApproval");
            DropTable("EXCEED.BudgetCategory");
            DropTable("EXCEED.BudgetAllocation");
        }
    }
}
