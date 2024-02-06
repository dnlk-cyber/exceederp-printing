namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printfurniture : DbMigration
    {
        public override void Up()
        {
           // DropPrimaryKey("Printing.IdleTimeNonProductiveNonControllables");
            CreateTable(
                "Manufacturing.DirectLaborCosts",
                c => new
                    {
                        DirectLaborCostId = c.Int(nullable: false, identity: true),
                        FurnitureCategoryId = c.Int(nullable: false),
                        FurnitureEstimationId = c.Int(nullable: false),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DirectLaborCostId)
                .ForeignKey("Manufacturing.FurnitureCategories", t => t.FurnitureCategoryId, cascadeDelete: true)
                .ForeignKey("Manufacturing.FurnitureCostEstimations", t => t.FurnitureEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureCategoryId)
                .Index(t => t.FurnitureEstimationId);
            
            CreateTable(
                "Manufacturing.FurnitureCategories",
                c => new
                    {
                        FurnitureCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureCategoryId);
            
            CreateTable(
                "Manufacturing.FurnitureCostEstimations",
                c => new
                    {
                        FurnitureEstimationId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JT = c.String(),
                        JobNumber = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Others = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        CheckedBy = c.String(),
                        ApprovedBy = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureEstimationId);
            
            CreateTable(
                "Manufacturing.FurnitureMaterialCosts",
                c => new
                    {
                        FurnitureMaterialCostId = c.Int(nullable: false, identity: true),
                        FurnitureEstimationId = c.Int(nullable: false),
                        InventoryCategoryId = c.String(),
                        InventoryId = c.String(),
                        JobId = c.Int(),
                        StoreCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureMaterialCostId)
                .ForeignKey("Manufacturing.FurnitureCostEstimations", t => t.FurnitureEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureEstimationId);
            
            CreateTable(
                "EXCEED.FurnitureChangeOrder",
                c => new
                    {
                        FurnitureChangeOrderId = c.Int(nullable: false, identity: true),
                        FurnitureJobId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                        Quantity = c.Int(nullable: false),
                        Size = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceNo = c.String(),
                        OrderNo = c.String(),
                        PreparedBy = c.String(),
                        TypeAndQuantityOfMaterialNeeded = c.String(),
                        Checked = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureChangeOrderId);
            
            CreateTable(
                "Manufacturing.FurnitureDailyLaborRates",
                c => new
                    {
                        FurnitureDailyLaborRateId = c.Int(nullable: false, identity: true),
                        CostCenter = c.String(),
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
                .PrimaryKey(t => t.FurnitureDailyLaborRateId);
            
            CreateTable(
                "Manufacturing.FurnitureDeliveredJobs",
                c => new
                    {
                        FurnitureDeliveredJobId = c.Int(nullable: false, identity: true),
                        FurnitureJobCostId = c.Int(nullable: false),
                        DoNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureDeliveredJobId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureJobCosts",
                c => new
                    {
                        FurnitureJobCostId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureDLRHours",
                c => new
                    {
                        FurnitureDLRHourId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.FurnitureDLRHourId);
            
            CreateTable(
                "Manufacturing.FurnitureDLRSalaries",
                c => new
                    {
                        FurnitureDLRSalaryId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.FurnitureDLRSalaryId);
            
            CreateTable(
                "Manufacturing.FurnitureFinishedGoodItems",
                c => new
                    {
                        FurnitureFinishedGoodsId = c.Int(nullable: false, identity: true),
                        FurnitureFinishedGoodReceivingId = c.Int(nullable: false),
                        CodeNo = c.String(),
                        Description = c.String(),
                        Unit = c.String(),
                        Quantity = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureFinishedGoodsId);
            
            CreateTable(
                "Manufacturing.FurnitureFinishedJobs",
                c => new
                    {
                        FurnitureFinishedJobId = c.Int(nullable: false, identity: true),
                        FurnitureJobCostId = c.Int(nullable: false),
                        FgrNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureFinishedJobId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureHalfYearlyProductionPlans",
                c => new
                    {
                        FurnitureHalfYearlyProductionPlanId = c.Int(nullable: false, identity: true),
                        YearlyProductionPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureJobProcuctionPlan_FurnitureJobProcuctionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureHalfYearlyProductionPlanId)
                .ForeignKey("Manufacturing.FurnitureYearlyProductionPlans", t => t.FurnitureJobProcuctionPlan_FurnitureJobProcuctionPlanId)
                .Index(t => t.FurnitureJobProcuctionPlan_FurnitureJobProcuctionPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureYearlyProductionPlans",
                c => new
                    {
                        FurnitureJobProcuctionPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureJobProcuctionPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Manufacturing.FurnitureQuarterlyProductionPlans",
                c => new
                    {
                        FurnitureQuarterlyProductionPlanId = c.Int(nullable: false, identity: true),
                        FurnitureHalfYearlyProductionPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureQuarterlyProductionPlanId)
                .ForeignKey("Manufacturing.FurnitureHalfYearlyProductionPlans", t => t.FurnitureHalfYearlyProductionPlanId, cascadeDelete: true)
                .Index(t => t.FurnitureHalfYearlyProductionPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimeCategories",
                c => new
                    {
                        FurnitureIdleTimeCategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        IsControllable = c.Boolean(nullable: false),
                        CategoryDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeCategoryId);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimeNonProductives",
                c => new
                    {
                        FurnitureIdleTimeNonProductiveControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureIdleTime_FurnitureIdleTimeID = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeNonProductiveControllableID)
                .ForeignKey("Printing.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .ForeignKey("Manufacturing.FurnitureIdleTimes", t => t.FurnitureIdleTime_FurnitureIdleTimeID)
                .Index(t => t.IdleTimeID)
                .Index(t => t.FurnitureIdleTime_FurnitureIdleTimeID);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimeNonProductiveNonControllables",
                c => new
                    {
                        FurnitureIdleTimeNonProductiveNonControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureIdleTime_FurnitureIdleTimeID = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeNonProductiveNonControllableID)
                .ForeignKey("Printing.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .ForeignKey("Manufacturing.FurnitureIdleTimes", t => t.FurnitureIdleTime_FurnitureIdleTimeID)
                .Index(t => t.IdleTimeID)
                .Index(t => t.FurnitureIdleTime_FurnitureIdleTimeID);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimeOTs",
                c => new
                    {
                        FurnitureIdleTimeOTID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureIdleTime_FurnitureIdleTimeID = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeOTID)
                .ForeignKey("Manufacturing.FurnitureIdleTimes", t => t.FurnitureIdleTime_FurnitureIdleTimeID)
                .Index(t => t.FurnitureIdleTime_FurnitureIdleTimeID);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimeProductives",
                c => new
                    {
                        FurnitureIdleTimeProductiveID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureIdleTime_FurnitureIdleTimeID = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeProductiveID)
                .ForeignKey("Printing.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .ForeignKey("Manufacturing.FurnitureIdleTimes", t => t.FurnitureIdleTime_FurnitureIdleTimeID)
                .Index(t => t.IdleTimeID)
                .Index(t => t.FurnitureIdleTime_FurnitureIdleTimeID);
            
            CreateTable(
                "Manufacturing.FurnitureIdleTimes",
                c => new
                    {
                        FurnitureIdleTimeID = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureIdleTimeID);
            
            CreateTable(
                "Manufacturing.FurnitureJcPensions",
                c => new
                    {
                        FurnitureJcPensionId = c.Int(nullable: false, identity: true),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJcPensionId);
            
            CreateTable(
                "Manufacturing.FurnitureJobCategories",
                c => new
                    {
                        FurnitureJobCategoryId = c.Int(nullable: false, identity: true),
                        JobName = c.String(),
                        JobCode = c.String(),
                        HaveParent = c.Boolean(nullable: false),
                        FurnitureParentId = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobCategoryId);
            
            CreateTable(
                "Manufacturing.FurnitureJobChangeOfOrders",
                c => new
                    {
                        FurnitureJobChangeOfOrderId = c.Int(nullable: false, identity: true),
                        FurnitureChangeOrderId = c.Int(nullable: false),
                        AdditionalChange = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
                        FurnitureJobCostId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobChangeOfOrderId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureJobLaborCosts",
                c => new
                    {
                        FurnitureJobLaborCostId = c.Int(nullable: false, identity: true),
                        FurnitureJobCostId = c.Int(nullable: false),
                        CostCenter = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Reference = c.String(),
                        PermanetNormalHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanetLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanetOverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanetOTHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractNormalHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractOTHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractOverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Beginning = c.Boolean(nullable: false),
                        BeginningHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobLaborCostId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureJobMaterialCosts",
                c => new
                    {
                        FurnitureJobMaterialCostId = c.Int(nullable: false, identity: true),
                        FurnitureJobCostId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(),
                        Date = c.DateTime(nullable: false),
                        Reference = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobMaterialCostId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureJobMaterialReturns",
                c => new
                    {
                        FurnitureJobMaterialReturnId = c.Int(nullable: false, identity: true),
                        FurnitureJobCostId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(),
                        Reference = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDescription = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobMaterialReturnId)
                .ForeignKey("Manufacturing.FurnitureJobCosts", t => t.FurnitureJobCostId, cascadeDelete: true)
                .Index(t => t.FurnitureJobCostId);
            
            CreateTable(
                "Manufacturing.FurnitureJobOrderMaterial",
                c => new
                    {
                        FurnitureJobOrderMaterialId = c.Int(nullable: false, identity: true),
                        FurnitureJobId = c.Int(nullable: false),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Gram = c.Double(nullable: false),
                        Size = c.Double(nullable: false),
                        Quantity = c.Double(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                        SIVNo = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderMaterialId);
            
            CreateTable(
                "Manufacturing.FurnitureJobOrderSummaryCostCenterDetail",
                c => new
                    {
                        FurnitureJobOrderSummaryCostCenterDetailId = c.Int(nullable: false, identity: true),
                        FurnitureJobOrderSummaryCostCenterId = c.Int(nullable: false),
                        CostCenter = c.String(),
                        Date = c.DateTime(nullable: false),
                        DirectLaborTotalHour = c.Double(nullable: false),
                        Rate = c.Double(nullable: false),
                        DirectLaborAmount = c.Double(nullable: false),
                        DirectMachineTotalHour = c.Double(nullable: false),
                        OverHeadRate = c.Double(nullable: false),
                        OverHeadAmount = c.Double(nullable: false),
                        TotalCostOfTheCenter = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderSummaryCostCenterDetailId);
            
            CreateTable(
                "EXCEED.FurnitureJobOrderSummaryCostCenter",
                c => new
                    {
                        FurnitureJobOrderSummaryCostCenterId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderSummaryCostCenterId);
            
            CreateTable(
                "Manufacturing.FurnitureJobOrderSummary",
                c => new
                    {
                        FurnitureJobOrderSummaryId = c.Int(nullable: false, identity: true),
                        JobId = c.String(),
                        Product = c.String(),
                        Quantity = c.Int(nullable: false),
                        Page = c.Int(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        DirectMaterial = c.Double(nullable: false),
                        DirectLabor = c.Double(nullable: false),
                        ManufacturingOverHead = c.Double(nullable: false),
                        SellingAndDistribution = c.Double(nullable: false),
                        AdminAndGeneralExpense = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        ProfitOrLoss = c.Double(nullable: false),
                        Percentage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderSummaryId);
            
            CreateTable(
                "Manufacturing.FurnitureJobOrderTimeRegistration",
                c => new
                    {
                        FurnitureJobOrderTimeRegistrationId = c.Int(nullable: false, identity: true),
                        FurnitureJobId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderTimeRegistrationId);
            
            CreateTable(
                "Manufacturing.FurnitureJobOrderTimeSheet",
                c => new
                    {
                        FurnitureJobOrderTimeSheetId = c.Int(nullable: false, identity: true),
                        FurnitureJobOrderTimeRegistrationId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Work = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        ComplationTime = c.DateTime(nullable: false),
                        TotalTime = c.DateTime(nullable: false),
                        NoOfEmployee = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderTimeSheetId);
            
            CreateTable(
                "Manufacturing. FurnitureJob",
                c => new
                    {
                        FurnitureJobId = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        hv = c.Boolean(nullable: false),
                        IsExistingCustomer = c.Boolean(nullable: false),
                        FurnitureProformaId = c.Int(),
                        FurnitureCustomerId = c.Int(nullable: false),
                        JobNo = c.String(),
                        Status = c.Int(),
                        ProductionCustomertypeId = c.Int(nullable: false),
                        CustomerName = c.String(),
                        EstimatedBy = c.String(),
                        Code = c.String(),
                        BrandName = c.String(),
                        TinNo = c.String(),
                        ContactPhone = c.String(),
                        FurnitureJobTypeId = c.Int(nullable: false),
                        JT = c.String(),
                        Pages = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Copies = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitId = c.String(),
                        UnitPrice = c.Double(nullable: false),
                        BeforeTax = c.Double(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Contract = c.Boolean(nullable: false),
                        TotalImpression = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        TextWeight = c.String(),
                        CoverWeight = c.String(),
                        ColorText = c.String(),
                        Colorcover = c.String(),
                        FinishingSize = c.String(maxLength: 15),
                        PrintingLine = c.String(maxLength: 15),
                        PrintingMachine = c.String(maxLength: 15),
                        Sewing = c.Boolean(nullable: false),
                        New = c.Boolean(nullable: false),
                        PerfectBinding = c.Boolean(nullable: false),
                        Repeat = c.Boolean(nullable: false),
                        Lamnating = c.Boolean(nullable: false),
                        Varnish = c.Boolean(nullable: false),
                        Perforate = c.Boolean(nullable: false),
                        Camsur = c.Boolean(nullable: false),
                        GraphicDesignExpectedCompletetion = c.DateTime(),
                        PrintingExpectedCompletetion = c.DateTime(),
                        FinishExpectedCompletetion = c.DateTime(),
                        FinishingWorkflow = c.String(maxLength: 50),
                        MachineId = c.Int(),
                        ApprovedBy = c.String(),
                        CheckedBy = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        IsChecked = c.Boolean(nullable: false),
                        IsMaga = c.Boolean(nullable: false),
                        Reason = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobId);
            
            CreateTable(
                "Manufacturing. FurnitureJobStatuses",
                c => new
                    {
                        FurnitureJobStatusId = c.Int(nullable: false, identity: true),
                        FurnitureJobId = c.Int(nullable: false),
                        FurnitureUserId = c.String(),
                        Time = c.DateTime(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        JobPhase = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobStatusId)
                .ForeignKey("Manufacturing. FurnitureJob", t => t.FurnitureJobId, cascadeDelete: true)
                .Index(t => t.FurnitureJobId);
            
            CreateTable(
                "Manufacturing.FurnitureJobStatusLineItems",
                c => new
                    {
                        FurnitureJobStatusLineItemId = c.Int(nullable: false, identity: true),
                        FurnitureJobStatusId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Quantiy = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainigQuantiy = c.Decimal(precision: 18, scale: 2),
                        TotalQuantiy = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Invoice = c.String(),
                        SalesVoucherNo = c.String(),
                        Cash = c.Decimal(precision: 18, scale: 2),
                        Credit = c.Decimal(precision: 18, scale: 2),
                        CashCreditInvoice = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobStatusLineItemId)
                .ForeignKey("Manufacturing. FurnitureJobStatuses", t => t.FurnitureJobStatusId, cascadeDelete: true)
                .Index(t => t.FurnitureJobStatusId);
            
            CreateTable(
                "Manufacturing.FurnitureLaborCosts",
                c => new
                    {
                        FurnitureLaborCostId = c.Int(nullable: false, identity: true),
                        FurnitureSteps = c.Int(nullable: false),
                        EstimationFormId = c.Int(nullable: false),
                        FurnitureStepId = c.Int(nullable: false),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        EstimationForm_FurnitureEstimationId = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureLaborCostId)
                .ForeignKey("Manufacturing.FurnitureCostEstimations", t => t.EstimationForm_FurnitureEstimationId)
                .ForeignKey("Manufacturing.FurnitureSteps", t => t.FurnitureStepId, cascadeDelete: true)
                .Index(t => t.FurnitureStepId)
                .Index(t => t.EstimationForm_FurnitureEstimationId);
            
            CreateTable(
                "Manufacturing.FurnitureSteps",
                c => new
                    {
                        FurnitureStepId = c.Int(nullable: false, identity: true),
                        FurnitureSteps = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureStepId);
            
            CreateTable(
                "Manufacturing.FurnitureMachines",
                c => new
                    {
                        FurnitureMachineId = c.Int(nullable: false, identity: true),
                        MachineName = c.String(),
                        Category = c.Int(nullable: false),
                        SubCategory = c.Int(nullable: false),
                        Description = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.FurnitureMachineId);
            
            CreateTable(
                "Manufacturing.FurnitureMonthlyProductionPlans",
                c => new
                    {
                        FurnitureMonthlyProductionPlanId = c.Int(nullable: false, identity: true),
                        QuarterlyProductionPlanId = c.Int(nullable: false),
                        WorkingDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkingHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureMonthlyProductionPlanId)
                .ForeignKey("Manufacturing.FurnitureQuarterlyProductionPlans", t => t.QuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId)
                .Index(t => t.QuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureWeeklyProductionPlans",
                c => new
                    {
                        FurnitureWeeklyProductionPlanId = c.Int(nullable: false, identity: true),
                        FurnitureMonthlyProductionPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureWeeklyProductionPlanId)
                .ForeignKey("Manufacturing.FurnitureMonthlyProductionPlans", t => t.FurnitureMonthlyProductionPlanId, cascadeDelete: true)
                .Index(t => t.FurnitureMonthlyProductionPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureOverallCosts",
                c => new
                    {
                        FurnitureOverallCostId = c.Int(nullable: false, identity: true),
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
                        FurnitureEstimationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureOverallCostId)
                .ForeignKey("Manufacturing.FurnitureCostEstimations", t => t.FurnitureEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureEstimationId);
            
            CreateTable(
                "Manufacturing.FurniturePFProformaItems",
                c => new
                    {
                        FurniturePFProformaItemsId = c.Int(nullable: false, identity: true),
                        FurnitureProformaId = c.Int(nullable: false),
                        JT = c.String(),
                        FurnitureJobTypeId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Size = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BeforeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FurniturePFProformaItemsId);
            
            CreateTable(
                "Manufacturing.FurnitureProductDelivery",
                c => new
                    {
                        FurnitureProductDeliveryId = c.Int(nullable: false, identity: true),
                        FurnitureCustomerId = c.Int(nullable: false),
                        CustomerRefNo = c.String(),
                        CreditSaleNo = c.String(),
                        CashSaleNo = c.String(),
                        Cash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryDate = c.DateTime(),
                        PreparedBy = c.String(),
                        ApprovedBy = c.String(),
                        DeliveryStatus = c.Int(nullable: false),
                        IdentificationNo = c.String(),
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
                .PrimaryKey(t => t.FurnitureProductDeliveryId);
            
            CreateTable(
                "Manufacturing.FurnitureProductDeliveryItems",
                c => new
                    {
                        FurnitureProductDeliveryItemsId = c.Int(nullable: false, identity: true),
                        FurnitureProductDeliveryId = c.Int(nullable: false),
                        FurnitureJobId = c.Int(nullable: false),
                        ItemCategory = c.String(),
                        ItemCode = c.String(),
                        Description = c.String(),
                        UnitId = c.Int(nullable: false),
                        Quantity = c.Double(nullable: false),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureProductDeliveryItemsId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionCustomerTypes",
                c => new
                    {
                        FurnitureProductionCustomerTypeId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Location = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureProductionCustomerTypeId);
            
            CreateTable(
                "EXCEED.FurnitureProductionFinishedGoodsStoreReceiveDistribution",
                c => new
                    {
                        FurnitureFinishedGoodsStoreReceiveDistributionId = c.Int(nullable: false, identity: true),
                        FurnitureFinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        ReceiveItemIDTrackBack = c.Int(nullable: false),
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
                .PrimaryKey(t => t.FurnitureFinishedGoodsStoreReceiveDistributionId)
                .ForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceive", t => t.FurnitureFinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.FurnitureFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "EXCEED.FurnitureProductionFinishedGoodsStoreReceive",
                c => new
                    {
                        FurnitureFinishedGoodsStoreReceiveId = c.Int(nullable: false, identity: true),
                        FurnitureCustomerId = c.Int(nullable: false),
                        VoucherNo = c.String(),
                        StoreCode = c.String(),
                        FurnitureJobId = c.Int(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
                        FurnitureProductionPeriodId = c.Int(nullable: false),
                        DeliveredBy = c.String(),
                        ReceivedBy = c.String(),
                        Supervisor = c.String(),
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
                .PrimaryKey(t => t.FurnitureFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem",
                c => new
                    {
                        FurnitureFinishedGoodsStoreReceiveItemId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        FinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UOM = c.String(),
                        AccountCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        PrintingProductionFinishedGoodsStoreReceive_FurnitureFinishedGoodsStoreReceiveId = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureFinishedGoodsStoreReceiveItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .ForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceive", t => t.PrintingProductionFinishedGoodsStoreReceive_FurnitureFinishedGoodsStoreReceiveId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.PrintingProductionFinishedGoodsStoreReceive_FurnitureFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "EXCEED.FurnitureProductionFinishedGoodsStoreReceiveValidation",
                c => new
                    {
                        FurnitureFinishedGoodsStoreReceiveValidationId = c.Int(nullable: false, identity: true),
                        FurnitureFinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureFinishedGoodsStoreReceiveValidationId)
                .ForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceive", t => t.FurnitureFinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.FurnitureFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionJobTypePlans",
                c => new
                    {
                        FurnitureProductionJobTypePlanId = c.Int(nullable: false, identity: true),
                        FurnitureWeeklyProductionPlanId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureJobType_FurnitureJobCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.FurnitureProductionJobTypePlanId)
                .ForeignKey("Manufacturing.FurnitureJobCategories", t => t.FurnitureJobType_FurnitureJobCategoryId)
                .ForeignKey("Manufacturing.FurnitureWeeklyProductionPlans", t => t.FurnitureWeeklyProductionPlanId, cascadeDelete: true)
                .Index(t => t.FurnitureWeeklyProductionPlanId)
                .Index(t => t.FurnitureJobType_FurnitureJobCategoryId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionMonthlyJobTypePlans",
                c => new
                    {
                        FurnitureProductionMonthlyJobTypePlanId = c.Int(nullable: false, identity: true),
                        FurnitureProductionMonthlyPlanId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureProductionMonthlyJobTypePlanId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionMonthlyPlans",
                c => new
                    {
                        FurnitureProductionMonthlyPlanId = c.Int(nullable: false, identity: true),
                        FurnitureProductionYearlyPlanId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureProductionMonthlyPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionYearlyPlans",
                c => new
                    {
                        FurnitureProductionYearlyPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureProductionYearlyPlanId);
            
            CreateTable(
                "Manufacturing.FurnitureProforma",
                c => new
                    {
                        FurnitureProformaId = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(),
                        InvoiceDate = c.DateTime(),
                        DeliveryDate = c.DateTime(),
                        ValidityDate = c.DateTime(),
                        PaymentTerm = c.Int(nullable: false),
                        HaveEstimation = c.Boolean(nullable: false),
                        IsExistingCustomer = c.Boolean(nullable: false),
                        ProformaNo = c.String(),
                        InvoiceNo = c.String(),
                        EstimationFormId = c.Int(),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(),
                        BrandName = c.String(maxLength: 50),
                        TinNo = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        CheckedBy = c.String(),
                        ApprovedBy = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureProformaId);
            
            CreateTable(
                "EXCEED.FurnitureUnit",
                c => new
                    {
                        FurnitureUnitId = c.Int(nullable: false, identity: true),
                        UOM = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureUnitId);
            
            CreateTable(
                "Manufacturing.FurnitureYearlyOverHeadRates",
                c => new
                    {
                        FurnitureYearlyOverHeadRateId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        Values = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureYearlyOverHeadRateId);
            
            CreateTable(
                "EXCEED.FurnitureYearlyProductionPlan",
                c => new
                    {
                        FurnitureYearlyProductionPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FurnitureYearlyProductionPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.PrintingCriteriaCategory",
                c => new
                    {
                        PrintingCriteriaCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingCriteriaCategoryId);
            
            CreateTable(
                "Manufacturing. FurnitureGraphic",
                c => new
                    {
                        FurnitureGraphicID = c.Int(nullable: false, identity: true),
                        FurnitureJobID = c.Int(nullable: false),
                        FurnitureUserID = c.String(maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureGraphicID);
            
            CreateTable(
                "Manufacturing. FurnitureSettings",
                c => new
                    {
                        FurnitureSettingsId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 150),
                        CompanyAddress = c.String(nullable: false, maxLength: 15),
                        CompanyPhone = c.String(nullable: false, maxLength: 15),
                        CompanyEmail = c.String(nullable: false, maxLength: 50),
                        CompanyMail = c.String(nullable: false, maxLength: 50),
                        CompanyVatNo = c.String(nullable: false, maxLength: 50),
                        CompanyTIN = c.String(nullable: false, maxLength: 50),
                        CompanyLogo = c.Binary(nullable: false),
                        CompanyScreen = c.Binary(nullable: false),
                        SMSSenderStatus = c.String(nullable: false, maxLength: 15),
                        EditGrace = c.Int(nullable: false),
                        FiscalYearStart = c.DateTime(nullable: false),
                        DashboardCalendar = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.FurnitureSettingsId);
            
            AddColumn("Printing.HalfYearlyProductionPlans", "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId", c => c.Int());
            AddColumn("Printing.MonthlyProductionPlans", "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId", c => c.Int());
            //AddColumn("Printing.IdleTimeNonProductiveNonControllables", "PrintingIdleTimeNonProductiveNonControllableID", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("Printing.IdleTimeNonProductiveNonControllables", "PrintingIdleTimeNonProductiveNonControllableID");
            CreateIndex("Printing.MonthlyProductionPlans", "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId");
            CreateIndex("Printing.HalfYearlyProductionPlans", "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId");
            AddForeignKey("Printing.MonthlyProductionPlans", "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId", "Manufacturing.FurnitureQuarterlyProductionPlans", "FurnitureQuarterlyProductionPlanId");
            AddForeignKey("Printing.HalfYearlyProductionPlans", "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId", "EXCEED.FurnitureYearlyProductionPlan", "FurnitureYearlyProductionPlanId");
           // DropColumn("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeNonProductiveNonControllableID");
        }
        
        public override void Down()
        {
            AddColumn("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeNonProductiveNonControllableID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("Printing.HalfYearlyProductionPlans", "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId", "EXCEED.FurnitureYearlyProductionPlan");
            DropForeignKey("EXCEED.FurnitureYearlyProductionPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Manufacturing.FurnitureProductionJobTypePlans", "FurnitureWeeklyProductionPlanId", "Manufacturing.FurnitureWeeklyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureProductionJobTypePlans", "FurnitureJobType_FurnitureJobCategoryId", "Manufacturing.FurnitureJobCategories");
            DropForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveValidation", "FurnitureFinishedGoodsStoreReceiveId", "EXCEED.FurnitureProductionFinishedGoodsStoreReceive");
            DropForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", "PrintingProductionFinishedGoodsStoreReceive_FurnitureFinishedGoodsStoreReceiveId", "EXCEED.FurnitureProductionFinishedGoodsStoreReceive");
            DropForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveDistribution", "FurnitureFinishedGoodsStoreReceiveId", "EXCEED.FurnitureProductionFinishedGoodsStoreReceive");
            DropForeignKey("Manufacturing.FurnitureOverallCosts", "FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations");
            DropForeignKey("Manufacturing.FurnitureWeeklyProductionPlans", "FurnitureMonthlyProductionPlanId", "Manufacturing.FurnitureMonthlyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureMonthlyProductionPlans", "QuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId", "Manufacturing.FurnitureQuarterlyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureLaborCosts", "FurnitureStepId", "Manufacturing.FurnitureSteps");
            DropForeignKey("Manufacturing.FurnitureLaborCosts", "EstimationForm_FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations");
            DropForeignKey("Manufacturing.FurnitureJobStatusLineItems", "FurnitureJobStatusId", "Manufacturing. FurnitureJobStatuses");
            DropForeignKey("Manufacturing. FurnitureJobStatuses", "FurnitureJobId", "Manufacturing. FurnitureJob");
            DropForeignKey("Manufacturing.FurnitureJobMaterialReturns", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureJobMaterialCosts", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureJobLaborCosts", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureJobChangeOfOrders", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureIdleTimeProductives", "FurnitureIdleTime_FurnitureIdleTimeID", "Manufacturing.FurnitureIdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeOTs", "FurnitureIdleTime_FurnitureIdleTimeID", "Manufacturing.FurnitureIdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeNonProductiveNonControllables", "FurnitureIdleTime_FurnitureIdleTimeID", "Manufacturing.FurnitureIdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeNonProductives", "FurnitureIdleTime_FurnitureIdleTimeID", "Manufacturing.FurnitureIdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeProductives", "IdleTimeID", "Printing.IdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeNonProductiveNonControllables", "IdleTimeID", "Printing.IdleTimes");
            DropForeignKey("Manufacturing.FurnitureIdleTimeNonProductives", "IdleTimeID", "Printing.IdleTimes");
            DropForeignKey("Printing.MonthlyProductionPlans", "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId", "Manufacturing.FurnitureQuarterlyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureQuarterlyProductionPlans", "FurnitureHalfYearlyProductionPlanId", "Manufacturing.FurnitureHalfYearlyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureYearlyProductionPlans", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Manufacturing.FurnitureHalfYearlyProductionPlans", "FurnitureJobProcuctionPlan_FurnitureJobProcuctionPlanId", "Manufacturing.FurnitureYearlyProductionPlans");
            DropForeignKey("Manufacturing.FurnitureFinishedJobs", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureDeliveredJobs", "FurnitureJobCostId", "Manufacturing.FurnitureJobCosts");
            DropForeignKey("Manufacturing.FurnitureMaterialCosts", "FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations");
            DropForeignKey("Manufacturing.DirectLaborCosts", "FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations");
            DropForeignKey("Manufacturing.DirectLaborCosts", "FurnitureCategoryId", "Manufacturing.FurnitureCategories");
            DropIndex("EXCEED.FurnitureYearlyProductionPlan", new[] { "GlFiscalYearId" });
            DropIndex("Manufacturing.FurnitureProductionJobTypePlans", new[] { "FurnitureJobType_FurnitureJobCategoryId" });
            DropIndex("Manufacturing.FurnitureProductionJobTypePlans", new[] { "FurnitureWeeklyProductionPlanId" });
            DropIndex("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveValidation", new[] { "FurnitureFinishedGoodsStoreReceiveId" });
            DropIndex("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", new[] { "PrintingProductionFinishedGoodsStoreReceive_FurnitureFinishedGoodsStoreReceiveId" });
            DropIndex("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", new[] { "ItemCode" });
            DropIndex("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveDistribution", new[] { "FurnitureFinishedGoodsStoreReceiveId" });
            DropIndex("Manufacturing.FurnitureOverallCosts", new[] { "FurnitureEstimationId" });
            DropIndex("Manufacturing.FurnitureWeeklyProductionPlans", new[] { "FurnitureMonthlyProductionPlanId" });
            DropIndex("Manufacturing.FurnitureMonthlyProductionPlans", new[] { "QuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId" });
            DropIndex("Manufacturing.FurnitureLaborCosts", new[] { "EstimationForm_FurnitureEstimationId" });
            DropIndex("Manufacturing.FurnitureLaborCosts", new[] { "FurnitureStepId" });
            DropIndex("Manufacturing.FurnitureJobStatusLineItems", new[] { "FurnitureJobStatusId" });
            DropIndex("Manufacturing. FurnitureJobStatuses", new[] { "FurnitureJobId" });
            DropIndex("Manufacturing.FurnitureJobMaterialReturns", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureJobMaterialCosts", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureJobLaborCosts", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureJobChangeOfOrders", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureIdleTimeProductives", new[] { "FurnitureIdleTime_FurnitureIdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeProductives", new[] { "IdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeOTs", new[] { "FurnitureIdleTime_FurnitureIdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeNonProductiveNonControllables", new[] { "FurnitureIdleTime_FurnitureIdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeNonProductiveNonControllables", new[] { "IdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeNonProductives", new[] { "FurnitureIdleTime_FurnitureIdleTimeID" });
            DropIndex("Manufacturing.FurnitureIdleTimeNonProductives", new[] { "IdleTimeID" });
            DropIndex("Printing.HalfYearlyProductionPlans", new[] { "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId" });
            DropIndex("Printing.MonthlyProductionPlans", new[] { "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId" });
            DropIndex("Manufacturing.FurnitureQuarterlyProductionPlans", new[] { "FurnitureHalfYearlyProductionPlanId" });
            DropIndex("Manufacturing.FurnitureYearlyProductionPlans", new[] { "GlFiscalYearId" });
            DropIndex("Manufacturing.FurnitureHalfYearlyProductionPlans", new[] { "FurnitureJobProcuctionPlan_FurnitureJobProcuctionPlanId" });
            DropIndex("Manufacturing.FurnitureFinishedJobs", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureDeliveredJobs", new[] { "FurnitureJobCostId" });
            DropIndex("Manufacturing.FurnitureMaterialCosts", new[] { "FurnitureEstimationId" });
            DropIndex("Manufacturing.DirectLaborCosts", new[] { "FurnitureEstimationId" });
            DropIndex("Manufacturing.DirectLaborCosts", new[] { "FurnitureCategoryId" });
            DropPrimaryKey("Printing.IdleTimeNonProductiveNonControllables");
            DropColumn("Printing.IdleTimeNonProductiveNonControllables", "PrintingIdleTimeNonProductiveNonControllableID");
            DropColumn("Printing.MonthlyProductionPlans", "FurnitureQuarterlyProductionPlan_FurnitureQuarterlyProductionPlanId");
            DropColumn("Printing.HalfYearlyProductionPlans", "FurnitureYearlyProductionPlan_FurnitureYearlyProductionPlanId");
            DropTable("Manufacturing. FurnitureSettings");
            DropTable("Manufacturing. FurnitureGraphic");
            DropTable("EXCEED.PrintingCriteriaCategory");
            DropTable("EXCEED.FurnitureYearlyProductionPlan");
            DropTable("Manufacturing.FurnitureYearlyOverHeadRates");
            DropTable("EXCEED.FurnitureUnit");
            DropTable("Manufacturing.FurnitureProforma");
            DropTable("Manufacturing.FurnitureProductionYearlyPlans");
            DropTable("Manufacturing.FurnitureProductionMonthlyPlans");
            DropTable("Manufacturing.FurnitureProductionMonthlyJobTypePlans");
            DropTable("Manufacturing.FurnitureProductionJobTypePlans");
            DropTable("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveValidation");
            DropTable("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveItem");
            DropTable("EXCEED.FurnitureProductionFinishedGoodsStoreReceive");
            DropTable("EXCEED.FurnitureProductionFinishedGoodsStoreReceiveDistribution");
            DropTable("Manufacturing.FurnitureProductionCustomerTypes");
            DropTable("Manufacturing.FurnitureProductDeliveryItems");
            DropTable("Manufacturing.FurnitureProductDelivery");
            DropTable("Manufacturing.FurniturePFProformaItems");
            DropTable("Manufacturing.FurnitureOverallCosts");
            DropTable("Manufacturing.FurnitureWeeklyProductionPlans");
            DropTable("Manufacturing.FurnitureMonthlyProductionPlans");
            DropTable("Manufacturing.FurnitureMachines");
            DropTable("Manufacturing.FurnitureSteps");
            DropTable("Manufacturing.FurnitureLaborCosts");
            DropTable("Manufacturing.FurnitureJobStatusLineItems");
            DropTable("Manufacturing. FurnitureJobStatuses");
            DropTable("Manufacturing. FurnitureJob");
            DropTable("Manufacturing.FurnitureJobOrderTimeSheet");
            DropTable("Manufacturing.FurnitureJobOrderTimeRegistration");
            DropTable("Manufacturing.FurnitureJobOrderSummary");
            DropTable("EXCEED.FurnitureJobOrderSummaryCostCenter");
            DropTable("Manufacturing.FurnitureJobOrderSummaryCostCenterDetail");
            DropTable("Manufacturing.FurnitureJobOrderMaterial");
            DropTable("Manufacturing.FurnitureJobMaterialReturns");
            DropTable("Manufacturing.FurnitureJobMaterialCosts");
            DropTable("Manufacturing.FurnitureJobLaborCosts");
            DropTable("Manufacturing.FurnitureJobChangeOfOrders");
            DropTable("Manufacturing.FurnitureJobCategories");
            DropTable("Manufacturing.FurnitureJcPensions");
            DropTable("Manufacturing.FurnitureIdleTimes");
            DropTable("Manufacturing.FurnitureIdleTimeProductives");
            DropTable("Manufacturing.FurnitureIdleTimeOTs");
            DropTable("Manufacturing.FurnitureIdleTimeNonProductiveNonControllables");
            DropTable("Manufacturing.FurnitureIdleTimeNonProductives");
            DropTable("Manufacturing.FurnitureIdleTimeCategories");
            DropTable("Manufacturing.FurnitureQuarterlyProductionPlans");
            DropTable("Manufacturing.FurnitureYearlyProductionPlans");
            DropTable("Manufacturing.FurnitureHalfYearlyProductionPlans");
            DropTable("Manufacturing.FurnitureFinishedJobs");
            DropTable("Manufacturing.FurnitureFinishedGoodItems");
            DropTable("Manufacturing.FurnitureDLRSalaries");
            DropTable("Manufacturing.FurnitureDLRHours");
            DropTable("Manufacturing.FurnitureJobCosts");
            DropTable("Manufacturing.FurnitureDeliveredJobs");
            DropTable("Manufacturing.FurnitureDailyLaborRates");
            DropTable("EXCEED.FurnitureChangeOrder");
            DropTable("Manufacturing.FurnitureMaterialCosts");
            DropTable("Manufacturing.FurnitureCostEstimations");
            DropTable("Manufacturing.FurnitureCategories");
            DropTable("Manufacturing.DirectLaborCosts");
            AddPrimaryKey("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeNonProductiveNonControllableID");
        }
    }
}
