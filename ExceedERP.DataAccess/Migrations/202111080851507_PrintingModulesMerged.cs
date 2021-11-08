namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingModulesMerged : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.Machine", newName: "Machines");
            MoveTable(name: "JobCosting.JobOrderSummaryCostCenter", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.Machines", newSchema: "JobCosting");
            CreateTable(
                "PrintingEstimation.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ContactName = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        SubCity = c.String(),
                        Woreda = c.String(),
                        Pobox = c.String(),
                        VAT = c.Decimal(precision: 18, scale: 2),
                        Margin = c.Decimal(precision: 18, scale: 2),
                        OverHead = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "PrintingEstimation.EstimationForms",
                c => new
                    {
                        EstimationFormId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        JobNumber = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Size = c.String(nullable: false),
                        TextPaper = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TextPrint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CoverPrint = c.String(nullable: false),
                        BindingStyle = c.String(nullable: false),
                        Others = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationFormId)
                .ForeignKey("PrintingEstimation.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "PrintingEstimation.LaborCosts",
                c => new
                    {
                        LaborCostId = c.Int(nullable: false, identity: true),
                        PrintingSteps = c.Int(nullable: false),
                        EstimationFormId = c.Int(nullable: false),
                        PrintingStepId = c.Int(nullable: false),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LaborCostId)
                .ForeignKey("PrintingEstimation.EstimationForms", t => t.EstimationFormId, cascadeDelete: true)
                .ForeignKey("PrintingEstimation.PrintingSteps", t => t.PrintingStepId, cascadeDelete: true)
                .Index(t => t.EstimationFormId)
                .Index(t => t.PrintingStepId);
            
            CreateTable(
                "PrintingEstimation.PrintingSteps",
                c => new
                    {
                        PrintingStepId = c.Int(nullable: false, identity: true),
                        PrintingSteps = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingStepId);
            
            CreateTable(
                "PrintingEstimation.MaterialCosts",
                c => new
                    {
                        MaterialCostId = c.Int(nullable: false, identity: true),
                        EstimationFormId = c.Int(nullable: false),
                        InventoryId = c.String(),
                        InventoryCategoryId = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaterialCostId)
                .ForeignKey("PrintingEstimation.EstimationForms", t => t.EstimationFormId, cascadeDelete: true)
                .Index(t => t.EstimationFormId);
            
            CreateTable(
                "JobCosting.DailyLaborRates",
                c => new
                    {
                        DailyLaborRateId = c.Int(nullable: false, identity: true),
                        CostCenterId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        TotalSalary = c.Decimal(precision: 18, scale: 2),
                        TotalExpectedHour = c.Decimal(precision: 18, scale: 2),
                        DLR = c.Decimal(precision: 18, scale: 2),
                        EmployeeType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyLaborRateId);
            
            CreateTable(
                "JobCosting.DeliveredJobs",
                c => new
                    {
                        DeliveredJobId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        DoNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DeliveredJobId);
            
            CreateTable(
                "JobCosting.DLRHours",
                c => new
                    {
                        DLRHourId = c.Int(nullable: false, identity: true),
                        DailyLaborRateId = c.Int(nullable: false),
                        NoOfDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfWorkingHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfEmployee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DLRHourId);
            
            CreateTable(
                "JobCosting.DLRSalaries",
                c => new
                    {
                        DLRSalaryId = c.Int(nullable: false, identity: true),
                        DailyLaborRateId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(precision: 18, scale: 2),
                        LessIndirectLabor = c.Decimal(precision: 18, scale: 2),
                        NetSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LessAbsence = c.Decimal(precision: 18, scale: 2),
                        Total = c.Decimal(precision: 18, scale: 2),
                        Pension = c.Decimal(precision: 18, scale: 2),
                        TotalSalary = c.Decimal(precision: 18, scale: 2),
                        NoOfDay = c.Decimal(precision: 18, scale: 2),
                        RatePerDay = c.Decimal(precision: 18, scale: 2),
                        NoOfEmployee = c.Decimal(precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DLRSalaryId);
            
            CreateTable(
                "JobCosting.FinishedJobs",
                c => new
                    {
                        FinishedJobId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        FgrNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinishedJobId);
            
            CreateTable(
                "JobCosting.IdleTimeCategories",
                c => new
                    {
                        IdleTimeCategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        IsControllable = c.Boolean(nullable: false),
                        CategoryDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IdleTimeCategoryId);
            
            CreateTable(
                "JobCosting.IdleTimeNonProductives",
                c => new
                    {
                        IdleTimeNonProductiveControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        NPCManualHour = c.Decimal(precision: 18, scale: 2),
                        NPCManualPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCMechanicalHour = c.Decimal(precision: 18, scale: 2),
                        NPCMechanicalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCCameraHour = c.Decimal(precision: 18, scale: 2),
                        NPCCameraPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCOffsetHour = c.Decimal(precision: 18, scale: 2),
                        NPCOffsetPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCLetterPressHour = c.Decimal(precision: 18, scale: 2),
                        NPCLetterPressPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCBindingHour = c.Decimal(precision: 18, scale: 2),
                        NPCBindingPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCComputerHour = c.Decimal(precision: 18, scale: 2),
                        NPCComputerPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCWebHour = c.Decimal(precision: 18, scale: 2),
                        NPCWebPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPCTotalHour = c.Decimal(precision: 18, scale: 2),
                        NPCTotalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdleTimeNonProductiveControllableID)
                .ForeignKey("JobCosting.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .Index(t => t.IdleTimeID);
            
            CreateTable(
                "JobCosting.IdleTimes",
                c => new
                    {
                        IdleTimeID = c.Int(nullable: false, identity: true),
                        YearId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Period_GLPeriodId = c.Int(),
                        Year_GlFiscalYearId = c.Int(),
                    })
                .PrimaryKey(t => t.IdleTimeID)
                .ForeignKey("EXCEED.GLPeriod", t => t.Period_GLPeriodId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.Year_GlFiscalYearId)
                .Index(t => t.Period_GLPeriodId)
                .Index(t => t.Year_GlFiscalYearId);
            
            CreateTable(
                "JobCosting.IdleTimeNonProductiveNonControllables",
                c => new
                    {
                        IdleTimeNonProductiveNonControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        NPNCManualHour = c.Decimal(precision: 18, scale: 2),
                        NPNCManualPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCMechanicalHour = c.Decimal(precision: 18, scale: 2),
                        NPNCMechanicalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCCameraHour = c.Decimal(precision: 18, scale: 2),
                        NPNCCameraPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCOffsetHour = c.Decimal(precision: 18, scale: 2),
                        NPNCOffsetPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCLetterPressHour = c.Decimal(precision: 18, scale: 2),
                        NPNCLetterPressPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCBindingHour = c.Decimal(precision: 18, scale: 2),
                        NPNCBindingPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCComputerHour = c.Decimal(precision: 18, scale: 2),
                        NPNCComputerPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCWebHour = c.Decimal(precision: 18, scale: 2),
                        NPNCWebPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCTotalHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NPNCTotalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdleTimeNonProductiveNonControllableID)
                .ForeignKey("JobCosting.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .Index(t => t.IdleTimeID);
            
            CreateTable(
                "JobCosting.IdleTimeOTs",
                c => new
                    {
                        IdleTimeOTID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        OTManualHour = c.Decimal(precision: 18, scale: 2),
                        OTManualPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTMechanicalHour = c.Decimal(precision: 18, scale: 2),
                        OTMechanicalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTCameraHour = c.Decimal(precision: 18, scale: 2),
                        OTCameraPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTOffsetHour = c.Decimal(precision: 18, scale: 2),
                        OTOffsetPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTLetterPressHour = c.Decimal(precision: 18, scale: 2),
                        OTLetterPressPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTBindingHour = c.Decimal(precision: 18, scale: 2),
                        OTBindingPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTComputerHour = c.Decimal(precision: 18, scale: 2),
                        OTComputerPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTWebHour = c.Decimal(precision: 18, scale: 2),
                        OTWebPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OTTotalHour = c.Decimal(precision: 18, scale: 2),
                        OTTotalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdleTimeOTID)
                .ForeignKey("JobCosting.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .Index(t => t.IdleTimeID);
            
            CreateTable(
                "JobCosting.IdleTimeProductives",
                c => new
                    {
                        IdleTimeProductiveID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        ProductiveManualHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveManualPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveMechanicalHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveMechanicalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveCameraHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveCameraPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveOffsetHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveOffsetPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveLetterPressHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveLetterPressPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveBindingHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveBindingPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveComputerHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveComputerPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveWebHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveWebPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductiveTotalHour = c.Decimal(precision: 18, scale: 2),
                        ProductiveTotalPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdleTimeProductiveID)
                .ForeignKey("JobCosting.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .Index(t => t.IdleTimeID);
            
            CreateTable(
                "JobCosting.JobCategories",
                c => new
                    {
                        JobCategoryId = c.Int(nullable: false, identity: true),
                        JobName = c.String(),
                        JobCode = c.String(),
                        HaveParent = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobCategoryId);
            
            CreateTable(
                "JobCosting.JobChangeOfOrders",
                c => new
                    {
                        JobChangeOfOrderId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        ChangeOrderId = c.Int(nullable: false),
                        AdditionalChange = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobChangeOfOrderId);
            
            CreateTable(
                "JobCosting.JobCosts",
                c => new
                    {
                        JobCostId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        JobCategoryId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        JobReceivedDate = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReciptNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Void = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobCostId);
            
            CreateTable(
                "JobCosting.JobLaborCosts",
                c => new
                    {
                        JobLaborCostId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Reference = c.String(),
                        PermanetNormalHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanetOTHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractOTHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractNormalHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Beginning = c.Boolean(nullable: false),
                        BeginningHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobLaborCostId);
            
            CreateTable(
                "JobCosting.JobMaterialCosts",
                c => new
                    {
                        JobMaterialCostId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reference = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobMaterialCostId);
            
            CreateTable(
                "JobCosting.JobMaterialReturns",
                c => new
                    {
                        JobMaterialReturnId = c.Int(nullable: false, identity: true),
                        JobCostId = c.Int(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reference = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobMaterialReturnId);
            
            CreateTable(
                "ProductFollowUp.JobStatusEntityTypeConfiguration",
                c => new
                    {
                        JobStatusId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        UserId = c.String(),
                        Time = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        JobPhase = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobStatusId);
            
            CreateTable(
                "PrintingEstimation.OverallCosts",
                c => new
                    {
                        OverallCostId = c.Int(nullable: false, identity: true),
                        EstimationFormId = c.Int(nullable: false),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MarginalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NBT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TInpercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Graphic = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedNetPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OverallCostId)
                .ForeignKey("PrintingEstimation.EstimationForms", t => t.EstimationFormId, cascadeDelete: true)
                .Index(t => t.EstimationFormId);
            
            CreateTable(
                "JobCosting.Units",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        UOM = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.UnitId);
            
            CreateTable(
                "JobCosting.YearlyOverHeadRates",
                c => new
                    {
                        YearlyOverHeadRateId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        CostCenterID = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.YearlyOverHeadRateId);
            
            AddColumn("ProductFollowUp.FinishedGoodReceiving", "CustomerId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Job", "hv", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.Job", "IsExistingCustomer", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.Job", "CustomerId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Job", "Woreda", c => c.String());
            AddColumn("ProductFollowUp.Job", "CustomerVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "CustomerMargin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "CustomerOverHead", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "CustomerDescription", c => c.String());
            AddColumn("ProductFollowUp.Job", "OrderType", c => c.Int(nullable: false));
            AddColumn("EXCEED.JobType", "VAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.JobType", "Margin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("ProductFollowUp.ProductDelivery", "CustomerId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.ProductDeliveryItems", "UnitId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "InvoiceDate", c => c.DateTime(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "DeliveryDate", c => c.DateTime(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "ValidityDate", c => c.DateTime(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "PaymentTerm", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "HaveEstimation", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "IsExistingCustomer", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "InvoiceNo", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "EstimationFormId", c => c.Int());
            AddColumn("ProductFollowUp.Proforma", "CustomerId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "City", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "SubCity", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "Woreda", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "POBox", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "CustomerVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "CustomerMargin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "CustomerOverHead", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "CustomerDescription", c => c.String());
            DropColumn("ProductFollowUp.Job", "Wereda");
            DropColumn("EXCEED.PFProformaItems", "JobTypeItemId");
            DropColumn("EXCEED.PFProformaItems", "SubCategory");
            DropColumn("EXCEED.PFProformaItems", "Page");
            DropColumn("ProductFollowUp.ProductDeliveryItems", "Unit");
            DropTable("EXCEED.CommertialItems");
            DropTable("CRM.Tax");
        }
        
        public override void Down()
        {
            CreateTable(
                "CRM.Tax",
                c => new
                    {
                        TaxID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaxID);
            
            CreateTable(
                "EXCEED.CommertialItems",
                c => new
                    {
                        CommertialItemsId = c.Int(nullable: false, identity: true),
                        Job_Type = c.Int(nullable: false),
                        SubCategory = c.String(maxLength: 50),
                        Description = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.CommertialItemsId);
            
            AddColumn("ProductFollowUp.ProductDeliveryItems", "Unit", c => c.String());
            AddColumn("EXCEED.PFProformaItems", "Page", c => c.String());
            AddColumn("EXCEED.PFProformaItems", "SubCategory", c => c.String());
            AddColumn("EXCEED.PFProformaItems", "JobTypeItemId", c => c.Int());
            AddColumn("ProductFollowUp.Job", "Wereda", c => c.String());
            DropForeignKey("PrintingEstimation.OverallCosts", "EstimationFormId", "PrintingEstimation.EstimationForms");
            DropForeignKey("JobCosting.IdleTimes", "Year_GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("JobCosting.IdleTimes", "Period_GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("JobCosting.IdleTimeProductives", "IdleTimeID", "JobCosting.IdleTimes");
            DropForeignKey("JobCosting.IdleTimeOTs", "IdleTimeID", "JobCosting.IdleTimes");
            DropForeignKey("JobCosting.IdleTimeNonProductiveNonControllables", "IdleTimeID", "JobCosting.IdleTimes");
            DropForeignKey("JobCosting.IdleTimeNonProductives", "IdleTimeID", "JobCosting.IdleTimes");
            DropForeignKey("PrintingEstimation.MaterialCosts", "EstimationFormId", "PrintingEstimation.EstimationForms");
            DropForeignKey("PrintingEstimation.LaborCosts", "PrintingStepId", "PrintingEstimation.PrintingSteps");
            DropForeignKey("PrintingEstimation.LaborCosts", "EstimationFormId", "PrintingEstimation.EstimationForms");
            DropForeignKey("PrintingEstimation.EstimationForms", "CustomerId", "PrintingEstimation.Customers");
            DropIndex("PrintingEstimation.OverallCosts", new[] { "EstimationFormId" });
            DropIndex("JobCosting.IdleTimeProductives", new[] { "IdleTimeID" });
            DropIndex("JobCosting.IdleTimeOTs", new[] { "IdleTimeID" });
            DropIndex("JobCosting.IdleTimeNonProductiveNonControllables", new[] { "IdleTimeID" });
            DropIndex("JobCosting.IdleTimes", new[] { "Year_GlFiscalYearId" });
            DropIndex("JobCosting.IdleTimes", new[] { "Period_GLPeriodId" });
            DropIndex("JobCosting.IdleTimeNonProductives", new[] { "IdleTimeID" });
            DropIndex("PrintingEstimation.MaterialCosts", new[] { "EstimationFormId" });
            DropIndex("PrintingEstimation.LaborCosts", new[] { "PrintingStepId" });
            DropIndex("PrintingEstimation.LaborCosts", new[] { "EstimationFormId" });
            DropIndex("PrintingEstimation.EstimationForms", new[] { "CustomerId" });
            DropColumn("ProductFollowUp.Proforma", "CustomerDescription");
            DropColumn("ProductFollowUp.Proforma", "CustomerOverHead");
            DropColumn("ProductFollowUp.Proforma", "CustomerMargin");
            DropColumn("ProductFollowUp.Proforma", "CustomerVAT");
            DropColumn("ProductFollowUp.Proforma", "POBox");
            DropColumn("ProductFollowUp.Proforma", "Woreda");
            DropColumn("ProductFollowUp.Proforma", "SubCity");
            DropColumn("ProductFollowUp.Proforma", "City");
            DropColumn("ProductFollowUp.Proforma", "CustomerId");
            DropColumn("ProductFollowUp.Proforma", "EstimationFormId");
            DropColumn("ProductFollowUp.Proforma", "InvoiceNo");
            DropColumn("ProductFollowUp.Proforma", "IsExistingCustomer");
            DropColumn("ProductFollowUp.Proforma", "HaveEstimation");
            DropColumn("ProductFollowUp.Proforma", "PaymentTerm");
            DropColumn("ProductFollowUp.Proforma", "ValidityDate");
            DropColumn("ProductFollowUp.Proforma", "DeliveryDate");
            DropColumn("ProductFollowUp.Proforma", "InvoiceDate");
            DropColumn("ProductFollowUp.ProductDeliveryItems", "UnitId");
            DropColumn("ProductFollowUp.ProductDelivery", "CustomerId");
            DropColumn("EXCEED.JobType", "Margin");
            DropColumn("EXCEED.JobType", "VAT");
            DropColumn("ProductFollowUp.Job", "OrderType");
            DropColumn("ProductFollowUp.Job", "CustomerDescription");
            DropColumn("ProductFollowUp.Job", "CustomerOverHead");
            DropColumn("ProductFollowUp.Job", "CustomerMargin");
            DropColumn("ProductFollowUp.Job", "CustomerVAT");
            DropColumn("ProductFollowUp.Job", "Woreda");
            DropColumn("ProductFollowUp.Job", "CustomerId");
            DropColumn("ProductFollowUp.Job", "IsExistingCustomer");
            DropColumn("ProductFollowUp.Job", "hv");
            DropColumn("ProductFollowUp.FinishedGoodReceiving", "CustomerId");
            DropTable("JobCosting.YearlyOverHeadRates");
            DropTable("JobCosting.Units");
            DropTable("PrintingEstimation.OverallCosts");
            DropTable("ProductFollowUp.JobStatusEntityTypeConfiguration");
            DropTable("JobCosting.JobMaterialReturns");
            DropTable("JobCosting.JobMaterialCosts");
            DropTable("JobCosting.JobLaborCosts");
            DropTable("JobCosting.JobCosts");
            DropTable("JobCosting.JobChangeOfOrders");
            DropTable("JobCosting.JobCategories");
            DropTable("JobCosting.IdleTimeProductives");
            DropTable("JobCosting.IdleTimeOTs");
            DropTable("JobCosting.IdleTimeNonProductiveNonControllables");
            DropTable("JobCosting.IdleTimes");
            DropTable("JobCosting.IdleTimeNonProductives");
            DropTable("JobCosting.IdleTimeCategories");
            DropTable("JobCosting.FinishedJobs");
            DropTable("JobCosting.DLRSalaries");
            DropTable("JobCosting.DLRHours");
            DropTable("JobCosting.DeliveredJobs");
            DropTable("JobCosting.DailyLaborRates");
            DropTable("PrintingEstimation.MaterialCosts");
            DropTable("PrintingEstimation.PrintingSteps");
            DropTable("PrintingEstimation.LaborCosts");
            DropTable("PrintingEstimation.EstimationForms");
            DropTable("PrintingEstimation.Customers");
            MoveTable(name: "JobCosting.Machines", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.JobOrderSummaryCostCenter", newSchema: "JobCosting");
            RenameTable(name: "EXCEED.Machines", newName: "Machine");
        }
    }
}
