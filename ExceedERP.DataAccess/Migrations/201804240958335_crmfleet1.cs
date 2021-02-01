namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleet1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Budget.BudgetPlanApproval", "BudgetPlanId", "Budget.BudgetPlan");
            DropIndex("Budget.BudgetPlanApproval", new[] { "BudgetPlanId" });
            DropIndex("Budget.BudgetPlanMonthly", new[] { "BudgetPlan_BudgetPlanID" });
            DropIndex("CRM.ThingCategory", "IX_ThingCategory");
            DropColumn("Budget.BudgetPlanMonthly", "BudgetDefineID");
            RenameColumn(table: "Budget.BudgetPlanMonthly", name: "BudgetPlan_BudgetPlanID", newName: "BudgetDefineID");
            DropPrimaryKey("Budget.BudgetControl");
            CreateTable(
                "CRM.AdditionalCostLog",
                c => new
                    {
                        AdditionalCostLogId = c.Int(nullable: false, identity: true),
                        LineItemId = c.Int(nullable: false),
                        AdditionalCostId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AdditionalCostLogId)
                .ForeignKey("CRM.AdditionalCost", t => t.AdditionalCostId, cascadeDelete: true)
                .ForeignKey("CRM.LineItem", t => t.LineItemId, cascadeDelete: true)
                .Index(t => t.LineItemId)
                .Index(t => t.AdditionalCostId);
            
            CreateTable(
                "CRM.AdditionalCost",
                c => new
                    {
                        AdditionalCostId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        SalesAccountCode = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdditionalCostId);
            
            CreateTable(
                "CRM.SalesStore",
                c => new
                    {
                        SalesStoreId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(maxLength: 128),
                        Location = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesStoreId)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "CRM.AssignTicket",
                c => new
                    {
                        AssignTicketId = c.Int(nullable: false, identity: true),
                        CustomerTicketId = c.Int(nullable: false),
                        TicketDefintion = c.String(),
                        AssinedTo = c.String(),
                        IsTransfer = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        TransferTo = c.String(),
                        AssignedDate = c.DateTime(nullable: false),
                        ClosedDate = c.DateTime(nullable: false),
                        Age = c.String(),
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
                .PrimaryKey(t => t.AssignTicketId);
            
            CreateTable(
                "BudgetGAAP.CostOfServiceSold",
                c => new
                    {
                        CostOfServiceSoldId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CostOfServiceSoldId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "BudgetGAAP.Expenditure",
                c => new
                    {
                        ExpenditureId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ExpenditureId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "BudgetGAAP.ForcastedIncomeStatement",
                c => new
                    {
                        ForcastedIncomeStatementId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForcastedIncomeStatementId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "BudgetGAAP.InvestmentPlan",
                c => new
                    {
                        InvestmentPlanId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InvestmentPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "CRM.PriorityType",
                c => new
                    {
                        PriorityTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Color = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriorityTypeId);
            
            CreateTable(
                "EXCEED.ProductCategory",
                c => new
                    {
                        Index = c.Int(nullable: false),
                        ProductCategoryId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Parent = c.String(),
                        SalesAccountCode = c.String(nullable: false),
                        InventoryAccountCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductCategoryId)
                .Index(t => t.Index, unique: true, name: "IX_ThingCategory");
            
            CreateTable(
                "BudgetGAAP.ProfitLossStatementCategory",
                c => new
                    {
                        ProfitLossStatementCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ProfitLostType = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProfitLossStatementCategoryId);
            
            CreateTable(
                "BudgetGAAP.ProfitLossStatement",
                c => new
                    {
                        ProfitLossStatementId = c.Int(nullable: false, identity: true),
                        ProfitLossStatementCategoryId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProfitLossStatementId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("BudgetGAAP.ProfitLossStatementCategory", t => t.ProfitLossStatementCategoryId, cascadeDelete: true)
                .Index(t => t.ProfitLossStatementCategoryId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "FleetManagement.SiteVechicleTransferDetail",
                c => new
                    {
                        SiteVechicleTransferDetailId = c.Int(nullable: false, identity: true),
                        SiteVechicleTransferId = c.Guid(nullable: false),
                        Type = c.String(nullable: false),
                        Description = c.String(),
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
                .PrimaryKey(t => t.SiteVechicleTransferDetailId);
            
            CreateTable(
                "CRM.StatusType",
                c => new
                    {
                        StatusTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Color = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StatusTypeId);
            
            CreateTable(
                "CRM.SupportGroupDetail",
                c => new
                    {
                        SupportGroupDetailId = c.Int(nullable: false, identity: true),
                        SupportGroupId = c.Int(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupportGroupDetailId)
                .ForeignKey("CRM.SupportGroup", t => t.SupportGroupId, cascadeDelete: true)
                .Index(t => t.SupportGroupId);
            
            CreateTable(
                "CRM.SupportGroup",
                c => new
                    {
                        SupportGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupportGroupId);
            
            CreateTable(
                "Budget.TaskCategory",
                c => new
                    {
                        TaskCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskCategoryId);
            
            CreateTable(
                "Budget.TaskPlan",
                c => new
                    {
                        TaskPlanId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BudgetCategoryID = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLChartOfAccountCostCenterId = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
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
                        GLChartOfAccountCostCenter_Values = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TaskPlanId)
                .ForeignKey("EXCEED.GLChartOfAccountCostCenter", t => t.GLChartOfAccountCostCenter_Values)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("Budget.TaskCategory", t => t.BudgetCategoryID, cascadeDelete: true)
                .ForeignKey("Budget.TaskUnit", t => t.Unit, cascadeDelete: true)
                .Index(t => t.BudgetCategoryID)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.Unit)
                .Index(t => t.GLChartOfAccountCostCenter_Values);
            
            CreateTable(
                "Budget.TaskUnit",
                c => new
                    {
                        TaskUnitId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskUnitId);
            
            CreateTable(
                "Budget.TaskPerformaceApproval",
                c => new
                    {
                        TaskPerformaceApprovalId = c.Int(nullable: false, identity: true),
                        TaskPerformaceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskPerformaceApprovalId);
            
            CreateTable(
                "Budget.TaskPerformance",
                c => new
                    {
                        TaskPerformanceId = c.Int(nullable: false, identity: true),
                        BudgetCategoryID = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        GLChartOfAccountCostCenterId = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaskPlanId = c.Int(nullable: false),
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
                        GLChartOfAccountCostCenter_Values = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TaskPerformanceId)
                .ForeignKey("EXCEED.GLChartOfAccountCostCenter", t => t.GLChartOfAccountCostCenter_Values)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .ForeignKey("Budget.TaskCategory", t => t.BudgetCategoryID, cascadeDelete: true)
                .ForeignKey("Budget.TaskUnit", t => t.Unit, cascadeDelete: true)
                .Index(t => t.BudgetCategoryID)
                .Index(t => t.GLPeriodId)
                .Index(t => t.Unit)
                .Index(t => t.GLChartOfAccountCostCenter_Values);
            
            CreateTable(
                "Budget.TaskPlanApproval",
                c => new
                    {
                        TaskPlanApprovalId = c.Int(nullable: false, identity: true),
                        BudgetPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskPlanApprovalId);
            
            CreateTable(
                "Budget.TaskPlanMonthly",
                c => new
                    {
                        TaskPlanMonthlyId = c.Int(nullable: false, identity: true),
                        BudgetDefineID = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        MonthlyBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.TaskPlanMonthlyId)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "CRM.TicketStatusLog",
                c => new
                    {
                        TicketStatusLogId = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        User = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketStatusLogId);
            
            AddColumn("Budget.BudgetCategory", "BudgetControlId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenterId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", c => c.String(maxLength: 128));
            AddColumn("Budget.BudgetPlanMonthly", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "AccountType", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetUsage", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.CashSalesInvoice", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("CRM.CustomerTicket", "TicketDefintion", c => c.String());
            AddColumn("CRM.CustomerTicket", "AssignedTo", c => c.String());
            AddColumn("CRM.CustomerTicket", "StatusType", c => c.String(nullable: false));
            AddColumn("CRM.CustomerTicket", "StatusColor", c => c.String());
            AddColumn("CRM.CustomerTicket", "PriorityColor", c => c.String());
            AddColumn("CRM.CustomerTicket", "Status", c => c.Int(nullable: false));
            AddColumn("CRM.CustomerTicket", "IsSendForClosing", c => c.Boolean(nullable: false));
            AddColumn("CRM.CustomerTicket", "ClosedDate", c => c.DateTime(nullable: false));
            AddColumn("CRM.LineItem", "TaxWithHolding", c => c.Boolean(nullable: false));
            AddColumn("CRM.LineItem", "WithHoldingTax", c => c.String());
            AddColumn("CRM.LineItem", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.OrganizationCustomer", "TinNo", c => c.String());
            AddColumn("CRM.OrganizationCustomer", "EsclationLevel", c => c.String());
            AddColumn("Budget.PlanCategory", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("CRM.ThingCategoryDefaultAccount", "ProductCategoryId", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetAllocationApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetAllocation", "WorkUnit", c => c.String());
            AlterColumn("Budget.BudgetControl", "BudgetControlId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Budget.BudgetControl", "Name", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetPlanApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetPlan", "Unit", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetPlanMonthly", "BudgetDefineID", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetRequestApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetTransferApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetUsageApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetUsage", "WorkUnit", c => c.String(nullable: false));
            AlterColumn("CRM.CustomerTicket", "CustomerID", c => c.Int(nullable: false));
            AlterColumn("CRM.CustomerTicket", "Description", c => c.String(nullable: false));
            AlterColumn("CRM.CustomerTicket", "Priority", c => c.String(nullable: false));
            AlterColumn("CRM.LineItem", "StoreID", c => c.Int(nullable: false));
            AlterColumn("CRM.LineItem", "ThingID", c => c.Int(nullable: false));
            AddPrimaryKey("Budget.BudgetControl", "BudgetControlId");
            CreateIndex("CRM.LineItem", "StoreID");
            CreateIndex("CRM.LineItem", "ThingID");
            CreateIndex("Budget.BudgetCategory", "BudgetControlId");
            CreateIndex("Budget.BudgetPlanMonthly", "BudgetDefineID");
            CreateIndex("Budget.BudgetPlanMonthly", "GLPeriodId");
            //CreateIndex("Budget.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID");
            CreateIndex("Budget.BudgetPlan", "BudgetCategoryID");
            CreateIndex("Budget.BudgetPlan", "Unit");
            CreateIndex("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values");
            CreateIndex("Budget.PlanCategory", "GlFiscalYearId");
            CreateIndex("CRM.CustomerTicket", "CustomerID");
            AddForeignKey("CRM.LineItem", "ThingID", "CRM.Product", "ProductID", cascadeDelete: true);
            AddForeignKey("CRM.LineItem", "StoreID", "CRM.SalesStore", "SalesStoreId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetCategory", "BudgetControlId", "Budget.BudgetControl", "BudgetControlId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetPlan", "Unit", "Budget.BudgetCurrency", "BudgetCurrencyId", cascadeDelete: true);
            //AddForeignKey("Budget.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID", "Budget.BudgetPlan", "BudgetPlanID");
            AddForeignKey("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
            AddForeignKey("Budget.BudgetPlan", "BudgetCategoryID", "Budget.PlanCategory", "PlanCategoryID", cascadeDelete: true);
            AddForeignKey("Budget.PlanCategory", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetPlanMonthly", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("CRM.CustomerTicket", "CustomerID", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            DropColumn("Budget.BudgetCategory", "BudgetControl");
            DropColumn("Budget.BudgetPlan", "WorkUnit");
            DropColumn("Budget.BudgetPlanMonthly", "BudgetMonth");
            DropColumn("Budget.PlanCategory", "BudgetYear");
            DropColumn("CRM.ThingCategoryDefaultAccount", "ThingCategoryID");
            DropTable("EXCEED.CustomerTicketView");
            DropTable("CRM.ThingCategory");
        }
        
        public override void Down()
        {
            CreateTable(
                "CRM.ThingCategory",
                c => new
                    {
                        Index = c.Int(nullable: false),
                        ThingCategoryID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Parent = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ThingCategoryID);
            
            CreateTable(
                "EXCEED.CustomerTicketView",
                c => new
                    {
                        CustomerTicketID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        TicketGroup = c.String(),
                        EscalationLevel = c.String(),
                        Source = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CustomerTicketID);
            
            AddColumn("CRM.ThingCategoryDefaultAccount", "ThingCategoryID", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "BudgetYear", c => c.String(nullable: false));
            AddColumn("Budget.BudgetPlanMonthly", "BudgetMonth", c => c.String());
            AddColumn("Budget.BudgetPlan", "WorkUnit", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetCategory", "BudgetControl", c => c.String());
            DropForeignKey("Budget.TaskPlanMonthly", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Budget.TaskPerformance", "Unit", "Budget.TaskUnit");
            DropForeignKey("Budget.TaskPerformance", "BudgetCategoryID", "Budget.TaskCategory");
            DropForeignKey("Budget.TaskPerformance", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("Budget.TaskPlan", "Unit", "Budget.TaskUnit");
            DropForeignKey("Budget.TaskPlan", "BudgetCategoryID", "Budget.TaskCategory");
            DropForeignKey("Budget.TaskPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("CRM.SupportGroupDetail", "SupportGroupId", "CRM.SupportGroup");
            DropForeignKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId", "BudgetGAAP.ProfitLossStatementCategory");
            DropForeignKey("BudgetGAAP.ProfitLossStatement", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("BudgetGAAP.InvestmentPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("BudgetGAAP.ForcastedIncomeStatement", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("BudgetGAAP.Expenditure", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("CRM.CustomerTicket", "CustomerID", "CRM.OrganizationCustomer");
            DropForeignKey("BudgetGAAP.CostOfServiceSold", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Budget.BudgetPlanMonthly", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Budget.PlanCategory", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Budget.BudgetPlan", "BudgetCategoryID", "Budget.PlanCategory");
            DropForeignKey("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("Budget.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID", "Budget.BudgetPlan");
            DropForeignKey("Budget.BudgetPlan", "Unit", "Budget.BudgetCurrency");
            DropForeignKey("Budget.BudgetCategory", "BudgetControlId", "Budget.BudgetControl");
            DropForeignKey("CRM.LineItem", "StoreID", "CRM.SalesStore");
            DropForeignKey("CRM.SalesStore", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("CRM.LineItem", "ThingID", "CRM.Product");
            DropForeignKey("CRM.AdditionalCostLog", "LineItemId", "CRM.LineItem");
            DropForeignKey("CRM.AdditionalCostLog", "AdditionalCostId", "CRM.AdditionalCost");
            DropIndex("Budget.TaskPlanMonthly", new[] { "GLPeriodId" });
            DropIndex("Budget.TaskPerformance", new[] { "GLChartOfAccountCostCenter_Values" });
            DropIndex("Budget.TaskPerformance", new[] { "Unit" });
            DropIndex("Budget.TaskPerformance", new[] { "GLPeriodId" });
            DropIndex("Budget.TaskPerformance", new[] { "BudgetCategoryID" });
            DropIndex("Budget.TaskPlan", new[] { "GLChartOfAccountCostCenter_Values" });
            DropIndex("Budget.TaskPlan", new[] { "Unit" });
            DropIndex("Budget.TaskPlan", new[] { "GlFiscalYearId" });
            DropIndex("Budget.TaskPlan", new[] { "BudgetCategoryID" });
            DropIndex("CRM.SupportGroupDetail", new[] { "SupportGroupId" });
            DropIndex("BudgetGAAP.ProfitLossStatement", new[] { "GlFiscalYearId" });
            DropIndex("BudgetGAAP.ProfitLossStatement", new[] { "ProfitLossStatementCategoryId" });
            DropIndex("EXCEED.ProductCategory", "IX_ThingCategory");
            DropIndex("BudgetGAAP.InvestmentPlan", new[] { "GlFiscalYearId" });
            DropIndex("BudgetGAAP.ForcastedIncomeStatement", new[] { "GlFiscalYearId" });
            DropIndex("BudgetGAAP.Expenditure", new[] { "GlFiscalYearId" });
            DropIndex("CRM.CustomerTicket", new[] { "CustomerID" });
            DropIndex("BudgetGAAP.CostOfServiceSold", new[] { "GlFiscalYearId" });
            DropIndex("Budget.PlanCategory", new[] { "GlFiscalYearId" });
            DropIndex("Budget.BudgetPlan", new[] { "GLChartOfAccountCostCenter_Values" });
            DropIndex("Budget.BudgetPlan", new[] { "Unit" });
            DropIndex("Budget.BudgetPlan", new[] { "BudgetCategoryID" });
            DropIndex("Budget.BudgetPlanMonthly", new[] { "BudgetPlan_BudgetPlanID" });
            DropIndex("Budget.BudgetPlanMonthly", new[] { "GLPeriodId" });
            DropIndex("Budget.BudgetPlanMonthly", new[] { "BudgetDefineID" });
            DropIndex("Budget.BudgetCategory", new[] { "BudgetControlId" });
            DropIndex("CRM.SalesStore", new[] { "StoreCode" });
            DropIndex("CRM.LineItem", new[] { "ThingID" });
            DropIndex("CRM.LineItem", new[] { "StoreID" });
            DropIndex("CRM.AdditionalCostLog", new[] { "AdditionalCostId" });
            DropIndex("CRM.AdditionalCostLog", new[] { "LineItemId" });
            DropPrimaryKey("Budget.BudgetControl");
            AlterColumn("CRM.LineItem", "ThingID", c => c.String(nullable: false));
            AlterColumn("CRM.LineItem", "StoreID", c => c.String(nullable: false));
            AlterColumn("CRM.CustomerTicket", "Priority", c => c.Int(nullable: false));
            AlterColumn("CRM.CustomerTicket", "Description", c => c.String());
            AlterColumn("CRM.CustomerTicket", "CustomerID", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetUsage", "WorkUnit", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetUsageApproval", "Status", c => c.String());
            AlterColumn("Budget.BudgetTransferApproval", "Status", c => c.String());
            AlterColumn("Budget.BudgetRequestApproval", "Status", c => c.String());
            AlterColumn("Budget.BudgetPlanMonthly", "BudgetDefineID", c => c.Int());
            AlterColumn("Budget.BudgetPlan", "Unit", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetPlanApproval", "Status", c => c.String());
            AlterColumn("Budget.BudgetControl", "Name", c => c.String());
            AlterColumn("Budget.BudgetControl", "BudgetControlId", c => c.Guid(nullable: false));
            AlterColumn("Budget.BudgetAllocation", "WorkUnit", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetAllocationApproval", "Status", c => c.String());
            DropColumn("CRM.ThingCategoryDefaultAccount", "ProductCategoryId");
            DropColumn("Budget.PlanCategory", "GlFiscalYearId");
            DropColumn("CRM.OrganizationCustomer", "EsclationLevel");
            DropColumn("CRM.OrganizationCustomer", "TinNo");
            DropColumn("CRM.LineItem", "Vat");
            DropColumn("CRM.LineItem", "WithHoldingTax");
            DropColumn("CRM.LineItem", "TaxWithHolding");
            DropColumn("CRM.CustomerTicket", "ClosedDate");
            DropColumn("CRM.CustomerTicket", "IsSendForClosing");
            DropColumn("CRM.CustomerTicket", "Status");
            DropColumn("CRM.CustomerTicket", "PriorityColor");
            DropColumn("CRM.CustomerTicket", "StatusColor");
            DropColumn("CRM.CustomerTicket", "StatusType");
            DropColumn("CRM.CustomerTicket", "AssignedTo");
            DropColumn("CRM.CustomerTicket", "TicketDefintion");
            DropColumn("CRM.CashSalesInvoice", "IsClosed");
            DropColumn("Budget.BudgetUsage", "Credit");
            DropColumn("Budget.BudgetUsage", "Debit");
            DropColumn("Budget.BudgetUsage", "AccountType");
            DropColumn("Budget.BudgetPlanMonthly", "GLPeriodId");
            DropColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values");
            DropColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenterId");
            DropColumn("Budget.BudgetCategory", "BudgetControlId");
            DropTable("CRM.TicketStatusLog");
            DropTable("Budget.TaskPlanMonthly");
            DropTable("Budget.TaskPlanApproval");
            DropTable("Budget.TaskPerformance");
            DropTable("Budget.TaskPerformaceApproval");
            DropTable("Budget.TaskUnit");
            DropTable("Budget.TaskPlan");
            DropTable("Budget.TaskCategory");
            DropTable("CRM.SupportGroup");
            DropTable("CRM.SupportGroupDetail");
            DropTable("CRM.StatusType");
            DropTable("FleetManagement.SiteVechicleTransferDetail");
            DropTable("BudgetGAAP.ProfitLossStatement");
            DropTable("BudgetGAAP.ProfitLossStatementCategory");
            DropTable("EXCEED.ProductCategory");
            DropTable("CRM.PriorityType");
            DropTable("BudgetGAAP.InvestmentPlan");
            DropTable("BudgetGAAP.ForcastedIncomeStatement");
            DropTable("BudgetGAAP.Expenditure");
            DropTable("BudgetGAAP.CostOfServiceSold");
            DropTable("CRM.AssignTicket");
            DropTable("CRM.SalesStore");
            DropTable("CRM.AdditionalCost");
            DropTable("CRM.AdditionalCostLog");
            AddPrimaryKey("Budget.BudgetControl", "BudgetControlId");
            RenameColumn(table: "Budget.BudgetPlanMonthly", name: "BudgetDefineID", newName: "BudgetPlan_BudgetPlanID");
            AddColumn("Budget.BudgetPlanMonthly", "BudgetDefineID", c => c.Int(nullable: false));
            CreateIndex("CRM.ThingCategory", "Index", unique: true, name: "IX_ThingCategory");
            CreateIndex("Budget.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID");
            CreateIndex("Budget.BudgetPlanApproval", "BudgetPlanId");
            AddForeignKey("Budget.BudgetPlanApproval", "BudgetPlanId", "Budget.BudgetPlan", "BudgetPlanID", cascadeDelete: true);
        }
    }
}
