namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleetbudget : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FleetManagement.EquipmentPlanandEvaluations", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.EquipmentPlanandEvaluations", new[] { "FleetsID" });
            RenameColumn(table: "FleetManagement.EquipmentPlanandEvaluations", name: "FleetsID", newName: "Fleets_FleetsID");
            CreateTable(
                "FleetManagement.AdvanceMeasure",
                c => new
                    {
                        AdvanceMeasureId = c.Guid(nullable: false),
                        Description = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvanceMeasureId);
            
            CreateTable(
                "FleetManagement.CannibalizationDetail",
                c => new
                    {
                        CannibalizationDetailId = c.Guid(nullable: false),
                        CannibalizationId = c.Guid(nullable: false),
                        Model = c.String(nullable: false),
                        UnitofMeasurement = c.String(),
                        PartName = c.String(nullable: false),
                        PartType = c.String(),
                        WHPRNo = c.String(),
                        Quantity = c.Int(nullable: false),
                        Status = c.String(),
                        TakenBy = c.String(),
                        Description = c.String(),
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
                .PrimaryKey(t => t.CannibalizationDetailId);
            
            CreateTable(
                "FleetManagement.DailyCareCategory",
                c => new
                    {
                        DailyCareCategoryId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyCareCategoryId);
            
            CreateTable(
                "FleetManagement.DailyCareDetailItem",
                c => new
                    {
                        DailyCareDetailItemId = c.Guid(nullable: false),
                        DailyCareDetailLogId = c.Guid(nullable: false),
                        Description = c.String(nullable: false),
                        Day = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyCareDetailItemId)
                .ForeignKey("FleetManagement.DailyCareDetailLog", t => t.DailyCareDetailLogId, cascadeDelete: true)
                .Index(t => t.DailyCareDetailLogId);
            
            CreateTable(
                "FleetManagement.DailyCareDetailLog",
                c => new
                    {
                        DailyCareDetailLogId = c.Guid(nullable: false),
                        DailyCareLogId = c.Guid(nullable: false),
                        DailyCareCategoryId = c.Guid(nullable: false),
                        DailyCareId = c.Guid(nullable: false),
                        Description = c.String(),
                        IsActive = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyCareDetailLogId)
                .ForeignKey("FleetManagement.DailyCare", t => t.DailyCareId, cascadeDelete: true)
                .ForeignKey("FleetManagement.DailyCareLog", t => t.DailyCareLogId, cascadeDelete: true)
                .Index(t => t.DailyCareLogId)
                .Index(t => t.DailyCareId);
            
            CreateTable(
                "FleetManagement.DailyCare",
                c => new
                    {
                        DailyCareId = c.Guid(nullable: false),
                        DailyCareCategoryId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyCareId)
                .ForeignKey("FleetManagement.DailyCareCategory", t => t.DailyCareCategoryId, cascadeDelete: true)
                .Index(t => t.DailyCareCategoryId);
            
            CreateTable(
                "FleetManagement.DailyCareLog",
                c => new
                    {
                        DailyCareLogId = c.Guid(nullable: false),
                        FleetsId = c.Guid(nullable: false),
                        Operator = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        ApprovalStatus = c.Int(nullable: false),
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
                .PrimaryKey(t => t.DailyCareLogId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsId, cascadeDelete: true)
                .Index(t => t.FleetsId);
            
            CreateTable(
                "FleetManagement.FailureReason",
                c => new
                    {
                        FailureReasonId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FailureReasonId);
            
            CreateTable(
                "FleetManagement.FleetInsuranceTypeDetail",
                c => new
                    {
                        FleetInsuranceTypeDetailId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceTypeDetailId);
            
            CreateTable(
                "FleetManagement.FleetInsuranceType",
                c => new
                    {
                        FleetInsuranceTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceTypeId);
            
            CreateTable(
                "FleetManagement.LogisticPlanDetail",
                c => new
                    {
                        LogisticPlanDetailId = c.Int(nullable: false, identity: true),
                        LogisticPlanId = c.Int(nullable: false),
                        Weeks = c.Int(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovalStatus = c.Int(nullable: false),
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
                        Fleets_FleetsID = c.Guid(),
                    })
                .PrimaryKey(t => t.LogisticPlanDetailId)
                .ForeignKey("FleetManagement.Fleets", t => t.Fleets_FleetsID)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("FleetManagement.Service", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.ItemCode)
                .Index(t => t.Fleets_FleetsID);
            
            CreateTable(
                "FleetManagement.LogisticPlan",
                c => new
                    {
                        LogisticPlanId = c.Int(nullable: false, identity: true),
                        EquipmentTypeId = c.Guid(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Model = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        Description = c.String(),
                        ApprovalStatus = c.Int(nullable: false),
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
                .PrimaryKey(t => t.LogisticPlanId)
                .ForeignKey("FleetManagement.EquipmentType", t => t.EquipmentTypeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EquipmentTypeId)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "FleetManagement.PMScheduleDetail",
                c => new
                    {
                        PMScheduleDetailId = c.Int(nullable: false, identity: true),
                        PMScheduleId = c.Int(nullable: false),
                        FleetsId = c.Guid(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Weeks = c.Int(nullable: false),
                        ServiceId = c.Guid(nullable: false),
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
                .PrimaryKey(t => t.PMScheduleDetailId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsId, cascadeDelete: true)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .ForeignKey("FleetManagement.Service", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.FleetsId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "FleetManagement.PMSchedule",
                c => new
                    {
                        PMScheduleId = c.Int(nullable: false, identity: true),
                        EquipmentTypeId = c.Guid(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Location = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PMScheduleId)
                .ForeignKey("FleetManagement.EquipmentType", t => t.EquipmentTypeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.EquipmentTypeId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "FleetManagement.ServiceCrewWorkLog",
                c => new
                    {
                        ServiceCrewWorkLogId = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        JobOrderDetailId = c.Guid(nullable: false),
                        MecanicId = c.Guid(nullable: false),
                        Date = c.Int(nullable: false),
                        WorkingHoursMorningFrom = c.DateTime(nullable: false),
                        WorkingHoursMorningto = c.DateTime(nullable: false),
                        WorkingHoursAfternoonFrom = c.DateTime(nullable: false),
                        WorkingHoursAfternoonTo = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.ServiceCrewWorkLogId)
                .ForeignKey("FleetManagement.JobOrderDetail", t => t.JobOrderDetailId, cascadeDelete: true)
                .ForeignKey("FleetManagement.MechanicCrew", t => t.MecanicId, cascadeDelete: true)
                .Index(t => t.JobOrderDetailId)
                .Index(t => t.MecanicId);
            
            AddColumn("FleetManagement.Fleets", "ReadingType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "MinFuelStandard", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Fleets", "MaxFuelStandard", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Fleets", "LastMaintenanceOdometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.Operator", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.Cannibalization", "IsOperational", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsBreakDown", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsScrap", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsPoor", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsFromOperational", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsFromBreakDown", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsFromScrap", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsFromPoor", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsPermanent", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsThirdOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "ThirdOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.Cannibalization", "ThirdOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.Cannibalization", "IsFourthSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "FourthSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.Cannibalization", "FourthSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.Cannibalization", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.Cannibalization", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.Cannibalization", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "SecondSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.Cannibalization", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.CheckListLog", "Type", c => c.Int(nullable: false));
            AddColumn("FleetManagement.CheckListLog", "IsAccessory", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.CheckList", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "Odometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentLocation", "Odometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.Issue", "JobOrderNo", c => c.String());
            AddColumn("FleetManagement.JobCauses", "Name", c => c.String(nullable: false));
            AddColumn("FleetManagement.JobOrder", "FailureReason", c => c.String());
            AddColumn("FleetManagement.JobOrder", "AdvanceMeasure", c => c.String());
            AddColumn("FleetManagement.JobOrder", "IsPM", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "JobCausesId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "Odometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Service", "RepairTypeID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.Service", "ServiceCode", c => c.String());
            AddColumn("FleetManagement.JobRequest", "IsPM", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobRequest", "LastKmHr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.JobStoreRequisition", "JobDetailId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.MaintenaceInspectionLog", "IsAccessory", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.MaintenanceTool", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MechanicCrew", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TimeSheet", "FinalKMHrReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.EquipmentPlanandEvaluations", "Fleets_FleetsID", c => c.Guid());
            AlterColumn("FleetManagement.EquipmentType", "Description", c => c.String());
            AlterColumn("FleetManagement.EquipmentType", "EquipmentTypeName", c => c.String(nullable: false));
            AlterColumn("CRM.ReceiveMethod", "Name", c => c.String(nullable: false));
            CreateIndex("FleetManagement.EquipmentPlanandEvaluations", "Fleets_FleetsID");
            CreateIndex("FleetManagement.CheckList", "EquipmentTypeId");
            CreateIndex("FleetManagement.JobOrder", "FleetsID");
            CreateIndex("FleetManagement.JobOrderDetail", "JobCausesId");
            CreateIndex("FleetManagement.Service", "RepairTypeID");
            CreateIndex("FleetManagement.Service", "SectionId");
            AddForeignKey("FleetManagement.CheckList", "EquipmentTypeId", "FleetManagement.EquipmentIdentity", "EquipmentIdentityID", cascadeDelete: true);
            AddForeignKey("FleetManagement.JobOrder", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("FleetManagement.JobOrderDetail", "JobCausesId", "FleetManagement.JobCauses", "JobCausesId", cascadeDelete: true);
            AddForeignKey("FleetManagement.Service", "RepairTypeID", "FleetManagement.RepairType", "RepairTypeID", cascadeDelete: true);
            AddForeignKey("FleetManagement.Service", "SectionId", "FleetManagement.WorkUnitSection", "WorkUnitSectionId", cascadeDelete: true);
            AddForeignKey("FleetManagement.EquipmentPlanandEvaluations", "Fleets_FleetsID", "FleetManagement.Fleets", "FleetsID");
            DropColumn("FleetManagement.Fleets", "FuelStandard");
            DropColumn("FleetManagement.FleetInsurance", "InsuranceType");
            DropColumn("FleetManagement.Cannibalization", "Model");
            DropColumn("FleetManagement.Cannibalization", "UnitofMeasurement");
            DropColumn("FleetManagement.Cannibalization", "PartName");
            DropColumn("FleetManagement.Cannibalization", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.Cannibalization", "Quantity", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "PartName", c => c.String(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "UnitofMeasurement", c => c.String(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "Model", c => c.String(nullable: false));
            AddColumn("FleetManagement.FleetInsurance", "InsuranceType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "FuelStandard", c => c.Int(nullable: false));
            DropForeignKey("FleetManagement.EquipmentPlanandEvaluations", "Fleets_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.ServiceCrewWorkLog", "MecanicId", "FleetManagement.MechanicCrew");
            DropForeignKey("FleetManagement.ServiceCrewWorkLog", "JobOrderDetailId", "FleetManagement.JobOrderDetail");
            DropForeignKey("FleetManagement.PMSchedule", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("FleetManagement.PMSchedule", "EquipmentTypeId", "FleetManagement.EquipmentType");
            DropForeignKey("FleetManagement.PMScheduleDetail", "ServiceId", "FleetManagement.Service");
            DropForeignKey("FleetManagement.PMScheduleDetail", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("FleetManagement.PMScheduleDetail", "FleetsId", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.LogisticPlan", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("FleetManagement.LogisticPlan", "EquipmentTypeId", "FleetManagement.EquipmentType");
            DropForeignKey("FleetManagement.LogisticPlanDetail", "ServiceId", "FleetManagement.Service");
            DropForeignKey("FleetManagement.LogisticPlanDetail", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("FleetManagement.LogisticPlanDetail", "Fleets_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.Service", "SectionId", "FleetManagement.WorkUnitSection");
            DropForeignKey("FleetManagement.Service", "RepairTypeID", "FleetManagement.RepairType");
            DropForeignKey("FleetManagement.JobOrderDetail", "JobCausesId", "FleetManagement.JobCauses");
            DropForeignKey("FleetManagement.JobOrder", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.DailyCareDetailItem", "DailyCareDetailLogId", "FleetManagement.DailyCareDetailLog");
            DropForeignKey("FleetManagement.DailyCareDetailLog", "DailyCareLogId", "FleetManagement.DailyCareLog");
            DropForeignKey("FleetManagement.DailyCareLog", "FleetsId", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.DailyCareDetailLog", "DailyCareId", "FleetManagement.DailyCare");
            DropForeignKey("FleetManagement.DailyCare", "DailyCareCategoryId", "FleetManagement.DailyCareCategory");
            DropForeignKey("FleetManagement.CheckList", "EquipmentTypeId", "FleetManagement.EquipmentIdentity");
            DropIndex("FleetManagement.ServiceCrewWorkLog", new[] { "MecanicId" });
            DropIndex("FleetManagement.ServiceCrewWorkLog", new[] { "JobOrderDetailId" });
            DropIndex("FleetManagement.PMSchedule", new[] { "GlFiscalYearId" });
            DropIndex("FleetManagement.PMSchedule", new[] { "EquipmentTypeId" });
            DropIndex("FleetManagement.PMScheduleDetail", new[] { "ServiceId" });
            DropIndex("FleetManagement.PMScheduleDetail", new[] { "GLPeriodId" });
            DropIndex("FleetManagement.PMScheduleDetail", new[] { "FleetsId" });
            DropIndex("FleetManagement.LogisticPlan", new[] { "GLPeriodId" });
            DropIndex("FleetManagement.LogisticPlan", new[] { "EquipmentTypeId" });
            DropIndex("FleetManagement.LogisticPlanDetail", new[] { "Fleets_FleetsID" });
            DropIndex("FleetManagement.LogisticPlanDetail", new[] { "ItemCode" });
            DropIndex("FleetManagement.LogisticPlanDetail", new[] { "ServiceId" });
            DropIndex("FleetManagement.Service", new[] { "SectionId" });
            DropIndex("FleetManagement.Service", new[] { "RepairTypeID" });
            DropIndex("FleetManagement.JobOrderDetail", new[] { "JobCausesId" });
            DropIndex("FleetManagement.JobOrder", new[] { "FleetsID" });
            DropIndex("FleetManagement.DailyCareLog", new[] { "FleetsId" });
            DropIndex("FleetManagement.DailyCare", new[] { "DailyCareCategoryId" });
            DropIndex("FleetManagement.DailyCareDetailLog", new[] { "DailyCareId" });
            DropIndex("FleetManagement.DailyCareDetailLog", new[] { "DailyCareLogId" });
            DropIndex("FleetManagement.DailyCareDetailItem", new[] { "DailyCareDetailLogId" });
            DropIndex("FleetManagement.CheckList", new[] { "EquipmentTypeId" });
            DropIndex("FleetManagement.EquipmentPlanandEvaluations", new[] { "Fleets_FleetsID" });
            AlterColumn("CRM.ReceiveMethod", "Name", c => c.String());
            AlterColumn("FleetManagement.EquipmentType", "EquipmentTypeName", c => c.String());
            AlterColumn("FleetManagement.EquipmentType", "Description", c => c.String(nullable: false));
            AlterColumn("FleetManagement.EquipmentPlanandEvaluations", "Fleets_FleetsID", c => c.Guid(nullable: false));
            DropColumn("FleetManagement.TimeSheet", "FinalKMHrReading");
            DropColumn("FleetManagement.MechanicCrew", "EmployeeId");
            DropColumn("FleetManagement.MaintenanceTool", "Price");
            DropColumn("FleetManagement.MaintenaceInspectionLog", "IsAccessory");
            DropColumn("FleetManagement.JobStoreRequisition", "JobDetailId");
            DropColumn("FleetManagement.JobRequest", "LastKmHr");
            DropColumn("FleetManagement.JobRequest", "IsPM");
            DropColumn("FleetManagement.Service", "ServiceCode");
            DropColumn("FleetManagement.Service", "RepairTypeID");
            DropColumn("FleetManagement.JobOrderDetail", "Odometer");
            DropColumn("FleetManagement.JobOrderDetail", "JobCausesId");
            DropColumn("FleetManagement.JobOrder", "IsPM");
            DropColumn("FleetManagement.JobOrder", "AdvanceMeasure");
            DropColumn("FleetManagement.JobOrder", "FailureReason");
            DropColumn("FleetManagement.JobCauses", "Name");
            DropColumn("EXCEED.Issue", "JobOrderNo");
            DropColumn("FleetManagement.EquipmentLocation", "Odometer");
            DropColumn("FleetManagement.DriverAssignment", "Odometer");
            DropColumn("FleetManagement.CheckList", "EquipmentTypeId");
            DropColumn("FleetManagement.CheckListLog", "IsAccessory");
            DropColumn("FleetManagement.CheckListLog", "Type");
            DropColumn("FleetManagement.Cannibalization", "SecondSendForApprovalTime");
            DropColumn("FleetManagement.Cannibalization", "SecondSendForApprovalBy");
            DropColumn("FleetManagement.Cannibalization", "IsSecondSendForApproval");
            DropColumn("FleetManagement.Cannibalization", "SecondOnlineApprovedTime");
            DropColumn("FleetManagement.Cannibalization", "SecondOnlineApprovedBy");
            DropColumn("FleetManagement.Cannibalization", "IsSecondOnlineApproved");
            DropColumn("FleetManagement.Cannibalization", "FourthSendForApprovalTime");
            DropColumn("FleetManagement.Cannibalization", "FourthSendForApprovalBy");
            DropColumn("FleetManagement.Cannibalization", "IsFourthSendForApproval");
            DropColumn("FleetManagement.Cannibalization", "ThirdOnlineApprovedTime");
            DropColumn("FleetManagement.Cannibalization", "ThirdOnlineApprovedBy");
            DropColumn("FleetManagement.Cannibalization", "IsThirdOnlineApproved");
            DropColumn("FleetManagement.Cannibalization", "IsPermanent");
            DropColumn("FleetManagement.Cannibalization", "IsFromPoor");
            DropColumn("FleetManagement.Cannibalization", "IsFromScrap");
            DropColumn("FleetManagement.Cannibalization", "IsFromBreakDown");
            DropColumn("FleetManagement.Cannibalization", "IsFromOperational");
            DropColumn("FleetManagement.Cannibalization", "IsPoor");
            DropColumn("FleetManagement.Cannibalization", "IsScrap");
            DropColumn("FleetManagement.Cannibalization", "IsBreakDown");
            DropColumn("FleetManagement.Cannibalization", "IsOperational");
            DropColumn("FleetManagement.Cannibalization", "Num");
            DropColumn("FleetManagement.Operator", "EmployeeId");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentTypeId");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentCategoryId");
            DropColumn("FleetManagement.Fleets", "LastMaintenanceOdometer");
            DropColumn("FleetManagement.Fleets", "MaxFuelStandard");
            DropColumn("FleetManagement.Fleets", "MinFuelStandard");
            DropColumn("FleetManagement.Fleets", "ReadingType");
            DropTable("FleetManagement.ServiceCrewWorkLog");
            DropTable("FleetManagement.PMSchedule");
            DropTable("FleetManagement.PMScheduleDetail");
            DropTable("FleetManagement.LogisticPlan");
            DropTable("FleetManagement.LogisticPlanDetail");
            DropTable("FleetManagement.FleetInsuranceType");
            DropTable("FleetManagement.FleetInsuranceTypeDetail");
            DropTable("FleetManagement.FailureReason");
            DropTable("FleetManagement.DailyCareLog");
            DropTable("FleetManagement.DailyCare");
            DropTable("FleetManagement.DailyCareDetailLog");
            DropTable("FleetManagement.DailyCareDetailItem");
            DropTable("FleetManagement.DailyCareCategory");
            DropTable("FleetManagement.CannibalizationDetail");
            DropTable("FleetManagement.AdvanceMeasure");
            RenameColumn(table: "FleetManagement.EquipmentPlanandEvaluations", name: "Fleets_FleetsID", newName: "FleetsID");
            CreateIndex("FleetManagement.EquipmentPlanandEvaluations", "FleetsID");
            AddForeignKey("FleetManagement.EquipmentPlanandEvaluations", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
        }
    }
}
