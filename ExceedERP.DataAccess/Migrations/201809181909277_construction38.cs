namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction38 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.CashFlowContractData",
                c => new
                    {
                        CashFlowContractDataId = c.Int(nullable: false, identity: true),
                        CashFlowYearId = c.Int(nullable: false),
                        TCPPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractorAmountWithVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DayWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOriginalContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplementaryAgreement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VariationWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowContractDataId)
                .ForeignKey("Construction.CashFlowYear", t => t.CashFlowYearId, cascadeDelete: true)
                .Index(t => t.CashFlowYearId);
            
            CreateTable(
                "Construction.CashFlowYear",
                c => new
                    {
                        CashFlowYearId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowYearId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Construction.CashFlowContractDataDetail",
                c => new
                    {
                        CashFlowContractDataDetailId = c.Int(nullable: false, identity: true),
                        CashFlowContractDataId = c.Int(nullable: false),
                        ConstructionPeriodId = c.Int(nullable: false),
                        MonthlyProgress = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CummulativeProgress = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractorAmountWithVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DayWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOriginalContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplementaryAgreement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VariationWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowContractDataDetailId)
                .ForeignKey("Construction.CashFlowContractData", t => t.CashFlowContractDataId, cascadeDelete: true)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .Index(t => t.CashFlowContractDataId)
                .Index(t => t.ConstructionPeriodId);
            
            CreateTable(
                "Construction.CashFlowDeduction",
                c => new
                    {
                        CashFlowDeductionId = c.Int(nullable: false, identity: true),
                        CashFlowYearId = c.Int(nullable: false),
                        MainContractAdvanceRepayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTaxes = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MainContractRetentionWithheld = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rebate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OmmissionWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Others = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowDeductionId)
                .ForeignKey("Construction.CashFlowYear", t => t.CashFlowYearId, cascadeDelete: true)
                .Index(t => t.CashFlowYearId);
            
            CreateTable(
                "Construction.CashFlowDeductionDetail",
                c => new
                    {
                        CashFlowDeductionDetailId = c.Int(nullable: false, identity: true),
                        CashFlowDeductionId = c.Int(nullable: false),
                        ConstructionPeriodId = c.Int(nullable: false),
                        MainContractAdvanceRepayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTaxes = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MainContractRetentionWithheld = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rebate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OmmissionWorks = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Others = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowDeductionDetailId)
                .ForeignKey("Construction.CashFlowDeduction", t => t.CashFlowDeductionId, cascadeDelete: true)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .Index(t => t.CashFlowDeductionId)
                .Index(t => t.ConstructionPeriodId);
            
            CreateTable(
                "Construction.CashFlowFinanceSourceDetail",
                c => new
                    {
                        CashFlowFinanceSourceDetailId = c.Int(nullable: false, identity: true),
                        CashFlowFinanceSourceId = c.Int(nullable: false),
                        ConstructionPeriodId = c.Int(nullable: false),
                        MainContractAdvancePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Revenue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuppliersCredit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorFinancing = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorRetentionWithholding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorAdvanceRepayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeFinancing = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MainContractRetentionReleased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSite = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFinanceSource = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowFinanceSourceDetailId)
                .ForeignKey("Construction.CashFlowFinanceSource", t => t.CashFlowFinanceSourceId, cascadeDelete: true)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .Index(t => t.CashFlowFinanceSourceId)
                .Index(t => t.ConstructionPeriodId);
            
            CreateTable(
                "Construction.CashFlowFinanceSource",
                c => new
                    {
                        CashFlowFinanceSourceId = c.Int(nullable: false, identity: true),
                        CashFlowYearId = c.Int(nullable: false),
                        MainContractAdvancePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Revenue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuppliersCredit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorFinancing = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorRetentionWithholding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractorAdvanceRepayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeFinancing = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MainContractRetentionReleased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSite = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFinanceSource = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashFlowFinanceSourceId)
                .ForeignKey("Construction.CashFlowYear", t => t.CashFlowYearId, cascadeDelete: true)
                .Index(t => t.CashFlowYearId);
            
            CreateTable(
                "Construction.NetOperationCashFlow",
                c => new
                    {
                        NetOperationCashFlowId = c.Int(nullable: false, identity: true),
                        CashFlowYearId = c.Int(nullable: false),
                        CummulativeNetOperationalCashFlow = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectMaterialExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectAndIndirectlaborExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectEquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectOverheadExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeOverheadExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractWorksAdvance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractPiecerateWorksSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeFinancingSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuppliersCreditSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DayWorksSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NetOperationCashFlowId)
                .ForeignKey("Construction.CashFlowYear", t => t.CashFlowYearId, cascadeDelete: true)
                .Index(t => t.CashFlowYearId);
            
            CreateTable(
                "Construction.NetOperationCashFlowDetail",
                c => new
                    {
                        NetOperationCashFlowDetailId = c.Int(nullable: false, identity: true),
                        NetOperationCashFlowId = c.Int(nullable: false),
                        ConstructionPeriodId = c.Int(nullable: false),
                        CummulativeNetOperationalCashFlow = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectMaterialExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectAndIndirectlaborExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectEquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectOverheadExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeOverheadExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractWorksAdvance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractPiecerateWorksSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeFinancingSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuppliersCreditSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DayWorksSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanSettlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NetOperationCashFlowDetailId)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .ForeignKey("Construction.NetOperationCashFlow", t => t.NetOperationCashFlowId, cascadeDelete: true)
                .Index(t => t.NetOperationCashFlowId)
                .Index(t => t.ConstructionPeriodId);
            
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Cement");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ReadyMixConcrete");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ReinforcementBar");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "FormWork");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Sand");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Ceramic");
        }
        
        public override void Down()
        {
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Ceramic", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Sand", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "FormWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ReinforcementBar", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ReadyMixConcrete", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Cement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("Construction.NetOperationCashFlowDetail", "NetOperationCashFlowId", "Construction.NetOperationCashFlow");
            DropForeignKey("Construction.NetOperationCashFlowDetail", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.NetOperationCashFlow", "CashFlowYearId", "Construction.CashFlowYear");
            DropForeignKey("Construction.CashFlowFinanceSourceDetail", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.CashFlowFinanceSourceDetail", "CashFlowFinanceSourceId", "Construction.CashFlowFinanceSource");
            DropForeignKey("Construction.CashFlowFinanceSource", "CashFlowYearId", "Construction.CashFlowYear");
            DropForeignKey("Construction.CashFlowDeductionDetail", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.CashFlowDeductionDetail", "CashFlowDeductionId", "Construction.CashFlowDeduction");
            DropForeignKey("Construction.CashFlowDeduction", "CashFlowYearId", "Construction.CashFlowYear");
            DropForeignKey("Construction.CashFlowContractDataDetail", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.CashFlowContractDataDetail", "CashFlowContractDataId", "Construction.CashFlowContractData");
            DropForeignKey("Construction.CashFlowContractData", "CashFlowYearId", "Construction.CashFlowYear");
            DropForeignKey("Construction.CashFlowYear", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.CashFlowYear", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("Construction.NetOperationCashFlowDetail", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.NetOperationCashFlowDetail", new[] { "NetOperationCashFlowId" });
            DropIndex("Construction.NetOperationCashFlow", new[] { "CashFlowYearId" });
            DropIndex("Construction.CashFlowFinanceSource", new[] { "CashFlowYearId" });
            DropIndex("Construction.CashFlowFinanceSourceDetail", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.CashFlowFinanceSourceDetail", new[] { "CashFlowFinanceSourceId" });
            DropIndex("Construction.CashFlowDeductionDetail", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.CashFlowDeductionDetail", new[] { "CashFlowDeductionId" });
            DropIndex("Construction.CashFlowDeduction", new[] { "CashFlowYearId" });
            DropIndex("Construction.CashFlowContractDataDetail", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.CashFlowContractDataDetail", new[] { "CashFlowContractDataId" });
            DropIndex("Construction.CashFlowYear", new[] { "GlFiscalYearId" });
            DropIndex("Construction.CashFlowYear", new[] { "ProjectInformationId" });
            DropIndex("Construction.CashFlowContractData", new[] { "CashFlowYearId" });
            DropTable("Construction.NetOperationCashFlowDetail");
            DropTable("Construction.NetOperationCashFlow");
            DropTable("Construction.CashFlowFinanceSource");
            DropTable("Construction.CashFlowFinanceSourceDetail");
            DropTable("Construction.CashFlowDeductionDetail");
            DropTable("Construction.CashFlowDeduction");
            DropTable("Construction.CashFlowContractDataDetail");
            DropTable("Construction.CashFlowYear");
            DropTable("Construction.CashFlowContractData");
        }
    }
}
