namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BudgetUpdatesMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Budget.BudgetPlan", "BranchId", "HRM.Branch");
            DropForeignKey("Budget.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID", "Budget.BudgetTransfer");
            DropForeignKey("Budget.BudgetPlanMonthly", "BudgetDefineID", "Budget.BudgetPlan");
            DropForeignKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId", "BudgetGAAP.ProfitLossStatementCategory");
            DropIndex("Budget.BudgetPlanMonthly", new[] { "BudgetTransfer_BudgetTransferID" });
            DropIndex("Budget.BudgetPlan", new[] { "BranchId" });
            DropIndex("Budget.BudgetTransferApproval", new[] { "BudgetTransferId" });
            DropIndex("Budget.BudgetUsageApproval", new[] { "BudgetUsageId" });
            DropIndex("BudgetGAAP.ProfitLossStatement", new[] { "ProfitLossStatementCategoryId" });
            DropPrimaryKey("BudgetGAAP.ProfitLossStatement");
            CreateTable(
                "Budget.BudgetCategoryDetail",
                c => new
                    {
                        BudgetCategoryDetailID = c.Int(nullable: false, identity: true),
                        BudgetCode = c.String(),
                        BudgetItem = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        BudgetCategoryDetailDescription = c.String(),
                        BudgetCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BudgetCategoryDetailID)
                .ForeignKey("Budget.BudgetCategory", t => t.BudgetCategoryId, cascadeDelete: true)
                .Index(t => t.BudgetCategoryId);
            
            CreateTable(
                "Budget.BudgetMonthlyReserve",
                c => new
                    {
                        BudgetMonthlyReserveId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        SourceId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReserveStatus = c.Int(nullable: false),
                        BudgetPlanMonthlyID = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        BudgetPlanMonthly_BudgetPlanMonthlyID = c.Int(),
                        BudgetPlanMonthly_BudgetPlanMonthlyID1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BudgetMonthlyReserveId)
                .ForeignKey("Budget.BudgetPlanMonthly", t => t.BudgetPlanMonthly_BudgetPlanMonthlyID)
                .ForeignKey("Budget.BudgetPlanMonthly", t => t.BudgetPlanMonthly_BudgetPlanMonthlyID1, cascadeDelete: true)
                .Index(t => t.BudgetPlanMonthly_BudgetPlanMonthlyID)
                .Index(t => t.BudgetPlanMonthly_BudgetPlanMonthlyID1);
            
            CreateTable(
                "Budget.BudgetPlanReserve",
                c => new
                    {
                        BudgetPlanReserveId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        SourceId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReserveStatus = c.Int(nullable: false),
                        BudgetPlanID = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanReserveId)
                .ForeignKey("Budget.BudgetPlan", t => t.BudgetPlanID, cascadeDelete: true)
                .Index(t => t.BudgetPlanID);
            
            CreateTable(
                "EXCEED.BudgetPlanAttachment",
                c => new
                    {
                        BudgetPlanAttachmentId = c.Int(nullable: false, identity: true),
                        PlanCategoryID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanAttachmentId)
                .ForeignKey("Budget.PlanCategory", t => t.PlanCategoryID, cascadeDelete: true)
                .Index(t => t.PlanCategoryID);
            
            CreateTable(
                "EXCEED.BudgetPlanDepartmentApproval",
                c => new
                    {
                        BudgetPlanDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        BudgetPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanDepartmentApprovalId);
            
            CreateTable(
                "EXCEED.BudgetPlanRevisionApproval",
                c => new
                    {
                        BudgetPlanRevisionApprovalId = c.Int(nullable: false, identity: true),
                        BudgetPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanRevisionApprovalId);
            
            CreateTable(
                "Budget.BudgetRequestDetail",
                c => new
                    {
                        BudgetRequestDetailId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        Description = c.String(),
                        BudgetRequestId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.BudgetRequestDetailId)
                .ForeignKey("Budget.BudgetRequest", t => t.BudgetRequestId, cascadeDelete: true)
                .Index(t => t.BudgetRequestId);
            
            CreateTable(
                "EXCEED.BudgetUsageDetail",
                c => new
                    {
                        BudgetUsageDetailId = c.Int(nullable: false, identity: true),
                        BudgetUsageID = c.Int(nullable: false),
                        PlanCategoryId = c.Int(nullable: false),
                        BudgetCode = c.String(),
                        BudgetItem = c.String(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        UsageAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenter = c.String(),
                        OrgStructureId = c.Int(),
                        BranchId = c.Int(),
                        BusinessUnitId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetUsageDetailId)
                .ForeignKey("Budget.BudgetUsage", t => t.BudgetUsageID, cascadeDelete: true)
                .Index(t => t.BudgetUsageID);
            
            CreateTable(
                "EXCEED.FinancialRatio",
                c => new
                    {
                        FinancialRatioId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Name = c.String(),
                        Minimum = c.Single(nullable: false),
                        Maximum = c.Single(nullable: false),
                        Average = c.Single(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinancialRatioId);
            
            CreateTable(
                "EXCEED.PurchaseRequisitionBudgetValidation",
                c => new
                    {
                        PurchaseRequisitionBudgetValidationId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionBudgetValidationId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            AddColumn("Budget.BudgetCategory", "BudgetCategoryCode", c => c.String(nullable: false));
            AddColumn("Budget.BudgetCategory", "BudgetType", c => c.String());
            AddColumn("Budget.BudgetControl", "ControlPeriod", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlanMonthly", "TransferedOutBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetPlan", "PlanCategoryId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "FiscalYearId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "BudgetCode", c => c.String());
            AddColumn("Budget.BudgetPlan", "BudgetItem", c => c.String(nullable: false));
            AddColumn("Budget.BudgetPlan", "CostCenter", c => c.String());
            AddColumn("Budget.BudgetPlan", "OrgStructureId", c => c.Int());
            AddColumn("Budget.BudgetPlan", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetRequest", "AccountCode", c => c.String());
            AddColumn("Budget.BudgetTransfer", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetTransfer", "FromCostCenter", c => c.String());
            AddColumn("Budget.BudgetTransfer", "FromOrgStructureId", c => c.Int());
            AddColumn("Budget.BudgetTransfer", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetTransfer", "FromBusinessUnitId", c => c.Int());
            AddColumn("Budget.BudgetTransfer", "FromGlPeriodId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetTransfer", "ToCostCenter", c => c.String());
            AddColumn("Budget.BudgetTransfer", "ToOrgStructureId", c => c.Int());
            AddColumn("Budget.BudgetTransfer", "ToBranchId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetTransfer", "ToBusinessUnitId", c => c.Int());
            AddColumn("Budget.BudgetTransfer", "ToGlPeriodId", c => c.Int());
            AddColumn("Budget.BudgetTransfer", "EqualToAllPeriods", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetUsage", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "BranchId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "EmployeeId", c => c.Int());
            AddColumn("Budget.BudgetUsage", "ReasonOfPayment", c => c.String());
            AddColumn("Budget.BudgetUsage", "RequestDate", c => c.DateTime(nullable: false));
            AddColumn("Budget.BudgetUsage", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetUsage", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.DashboardDictionary", "GlFiscalYearId", c => c.Int());
            AddColumn("EXCEED.FundTransfer", "PaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FundTransfer", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.PlanCategory", "BudgetCategoryID", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "BranchId", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "OrgStuructureId", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("Budget.PlanCategory", "IsRevisionSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "RevisionSendForApprovalBy", c => c.String());
            AddColumn("Budget.PlanCategory", "RevisionSendForApprovalTime", c => c.DateTime());
            AddColumn("Budget.PlanCategory", "IsOpenforrevision", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "IsRevisionOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "RevisionOnlineApprovedBy", c => c.String());
            AddColumn("Budget.PlanCategory", "RevisionOnlineApprovedTime", c => c.DateTime());
            AddColumn("Budget.PlanCategory", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("Budget.PlanCategory", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("Budget.PlanCategory", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("Budget.PlanCategory", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("Budget.PlanCategory", "ForwardedFrom", c => c.String());
            AddColumn("Budget.PlanCategory", "ForwardedTo", c => c.String());
            AddColumn("Budget.PlanCategory", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "CurrentApprovalStep", c => c.Int());
            AddColumn("Budget.PlanCategory", "ForwardedTime", c => c.DateTime());
            AddColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategory_ProfitLossStatementCategoryId", c => c.Int());
            AlterColumn("Budget.BudgetPlan", "BranchId", c => c.Int());
            AlterColumn("Budget.BudgetRequest", "PlanCategoryId", c => c.Int(nullable: false));
            AlterColumn("Budget.BudgetRequestTransferLog", "PlanCategoryId", c => c.Int(nullable: false));
            AlterColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementId", c => c.Int(nullable: false));
            AlterColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId");
            CreateIndex("Budget.BudgetTransferApproval", "BudgetTransferID");
            CreateIndex("Budget.BudgetUsageApproval", "BudgetUsageID");
            CreateIndex("Budget.BudgetUsage", "GLPeriodId");
            CreateIndex("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategory_ProfitLossStatementCategoryId");
            AddForeignKey("Budget.BudgetUsage", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetPlanMonthly", "BudgetDefineID", "Budget.BudgetPlan", "BudgetPlanID", cascadeDelete: true);
            AddForeignKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategory_ProfitLossStatementCategoryId", "BudgetGAAP.ProfitLossStatementCategory", "ProfitLossStatementCategoryId");
            DropColumn("Budget.BudgetCategory", "Description");
            DropColumn("Budget.BudgetPlanMonthly", "AccountCode");
            DropColumn("Budget.BudgetPlanMonthly", "AccountDesc");
            DropColumn("Budget.BudgetPlanMonthly", "Unit");
            DropColumn("Budget.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID");
            DropColumn("Budget.BudgetPlan", "BudgetCategoryID");
            DropColumn("Budget.BudgetPlan", "BudgetYear");
            DropColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenterId");
            DropColumn("Budget.BudgetPlan", "WorkUnitName");
            DropColumn("Budget.BudgetTransfer", "FromBranch");
            DropColumn("Budget.BudgetTransfer", "ToBranch");
            DropColumn("Budget.BudgetTransfer", "BudgetGroup");
            DropColumn("Budget.BudgetTransfer", "BudgetYear");
            DropColumn("Budget.BudgetTransfer", "FromWorkUnit");
            DropColumn("Budget.BudgetTransfer", "ToWorkUnit");
            DropColumn("Budget.BudgetTransfer", "Month");
            DropColumn("Budget.BudgetTransfer", "AccountDesc");
            DropColumn("Budget.BudgetTransfer", "Unit");
            DropColumn("Budget.BudgetUsage", "BudgetYear");
            DropColumn("Budget.BudgetUsage", "PlanCategoryId");
            DropColumn("Budget.BudgetUsage", "WorkUnit");
            DropColumn("Budget.BudgetUsage", "BudgetMonth");
            DropColumn("Budget.BudgetUsage", "AccountCode");
            DropColumn("Budget.BudgetUsage", "AccountDesc");
            DropColumn("Budget.BudgetUsage", "AccountType");
            DropColumn("Budget.BudgetUsage", "Unit");
            DropColumn("Budget.BudgetUsage", "MonthlyActuals");
            DropColumn("Budget.BudgetUsage", "Debit");
            DropColumn("Budget.BudgetUsage", "Credit");
        }
        
        public override void Down()
        {
            AddColumn("Budget.BudgetUsage", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetUsage", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetUsage", "MonthlyActuals", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetUsage", "Unit", c => c.String(nullable: false));
            AddColumn("Budget.BudgetUsage", "AccountType", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "AccountDesc", c => c.String());
            AddColumn("Budget.BudgetUsage", "AccountCode", c => c.String(nullable: false));
            AddColumn("Budget.BudgetUsage", "BudgetMonth", c => c.String(nullable: false));
            AddColumn("Budget.BudgetUsage", "WorkUnit", c => c.String(nullable: false));
            AddColumn("Budget.BudgetUsage", "PlanCategoryId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetUsage", "BudgetYear", c => c.String());
            AddColumn("Budget.BudgetTransfer", "Unit", c => c.String(nullable: false));
            AddColumn("Budget.BudgetTransfer", "AccountDesc", c => c.String());
            AddColumn("Budget.BudgetTransfer", "Month", c => c.String());
            AddColumn("Budget.BudgetTransfer", "ToWorkUnit", c => c.String());
            AddColumn("Budget.BudgetTransfer", "FromWorkUnit", c => c.String());
            AddColumn("Budget.BudgetTransfer", "BudgetYear", c => c.String());
            AddColumn("Budget.BudgetTransfer", "BudgetGroup", c => c.String());
            AddColumn("Budget.BudgetTransfer", "ToBranch", c => c.String());
            AddColumn("Budget.BudgetTransfer", "FromBranch", c => c.String());
            AddColumn("Budget.BudgetPlan", "WorkUnitName", c => c.String());
            AddColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenterId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "BudgetYear", c => c.String());
            AddColumn("Budget.BudgetPlan", "BudgetCategoryID", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID", c => c.Int());
            AddColumn("Budget.BudgetPlanMonthly", "Unit", c => c.String());
            AddColumn("Budget.BudgetPlanMonthly", "AccountDesc", c => c.String());
            AddColumn("Budget.BudgetPlanMonthly", "AccountCode", c => c.String());
            AddColumn("Budget.BudgetCategory", "Description", c => c.String());
            DropForeignKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategory_ProfitLossStatementCategoryId", "BudgetGAAP.ProfitLossStatementCategory");
            DropForeignKey("Budget.BudgetPlanMonthly", "BudgetDefineID", "Budget.BudgetPlan");
            DropForeignKey("EXCEED.PurchaseRequisitionBudgetValidation", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("Budget.BudgetUsage", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.BudgetUsageDetail", "BudgetUsageID", "Budget.BudgetUsage");
            DropForeignKey("Budget.BudgetRequestDetail", "BudgetRequestId", "Budget.BudgetRequest");
            DropForeignKey("EXCEED.BudgetPlanAttachment", "PlanCategoryID", "Budget.PlanCategory");
            DropForeignKey("Budget.BudgetMonthlyReserve", "BudgetPlanMonthly_BudgetPlanMonthlyID1", "Budget.BudgetPlanMonthly");
            DropForeignKey("Budget.BudgetPlanReserve", "BudgetPlanID", "Budget.BudgetPlan");
            DropForeignKey("Budget.BudgetMonthlyReserve", "BudgetPlanMonthly_BudgetPlanMonthlyID", "Budget.BudgetPlanMonthly");
            DropForeignKey("Budget.BudgetCategoryDetail", "BudgetCategoryId", "Budget.BudgetCategory");
            DropIndex("EXCEED.PurchaseRequisitionBudgetValidation", new[] { "PurchaseRequisitionID" });
            DropIndex("BudgetGAAP.ProfitLossStatement", new[] { "ProfitLossStatementCategory_ProfitLossStatementCategoryId" });
            DropIndex("EXCEED.BudgetUsageDetail", new[] { "BudgetUsageID" });
            DropIndex("Budget.BudgetUsage", new[] { "GLPeriodId" });
            DropIndex("Budget.BudgetUsageApproval", new[] { "BudgetUsageID" });
            DropIndex("Budget.BudgetTransferApproval", new[] { "BudgetTransferID" });
            DropIndex("Budget.BudgetRequestDetail", new[] { "BudgetRequestId" });
            DropIndex("EXCEED.BudgetPlanAttachment", new[] { "PlanCategoryID" });
            DropIndex("Budget.BudgetPlanReserve", new[] { "BudgetPlanID" });
            DropIndex("Budget.BudgetMonthlyReserve", new[] { "BudgetPlanMonthly_BudgetPlanMonthlyID1" });
            DropIndex("Budget.BudgetMonthlyReserve", new[] { "BudgetPlanMonthly_BudgetPlanMonthlyID" });
            DropIndex("Budget.BudgetCategoryDetail", new[] { "BudgetCategoryId" });
            DropPrimaryKey("BudgetGAAP.ProfitLossStatement");
            AlterColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId", c => c.Int(nullable: false));
            AlterColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Budget.BudgetRequestTransferLog", "PlanCategoryId", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetRequest", "PlanCategoryId", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetPlan", "BranchId", c => c.Int(nullable: false));
            DropColumn("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategory_ProfitLossStatementCategoryId");
            DropColumn("Budget.PlanCategory", "ForwardedTime");
            DropColumn("Budget.PlanCategory", "CurrentApprovalStep");
            DropColumn("Budget.PlanCategory", "IsFinalApproval");
            DropColumn("Budget.PlanCategory", "ForwardedTo");
            DropColumn("Budget.PlanCategory", "ForwardedFrom");
            DropColumn("Budget.PlanCategory", "DepartmentSendForApprovalTime");
            DropColumn("Budget.PlanCategory", "DepartmentSendForApprovalBy");
            DropColumn("Budget.PlanCategory", "IsDepartmentSendForApproval");
            DropColumn("Budget.PlanCategory", "DepartmentOnlineApprovedTime");
            DropColumn("Budget.PlanCategory", "DepartmentOnlineApprovedBy");
            DropColumn("Budget.PlanCategory", "IsDepartmentOnlineApproved");
            DropColumn("Budget.PlanCategory", "RevisionOnlineApprovedTime");
            DropColumn("Budget.PlanCategory", "RevisionOnlineApprovedBy");
            DropColumn("Budget.PlanCategory", "IsRevisionOnlineApproved");
            DropColumn("Budget.PlanCategory", "IsOpenforrevision");
            DropColumn("Budget.PlanCategory", "RevisionSendForApprovalTime");
            DropColumn("Budget.PlanCategory", "RevisionSendForApprovalBy");
            DropColumn("Budget.PlanCategory", "IsRevisionSendForApproval");
            DropColumn("Budget.PlanCategory", "EmployeeId");
            DropColumn("Budget.PlanCategory", "OrgStuructureId");
            DropColumn("Budget.PlanCategory", "BusinessUnitId");
            DropColumn("Budget.PlanCategory", "BranchId");
            DropColumn("Budget.PlanCategory", "BudgetCategoryID");
            DropColumn("EXCEED.FundTransfer", "AmountDue");
            DropColumn("EXCEED.FundTransfer", "PaymentAmount");
            DropColumn("EXCEED.DashboardDictionary", "GlFiscalYearId");
            DropColumn("Budget.BudgetUsage", "NetAmount");
            DropColumn("Budget.BudgetUsage", "Amount");
            DropColumn("Budget.BudgetUsage", "RequestDate");
            DropColumn("Budget.BudgetUsage", "ReasonOfPayment");
            DropColumn("Budget.BudgetUsage", "EmployeeId");
            DropColumn("Budget.BudgetUsage", "BusinessUnitId");
            DropColumn("Budget.BudgetUsage", "BranchId");
            DropColumn("Budget.BudgetUsage", "GLPeriodId");
            DropColumn("Budget.BudgetUsage", "GlFiscalYearId");
            DropColumn("Budget.BudgetTransfer", "EqualToAllPeriods");
            DropColumn("Budget.BudgetTransfer", "ToGlPeriodId");
            DropColumn("Budget.BudgetTransfer", "ToBusinessUnitId");
            DropColumn("Budget.BudgetTransfer", "ToBranchId");
            DropColumn("Budget.BudgetTransfer", "ToOrgStructureId");
            DropColumn("Budget.BudgetTransfer", "ToCostCenter");
            DropColumn("Budget.BudgetTransfer", "FromGlPeriodId");
            DropColumn("Budget.BudgetTransfer", "FromBusinessUnitId");
            DropColumn("Budget.BudgetTransfer", "FromBranchId");
            DropColumn("Budget.BudgetTransfer", "FromOrgStructureId");
            DropColumn("Budget.BudgetTransfer", "FromCostCenter");
            DropColumn("Budget.BudgetTransfer", "GlFiscalYearId");
            DropColumn("Budget.BudgetRequest", "AccountCode");
            DropColumn("Budget.BudgetPlan", "BusinessUnitId");
            DropColumn("Budget.BudgetPlan", "OrgStructureId");
            DropColumn("Budget.BudgetPlan", "CostCenter");
            DropColumn("Budget.BudgetPlan", "BudgetItem");
            DropColumn("Budget.BudgetPlan", "BudgetCode");
            DropColumn("Budget.BudgetPlan", "FiscalYearId");
            DropColumn("Budget.BudgetPlan", "PlanCategoryId");
            DropColumn("Budget.BudgetPlanMonthly", "TransferedOutBudget");
            DropColumn("Budget.BudgetControl", "ControlPeriod");
            DropColumn("Budget.BudgetCategory", "BudgetType");
            DropColumn("Budget.BudgetCategory", "BudgetCategoryCode");
            DropTable("EXCEED.PurchaseRequisitionBudgetValidation");
            DropTable("EXCEED.FinancialRatio");
            DropTable("EXCEED.BudgetUsageDetail");
            DropTable("Budget.BudgetRequestDetail");
            DropTable("EXCEED.BudgetPlanRevisionApproval");
            DropTable("EXCEED.BudgetPlanDepartmentApproval");
            DropTable("EXCEED.BudgetPlanAttachment");
            DropTable("Budget.BudgetPlanReserve");
            DropTable("Budget.BudgetMonthlyReserve");
            DropTable("Budget.BudgetCategoryDetail");
            AddPrimaryKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementId");
            CreateIndex("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId");
            CreateIndex("Budget.BudgetUsageApproval", "BudgetUsageId");
            CreateIndex("Budget.BudgetTransferApproval", "BudgetTransferId");
            CreateIndex("Budget.BudgetPlan", "BranchId");
            CreateIndex("Budget.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID");
            AddForeignKey("BudgetGAAP.ProfitLossStatement", "ProfitLossStatementCategoryId", "BudgetGAAP.ProfitLossStatementCategory", "ProfitLossStatementCategoryId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetPlanMonthly", "BudgetDefineID", "Budget.BudgetPlan", "BudgetPlanID");
            AddForeignKey("Budget.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID", "Budget.BudgetTransfer", "BudgetTransferID");
            AddForeignKey("Budget.BudgetPlan", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
    }
}
