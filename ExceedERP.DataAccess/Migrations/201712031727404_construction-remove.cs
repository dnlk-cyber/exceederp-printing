namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constructionremove : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectCalander", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectConstraintsAndRisk", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectDefineBudget", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.EquipmentFuelconsumption", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentHour", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentMobilization", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentPerformance", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization");
            DropForeignKey("Construction.ProjectEquipmentUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "EmployeeId", "HRM.Employee");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialDelivery", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialIssued", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialRequest", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectWorkBreakDown", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectDirectLabour", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectEquipment", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectMaterial", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.WorkItemPerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.WorkItemPerProject", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.EquipmentMobilization", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.LineDate", "PeriodId", "Common.Period");
            DropForeignKey("Construction.LineDate", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.MaterialUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.Retention", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.ProjectContract", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstimationBoq", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstimationBoqDetail", "ProjectEstimationBoqBreakdownId", "Construction.ProjectEstimnBoqBreakdown");
            DropForeignKey("Construction.ProjectEstmnCoefficients", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnEquipment", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnIndirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstmnMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEstmnMaterial", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnRawData", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnTransportCost", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnUnitFactory", "LookupId", "Common.Lookup");
            DropForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMachineryRentHeader", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate");
            DropForeignKey("EXCEED.ProjectTaskSchedule", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Schedule.Schedule", "GanttTask1_ID", "Schedule.Schedule");
            DropForeignKey("Schedule.Schedule", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectSubContract", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.TakeOffLineForKgPerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropForeignKey("Construction.TakeOffLineForKgPerProject", "UnitRateId", "Construction.UnitRate");
            DropForeignKey("Construction.TakeOffLinePerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.ProjectCalander", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectConstraintsAndRisk", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectDefineBudget", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectBudgetRequest", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetTransfer", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetUtilization", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectEquipmentUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.EquipmentFuelconsumption", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentHour", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentMobilization", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEquipmentPerformance", new[] { "ProjectEquipmentUtilizationId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "EmployeeId" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialDelivery", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialIssued", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectMaterialRequest", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectWorkBreakDown", new[] { "ProjectInformationId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "ProjectInformationId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectEquipment", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectMaterial", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectMaterial", new[] { "ItemCode" });
            DropIndex("Construction.EquipmentMobilization", new[] { "LookupId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectInformationId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.LineDate", new[] { "ProjectInformationId" });
            DropIndex("Construction.LineDate", new[] { "PeriodId" });
            DropIndex("Construction.MaterialUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.PaymentCertificate", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.Retention", new[] { "ProjectInformationId" });
            DropIndex("Construction.Retention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.ProjectContract", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectEstimationName_ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstimationBoqDetail", new[] { "ProjectEstimationBoqBreakdownId" });
            DropIndex("Construction.ProjectEstmnCoefficients", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnIndirectLabour", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnMaterial", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.ProjectEstmnMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ProjectEstmnRawData", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnTransportCost", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnUnitFactory", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnBoqSummary", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectMachineryRentHeader", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "ProjectTakeOffId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "UnitRateId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "ProjectTakeOffId" });
            DropIndex("EXCEED.ProjectTaskSchedule", new[] { "ProjectInformationId" });
            DropIndex("Schedule.Schedule", new[] { "ProjectInformationId" });
            DropIndex("Schedule.Schedule", new[] { "GanttTask1_ID" });
            DropIndex("Construction.ProjectSubContract", new[] { "ProjectInformationId" });
            DropIndex("Construction.TakeOffLineForKgPerProject", new[] { "UnitRateId" });
            DropIndex("Construction.TakeOffLineForKgPerProject", new[] { "TakeOffPerProjectId" });
            DropIndex("Construction.TakeOffLinePerProject", new[] { "TakeOffPerProjectId" });
            DropTable("Construction.ProjectPaymenAdvance");
            DropTable("Construction.ProjectInformation");
            DropTable("Construction.ProjectCalander");
            DropTable("Construction.ProjectConstraintsAndRisk");
            DropTable("Construction.ProjectDefineBudget");
            DropTable("Construction.ProjectBudgetRequest");
            DropTable("Construction.ProjectBudgetTransfer");
            DropTable("Construction.ProjectBudgetUtilization");
            DropTable("Construction.ProjectEquipmentUtilization");
            DropTable("Construction.EquipmentFuelconsumption");
            DropTable("Construction.ProjectEquipmentHour");
            DropTable("Construction.ProjectEquipmentMobilization");
            DropTable("Construction.ProjectEquipmentPerformance");
            DropTable("Construction.ProjectEstimnBoqBreakdown");
            DropTable("Construction.WorkCategory");
            DropTable("Construction.ProjectHumanResourceUtilization");
            DropTable("Construction.ProjectMaterialDelivery");
            DropTable("Construction.ProjectMaterialIssued");
            DropTable("Construction.ProjectMaterialRequest");
            DropTable("Construction.ProjectWorkBreakDown");
            DropTable("Construction.WorkItemPerProject");
            DropTable("Construction.ProjectDirectLabour");
            DropTable("Construction.ProjectEstmnIndexLabour");
            DropTable("Construction.ProjectEquipment");
            DropTable("Construction.ProjectMaterial");
            DropTable("Construction.EquipmentMobilization");
            DropTable("Construction.InternalMachinaryRent");
            DropTable("EXCEED.GanttDependency");
            DropTable("EXCEED.GanttResourceAssignment");
            DropTable("EXCEED.GanttResource");
            DropTable("Construction.PaymenCertificatetLine");
            DropTable("Construction.LineDate");
            DropTable("Construction.MachinaryRent");
            DropTable("Construction.MapWorkItemCategory");
            DropTable("Construction.MaterialUtilization");
            DropTable("Construction.PaymentCertificate");
            DropTable("Construction.Retention");
            DropTable("Construction.ProjectContract");
            DropTable("Construction.ProjectEstimationBoq");
            DropTable("Construction.ProjectEstimationBoqDetail");
            DropTable("Construction.ProjectEstmnCoefficients");
            DropTable("Construction.ProjectEstmnDirectLabour");
            DropTable("Construction.ProjectEstimnWorkItem");
            DropTable("Construction.ProjectEstmnEquipment");
            DropTable("Construction.ProjectEstmnIndirectLabour");
            DropTable("Construction.ProjectEstmnMaterial");
            DropTable("Construction.ProjectEstmnMobilization");
            DropTable("Construction.ProjectEstimationName");
            DropTable("Construction.ProjectEstmnRawData");
            DropTable("Construction.ProjectEstmnTransportCost");
            DropTable("Construction.ProjectEstmnUnitFactory");
            DropTable("Construction.ProjectEstmnWorkCategory");
            DropTable("Construction.ProjectEstmnBoqSummary");
            DropTable("Construction.ProjectMachineryRentHeader");
            DropTable("Construction.ProjectTakeOff");
            DropTable("Construction.ProjectTakeOffLine");
            DropTable("Construction.ProjectTakeOffLineForKg");
            DropTable("Construction.UnitRate");
            DropTable("EXCEED.ProjectTaskSchedule");
            DropTable("Schedule.Schedule");
            DropTable("Construction.ProjectSubContract");
            DropTable("Construction.TakeOffLineForKgPerProject");
            DropTable("Construction.TakeOffPerProject");
            DropTable("Construction.TakeOffLinePerProject");
        }
        
        public override void Down()
        {
            CreateTable(
                "Construction.TakeOffLinePerProject",
                c => new
                    {
                        TakeOffLinePerProjectId = c.Int(nullable: false, identity: true),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                        D = c.Decimal(nullable: false, precision: 18, scale: 2),
                        S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 400),
                        TakeOffPerProjectId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffLinePerProjectId);
            
            CreateTable(
                "Construction.TakeOffPerProject",
                c => new
                    {
                        TakeOffPerProjectId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffPerProjectId);
            
            CreateTable(
                "Construction.TakeOffLineForKgPerProject",
                c => new
                    {
                        TakeOffLineForKgPerProjectId = c.Int(nullable: false, identity: true),
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
                        TakeOffPerProjectId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TakeOffLineForKgPerProjectId);
            
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectSubContractId);
            
            CreateTable(
                "Schedule.Schedule",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ParentID = c.Int(),
                        OrderID = c.Int(nullable: false),
                        Title = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        PercentComplete = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expanded = c.Boolean(nullable: false),
                        Summary = c.Boolean(nullable: false),
                        ProjectCode = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        GanttTask1_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ProjectTaskScheduleId);
            
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
                .PrimaryKey(t => t.ProjectTakeOffLineForKgId);
            
            CreateTable(
                "Construction.ProjectTakeOffLine",
                c => new
                    {
                        ProjectTakeOffLineId = c.Int(nullable: false, identity: true),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                        D = c.Decimal(nullable: false, precision: 18, scale: 2),
                        S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 400),
                        ProjectTakeOffId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineId);
            
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
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId);
            
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqSummaryId);
            
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
                .PrimaryKey(t => t.ProjectEstimationUnitFactoryId);
            
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
                .PrimaryKey(t => t.ProjectEstimationTransportCostId);
            
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
                .PrimaryKey(t => t.ProjectEstimationRawDataId);
            
            CreateTable(
                "Construction.ProjectEstimationName",
                c => new
                    {
                        ProjectEstimationNameId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        ProjectLocation = c.String(nullable: false, maxLength: 200),
                        ClientName = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationNameId);
            
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationMaterialId);
            
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndirectLabourId);
            
            CreateTable(
                "Construction.ProjectEstmnEquipment",
                c => new
                    {
                        ProjectEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        AssetNumber = c.String(nullable: false, maxLength: 100),
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
                .PrimaryKey(t => t.ProjectEstimationEquipmentId);
            
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
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.Int(nullable: false),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.ProjectEstmnDirectLabour",
                c => new
                    {
                        ProjectEstimationDirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationDirectLabourId);
            
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
                .PrimaryKey(t => t.ProjectEstimationCoefficientsId);
            
            CreateTable(
                "Construction.ProjectEstimationBoqDetail",
                c => new
                    {
                        ProjectEstmnBoqDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqDetailId);
            
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
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ProjectEstimationName_ProjectEstimationNameId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqid);
            
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectContractId);
            
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
                .PrimaryKey(t => t.ProjectPaymenRetentionId);
            
            CreateTable(
                "Construction.PaymentCertificate",
                c => new
                    {
                        ProjectPaymentCertificateId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(maxLength: 100),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymentCertificateId);
            
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
                .PrimaryKey(t => t.MaterialUtilizationId);
            
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
                "Construction.MachinaryRent",
                c => new
                    {
                        MachinaryRentId = c.Int(nullable: false, identity: true),
                        FleetEquipmentRentFromExternalID = c.Int(nullable: false),
                        Model = c.String(maxLength: 200),
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
                .PrimaryKey(t => t.MachinaryRentId);
            
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
                .PrimaryKey(t => t.LineDateId);
            
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
                .PrimaryKey(t => t.ProjectPaymenCertificatetLineId);
            
            CreateTable(
                "EXCEED.GanttResource",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "EXCEED.GanttResourceAssignment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        Units = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "EXCEED.GanttDependency",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PredecessorID = c.Int(nullable: false),
                        SuccessorID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "Construction.InternalMachinaryRent",
                c => new
                    {
                        InternalMachinaryRentId = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(nullable: false, maxLength: 100),
                        PurchaseCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumLife = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepCostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyRentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalMachinaryRentId);
            
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
                .PrimaryKey(t => t.EquipmentMobilizationId);
            
            CreateTable(
                "Construction.ProjectMaterial",
                c => new
                    {
                        ProjectMaterialId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
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
                .PrimaryKey(t => t.ProjectMaterialId);
            
            CreateTable(
                "Construction.ProjectEquipment",
                c => new
                    {
                        ProjectEquipmentId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        AssetNumber = c.String(nullable: false, maxLength: 100),
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
                .PrimaryKey(t => t.ProjectEquipmentId);
            
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
                        FormHr = c.Boolean(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.ProjectDirectLabour",
                c => new
                    {
                        ProjectDirectLabourId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectDirectLabourId);
            
            CreateTable(
                "Construction.WorkItemPerProject",
                c => new
                    {
                        WorkItemPerProjectId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
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
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.Int(nullable: false),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkItemPerProjectId);
            
            CreateTable(
                "Construction.ProjectWorkBreakDown",
                c => new
                    {
                        ProjectWorkBreakDownId = c.Int(nullable: false, identity: true),
                        TaskCode = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectWorkBreakDownId);
            
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
                .PrimaryKey(t => t.ProjectMaterialRequestId);
            
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
                .PrimaryKey(t => t.ProjectMaterialIssuedId);
            
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
                .PrimaryKey(t => t.ProjectMaterialDeliveryId);
            
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
                .PrimaryKey(t => t.ProjectHumanResourceUtilizationId);
            
            CreateTable(
                "Construction.WorkCategory",
                c => new
                    {
                        WorkCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        StructureType = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.ProjectEstimnBoqBreakdown",
                c => new
                    {
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        BillNumber = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        Unit = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StructureType = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        Category = c.String(),
                        Type = c.String(),
                        Index = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqBreakdownId);
            
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
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentPerformanceId);
            
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
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentMobilizationId);
            
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
                .PrimaryKey(t => t.ProjectEquipmentHourId);
            
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
                .PrimaryKey(t => t.ProjectEquipmentFuelconsumptionId);
            
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
                .PrimaryKey(t => t.ProjectEquipmentUtilizationId);
            
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
                .PrimaryKey(t => t.ProjectBudgetUtilizationId);
            
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
                .PrimaryKey(t => t.ProjectBudgetTransferId);
            
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
                .PrimaryKey(t => t.ProjectBudgetRequestId);
            
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
                .PrimaryKey(t => t.ProjectDefineBudgetId);
            
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
                .PrimaryKey(t => t.ProjectConstraintsAndRiskId);
            
            CreateTable(
                "Construction.ProjectCalander",
                c => new
                    {
                        ProjectCalanderId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.String(maxLength: 50),
                        ToHour = c.String(maxLength: 50),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCalanderId);
            
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
                        ProjectStatus = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectInformationId);
            
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
                .PrimaryKey(t => t.ProjectPaymenAdvanceId);
            
            CreateIndex("Construction.TakeOffLinePerProject", "TakeOffPerProjectId");
            CreateIndex("Construction.TakeOffLineForKgPerProject", "TakeOffPerProjectId");
            CreateIndex("Construction.TakeOffLineForKgPerProject", "UnitRateId");
            CreateIndex("Construction.ProjectSubContract", "ProjectInformationId");
            CreateIndex("Schedule.Schedule", "GanttTask1_ID");
            CreateIndex("Schedule.Schedule", "ProjectInformationId");
            CreateIndex("EXCEED.ProjectTaskSchedule", "ProjectInformationId");
            CreateIndex("Construction.ProjectTakeOffLineForKg", "ProjectTakeOffId");
            CreateIndex("Construction.ProjectTakeOffLineForKg", "UnitRateId");
            CreateIndex("Construction.ProjectTakeOffLine", "ProjectTakeOffId");
            CreateIndex("Construction.ProjectMachineryRentHeader", "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstmnBoqSummary", "ProjectInformationId");
            CreateIndex("Construction.ProjectEstmnUnitFactory", "LookupId");
            CreateIndex("Construction.ProjectEstmnTransportCost", "LookupId");
            CreateIndex("Construction.ProjectEstmnRawData", "LookupId");
            CreateIndex("Construction.ProjectEstmnMaterial", "ItemCode");
            CreateIndex("Construction.ProjectEstmnMaterial", "ProjectEstimationWorkItemId");
            CreateIndex("Construction.ProjectEstmnIndirectLabour", "LookupId");
            CreateIndex("Construction.ProjectEstmnIndirectLabour", "ProjectEstimationWorkItemId");
            CreateIndex("Construction.ProjectEstmnEquipment", "ProjectEstimationWorkItemId");
            CreateIndex("Construction.ProjectEstimnWorkItem", "WorkCategoryId");
            CreateIndex("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId");
            CreateIndex("Construction.ProjectEstmnDirectLabour", "ProjectEstimationWorkItemId");
            CreateIndex("Construction.ProjectEstmnCoefficients", "LookupId");
            CreateIndex("Construction.ProjectEstimationBoqDetail", "ProjectEstimationBoqBreakdownId");
            CreateIndex("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstimationBoq", "LookupId");
            CreateIndex("Construction.ProjectContract", "ProjectInformationId");
            CreateIndex("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("Construction.Retention", "ProjectInformationId");
            CreateIndex("Construction.PaymentCertificate", "Project_ProjectInformationId");
            CreateIndex("Construction.MaterialUtilization", "ProjectInformationId");
            CreateIndex("Construction.LineDate", "PeriodId");
            CreateIndex("Construction.LineDate", "ProjectInformationId");
            CreateIndex("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("Construction.PaymenCertificatetLine", "ProjectInformationId");
            CreateIndex("Construction.EquipmentMobilization", "LookupId");
            CreateIndex("Construction.ProjectMaterial", "ItemCode");
            CreateIndex("Construction.ProjectMaterial", "WorkItemPerProjectId");
            CreateIndex("Construction.ProjectEquipment", "WorkItemPerProjectId");
            CreateIndex("Construction.ProjectDirectLabour", "ProjectEstimationIndexLabourId");
            CreateIndex("Construction.ProjectDirectLabour", "WorkItemPerProjectId");
            CreateIndex("Construction.WorkItemPerProject", "WorkCategoryId");
            CreateIndex("Construction.WorkItemPerProject", "ProjectInformationId");
            CreateIndex("Construction.ProjectWorkBreakDown", "ProjectInformationId");
            CreateIndex("Construction.ProjectMaterialRequest", "Project_ProjectInformationId");
            CreateIndex("Construction.ProjectMaterialIssued", "Project_ProjectInformationId");
            CreateIndex("Construction.ProjectMaterialDelivery", "Project_ProjectInformationId");
            CreateIndex("Construction.ProjectHumanResourceUtilization", "ProjectInformationId");
            CreateIndex("Construction.ProjectHumanResourceUtilization", "EmployeeId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId");
            CreateIndex("Construction.ProjectEquipmentPerformance", "ProjectEquipmentUtilizationId");
            CreateIndex("Construction.ProjectEquipmentMobilization", "ProjectEquipmentUtilizationId");
            CreateIndex("Construction.ProjectEquipmentHour", "ProjectEquipmentUtilizationId");
            CreateIndex("Construction.EquipmentFuelconsumption", "ProjectEquipmentUtilizationId");
            CreateIndex("Construction.ProjectEquipmentUtilization", "ProjectInformationId");
            CreateIndex("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectDefineBudget", "ProjectInformationId");
            CreateIndex("Construction.ProjectConstraintsAndRisk", "ProjectInformationId");
            CreateIndex("Construction.ProjectCalander", "ProjectInformationId");
            CreateIndex("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("Construction.ProjectPaymenAdvance", "ProjectInformationId");
            AddForeignKey("Construction.TakeOffLinePerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject", "TakeOffPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.TakeOffLineForKgPerProject", "UnitRateId", "Construction.UnitRate", "UnitRateId", cascadeDelete: true);
            AddForeignKey("Construction.TakeOffLineForKgPerProject", "TakeOffPerProjectId", "Construction.TakeOffPerProject", "TakeOffPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectSubContract", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Schedule.Schedule", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Schedule.Schedule", "GanttTask1_ID", "Schedule.Schedule", "ID");
            AddForeignKey("EXCEED.ProjectTaskSchedule", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate", "UnitRateId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectTakeOffLineForKg", "ProjectTakeOffId", "Construction.ProjectTakeOff", "ProjectTakeOffId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOffId", "Construction.ProjectTakeOff", "ProjectTakeOffId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectMachineryRentHeader", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnUnitFactory", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnTransportCost", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnRawData", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId");
            AddForeignKey("Construction.ProjectEstimnWorkItem", "WorkCategoryId", "Construction.WorkCategory", "WorkCategoryId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnMaterial", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnMaterial", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnIndirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnIndirectLabour", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnEquipment", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem", "ProjectEstimationWorkItemId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour", "ProjectEstimationIndexLabourId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnCoefficients", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimationBoqDetail", "ProjectEstimationBoqBreakdownId", "Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationBoqBreakdownId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimationBoq", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectContract", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("Construction.Retention", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.MaterialUtilization", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.LineDate", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.LineDate", "PeriodId", "Common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Construction.PaymenCertificatetLine", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.EquipmentMobilization", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.WorkItemPerProject", "WorkCategoryId", "Construction.WorkCategory", "WorkCategoryId", cascadeDelete: true);
            AddForeignKey("Construction.WorkItemPerProject", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectMaterial", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectMaterial", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEquipment", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectDirectLabour", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour", "ProjectEstimationIndexLabourId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectWorkBreakDown", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectMaterialRequest", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.ProjectMaterialIssued", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.ProjectMaterialDelivery", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.ProjectHumanResourceUtilization", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectHumanResourceUtilization", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId", "Construction.WorkCategory", "WorkCategoryId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEquipmentUtilization", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEquipmentPerformance", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEquipmentMobilization", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEquipmentHour", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId", cascadeDelete: true);
            AddForeignKey("Construction.EquipmentFuelconsumption", "ProjectEquipmentUtilizationId", "Construction.ProjectEquipmentUtilization", "ProjectEquipmentUtilizationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectDefineBudget", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectConstraintsAndRisk", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectCalander", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
        }
    }
}
