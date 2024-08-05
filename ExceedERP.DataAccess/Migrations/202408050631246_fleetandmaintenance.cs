namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetandmaintenance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.EquipmentRate",
                c => new
                    {
                        EquipmentRateId = c.Int(nullable: false, identity: true),
                        EquipmentCatagoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        ManufacturingYearFrom = c.DateTime(nullable: false),
                        ManufacturingYearTo = c.DateTime(nullable: false),
                        UoM = c.String(),
                        CapacityFrom = c.Decimal(precision: 18, scale: 2),
                        CapacityTo = c.Decimal(precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetRentalRateId = c.Int(nullable: false),
                        DistanceFrom = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DistanceTo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DumpTruckTypeOfWork = c.Int(nullable: false),
                        SprayingType = c.Int(nullable: false),
                        Remark = c.String(),
                        RoadType = c.Int(nullable: false),
                        NumberOfSeatsFrom = c.Int(nullable: false),
                        NumberOfSeatsTo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EquipmentRateId)
                .ForeignKey("FleetManagement.FleetRentalRate", t => t.FleetRentalRateId, cascadeDelete: true)
                .Index(t => t.FleetRentalRateId);
            
            CreateTable(
                "FleetManagement.FleetRentalRate",
                c => new
                    {
                        FleetRentalRateId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Year = c.DateTime(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FleetRentalRateId);
            
            CreateTable(
                "Maintenance.MaintenancePlanDetail",
                c => new
                    {
                        MaintenancePlanDetailId = c.Int(nullable: false, identity: true),
                        GlPeriodId = c.Int(nullable: false),
                        CMQuantity = c.Int(nullable: false),
                        PMQuantity = c.Int(nullable: false),
                        CMCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PMCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenancePlanId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenancePlanDetailId)
                .ForeignKey("Maintenance.MaintenancePlan", t => t.MaintenancePlanId, cascadeDelete: true)
                .Index(t => t.MaintenancePlanId);
            
            CreateTable(
                "Maintenance.MaintenancePlan",
                c => new
                    {
                        MaintenancePlanId = c.Int(nullable: false, identity: true),
                        EquipmentCategoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        TotalQuantity = c.Int(nullable: false),
                        AverageAvailableQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageMaintenanceCostPM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageMaintenanceCostCM = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.MaintenancePlanId);
            
            AddColumn("FleetManagement.Fleets", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentCategoryId", c => c.Guid());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentTypeId", c => c.Guid());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentNameId", c => c.Guid());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "Downhour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheetDetail", "DumpTruckTypeOfWork", c => c.Int(nullable: false));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentCategoryId", c => c.Guid());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentTypeId", c => c.Guid());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentNameId", c => c.Guid());
            AddColumn("Maintenance.ExternalServiceCenterPayment", "GlTaxRateId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetAnnualPlanDetail", "GlPeriodId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetAnnualPlanDetail", "DateFrom", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.FleetAnnualPlanDetail", "DateTo", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.FleetAnnualPlan", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetAnnualPlan", "EquipmentGrade", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "Description", c => c.String());
            AddColumn("Maintenance.JobOrderServiceCenterDetail", "JobOrderDetailId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.LightVehicleTimesheet", "EquipmentCategoryId", c => c.Guid());
            AddColumn("FleetManagement.LightVehicleTimesheet", "EquipmentTypeId", c => c.Guid());
            AddColumn("FleetManagement.LightVehicleTimesheet", "EquipmentNameId", c => c.Guid());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentCategoryId", c => c.Guid());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentTypeId", c => c.Guid());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentNameId", c => c.Guid());
            AddColumn("EXCEED.UserBusinessUnitMap", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.WagonDrillTimesheet", "EquipmentCategoryId", c => c.Guid());
            AddColumn("FleetManagement.WagonDrillTimesheet", "EquipmentTypeId", c => c.Guid());
            AddColumn("FleetManagement.WagonDrillTimesheet", "EquipmentNameId", c => c.Guid());
            AlterColumn("EXCEED.UserBusinessUnitMap", "BusinessUnitId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("Maintenance.MaintenancePlanDetail", "MaintenancePlanId", "Maintenance.MaintenancePlan");
            DropForeignKey("FleetManagement.EquipmentRate", "FleetRentalRateId", "FleetManagement.FleetRentalRate");
            DropIndex("Maintenance.MaintenancePlanDetail", new[] { "MaintenancePlanId" });
            DropIndex("FleetManagement.EquipmentRate", new[] { "FleetRentalRateId" });
            AlterColumn("EXCEED.UserBusinessUnitMap", "BusinessUnitId", c => c.String());
            DropColumn("FleetManagement.WagonDrillTimesheet", "EquipmentNameId");
            DropColumn("FleetManagement.WagonDrillTimesheet", "EquipmentTypeId");
            DropColumn("FleetManagement.WagonDrillTimesheet", "EquipmentCategoryId");
            DropColumn("EXCEED.UserBusinessUnitMap", "IsDefault");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentNameId");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentTypeId");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "EquipmentCategoryId");
            DropColumn("FleetManagement.LightVehicleTimesheet", "EquipmentNameId");
            DropColumn("FleetManagement.LightVehicleTimesheet", "EquipmentTypeId");
            DropColumn("FleetManagement.LightVehicleTimesheet", "EquipmentCategoryId");
            DropColumn("Maintenance.JobOrderServiceCenterDetail", "JobOrderDetailId");
            DropColumn("Maintenance.JobOrder", "Description");
            DropColumn("FleetManagement.FleetAnnualPlan", "EquipmentGrade");
            DropColumn("FleetManagement.FleetAnnualPlan", "GlFiscalYearId");
            DropColumn("FleetManagement.FleetAnnualPlanDetail", "DateTo");
            DropColumn("FleetManagement.FleetAnnualPlanDetail", "DateFrom");
            DropColumn("FleetManagement.FleetAnnualPlanDetail", "GlPeriodId");
            DropColumn("Maintenance.ExternalServiceCenterPayment", "GlTaxRateId");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentNameId");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentTypeId");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "EquipmentCategoryId");
            DropColumn("FleetManagement.DumpTruckTimeSheetDetail", "DumpTruckTypeOfWork");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "Downhour");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentNameId");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentTypeId");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "EquipmentCategoryId");
            DropColumn("FleetManagement.Fleets", "BranchId");
            DropTable("Maintenance.MaintenancePlan");
            DropTable("Maintenance.MaintenancePlanDetail");
            DropTable("FleetManagement.FleetRentalRate");
            DropTable("FleetManagement.EquipmentRate");
        }
    }
}
