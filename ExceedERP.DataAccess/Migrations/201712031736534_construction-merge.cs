namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constructionmerge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ProjectPaymenAdvance",
                c => new
                    {
                        ProjectPaymenAdvanceId = c.Int(nullable: false, identity: true),
                        AdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenAdvanceId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "Construction.ProjectInformation",
                c => new
                    {
                        ProjectInformationId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Code = c.String(maxLength: 50),
                        Category = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectScope = c.String(),
                        EmployeeId = c.Int(),
                        Client = c.String(maxLength: 200),
                        Consultant = c.String(maxLength: 200),
                        ProjectTypeId = c.Int(nullable: false),
                        ProjectStatus = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectCalander",
                c => new
                    {
                        ProjectCalanderId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.DateTime(nullable: false),
                        ToHour = c.DateTime(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Selected = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCalanderId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectConstraintsAndRisk",
                c => new
                    {
                        ProjectConstraintsAndRiskId = c.Int(nullable: false, identity: true),
                        Assumptions = c.String(),
                        Constraints = c.String(),
                        RiskDescription = c.String(),
                        RiskStatus = c.Int(nullable: false),
                        RiskImpact = c.Int(nullable: false),
                        RiskProbablity = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectConstraintsAndRiskId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectDefineBudget",
                c => new
                    {
                        ProjectDefineBudgetId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        BudgetCode = c.String(nullable: false, maxLength: 100),
                        TotalBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 200),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectDefineBudgetId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectBudgetRequest",
                c => new
                    {
                        ProjectBudgetRequestId = c.Int(nullable: false, identity: true),
                        ProjectDefineBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false, maxLength: 200),
                        Requested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectBudgetRequestId)
                .ForeignKey("Construction.ProjectDefineBudget", t => t.ProjectDefineBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectDefineBudgetId);
            
            CreateTable(
                "Construction.ProjectBudgetTransfer",
                c => new
                    {
                        ProjectBudgetTransferId = c.Int(nullable: false, identity: true),
                        ProjectDefineBudgetId = c.Int(nullable: false),
                        PreviousTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectBudgetTransferId)
                .ForeignKey("Construction.ProjectDefineBudget", t => t.ProjectDefineBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectDefineBudgetId);
            
            CreateTable(
                "Construction.ProjectBudgetUtilization",
                c => new
                    {
                        ProjectBudgetUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectDefineBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false, maxLength: 200),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Utilized = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectBudgetUtilizationId)
                .ForeignKey("Construction.ProjectDefineBudget", t => t.ProjectDefineBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectDefineBudgetId);
            
            CreateTable(
                "Construction.ProjectEquipmentUtilization",
                c => new
                    {
                        ProjectEquipmentUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentUtilizationId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.EquipmentFuelconsumption",
                c => new
                    {
                        ProjectEquipmentFuelconsumptionId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentUtilizationId = c.Int(nullable: false),
                        PreviousDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPreviousConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCurrentConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalToDateConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentFuelconsumptionId)
                .ForeignKey("Construction.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilizationId, cascadeDelete: true)
                .Index(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "Construction.ProjectEquipmentHour",
                c => new
                    {
                        ProjectEquipmentHourId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentUtilizationId = c.Int(nullable: false),
                        PlannedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverallEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(maxLength: 200),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentHourId)
                .ForeignKey("Construction.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilizationId, cascadeDelete: true)
                .Index(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "Construction.ProjectEquipmentMobilization",
                c => new
                    {
                        ProjectEquipmentMobilizationId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentUtilizationId = c.Int(nullable: false),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentMobilizationId)
                .ForeignKey("Construction.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilizationId, cascadeDelete: true)
                .Index(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "Construction.ProjectEquipmentPerformance",
                c => new
                    {
                        ProjectEquipmentPerformanceId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentUtilizationId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PreviousPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDatePerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentPerformanceId)
                .ForeignKey("Construction.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilizationId, cascadeDelete: true)
                .Index(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "Construction.ProjectEstimnBoqBreakdown",
                c => new
                    {
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        BillNumber = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ConfigurationId = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        Category = c.String(),
                        Type = c.String(),
                        Index = c.Int(nullable: false),
                        BlockId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqBreakdownId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.UnitOfMeasurement",
                c => new
                    {
                        UnitOfMeasurementId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Abbreviation = c.String(),
                        TakeOffSheetType = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.ProjectHumanResourceUtilization",
                c => new
                    {
                        ProjectHumanResourceUtilizationId = c.Int(nullable: false, identity: true),
                        CrewCode = c.String(nullable: false, maxLength: 100),
                        CrewName = c.String(nullable: false, maxLength: 200),
                        EmployeeId = c.Int(nullable: false),
                        CombinationOfCrew = c.String(nullable: false, maxLength: 200),
                        TypeOfWork = c.String(nullable: false, maxLength: 200),
                        CrewOutput = c.String(nullable: false, maxLength: 200),
                        CrewDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CrewIndirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectHumanResourceUtilizationId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectMaterialDelivery",
                c => new
                    {
                        ProjectMaterialDeliveryId = c.Int(nullable: false, identity: true),
                        DeliveryDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 100),
                        DeliveredQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(maxLength: 200),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectMaterialDeliveryId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectMaterialIssued",
                c => new
                    {
                        ProjectMaterialIssuedId = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 100),
                        IssuedQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(maxLength: 200),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectMaterialIssuedId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectMaterialRequest",
                c => new
                    {
                        ProjectMaterialRequestId = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 100),
                        RequestedQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(maxLength: 200),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectMaterialRequestId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectWorkBreakDown",
                c => new
                    {
                        ProjectWorkBreakDownId = c.Int(nullable: false, identity: true),
                        WBSCode = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        ProjectInformationId = c.Int(nullable: false),
                        OriginalDuration = c.Int(nullable: false),
                        RemainingDuration = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectWorkBreakDownId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Task.Task",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WorkItemDurationId = c.Int(),
                        ParentID = c.Int(),
                        OrderID = c.Int(nullable: false),
                        ProjectWorkBreakDownId = c.Int(nullable: false),
                        Title = c.String(),
                        TaskCode = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        PercentComplete = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expanded = c.Boolean(nullable: false),
                        Summary = c.Boolean(nullable: false),
                        Task1_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("Construction.ProjectWorkBreakDown", t => t.ProjectWorkBreakDownId, cascadeDelete: true)
                .ForeignKey("Task.Task", t => t.Task1_ID)
                .ForeignKey("Construction.WorkItemDuration", t => t.WorkItemDurationId)
                .Index(t => t.WorkItemDurationId)
                .Index(t => t.ProjectWorkBreakDownId)
                .Index(t => t.Task1_ID);
            
            CreateTable(
                "Construction.WorkItemDuration",
                c => new
                    {
                        WorkItemDurationId = c.Int(nullable: false, identity: true),
                        WorkItemId = c.Int(nullable: false),
                        WorkDescription = c.String(),
                        TakeOffPerProjectId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ProductivityOfCrewPerDay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DurationPerCrew = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NumberOfCrew = c.Int(nullable: false),
                        EstimatedDuration = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.WorkItemDurationId)
                .ForeignKey("Construction.TakeOffPerProject", t => t.TakeOffPerProjectId, cascadeDelete: true)
                .Index(t => t.TakeOffPerProjectId);
            
            CreateTable(
                "Construction.TakeOffPerProject",
                c => new
                    {
                        TakeOffPerProjectId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffPerProjectId);
            
            CreateTable(
                "Construction.WorkItemPerProject",
                c => new
                    {
                        WorkItemPerProjectId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemName = c.String(maxLength: 200),
                        TotalQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTypeId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                        BlockId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkItemPerProjectId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.ProjectDirectLabour",
                c => new
                    {
                        ProjectDirectLabourId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectDirectLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.ProjectEstmnIndexLabour",
                c => new
                    {
                        ProjectEstimationIndexLabourId = c.Int(nullable: false, identity: true),
                        ProjectJobTitleId = c.Int(nullable: false),
                        WorkingDays = c.Int(),
                        ApprovedIndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlyAverageSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryperdayMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryperdayMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryperMonthMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryperMonthMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BirrMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdealBirrDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualBirrDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdealBirrDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualBirrDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DesertBirrDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetBirrDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetBirrHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FormHr = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndexLabourId)
                .ForeignKey("Construction.ProjectJobTitle", t => t.ProjectJobTitleId, cascadeDelete: true)
                .Index(t => t.ProjectJobTitleId);
            
            CreateTable(
                "Construction.ProjectJobTitle",
                c => new
                    {
                        ProjectJobTitleId = c.Int(nullable: false, identity: true),
                        RankId = c.Int(),
                        StepId = c.Int(),
                        LookupId = c.Int(nullable: false),
                        JobTitle = c.String(),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectJobTitleId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId)
                .ForeignKey("HRM.Step", t => t.StepId)
                .Index(t => t.RankId)
                .Index(t => t.StepId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEquipment",
                c => new
                    {
                        ProjectEquipmentId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        AssetNumber = c.Guid(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentId)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId);
            
            CreateTable(
                "Construction.ProjectMaterial",
                c => new
                    {
                        ProjectMaterialId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        ItemCategoryCode = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.WorkCategory",
                c => new
                    {
                        WorkCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        ConfigurationId = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkCategoryId)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .Index(t => t.ConfigurationId);
            
            CreateTable(
                "Construction.AreaWorkItem",
                c => new
                    {
                        AreaWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Area = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CylinderLock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        BlockId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AreaWorkItemId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.BarDiameter",
                c => new
                    {
                        BarDiameterId = c.Int(nullable: false, identity: true),
                        ConcatnatedCode = c.String(),
                        Diameter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.BarDiameterId);
            
            CreateTable(
                "Construction.BlockMapping",
                c => new
                    {
                        BlockMappingId = c.Int(nullable: false, identity: true),
                        BlockId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BlockMappingId)
                .ForeignKey("Construction.Block", t => t.BlockId, cascadeDelete: true)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .Index(t => t.BlockId)
                .Index(t => t.ConfigurationId);
            
            CreateTable(
                "Construction.Block",
                c => new
                    {
                        BlockId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProjectTypeId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BlockId);
            
            CreateTable(
                "Construction.CertifiedPayment",
                c => new
                    {
                        CertifiedPaymentId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CertifiedPaymentId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.ProjectContract",
                c => new
                    {
                        ProjectContractId = c.Int(nullable: false, identity: true),
                        ContractSigningDate = c.DateTime(nullable: false),
                        CommencementDate = c.DateTime(nullable: false),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractTime = c.DateTime(nullable: false),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectContractId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ContractClaim",
                c => new
                    {
                        ContractClaimId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        ContractClaimTypeId = c.Int(nullable: false),
                        RefNumber = c.String(),
                        IssuedDate = c.DateTime(nullable: false),
                        IssuedBy = c.String(),
                        ClaimStatus = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ContractClaimId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .ForeignKey("Construction.ContractClaimType", t => t.ContractClaimTypeId, cascadeDelete: true)
                .Index(t => t.ProjectContractId)
                .Index(t => t.ContractClaimTypeId);
            
            CreateTable(
                "Construction.ContractClaimType",
                c => new
                    {
                        ContractClaimTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ContractClaimTypeId);
            
            CreateTable(
                "Construction.ExtensionOfTime",
                c => new
                    {
                        ExtensionOfTimeId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        Reason = c.String(),
                        Justification = c.String(),
                        RequestedDays = c.Int(nullable: false),
                        ApprovedDays = c.Int(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.ExtensionOfTimeId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.ExecutedWork",
                c => new
                    {
                        ExecutedWorkId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        Title = c.String(),
                        PlanComplitionDate = c.DateTime(nullable: false),
                        ForecastCompletionDateLastMonth = c.DateTime(nullable: false),
                        CurrentForecastCompletionDate = c.DateTime(nullable: false),
                        ActualCompletionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ExecutedWorkId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.ContractRevision",
                c => new
                    {
                        ContractRevisionId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        RevisionDate = c.DateTime(nullable: false),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ContractRevisionId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.SupplementalAgreement",
                c => new
                    {
                        SupplementalAgreementId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        SupplementAgreementNumber = c.String(),
                    })
                .PrimaryKey(t => t.SupplementalAgreementId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.ContractSuspensionOrder",
                c => new
                    {
                        ContractSuspensionOrderId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        SuspensionOrderNumber = c.String(),
                        ReasonOfSuspension = c.String(),
                        AffectedWork = c.String(),
                        SuspensionAmount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContractSuspensionOrderId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.ContractVariationOrder",
                c => new
                    {
                        ContractVariationOrderId = c.Int(nullable: false, identity: true),
                        VariationOrderTypeId = c.Int(nullable: false),
                        ProjectContractId = c.Int(nullable: false),
                        VariationOrderNumber = c.String(),
                        OrderStatus = c.String(),
                        Priority = c.String(),
                        DateIssued = c.DateTime(nullable: false),
                        IssuedBy = c.String(),
                        ChangeCost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ContractVariationOrderId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .ForeignKey("Construction.VariationOrderType", t => t.VariationOrderTypeId, cascadeDelete: true)
                .Index(t => t.VariationOrderTypeId)
                .Index(t => t.ProjectContractId);
            
            CreateTable(
                "Construction.VariationOrderType",
                c => new
                    {
                        VariationOrderTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.VariationOrderTypeId);
            
            CreateTable(
                "Construction.ConstructionItemCategory",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        Name = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode);
            
            CreateTable(
                "Construction.CrewEquipment",
                c => new
                    {
                        CrewEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectCrewId = c.Int(nullable: false),
                        Name = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CrewEquipmentId)
                .ForeignKey("Construction.ProjectCrew", t => t.ProjectCrewId, cascadeDelete: true)
                .Index(t => t.ProjectCrewId);
            
            CreateTable(
                "Construction.ProjectCrew",
                c => new
                    {
                        ProjectCrewId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        CrewCode = c.String(),
                        CrewLeader = c.String(),
                        AmountofWorkDone = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCrewCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectCrewId)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.CrewHumanResource",
                c => new
                    {
                        CrewHumanResourceId = c.Int(nullable: false, identity: true),
                        ProjectCrewId = c.Int(nullable: false),
                        Name = c.String(),
                        JobTitle = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CrewHumanResourceId)
                .ForeignKey("Construction.ProjectCrew", t => t.ProjectCrewId, cascadeDelete: true)
                .Index(t => t.ProjectCrewId);
            
            CreateTable(
                "Construction.CrewMaterial",
                c => new
                    {
                        CrewMaterialId = c.Int(nullable: false, identity: true),
                        ProjectCrewId = c.Int(nullable: false),
                        Name = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CrewMaterialId)
                .ForeignKey("Construction.ProjectCrew", t => t.ProjectCrewId, cascadeDelete: true)
                .Index(t => t.ProjectCrewId);
            
            CreateTable(
                "Construction.DeformedBar",
                c => new
                    {
                        DeformedBarId = c.Int(nullable: false, identity: true),
                        ConcatnatedCode = c.String(),
                        Name = c.String(),
                        Diameter = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.DeformedBarId);
            
            CreateTable(
                "Task.Dependency",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PredecessorID = c.Int(nullable: false),
                        SuccessorID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "Construction.EarnedValueManagementTermDetail",
                c => new
                    {
                        EarnedValueManagementTermDetailId = c.Int(nullable: false, identity: true),
                        EarnedValueManagementTermId = c.Int(nullable: false),
                        OperandOne = c.String(),
                        OperandTwo = c.String(),
                        Operator = c.Int(nullable: false),
                        HiddenFormulaValue = c.String(),
                        ConstantValue = c.Decimal(precision: 18, scale: 2),
                        Formula = c.String(),
                        OperandOneHiddenValue = c.String(),
                        OperandTwoHiddenValue = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValueManagementTermDetailId)
                .ForeignKey("Construction.EarnedValueManagementTerms", t => t.EarnedValueManagementTermId, cascadeDelete: true)
                .Index(t => t.EarnedValueManagementTermId);
            
            CreateTable(
                "Construction.EarnedValueManagementTerms",
                c => new
                    {
                        EarnedValueManagementTermId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Term = c.String(),
                        Abbreviation = c.String(),
                        Definition = c.String(),
                        Formula = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValueManagementTermId);
            
            CreateTable(
                "Construction.EarnedValuePerProject",
                c => new
                    {
                        EarnedValuePerProjectId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        Year = c.String(),
                        Month = c.Int(nullable: false),
                        BAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BCWS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BCWP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ACWP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PCP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PCA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CV = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CVPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SV = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SVPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TCPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TSPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ETC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ISAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValuePerProjectId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.EquipmentMobilization",
                c => new
                    {
                        EquipmentMobilizationId = c.Int(nullable: false, identity: true),
                        ItemNumber = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Trip = c.Double(nullable: false),
                        Distance = c.Double(nullable: false),
                        Speed = c.Double(nullable: false),
                        Houre = c.Double(nullable: false),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EquipmentMobilizationId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.SubContractWorkItemEstmn",
                c => new
                    {
                        SubContractWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SelectedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTypeId = c.Int(nullable: false),
                        BlockId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubContractWorkItemId)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.EVMTermOperandComponent",
                c => new
                    {
                        EVMTermOperandComponentId = c.Int(nullable: false, identity: true),
                        EarnedValueManagementTermDetailId = c.Int(nullable: false),
                        OperandOne = c.String(),
                        OperandTwo = c.String(),
                        Operator = c.Int(nullable: false),
                        HiddenFormulaValue = c.String(),
                        ConstantValue = c.Decimal(precision: 18, scale: 2),
                        ParentOperand = c.Int(nullable: false),
                        Formula = c.String(),
                    })
                .PrimaryKey(t => t.EVMTermOperandComponentId)
                .ForeignKey("Construction.EarnedValueManagementTermDetail", t => t.EarnedValueManagementTermDetailId, cascadeDelete: true)
                .Index(t => t.EarnedValueManagementTermDetailId);
            
            CreateTable(
                "Construction.InternalMachinaryRent",
                c => new
                    {
                        InternalMachinaryRentId = c.Int(nullable: false, identity: true),
                        FleetsID = c.Guid(nullable: false),
                        Model = c.String(),
                        Capacity = c.Int(),
                        PurchaseCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumLife = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HoursUsedPerYear = c.Int(nullable: false),
                        DepCostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvestmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOwningCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintAndRepair = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TyreCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOperatingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlyRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexForSeverCondition = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RatePerHourWithoutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalMachinaryRentId);
            
            CreateTable(
                "Construction.LaborRateIndexFactorAttributeName",
                c => new
                    {
                        LaborRateIndexFactorAttributeNameId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.LaborRateIndexFactorAttributeNameId);
            
            CreateTable(
                "Construction.LaborRateIndexFactorAttribute",
                c => new
                    {
                        LaborRateIndexFactorAttributeId = c.Int(nullable: false, identity: true),
                        LaborRateIndexFactorAttributeNameId = c.Int(nullable: false),
                        CalculationDescription = c.String(),
                        NumberOfDays = c.Int(),
                        Index = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LaborRateIndexFactorAttributeId)
                .ForeignKey("Construction.LaborRateIndexFactorAttributeName", t => t.LaborRateIndexFactorAttributeNameId, cascadeDelete: true)
                .Index(t => t.LaborRateIndexFactorAttributeNameId);
            
            CreateTable(
                "Construction.PaymenCertificatetLine",
                c => new
                    {
                        ProjectPaymenCertificatetLineId = c.Int(nullable: false, identity: true),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryContract = c.String(maxLength: 100),
                        VariationOrder = c.String(maxLength: 200),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenCertificatetLineId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "Construction.LineDate",
                c => new
                    {
                        LineDateId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        AwardDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractSigningDate = c.DateTime(nullable: false),
                        SiteHandOverDate = c.DateTime(nullable: false),
                        EstimatedStartDate = c.DateTime(nullable: false),
                        EstimatedEndDate = c.DateTime(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LineDateId)
                .ForeignKey("Common.Period", t => t.PeriodId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Construction.MachinaryRent",
                c => new
                    {
                        MachinaryRentId = c.Int(nullable: false, identity: true),
                        RentOtherSuppliersID = c.Guid(nullable: false),
                        FuelType = c.Int(nullable: false),
                        Model = c.String(maxLength: 200),
                        RatedCapaciy = c.String(),
                        HP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YearManufacture = c.String(maxLength: 50),
                        Rental = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BirrPerLitter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCostPerHr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalWithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MachinaryRentId)
                .ForeignKey("FleetManagement.RentOtherSuppliers", t => t.RentOtherSuppliersID, cascadeDelete: true)
                .Index(t => t.RentOtherSuppliersID);
            
            CreateTable(
                "Construction.MapWorkItemCategory",
                c => new
                    {
                        MapWorkCategoryId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MapWorkCategoryId);
            
            CreateTable(
                "Construction.MaterialPriceIndex",
                c => new
                    {
                        MaterialPriceIndexId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Uom = c.String(),
                        SourceType = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrginalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrginalCostGRN = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExternalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoadingUnloading = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaterialPriceIndexId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.BranchId);
            
            CreateTable(
                "Construction.MaterialUtilization",
                c => new
                    {
                        MaterialUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectMaterialId = c.Int(nullable: false),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialIssuedToSite = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaterialUtilizationId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.MethodOfWork",
                c => new
                    {
                        MethodOfWorkId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Abbreviation = c.String(),
                    })
                .PrimaryKey(t => t.MethodOfWorkId);
            
            CreateTable(
                "Construction.MonthlyFuelCost",
                c => new
                    {
                        MonthlyFuelCostId = c.Int(nullable: false, identity: true),
                        FuelType = c.Int(nullable: false),
                        Year = c.String(nullable: false),
                        Month = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.MonthlyFuelCostId);
            
            CreateTable(
                "Construction.PaymentCertificate",
                c => new
                    {
                        ProjectPaymentCertificateId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymentCertificateId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.Retention",
                c => new
                    {
                        ProjectPaymenRetentionId = c.Int(nullable: false, identity: true),
                        RetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenRetentionId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "Construction.EstimationAreaWorkItem",
                c => new
                    {
                        AreaWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Area = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CylinderLock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTypeId = c.Int(nullable: false),
                        BlockId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AreaWorkItemId)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.ProjectEstimationBoq",
                c => new
                    {
                        ProjectEstimationBoqid = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        TypeOfBoq = c.Int(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ProjectEstimationName_ProjectEstimationNameId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqid)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationName_ProjectEstimationNameId)
                .Index(t => t.LookupId)
                .Index(t => t.ProjectEstimationName_ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectEstimationBoqDetail",
                c => new
                    {
                        ProjectEstmnBoqDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqDetailId)
                .ForeignKey("Construction.ProjectEstimnBoqBreakdown", t => t.ProjectEstimationBoqBreakdownId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationBoqBreakdownId);
            
            CreateTable(
                "Construction.ProjectEstmnCoefficients",
                c => new
                    {
                        ProjectEstimationCoefficientsId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Kg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shrinkage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Wastage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationCoefficientsId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnDirectLabour",
                c => new
                    {
                        ProjectEstimationDirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationDirectLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.ProjectEstimnWorkItem",
                c => new
                    {
                        ProjectEstimationWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        TotalQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTypeId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                        BlockId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkItemId)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.ProjectEstmnEquipment",
                c => new
                    {
                        ProjectEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        AssetNumber = c.Guid(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationEquipmentId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.ProjectEstmnIndirectLabour",
                c => new
                    {
                        ProjectEstimationIndirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        LabourName = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndirectLabourId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnMaterial",
                c => new
                    {
                        ProjectEstimationMaterialId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 100),
                        ItemCode = c.String(nullable: false, maxLength: 100),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationMaterialId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.ProjectEstmnMobilization",
                c => new
                    {
                        ProjectEstimationMobilizationId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        TypeOfMachinery = c.String(nullable: false, maxLength: 100),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Trip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Speed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffName = c.String(nullable: false, maxLength: 200),
                        StaffQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdiem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationMobilizationId);
            
            CreateTable(
                "Construction.ProjectEstimationName",
                c => new
                    {
                        ProjectEstimationNameId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        BranchId = c.Int(nullable: false),
                        ClientName = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationNameId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "Construction.ProjectEstmnRawData",
                c => new
                    {
                        ProjectEstimationRawDataId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Category = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationRawDataId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnTransportCost",
                c => new
                    {
                        ProjectEstimationTransportCostId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LookupId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationTransportCostId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnUnitFactory",
                c => new
                    {
                        ProjectEstimationUnitFactoryId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        FactoryName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitFactoryId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnWorkCategory",
                c => new
                    {
                        ProjectEstimationWorkCategoryId = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 200),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkCategoryId);
            
            CreateTable(
                "Construction.ProjectEstmnBoqSummary",
                c => new
                    {
                        ProjectEstmnBoqSummaryId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Category = c.String(),
                        Unit = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        BlockId = c.Int(nullable: false),
                        Block = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqSummaryId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectMachineryRentHeader",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectEstimationNameId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectTakeOff",
                c => new
                    {
                        ProjectTakeOffId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffId);
            
            CreateTable(
                "Construction.ProjectTakeOffLine",
                c => new
                    {
                        ProjectTakeOffLineId = c.Int(nullable: false, identity: true),
                        T = c.Decimal(nullable: false, precision: 18, scale: 4),
                        D = c.Decimal(nullable: false, precision: 18, scale: 4),
                        S = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Description = c.String(maxLength: 400),
                        ProjectTakeOffId = c.Int(nullable: false),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineId)
                .ForeignKey("Construction.ProjectTakeOff", t => t.ProjectTakeOffId, cascadeDelete: true)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .Index(t => t.ProjectTakeOffId)
                .Index(t => t.UnitOfMeasurementId);
            
            CreateTable(
                "Construction.ProjectTakeOffLineForKg",
                c => new
                    {
                        ProjectTakeOffLineForKgId = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 200),
                        Type = c.String(),
                        Shape = c.String(),
                        UnitRateId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfBars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfElement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBar = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLength = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTakeOffId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineForKgId)
                .ForeignKey("Construction.ProjectTakeOff", t => t.ProjectTakeOffId, cascadeDelete: true)
                .ForeignKey("Construction.UnitRate", t => t.UnitRateId, cascadeDelete: true)
                .Index(t => t.UnitRateId)
                .Index(t => t.ProjectTakeOffId);
            
            CreateTable(
                "Construction.UnitRate",
                c => new
                    {
                        UnitRateId = c.Int(nullable: false, identity: true),
                        Diameter = c.String(),
                        UnitWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.UnitRateId);
            
            CreateTable(
                "EXCEED.ProjectTaskSchedule",
                c => new
                    {
                        ProjectTaskScheduleId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(maxLength: 100),
                        Duration = c.String(maxLength: 50),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CompletePercentage = c.String(maxLength: 50),
                        ProjectInformationId = c.Int(nullable: false),
                        RecurrenceId = c.Int(nullable: false),
                        StartTimezone = c.String(),
                        EndTimezone = c.String(),
                        Description = c.String(),
                        IsAllDay = c.Boolean(nullable: false),
                        RecurrenceException = c.String(),
                        RecurrenceRule = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTaskScheduleId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectTypeMapping",
                c => new
                    {
                        ProjectTypeMappingId = c.Int(nullable: false, identity: true),
                        ProjectTypeId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTypeMappingId)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectType", t => t.ProjectTypeId, cascadeDelete: true)
                .Index(t => t.ProjectTypeId)
                .Index(t => t.ConfigurationId);
            
            CreateTable(
                "Construction.ProjectType",
                c => new
                    {
                        ProjectTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProjectTypes = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTypeId);
            
            CreateTable(
                "Task.ResourceAssignment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        Units = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "Task.Resource",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color = c.String(),
                        ResourceTypeId = c.Int(nullable: false),
                        UnitofMeasure = c.String(),
                        PrimaryRole = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("Task.ResourceType", t => t.ResourceTypeId, cascadeDelete: true)
                .Index(t => t.ResourceTypeId);
            
            CreateTable(
                "Task.ResourceType",
                c => new
                    {
                        ResourceTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ResourceTypeId);
            
            CreateTable(
                "Construction.ResourceUtilizationDirectLabour",
                c => new
                    {
                        ResourceUtilizationDirectLabourId = c.Int(nullable: false, identity: true),
                        ResourceUtilizationWorkItemId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationDirectLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.ResourceUtilizationWorkItem", t => t.ResourceUtilizationWorkItemId, cascadeDelete: true)
                .Index(t => t.ResourceUtilizationWorkItemId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.ResourceUtilizationWorkItem",
                c => new
                    {
                        ResourceUtilizationWorkItemId = c.Int(nullable: false, identity: true),
                        MethodOfWorkId = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        Category = c.String(),
                        ItemName = c.String(),
                        UnitOfMeasurnment = c.String(),
                        ExecutedWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkDurationInHoursPerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutPutPerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectContractUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Income = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverheadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationWorkItemId)
                .ForeignKey("Construction.MethodOfWork", t => t.MethodOfWorkId, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.MethodOfWorkId)
                .Index(t => t.WorkItemPerProjectId);
            
            CreateTable(
                "Construction.ResourceUtilizationEquipment",
                c => new
                    {
                        ResourceUtilizationEquipmentId = c.Int(nullable: false, identity: true),
                        ResourceUtilizationWorkItemId = c.Int(nullable: false),
                        AssetNumber = c.Guid(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationEquipmentId)
                .ForeignKey("Construction.ResourceUtilizationWorkItem", t => t.ResourceUtilizationWorkItemId, cascadeDelete: true)
                .Index(t => t.ResourceUtilizationWorkItemId);
            
            CreateTable(
                "Construction.ResourceUtilizationMaterial",
                c => new
                    {
                        ResourceUtilizationMaterialId = c.Int(nullable: false, identity: true),
                        ResourceUtilizationWorkItemId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        ItemCategoryCode = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.ResourceUtilizationWorkItem", t => t.ResourceUtilizationWorkItemId, cascadeDelete: true)
                .Index(t => t.ResourceUtilizationWorkItemId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.RoadMapping",
                c => new
                    {
                        RoadMappingId = c.Int(nullable: false, identity: true),
                        RoadId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RoadMappingId)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .ForeignKey("Construction.Road", t => t.RoadId, cascadeDelete: true)
                .Index(t => t.RoadId)
                .Index(t => t.ConfigurationId);
            
            CreateTable(
                "Construction.Road",
                c => new
                    {
                        RoadId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RoadId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.Calander",
                c => new
                    {
                        SettingCalendarId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.DateTime(nullable: false),
                        ToHour = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SettingCalendarId);
            
            CreateTable(
                "Construction.SubContractWorkItem",
                c => new
                    {
                        SubContractWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SelectedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        BlockId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubContractWorkItemId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.UnitOfMeasurementId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.TakeOffLineForKgPerProject",
                c => new
                    {
                        TakeOffLineForKgPerProjectId = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                        Shape = c.String(),
                        DeformedBarId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Diameter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NoOfBars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfElement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBar = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalLength = c.Decimal(nullable: false, precision: 18, scale: 4),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Description = c.String(maxLength: 200),
                        Type = c.String(),
                        TakeOffPerProjectId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffLineForKgPerProjectId)
                .ForeignKey("Construction.DeformedBar", t => t.DeformedBarId, cascadeDelete: true)
                .ForeignKey("Construction.TakeOffPerProject", t => t.TakeOffPerProjectId, cascadeDelete: true)
                .Index(t => t.DeformedBarId)
                .Index(t => t.TakeOffPerProjectId);
            
            CreateTable(
                "Construction.TakeOffLinePerProject",
                c => new
                    {
                        TakeOffLinePerProjectId = c.Int(nullable: false, identity: true),
                        No = c.Decimal(nullable: false, precision: 18, scale: 4),
                        D = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Description = c.String(maxLength: 400),
                        TakeOffPerProjectId = c.Int(nullable: false),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Depth = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffLinePerProjectId)
                .ForeignKey("Construction.TakeOffPerProject", t => t.TakeOffPerProjectId, cascadeDelete: true)
                .Index(t => t.TakeOffPerProjectId);
            
            CreateTable(
                "Construction.ProjectSubContract",
                c => new
                    {
                        ProjectContractId = c.Int(nullable: false),
                        WorkType = c.String(maxLength: 100),
                        SubContractor = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ProjectContractId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId)
                .Index(t => t.ProjectContractId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ProjectSubContract", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.TakeOffLinePerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropForeignKey("Construction.TakeOffLineForKgPerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropForeignKey("Construction.TakeOffLineForKgPerProject", "DeformedBarId", "Construction.DeformedBar");
            DropForeignKey("Construction.SubContractWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.SubContractWorkItem", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.SubContractWorkItem", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.RoadMapping", "RoadId", "Construction.Road");
            DropForeignKey("Construction.Road", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.RoadMapping", "ConfigurationId", "Common.Configuration");
            DropForeignKey("Construction.ResourceUtilizationWorkItem", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ResourceUtilizationWorkItem", "MethodOfWorkId", "Construction.MethodOfWork");
            DropForeignKey("Construction.ResourceUtilizationMaterial", "ResourceUtilizationWorkItemId", "Construction.ResourceUtilizationWorkItem");
            DropForeignKey("Construction.ResourceUtilizationMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ResourceUtilizationDirectLabour", "ResourceUtilizationWorkItemId", "Construction.ResourceUtilizationWorkItem");
            DropForeignKey("Construction.ResourceUtilizationEquipment", "ResourceUtilizationWorkItemId", "Construction.ResourceUtilizationWorkItem");
            DropForeignKey("Construction.ResourceUtilizationDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Task.Resource", "ResourceTypeId", "Task.ResourceType");
            DropForeignKey("Construction.ProjectTypeMapping", "ProjectTypeId", "Construction.ProjectType");
            DropForeignKey("Construction.ProjectTypeMapping", "ConfigurationId", "Common.Configuration");
            DropForeignKey("EXCEED.ProjectTaskSchedule", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectTakeOffLine", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectMachineryRentHeader", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstmnUnitFactory", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnTransportCost", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnRawData", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstimationName", "BranchId", "HRM.Branch");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.ProjectEstmnMaterial", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnEquipment", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectEstmnCoefficients", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstimationBoqDetail", "ProjectEstimationBoqBreakdownId", "Construction.ProjectEstimnBoqBreakdown");
            DropForeignKey("Construction.ProjectEstimationBoq", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.EstimationAreaWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.Retention", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.MaterialUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.MaterialPriceIndex", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.MaterialPriceIndex", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.MaterialPriceIndex", "BranchId", "HRM.Branch");
            DropForeignKey("Construction.MachinaryRent", "RentOtherSuppliersID", "FleetManagement.RentOtherSuppliers");
            DropForeignKey("Construction.LineDate", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.LineDate", "PeriodId", "Common.Period");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.LaborRateIndexFactorAttribute", "LaborRateIndexFactorAttributeNameId", "Construction.LaborRateIndexFactorAttributeName");
            DropForeignKey("Construction.EVMTermOperandComponent", "EarnedValueManagementTermDetailId", "Construction.EarnedValueManagementTermDetail");
            DropForeignKey("Construction.SubContractWorkItemEstmn", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.SubContractWorkItemEstmn", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.EquipmentMobilization", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.EarnedValuePerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.EarnedValueManagementTermDetail", "EarnedValueManagementTermId", "Construction.EarnedValueManagementTerms");
            DropForeignKey("Construction.ProjectCrew", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.CrewMaterial", "ProjectCrewId", "Construction.ProjectCrew");
            DropForeignKey("Construction.CrewHumanResource", "ProjectCrewId", "Construction.ProjectCrew");
            DropForeignKey("Construction.CrewEquipment", "ProjectCrewId", "Construction.ProjectCrew");
            DropForeignKey("Construction.ConstructionItemCategory", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.ContractVariationOrder", "VariationOrderTypeId", "Construction.VariationOrderType");
            DropForeignKey("Construction.ContractVariationOrder", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.ContractSuspensionOrder", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.SupplementalAgreement", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.ContractRevision", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.ProjectContract", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ExecutedWork", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.ExtensionOfTime", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.ContractClaim", "ContractClaimTypeId", "Construction.ContractClaimType");
            DropForeignKey("Construction.ContractClaim", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.CertifiedPayment", "ProjectContractId", "Construction.ProjectContract");
            DropForeignKey("Construction.BlockMapping", "ConfigurationId", "Common.Configuration");
            DropForeignKey("Construction.BlockMapping", "BlockId", "Construction.Block");
            DropForeignKey("Construction.AreaWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.AreaWorkItem", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.WorkItemPerProject", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.WorkCategory", "ConfigurationId", "Common.Configuration");
            DropForeignKey("Construction.WorkItemPerProject", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.WorkItemPerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterial", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEquipment", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectDirectLabour", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectEstmnIndexLabour", "ProjectJobTitleId", "Construction.ProjectJobTitle");
            DropForeignKey("Construction.ProjectJobTitle", "StepId", "HRM.Step");
            DropForeignKey("Construction.ProjectJobTitle", "RankId", "HRM.Rank");
            DropForeignKey("Construction.ProjectJobTitle", "LookupId", "Common.Lookup");
            DropForeignKey("Task.Task", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropForeignKey("Construction.WorkItemDuration", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropForeignKey("Task.Task", "Task1_ID", "Task.Task");
            DropForeignKey("Task.Task", "ProjectWorkBreakDownId", "Construction.ProjectWorkBreakDown");
            DropForeignKey("Construction.ProjectWorkBreakDown", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialRequest", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialIssued", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialDelivery", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "EmployeeId", "HRM.Employee");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEquipmentUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEquipmentPerformance", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentMobilization", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentHour", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.EquipmentFuelconsumption", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectDefineBudget", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectConstraintsAndRisk", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectCalander", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ProjectSubContract", new[] { "ProjectContractId" });
            DropIndex("Construction.TakeOffLinePerProject", new[] { "TakeOffPerProjectId" });
            DropIndex("Construction.TakeOffLineForKgPerProject", new[] { "TakeOffPerProjectId" });
            DropIndex("Construction.TakeOffLineForKgPerProject", new[] { "DeformedBarId" });
            DropIndex("Construction.SubContractWorkItem", new[] { "ProjectInformationId" });
            DropIndex("Construction.SubContractWorkItem", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.SubContractWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.Road", new[] { "ProjectInformationId" });
            DropIndex("Construction.RoadMapping", new[] { "ConfigurationId" });
            DropIndex("Construction.RoadMapping", new[] { "RoadId" });
            DropIndex("Construction.ResourceUtilizationMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ResourceUtilizationMaterial", new[] { "ResourceUtilizationWorkItemId" });
            DropIndex("Construction.ResourceUtilizationEquipment", new[] { "ResourceUtilizationWorkItemId" });
            DropIndex("Construction.ResourceUtilizationWorkItem", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ResourceUtilizationWorkItem", new[] { "MethodOfWorkId" });
            DropIndex("Construction.ResourceUtilizationDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ResourceUtilizationDirectLabour", new[] { "ResourceUtilizationWorkItemId" });
            DropIndex("Task.Resource", new[] { "ResourceTypeId" });
            DropIndex("Construction.ProjectTypeMapping", new[] { "ConfigurationId" });
            DropIndex("Construction.ProjectTypeMapping", new[] { "ProjectTypeId" });
            DropIndex("EXCEED.ProjectTaskSchedule", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "ProjectTakeOffId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "UnitRateId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "ProjectTakeOffId" });
            DropIndex("Construction.ProjectMachineryRentHeader", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstmnBoqSummary", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEstmnUnitFactory", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnTransportCost", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnRawData", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimationName", new[] { "BranchId" });
            DropIndex("Construction.ProjectEstmnMaterial", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnCoefficients", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimationBoqDetail", new[] { "ProjectEstimationBoqBreakdownId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectEstimationName_ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "LookupId" });
            DropIndex("Construction.EstimationAreaWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.Retention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.Retention", new[] { "ProjectInformationId" });
            DropIndex("Construction.PaymentCertificate", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.MaterialUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.MaterialPriceIndex", new[] { "BranchId" });
            DropIndex("Construction.MaterialPriceIndex", new[] { "ItemCode" });
            DropIndex("Construction.MaterialPriceIndex", new[] { "ItemCategoryCode" });
            DropIndex("Construction.MachinaryRent", new[] { "RentOtherSuppliersID" });
            DropIndex("Construction.LineDate", new[] { "PeriodId" });
            DropIndex("Construction.LineDate", new[] { "ProjectInformationId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectInformationId" });
            DropIndex("Construction.LaborRateIndexFactorAttribute", new[] { "LaborRateIndexFactorAttributeNameId" });
            DropIndex("Construction.EVMTermOperandComponent", new[] { "EarnedValueManagementTermDetailId" });
            DropIndex("Construction.SubContractWorkItemEstmn", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.SubContractWorkItemEstmn", new[] { "WorkCategoryId" });
            DropIndex("Construction.EquipmentMobilization", new[] { "LookupId" });
            DropIndex("Construction.EarnedValuePerProject", new[] { "ProjectInformationId" });
            DropIndex("Construction.EarnedValueManagementTermDetail", new[] { "EarnedValueManagementTermId" });
            DropIndex("Construction.CrewMaterial", new[] { "ProjectCrewId" });
            DropIndex("Construction.CrewHumanResource", new[] { "ProjectCrewId" });
            DropIndex("Construction.ProjectCrew", new[] { "WorkCategoryId" });
            DropIndex("Construction.CrewEquipment", new[] { "ProjectCrewId" });
            DropIndex("Construction.ConstructionItemCategory", new[] { "ItemCategoryCode" });
            DropIndex("Construction.ContractVariationOrder", new[] { "ProjectContractId" });
            DropIndex("Construction.ContractVariationOrder", new[] { "VariationOrderTypeId" });
            DropIndex("Construction.ContractSuspensionOrder", new[] { "ProjectContractId" });
            DropIndex("Construction.SupplementalAgreement", new[] { "ProjectContractId" });
            DropIndex("Construction.ContractRevision", new[] { "ProjectContractId" });
            DropIndex("Construction.ExecutedWork", new[] { "ProjectContractId" });
            DropIndex("Construction.ExtensionOfTime", new[] { "ProjectContractId" });
            DropIndex("Construction.ContractClaim", new[] { "ContractClaimTypeId" });
            DropIndex("Construction.ContractClaim", new[] { "ProjectContractId" });
            DropIndex("Construction.ProjectContract", new[] { "ProjectInformationId" });
            DropIndex("Construction.CertifiedPayment", new[] { "ProjectContractId" });
            DropIndex("Construction.BlockMapping", new[] { "ConfigurationId" });
            DropIndex("Construction.BlockMapping", new[] { "BlockId" });
            DropIndex("Construction.AreaWorkItem", new[] { "ProjectInformationId" });
            DropIndex("Construction.AreaWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.WorkCategory", new[] { "ConfigurationId" });
            DropIndex("Construction.ProjectMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ProjectMaterial", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectEquipment", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectJobTitle", new[] { "LookupId" });
            DropIndex("Construction.ProjectJobTitle", new[] { "StepId" });
            DropIndex("Construction.ProjectJobTitle", new[] { "RankId" });
            DropIndex("Construction.ProjectEstmnIndexLabour", new[] { "ProjectJobTitleId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "WorkCategoryId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "ProjectInformationId" });
            DropIndex("Construction.WorkItemDuration", new[] { "TakeOffPerProjectId" });
            DropIndex("Task.Task", new[] { "Task1_ID" });
            DropIndex("Task.Task", new[] { "ProjectWorkBreakDownId" });
            DropIndex("Task.Task", new[] { "WorkItemDurationId" });
            DropIndex("Construction.ProjectWorkBreakDown", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialRequest", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialIssued", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialDelivery", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "EmployeeId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEquipmentPerformance", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentMobilization", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentHour", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.EquipmentFuelconsumption", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectBudgetUtilization", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetTransfer", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetRequest", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectDefineBudget", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectConstraintsAndRisk", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectCalander", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectInformationId" });
            DropTable("Construction.ProjectSubContract");
            DropTable("Construction.TakeOffLinePerProject");
            DropTable("Construction.TakeOffLineForKgPerProject");
            DropTable("Construction.SubContractWorkItem");
            DropTable("Construction.Calander");
            DropTable("Construction.Road");
            DropTable("Construction.RoadMapping");
            DropTable("Construction.ResourceUtilizationMaterial");
            DropTable("Construction.ResourceUtilizationEquipment");
            DropTable("Construction.ResourceUtilizationWorkItem");
            DropTable("Construction.ResourceUtilizationDirectLabour");
            DropTable("Task.ResourceType");
            DropTable("Task.Resource");
            DropTable("Task.ResourceAssignment");
            DropTable("Construction.ProjectType");
            DropTable("Construction.ProjectTypeMapping");
            DropTable("EXCEED.ProjectTaskSchedule");
            DropTable("Construction.UnitRate");
            DropTable("Construction.ProjectTakeOffLineForKg");
            DropTable("Construction.ProjectTakeOffLine");
            DropTable("Construction.ProjectTakeOff");
            DropTable("Construction.ProjectMachineryRentHeader");
            DropTable("Construction.ProjectEstmnBoqSummary");
            DropTable("Construction.ProjectEstmnWorkCategory");
            DropTable("Construction.ProjectEstmnUnitFactory");
            DropTable("Construction.ProjectEstmnTransportCost");
            DropTable("Construction.ProjectEstmnRawData");
            DropTable("Construction.ProjectEstimationName");
            DropTable("Construction.ProjectEstmnMobilization");
            DropTable("Construction.ProjectEstmnMaterial");
            DropTable("Construction.ProjectEstmnIndirectLabour");
            DropTable("Construction.ProjectEstmnEquipment");
            DropTable("Construction.ProjectEstimnWorkItem");
            DropTable("Construction.ProjectEstmnDirectLabour");
            DropTable("Construction.ProjectEstmnCoefficients");
            DropTable("Construction.ProjectEstimationBoqDetail");
            DropTable("Construction.ProjectEstimationBoq");
            DropTable("Construction.EstimationAreaWorkItem");
            DropTable("Construction.Retention");
            DropTable("Construction.PaymentCertificate");
            DropTable("Construction.MonthlyFuelCost");
            DropTable("Construction.MethodOfWork");
            DropTable("Construction.MaterialUtilization");
            DropTable("Construction.MaterialPriceIndex");
            DropTable("Construction.MapWorkItemCategory");
            DropTable("Construction.MachinaryRent");
            DropTable("Construction.LineDate");
            DropTable("Construction.PaymenCertificatetLine");
            DropTable("Construction.LaborRateIndexFactorAttribute");
            DropTable("Construction.LaborRateIndexFactorAttributeName");
            DropTable("Construction.InternalMachinaryRent");
            DropTable("Construction.EVMTermOperandComponent");
            DropTable("Construction.SubContractWorkItemEstmn");
            DropTable("Construction.EquipmentMobilization");
            DropTable("Construction.EarnedValuePerProject");
            DropTable("Construction.EarnedValueManagementTerms");
            DropTable("Construction.EarnedValueManagementTermDetail");
            DropTable("Task.Dependency");
            DropTable("Construction.DeformedBar");
            DropTable("Construction.CrewMaterial");
            DropTable("Construction.CrewHumanResource");
            DropTable("Construction.ProjectCrew");
            DropTable("Construction.CrewEquipment");
            DropTable("Construction.ConstructionItemCategory");
            DropTable("Construction.VariationOrderType");
            DropTable("Construction.ContractVariationOrder");
            DropTable("Construction.ContractSuspensionOrder");
            DropTable("Construction.SupplementalAgreement");
            DropTable("Construction.ContractRevision");
            DropTable("Construction.ExecutedWork");
            DropTable("Construction.ExtensionOfTime");
            DropTable("Construction.ContractClaimType");
            DropTable("Construction.ContractClaim");
            DropTable("Construction.ProjectContract");
            DropTable("Construction.CertifiedPayment");
            DropTable("Construction.Block");
            DropTable("Construction.BlockMapping");
            DropTable("Construction.BarDiameter");
            DropTable("Construction.AreaWorkItem");
            DropTable("Construction.WorkCategory");
            DropTable("Construction.ProjectMaterial");
            DropTable("Construction.ProjectEquipment");
            DropTable("Construction.ProjectJobTitle");
            DropTable("Construction.ProjectEstmnIndexLabour");
            DropTable("Construction.ProjectDirectLabour");
            DropTable("Construction.WorkItemPerProject");
            DropTable("Construction.TakeOffPerProject");
            DropTable("Construction.WorkItemDuration");
            DropTable("Task.Task");
            DropTable("Construction.ProjectWorkBreakDown");
            DropTable("Construction.ProjectMaterialRequest");
            DropTable("Construction.ProjectMaterialIssued");
            DropTable("Construction.ProjectMaterialDelivery");
            DropTable("Construction.ProjectHumanResourceUtilization");
            DropTable("Construction.UnitOfMeasurement");
            DropTable("Construction.ProjectEstimnBoqBreakdown");
            DropTable("Construction.ProjectEquipmentPerformance");
            DropTable("Construction.ProjectEquipmentMobilization");
            DropTable("Construction.ProjectEquipmentHour");
            DropTable("Construction.EquipmentFuelconsumption");
            DropTable("Construction.ProjectEquipmentUtilization");
            DropTable("Construction.ProjectBudgetUtilization");
            DropTable("Construction.ProjectBudgetTransfer");
            DropTable("Construction.ProjectBudgetRequest");
            DropTable("Construction.ProjectDefineBudget");
            DropTable("Construction.ProjectConstraintsAndRisk");
            DropTable("Construction.ProjectCalander");
            DropTable("Construction.ProjectInformation");
            DropTable("Construction.ProjectPaymenAdvance");
        }
    }
}
