namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gebeyaclassremoveproject : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ProjectBudget", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectCalendar", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectConstraintsAndRisk", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectdDocument", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectDefineBudget", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectEquipmentUtilization", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectEquipmentFuelconsumption", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentHour", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentMobilization", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentPerformance", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectHumanResourceUtilization", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialDelivery", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialIssued", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialRequest", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectWorkBreakDown", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectBudgetRequest", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectBudgetTransfer", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectBudgetUtilization", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectContract", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectEstimationBoq", "ProjectEstimationNameId", "EXCEED.ProjectEstimationName");
            DropForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoq_ProjectEstimationNameId", "EXCEED.ProjectEstimationName");
            DropForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoqid", "EXCEED.ProjectEstimationBoq");
            DropForeignKey("EXCEED.ProjectEstimationDirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationEquipment", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationIndirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationMaterial", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationUnitRate_ProjectEstimationUnitRateId", "EXCEED.ProjectEstimationUnitRate");
            DropForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkCategoryId", "EXCEED.ProjectEstimationWorkCategory");
            DropForeignKey("EXCEED.ProjectPaymentCertificate", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectPaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectPaymenRetention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectSubContract", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectTakeOff", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "EXCEED.ProjectTakeOff");
            DropForeignKey("EXCEED.ProjectTaskSchedule", "Project_Code", "EXCEED.ProjectInformation");
            DropIndex("EXCEED.ProjectBudget", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectCalendar", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectConstraintsAndRisk", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectdDocument", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectDefineBudget", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectEquipmentUtilization", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectEquipmentFuelconsumption", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentHour", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentMobilization", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentPerformance", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectHumanResourceUtilization", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialDelivery", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialIssued", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialRequest", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectWorkBreakDown", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectBudgetRequest", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectBudgetTransfer", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectBudgetUtilization", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectContract", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectEstimationBoq", new[] { "ProjectEstimationNameId" });
            DropIndex("EXCEED.ProjectEstimationBoqBreakdown", new[] { "ProjectEstimationBoqid" });
            DropIndex("EXCEED.ProjectEstimationBoqBreakdown", new[] { "ProjectEstimationBoq_ProjectEstimationNameId" });
            DropIndex("EXCEED.ProjectEstimationDirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationWorkItem", new[] { "ProjectEstimationWorkCategoryId" });
            DropIndex("EXCEED.ProjectEstimationWorkItem", new[] { "ProjectEstimationUnitRate_ProjectEstimationUnitRateId" });
            DropIndex("EXCEED.ProjectEstimationEquipment", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationIndirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationMaterial", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectPaymentCertificate", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectPaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectPaymenRetention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectSubContract", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectTakeOff", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectTakeOffLine", new[] { "ProjectTakeOff_ProjectTakeOffId" });
            DropIndex("EXCEED.ProjectTaskSchedule", new[] { "Project_Code" });
            DropTable("EXCEED.ProjectBudget");
            DropTable("EXCEED.ProjectInformation");
            DropTable("EXCEED.ProjectCalendar");
            DropTable("EXCEED.ProjectConstraintsAndRisk");
            DropTable("EXCEED.ProjectdDocument");
            DropTable("EXCEED.ProjectDefineBudget");
            DropTable("EXCEED.ProjectEquipmentUtilization");
            DropTable("EXCEED.ProjectEquipmentFuelconsumption");
            DropTable("EXCEED.ProjectEquipmentHour");
            DropTable("EXCEED.ProjectEquipmentMobilization");
            DropTable("EXCEED.ProjectEquipmentPerformance");
            DropTable("EXCEED.ProjectHumanResourceUtilization");
            DropTable("EXCEED.ProjectMaterialDelivery");
            DropTable("EXCEED.ProjectMaterialIssued");
            DropTable("EXCEED.ProjectMaterialRequest");
            DropTable("EXCEED.ProjectWorkBreakDown");
            DropTable("EXCEED.ProjectBudgetRequest");
            DropTable("EXCEED.ProjectBudgetTransfer");
            DropTable("EXCEED.ProjectBudgetUtilization");
            DropTable("EXCEED.ProjectContract");
            DropTable("EXCEED.ProjectEstimationBoq");
            DropTable("EXCEED.ProjectEstimationBoqBreakdown");
            DropTable("EXCEED.ProjectEstimationName");
            DropTable("EXCEED.ProjectEstimationCoefficients");
            DropTable("EXCEED.ProjectEstimationDirectLabour");
            DropTable("EXCEED.ProjectEstimationWorkItem");
            DropTable("EXCEED.ProjectEstimationEquipment");
            DropTable("EXCEED.ProjectEstimationIndirectLabour");
            DropTable("EXCEED.ProjectEstimationMaterial");
            DropTable("EXCEED.ProjectEstimationUnitRate");
            DropTable("EXCEED.ProjectEstimationWorkCategory");
            DropTable("EXCEED.ProjectEstimationIndexLabour");
            DropTable("EXCEED.ProjectEstimationMachineryRent");
            DropTable("EXCEED.ProjectEstimationMobilization");
            DropTable("EXCEED.ProjectEstimationRawData");
            DropTable("EXCEED.ProjectEstimationTransportCost");
            DropTable("EXCEED.ProjectEstimationUnitFactory");
            DropTable("EXCEED.ProjectPaymenAdvance");
            DropTable("EXCEED.ProjectPaymentCertificate");
            DropTable("EXCEED.ProjectPaymenCertificatetLine");
            DropTable("EXCEED.ProjectPaymenRetention");
            DropTable("EXCEED.ProjectSubContract");
            DropTable("EXCEED.ProjectTakeOff");
            DropTable("EXCEED.ProjectTakeOffLine");
            DropTable("EXCEED.ProjectTaskSchedule");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ProjectTaskSchedule",
                c => new
                    {
                        ProjectTaskScheduleId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(),
                        Duration = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CompletePercentage = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectTaskScheduleId);
            
            CreateTable(
                "EXCEED.ProjectTakeOffLine",
                c => new
                    {
                        ProjectTakeOffLineId = c.Int(nullable: false, identity: true),
                        T = c.String(),
                        D = c.String(),
                        S = c.String(),
                        Description = c.String(),
                        ProjectTakeOff_ProjectTakeOffId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineId);
            
            CreateTable(
                "EXCEED.ProjectTakeOff",
                c => new
                    {
                        ProjectTakeOffId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectTakeOffId);
            
            CreateTable(
                "EXCEED.ProjectSubContract",
                c => new
                    {
                        ProjectSubContractId = c.Int(nullable: false, identity: true),
                        WorkType = c.String(),
                        SubContractor = c.String(),
                        ContractSigningDate = c.DateTime(nullable: false),
                        CommencementDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectSubContractId);
            
            CreateTable(
                "EXCEED.ProjectPaymenRetention",
                c => new
                    {
                        ProjectPaymenRetentionId = c.Int(nullable: false, identity: true),
                        RetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenRetentionId);
            
            CreateTable(
                "EXCEED.ProjectPaymenCertificatetLine",
                c => new
                    {
                        ProjectPaymenCertificatetLineId = c.Int(nullable: false, identity: true),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryContract = c.String(),
                        VariationOrder = c.String(),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.String(),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenCertificatetLineId);
            
            CreateTable(
                "EXCEED.ProjectPaymentCertificate",
                c => new
                    {
                        ProjectPaymentCertificateId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectPaymentCertificateId);
            
            CreateTable(
                "EXCEED.ProjectPaymenAdvance",
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
                .PrimaryKey(t => t.ProjectPaymenAdvanceId);
            
            CreateTable(
                "EXCEED.ProjectEstimationUnitFactory",
                c => new
                    {
                        ProjectEstimationUnitFactoryId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        FactoryName = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitFactoryId);
            
            CreateTable(
                "EXCEED.ProjectEstimationTransportCost",
                c => new
                    {
                        ProjectEstimationTransportCostId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationTransportCostId);
            
            CreateTable(
                "EXCEED.ProjectEstimationRawData",
                c => new
                    {
                        ProjectEstimationRawDataId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationRawDataId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMobilization",
                c => new
                    {
                        ProjectEstimationMobilizationId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        TypeOfMachinery = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Trip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Speed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffName = c.String(nullable: false),
                        StaffQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdiem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMobilizationId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMachineryRent",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemNo = c.String(nullable: false),
                        TypeEquipment = c.String(nullable: false),
                        Model = c.String(nullable: false),
                        YearManufacture = c.String(nullable: false),
                        PowerOutput = c.String(nullable: false),
                        WithoutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId);
            
            CreateTable(
                "EXCEED.ProjectEstimationIndexLabour",
                c => new
                    {
                        ProjectEstimationIndexLabourId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        LabourByTrade = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        IndexFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BirrMonth = c.String(nullable: false),
                        IdealBirrDay = c.String(nullable: false),
                        ActualBirrDay = c.String(nullable: false),
                        IdealBirrDays = c.String(nullable: false),
                        ActualBirrDays = c.String(nullable: false),
                        DesertBirrDays = c.String(nullable: false),
                        NetBirrDays = c.String(nullable: false),
                        NetBirrHour = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "EXCEED.ProjectEstimationWorkCategory",
                c => new
                    {
                        ProjectEstimationWorkCategoryId = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkCategoryId);
            
            CreateTable(
                "EXCEED.ProjectEstimationUnitRate",
                c => new
                    {
                        ProjectEstimationUnitRateId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Overheard = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitRateId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMaterial",
                c => new
                    {
                        ProjectEstimationMaterialId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        TypeOfMaterial = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMaterialId);
            
            CreateTable(
                "EXCEED.ProjectEstimationIndirectLabour",
                c => new
                    {
                        ProjectEstimationIndirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        LabourName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndirectLabourId);
            
            CreateTable(
                "EXCEED.ProjectEstimationEquipment",
                c => new
                    {
                        ProjectEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationEquipmentId);
            
            CreateTable(
                "EXCEED.ProjectEstimationWorkItem",
                c => new
                    {
                        ProjectEstimationWorkItemId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkCategoryId = c.Int(nullable: false),
                        Category = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        DirectLabour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndirectLabour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEstimationUnitRate_ProjectEstimationUnitRateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "EXCEED.ProjectEstimationDirectLabour",
                c => new
                    {
                        ProjectEstimationDirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        LabourName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationDirectLabourId);
            
            CreateTable(
                "EXCEED.ProjectEstimationCoefficients",
                c => new
                    {
                        ProjectEstimationCoefficientsId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Kg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shrinkage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Wastage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationCoefficientsId);
            
            CreateTable(
                "EXCEED.ProjectEstimationName",
                c => new
                    {
                        ProjectEstimationNameId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        ClientName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "EXCEED.ProjectEstimationBoqBreakdown",
                c => new
                    {
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqid = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEstimationBoq_ProjectEstimationNameId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqBreakdownId);
            
            CreateTable(
                "EXCEED.ProjectEstimationBoq",
                c => new
                    {
                        ProjectEstimationBoqid = c.Int(nullable: false, identity: true),
                        ProjectEstimationNameId = c.Int(nullable: false),
                        TypeOfBoq = c.String(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqid);
            
            CreateTable(
                "EXCEED.ProjectContract",
                c => new
                    {
                        ProjectContractId = c.Int(nullable: false, identity: true),
                        VariationOrder = c.String(),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectContractId);
            
            CreateTable(
                "EXCEED.ProjectBudgetUtilization",
                c => new
                    {
                        ProjectBudgetUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Utilized = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectBudgetTransfer",
                c => new
                    {
                        ProjectBudgetTransferId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        PreviousTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetTransferId);
            
            CreateTable(
                "EXCEED.ProjectBudgetRequest",
                c => new
                    {
                        ProjectBudgetRequestId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false),
                        Requested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetRequestId);
            
            CreateTable(
                "EXCEED.ProjectWorkBreakDown",
                c => new
                    {
                        ProjectWorkBreakDownId = c.Int(nullable: false, identity: true),
                        TaskCode = c.String(),
                        Description = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectWorkBreakDownId);
            
            CreateTable(
                "EXCEED.ProjectMaterialRequest",
                c => new
                    {
                        ProjectMaterialRequestId = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        RequestedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialRequestId);
            
            CreateTable(
                "EXCEED.ProjectMaterialIssued",
                c => new
                    {
                        ProjectMaterialIssuedId = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialIssuedId);
            
            CreateTable(
                "EXCEED.ProjectMaterialDelivery",
                c => new
                    {
                        ProjectMaterialDeliveryId = c.Int(nullable: false, identity: true),
                        DeliveryDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        DeliveredQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialDeliveryId);
            
            CreateTable(
                "EXCEED.ProjectHumanResourceUtilization",
                c => new
                    {
                        ProjectHumanResourceUtilizationId = c.Int(nullable: false, identity: true),
                        CrewCode = c.String(nullable: false),
                        CrewName = c.String(nullable: false),
                        CrewLeader = c.String(nullable: false),
                        CombinationOfCrew = c.String(nullable: false),
                        TypeOfWork = c.String(nullable: false),
                        CrewOutput = c.String(nullable: false),
                        CrewDirectCost = c.String(nullable: false),
                        CrewIndirectCost = c.String(nullable: false),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectHumanResourceUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentPerformance",
                c => new
                    {
                        ProjectEquipmentPerformanceId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        PreviousPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDatePerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentPerformanceId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentMobilization",
                c => new
                    {
                        ProjectEquipmentMobilizationId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(nullable: false),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentMobilizationId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentHour",
                c => new
                    {
                        ProjectEquipmentHourId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        PlannedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverallEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentHourId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentFuelconsumption",
                c => new
                    {
                        ProjectEquipmentFuelconsumptionId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        PreviousDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPreviousConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCurrentConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalToDateConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentFuelconsumptionId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentUtilization",
                c => new
                    {
                        ProjectEquipmentUtilizationId = c.Int(nullable: false, identity: true),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectEquipmentUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectDefineBudget",
                c => new
                    {
                        ProjectDefineBudgetId = c.Int(nullable: false, identity: true),
                        BudgetCode = c.String(nullable: false),
                        TotalBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectDefineBudgetId);
            
            CreateTable(
                "EXCEED.ProjectdDocument",
                c => new
                    {
                        ProjectdDocumentId = c.Int(nullable: false, identity: true),
                        DocumentType = c.String(),
                        File = c.Binary(),
                        Description = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectdDocumentId);
            
            CreateTable(
                "EXCEED.ProjectConstraintsAndRisk",
                c => new
                    {
                        ProjectConstraintsAndRiskId = c.Int(nullable: false, identity: true),
                        Assumptions = c.String(),
                        Constraints = c.String(),
                        RiskDescription = c.String(),
                        RiskStatus = c.Int(nullable: false),
                        RiskImpact = c.Int(nullable: false),
                        RiskProbablity = c.Int(nullable: false),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectConstraintsAndRiskId);
            
            CreateTable(
                "EXCEED.ProjectCalendar",
                c => new
                    {
                        ProjectCalendarId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.String(),
                        ToHour = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectCalendarId);
            
            CreateTable(
                "EXCEED.ProjectInformation",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        ProjectInformationId = c.Int(nullable: false),
                        Name = c.String(),
                        Category = c.Int(nullable: false),
                        Location = c.String(),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectScope = c.String(),
                        ProjectManager = c.String(),
                        Client = c.String(),
                        Consultant = c.String(),
                        AwardDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractSigningDate = c.DateTime(nullable: false),
                        SiteHandOverDate = c.DateTime(nullable: false),
                        EstimatedStartDate = c.DateTime(nullable: false),
                        EstimatedEndDate = c.DateTime(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        LiabilityPeriod = c.String(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "EXCEED.ProjectBudget",
                c => new
                    {
                        ProjectBudgetId = c.Int(nullable: false, identity: true),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectBudgetId);
            
            CreateIndex("EXCEED.ProjectTaskSchedule", "Project_Code");
            CreateIndex("EXCEED.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId");
            CreateIndex("EXCEED.ProjectTakeOff", "Project_Code");
            CreateIndex("EXCEED.ProjectSubContract", "Project_Code");
            CreateIndex("EXCEED.ProjectPaymenRetention", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("EXCEED.ProjectPaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("EXCEED.ProjectPaymentCertificate", "Project_Code");
            CreateIndex("EXCEED.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("EXCEED.ProjectEstimationMaterial", "ProjectEstimationWorkItemId");
            CreateIndex("EXCEED.ProjectEstimationIndirectLabour", "ProjectEstimationWorkItemId");
            CreateIndex("EXCEED.ProjectEstimationEquipment", "ProjectEstimationWorkItemId");
            CreateIndex("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationUnitRate_ProjectEstimationUnitRateId");
            CreateIndex("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkCategoryId");
            CreateIndex("EXCEED.ProjectEstimationDirectLabour", "ProjectEstimationWorkItemId");
            CreateIndex("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoq_ProjectEstimationNameId");
            CreateIndex("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoqid");
            CreateIndex("EXCEED.ProjectEstimationBoq", "ProjectEstimationNameId");
            CreateIndex("EXCEED.ProjectContract", "Project_Code");
            CreateIndex("EXCEED.ProjectBudgetUtilization", "ProjectBudgetId");
            CreateIndex("EXCEED.ProjectBudgetTransfer", "ProjectBudgetId");
            CreateIndex("EXCEED.ProjectBudgetRequest", "ProjectBudgetId");
            CreateIndex("EXCEED.ProjectWorkBreakDown", "Project_Code");
            CreateIndex("EXCEED.ProjectMaterialRequest", "Project_Code");
            CreateIndex("EXCEED.ProjectMaterialIssued", "Project_Code");
            CreateIndex("EXCEED.ProjectMaterialDelivery", "Project_Code");
            CreateIndex("EXCEED.ProjectHumanResourceUtilization", "Project_Code");
            CreateIndex("EXCEED.ProjectEquipmentPerformance", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId");
            CreateIndex("EXCEED.ProjectEquipmentMobilization", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId");
            CreateIndex("EXCEED.ProjectEquipmentHour", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId");
            CreateIndex("EXCEED.ProjectEquipmentFuelconsumption", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId");
            CreateIndex("EXCEED.ProjectEquipmentUtilization", "Project_Code");
            CreateIndex("EXCEED.ProjectDefineBudget", "Project_Code");
            CreateIndex("EXCEED.ProjectdDocument", "Project_Code");
            CreateIndex("EXCEED.ProjectConstraintsAndRisk", "Project_Code");
            CreateIndex("EXCEED.ProjectCalendar", "Project_Code");
            CreateIndex("EXCEED.ProjectBudget", "Project_Code");
            AddForeignKey("EXCEED.ProjectTaskSchedule", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "EXCEED.ProjectTakeOff", "ProjectTakeOffId");
            AddForeignKey("EXCEED.ProjectTakeOff", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectSubContract", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectPaymenRetention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("EXCEED.ProjectPaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("EXCEED.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("EXCEED.ProjectPaymentCertificate", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkCategoryId", "EXCEED.ProjectEstimationWorkCategory", "ProjectEstimationWorkCategoryId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationUnitRate_ProjectEstimationUnitRateId", "EXCEED.ProjectEstimationUnitRate", "ProjectEstimationUnitRateId");
            AddForeignKey("EXCEED.ProjectEstimationMaterial", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationIndirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationEquipment", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationDirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoqid", "EXCEED.ProjectEstimationBoq", "ProjectEstimationBoqid", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoq_ProjectEstimationNameId", "EXCEED.ProjectEstimationName", "ProjectEstimationNameId");
            AddForeignKey("EXCEED.ProjectEstimationBoq", "ProjectEstimationNameId", "EXCEED.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectContract", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectBudgetUtilization", "ProjectBudgetId", "EXCEED.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectBudgetTransfer", "ProjectBudgetId", "EXCEED.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectBudgetRequest", "ProjectBudgetId", "EXCEED.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("EXCEED.ProjectWorkBreakDown", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectMaterialRequest", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectMaterialIssued", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectMaterialDelivery", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectHumanResourceUtilization", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectEquipmentPerformance", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId");
            AddForeignKey("EXCEED.ProjectEquipmentMobilization", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId");
            AddForeignKey("EXCEED.ProjectEquipmentHour", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId");
            AddForeignKey("EXCEED.ProjectEquipmentFuelconsumption", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId");
            AddForeignKey("EXCEED.ProjectEquipmentUtilization", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectDefineBudget", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectdDocument", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectConstraintsAndRisk", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectCalendar", "Project_Code", "EXCEED.ProjectInformation", "Code");
            AddForeignKey("EXCEED.ProjectBudget", "Project_Code", "EXCEED.ProjectInformation", "Code");
        }
    }
}
