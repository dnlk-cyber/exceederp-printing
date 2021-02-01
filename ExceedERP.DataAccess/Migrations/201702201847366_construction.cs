namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction : DbMigration
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
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenAdvanceId)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
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
                "Construction.ProjectInformation",
                c => new
                    {
                        ProjectInformationId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Code = c.String(maxLength: 50),
                        Category = c.Int(nullable: false),
                        Location = c.String(maxLength: 100),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectScope = c.String(),
                        ProjectManager = c.String(maxLength: 100),
                        Client = c.String(maxLength: 200),
                        Consultant = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectBudget",
                c => new
                    {
                        ProjectBudgetId = c.Int(nullable: false, identity: true),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectBudgetId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectBudgetRequest",
                c => new
                    {
                        ProjectBudgetRequestId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false, maxLength: 200),
                        Requested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetRequestId)
                .ForeignKey("Construction.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "Construction.ProjectBudgetTransfer",
                c => new
                    {
                        ProjectBudgetTransferId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        PreviousTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetTransferId)
                .ForeignKey("Construction.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "Construction.ProjectBudgetUtilization",
                c => new
                    {
                        ProjectBudgetUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false, maxLength: 200),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Utilized = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetUtilizationId)
                .ForeignKey("Construction.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "Construction.ProjectCalander",
                c => new
                    {
                        ProjectCalanderId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.String(maxLength: 50),
                        ToHour = c.String(maxLength: 50),
                        ProjectInformationId = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ProjectDefineBudgetId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectEquipmentUtilization",
                c => new
                    {
                        ProjectEquipmentUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
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
                        Decription = c.String(maxLength: 200),
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
                        EquipmentType = c.String(nullable: false),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(nullable: false),
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
                        EquipmentType = c.String(nullable: false),
                        PreviousPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDatePerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEquipmentPerformanceId)
                .ForeignKey("Construction.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilizationId, cascadeDelete: true)
                .Index(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "Construction.ProjectHumanResourceUtilization",
                c => new
                    {
                        ProjectHumanResourceUtilizationId = c.Int(nullable: false, identity: true),
                        CrewCode = c.String(nullable: false, maxLength: 100),
                        CrewName = c.String(nullable: false, maxLength: 200),
                        CrewLeader = c.String(nullable: false, maxLength: 200),
                        CombinationOfCrew = c.String(nullable: false, maxLength: 200),
                        TypeOfWork = c.String(nullable: false, maxLength: 200),
                        CrewOutput = c.String(nullable: false, maxLength: 200),
                        CrewDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CrewIndirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectHumanResourceUtilizationId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
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
                        TaskCode = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        ProjectInformationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectWorkBreakDownId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
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
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenCertificatetLineId)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "Construction.Retention",
                c => new
                    {
                        ProjectPaymenRetentionId = c.Int(nullable: false, identity: true),
                        RetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenRetentionId)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
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
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
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
                    })
                .PrimaryKey(t => t.LineDateId)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.PeriodId);
            
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
                "Payroll.PerdimeSetting",
                c => new
                    {
                        PerdimeSettingId = c.Int(nullable: false, identity: true),
                        Minmum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Maximum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PerdimeSettingId);
            
            CreateTable(
                "Construction.ProjectContract",
                c => new
                    {
                        ProjectContractId = c.Int(nullable: false, identity: true),
                        VariationOrder = c.String(maxLength: 200),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(maxLength: 200),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectContractId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectEstimationBoq",
                c => new
                    {
                        ProjectEstimationBoqid = c.Int(nullable: false, identity: true),
                        ProjectEstimationNameId = c.Int(nullable: false),
                        TypeOfBoq = c.Int(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqid)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstimationName",
                c => new
                    {
                        ProjectEstimationNameId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        ProjectLocation = c.String(nullable: false, maxLength: 200),
                        ClientName = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectEstimnBoqBreakdown",
                c => new
                    {
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqid = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        BillNumber = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqBreakdownId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
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
                    })
                .PrimaryKey(t => t.ProjectEstimationCoefficientsId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnDirectLabour",
                c => new
                    {
                        ProjectEstimationDirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        LabourName = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationDirectLabourId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.ProjectEstimnWorkItem",
                c => new
                    {
                        ProjectEstimationWorkItemId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        TotalQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectType = c.Int(nullable: false),
                        ProjectEstimationNameId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkItemId)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectEstmnEquipment",
                c => new
                    {
                        ProjectEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationEquipmentId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.ProjectEstmnIndirectLabour",
                c => new
                    {
                        ProjectEstimationIndirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        LabourName = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndirectLabourId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnMaterial",
                c => new
                    {
                        ProjectEstimationMaterialId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.ProjectEstmnIndexLabour",
                c => new
                    {
                        ProjectEstimationIndexLabourId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
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
                    })
                .PrimaryKey(t => t.ProjectEstimationIndexLabourId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnMachineryRent",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        TypeEquipment = c.Int(nullable: false),
                        Model = c.String(nullable: false, maxLength: 200),
                        YearManufacture = c.String(nullable: false, maxLength: 50),
                        PowerOutput = c.String(nullable: false, maxLength: 200),
                        WithoutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId);
            
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
                    })
                .PrimaryKey(t => t.ProjectEstimationMobilizationId);
            
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
                    })
                .PrimaryKey(t => t.ProjectEstimationRawDataId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
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
                    })
                .PrimaryKey(t => t.ProjectEstimationTransportCostId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
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
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitFactoryId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Construction.ProjectEstmnUnitRate",
                c => new
                    {
                        ProjectEstimationUnitRateId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Overheard = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitRateId);
            
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
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkCategoryId);
            
            CreateTable(
                "Construction.ProjectTakeOff",
                c => new
                    {
                        ProjectTakeOffId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffId)
                .ForeignKey("Construction.ProjectInformation", t => t.Project_ProjectInformationId)
                .Index(t => t.Project_ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectTakeOffLine",
                c => new
                    {
                        ProjectTakeOffLineId = c.Int(nullable: false, identity: true),
                        T = c.String(maxLength: 100),
                        D = c.String(maxLength: 100),
                        S = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        ProjectTakeOff_ProjectTakeOffId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineId)
                .ForeignKey("Construction.ProjectTakeOff", t => t.ProjectTakeOff_ProjectTakeOffId)
                .Index(t => t.ProjectTakeOff_ProjectTakeOffId);
            
            CreateTable(
                "Construction.ProjectSubContract",
                c => new
                    {
                        ProjectSubContractId = c.Int(nullable: false, identity: true),
                        WorkType = c.String(maxLength: 100),
                        SubContractor = c.String(maxLength: 200),
                        ContractSigningDate = c.DateTime(nullable: false),
                        CommencementDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(maxLength: 200),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectSubContractId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ProjectSubContract", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectTakeOff", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstmnUnitFactory", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstmnTransportCost", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstmnRawData", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstmnIndexLabour", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnMaterial", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstmnEquipment", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnEquipment", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnCoefficients", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstimationBoq", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectContract", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.LineDate", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.LineDate", "PeriodId", "common.Period");
            DropForeignKey("Construction.EquipmentMobilization", "LookupId", "common.Lookup");
            DropForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectWorkBreakDown", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialRequest", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialIssued", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialDelivery", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEquipmentPerformance", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentMobilization", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentHour", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.EquipmentFuelconsumption", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectDefineBudget", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectConstraintsAndRisk", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectCalander", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectBudgetUtilization", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectBudgetTransfer", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectBudgetRequest", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectBudget", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ProjectSubContract", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "ProjectTakeOff_ProjectTakeOffId" });
            DropIndex("Construction.ProjectTakeOff", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectEstmnUnitFactory", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnTransportCost", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnRawData", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnIndexLabour", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ProjectEstmnMaterial", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "ItemCode" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnCoefficients", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectContract", new[] { "ProjectInformationId" });
            DropIndex("Construction.LineDate", new[] { "PeriodId" });
            DropIndex("Construction.LineDate", new[] { "ProjectInformationId" });
            DropIndex("Construction.EquipmentMobilization", new[] { "LookupId" });
            DropIndex("Construction.Retention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.ProjectWorkBreakDown", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialRequest", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialIssued", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialDelivery", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEquipmentPerformance", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentMobilization", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentHour", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.EquipmentFuelconsumption", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectDefineBudget", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectConstraintsAndRisk", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectCalander", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectBudgetUtilization", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectBudgetTransfer", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectBudgetRequest", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectBudget", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.PaymentCertificate", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropTable("Construction.ProjectSubContract");
            DropTable("Construction.ProjectTakeOffLine");
            DropTable("Construction.ProjectTakeOff");
            DropTable("Construction.ProjectEstmnWorkCategory");
            DropTable("Construction.ProjectEstmnUnitRate");
            DropTable("Construction.ProjectEstmnUnitFactory");
            DropTable("Construction.ProjectEstmnTransportCost");
            DropTable("Construction.ProjectEstmnRawData");
            DropTable("Construction.ProjectEstmnMobilization");
            DropTable("Construction.ProjectEstmnMachineryRent");
            DropTable("Construction.ProjectEstmnIndexLabour");
            DropTable("Construction.ProjectEstmnMaterial");
            DropTable("Construction.ProjectEstmnIndirectLabour");
            DropTable("Construction.ProjectEstmnEquipment");
            DropTable("Construction.ProjectEstimnWorkItem");
            DropTable("Construction.ProjectEstmnDirectLabour");
            DropTable("Construction.ProjectEstmnCoefficients");
            DropTable("Construction.ProjectEstimnBoqBreakdown");
            DropTable("Construction.ProjectEstimationName");
            DropTable("Construction.ProjectEstimationBoq");
            DropTable("Construction.ProjectContract");
            DropTable("Payroll.PerdimeSetting");
            DropTable("Construction.MapWorkItemCategory");
            DropTable("Construction.LineDate");
            DropTable("Construction.EquipmentMobilization");
            DropTable("Construction.Retention");
            DropTable("Construction.PaymenCertificatetLine");
            DropTable("Construction.ProjectWorkBreakDown");
            DropTable("Construction.ProjectMaterialRequest");
            DropTable("Construction.ProjectMaterialIssued");
            DropTable("Construction.ProjectMaterialDelivery");
            DropTable("Construction.ProjectHumanResourceUtilization");
            DropTable("Construction.ProjectEquipmentPerformance");
            DropTable("Construction.ProjectEquipmentMobilization");
            DropTable("Construction.ProjectEquipmentHour");
            DropTable("Construction.EquipmentFuelconsumption");
            DropTable("Construction.ProjectEquipmentUtilization");
            DropTable("Construction.ProjectDefineBudget");
            DropTable("Construction.ProjectConstraintsAndRisk");
            DropTable("Construction.ProjectCalander");
            DropTable("Construction.ProjectBudgetUtilization");
            DropTable("Construction.ProjectBudgetTransfer");
            DropTable("Construction.ProjectBudgetRequest");
            DropTable("Construction.ProjectBudget");
            DropTable("Construction.ProjectInformation");
            DropTable("Construction.PaymentCertificate");
            DropTable("Construction.ProjectPaymenAdvance");
        }
    }
}
