namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FleetModelChangesMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FleetManagement.SitesVehicleTransfer", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.SitesVehicleTransfer", new[] { "FleetsID" });
            CreateTable(
                "FleetManagement.DriverAssignmentDocument",
                c => new
                    {
                        DriverAssignmentDocumentId = c.Int(nullable: false, identity: true),
                        DriverAssignmentId = c.String(),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.DriverAssignmentDocumentId);
            
            CreateTable(
                "FleetManagement.FuelRequestDetail",
                c => new
                    {
                        FuelRequestDetailId = c.Int(nullable: false, identity: true),
                        FuelRequestId = c.Int(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        PlateNo = c.String(),
                        VehicleTypeId = c.Guid(),
                        VehicleType = c.String(),
                        SideNo = c.String(),
                        OperatorId = c.Guid(),
                        Driver = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentName = c.String(),
                        DivisionId = c.Int(),
                        RecieptNo = c.String(),
                        DivisionName = c.String(),
                        OpeningKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClosingKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DistanceKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumedL = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelFilledL = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RequestDate = c.DateTime(nullable: false),
                        ConsumptionKMPerL = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationalDays = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelOwnerName = c.String(),
                        FuelOwnerPhoneNumber = c.String(),
                        IsFinanceApproved = c.Boolean(nullable: false),
                        FinanceApprovedBy = c.String(),
                        FinanceApprovedTime = c.DateTime(),
                        FinanceRejectionReason = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FuelRequestDetailId);
            
            CreateTable(
                "FleetManagement.FuelRequestDocument",
                c => new
                    {
                        FuelRequestDocumentId = c.Int(nullable: false, identity: true),
                        FuelRequestId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FuelRequestDocumentId);
            
            CreateTable(
                "FleetManagement.FuelRequest",
                c => new
                    {
                        FuelRequestId = c.Int(nullable: false, identity: true),
                        RequestorId = c.Int(nullable: false),
                        RequestorName = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentName = c.String(),
                        DivisionId = c.Int(),
                        DivisionName = c.String(),
                        RecieptNo = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelOwnerName = c.String(),
                        PhoneNumber = c.String(),
                        IsFinanceApproved = c.Boolean(nullable: false),
                        FinanceApprovedBy = c.String(),
                        FinanceApprovedTime = c.DateTime(),
                        FinanceRejectionReason = c.String(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
                        ForwardedTime = c.DateTime(),
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
                .PrimaryKey(t => t.FuelRequestId);
            
            CreateTable(
                "FleetManagement.VehicleDistributionSecondApprovalSetting",
                c => new
                    {
                        VehicleDistributionSecondApprovalSettingId = c.Int(nullable: false, identity: true),
                        NeedsSecondApproval = c.Boolean(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleDistributionSecondApprovalSettingId);
            
            CreateTable(
                "FleetManagement.VehicleDistributionSecondValidation",
                c => new
                    {
                        VehicleDistributionSecondValidationId = c.Int(nullable: false, identity: true),
                        VehicleDistributionID = c.String(),
                        Status = c.String(),
                        remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleDistributionSecondValidationId);
            
            AddColumn("FleetManagement.Accident", "CoveredByOtherBodies", c => c.String());
            AddColumn("FleetManagement.Fleets", "RentalInspectionRef", c => c.String());
            AddColumn("FleetManagement.Fleets", "IsExcavator", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "SumInsuBirr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "OnDamageInsLocation", c => c.String());
            AddColumn("FleetManagement.FleetInsurances", "OnDamageExpDate", c => c.DateTime());
            AddColumn("FleetManagement.FleetInsurances", "ThirdPartyInsuranceLocation", c => c.String());
            AddColumn("FleetManagement.FleetInsurances", "ThirdPartyInsuranceExpDate", c => c.DateTime());
            AddColumn("FleetManagement.FleetInsurances", "PVTInsurance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "InlandTransitOD", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "InlandTransitPVT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "AssignedTime", c => c.DateTime());
            AddColumn("FleetManagement.AssignedEquipment", "ReturnedTime", c => c.DateTime());
            AddColumn("FleetManagement.DriverAssignment", "DepartmentId", c => c.Int());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "ExcavatorWorkType", c => c.Int());
            AddColumn("FleetManagement.RentalEquipmentAdvancePayment", "IsCreatedAfterUpdate", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentalEquipmentAdvancePayment", "IsSummaryCalculated", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentalEquipmentAdvancePayment", "DateFrom", c => c.DateTime());
            AddColumn("FleetManagement.RentalEquipmentAdvancePayment", "DateTo", c => c.DateTime());
            AddColumn("FleetManagement.RentedEquipmentDailyOperation", "IsDetailUpdated", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "AdvancePaymentId", c => c.Int());
            AddColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "DateFrom", c => c.DateTime());
            AddColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "DateTo", c => c.DateTime());
            AddColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "IsAdvanceSummary", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "DepartmentId", c => c.Int());
            AddColumn("FleetManagement.RentFromExternalEquipment", "DayType", c => c.Int());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestNumber", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsRequestChecked", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestCheckedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestCheckedDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsRequestApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestApprovedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestApprovedDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsRequestReceived", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestReceivedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "RequestReceivedDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsDistributionChecked", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "DistributionCheckedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "DistributionCheckedDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsDistributionApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "DistributionApprovedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "DistributionApprovedDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsAccepted", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "AcceptedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "AcceptedTime", c => c.DateTime());
            AddColumn("FleetManagement.VehicleDemand", "RequestTime", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "Location", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.VehicleDistribution", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "SecondSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillDailyOperation", "IsDetailUpdated", c => c.Boolean(nullable: false));
            AlterColumn("FleetManagement.Fleets", "PlannerID", c => c.Guid());
            AlterColumn("FleetManagement.SitesVehicleTransfer", "FleetsID", c => c.Guid());
            AlterColumn("FleetManagement.SitesVehicleTransfer", "OperatorId", c => c.Guid());
            CreateIndex("FleetManagement.SitesVehicleTransfer", "FleetsID");
            AddForeignKey("FleetManagement.SitesVehicleTransfer", "FleetsID", "FleetManagement.Fleets", "FleetsID");
        }
        
        public override void Down()
        {
            DropForeignKey("FleetManagement.SitesVehicleTransfer", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.SitesVehicleTransfer", new[] { "FleetsID" });
            AlterColumn("FleetManagement.SitesVehicleTransfer", "OperatorId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.SitesVehicleTransfer", "FleetsID", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Fleets", "PlannerID", c => c.Guid(nullable: false));
            DropColumn("FleetManagement.WagonDrillDailyOperation", "IsDetailUpdated");
            DropColumn("FleetManagement.VehicleDistribution", "SecondSendForApprovalTime");
            DropColumn("FleetManagement.VehicleDistribution", "SecondSendForApprovalBy");
            DropColumn("FleetManagement.VehicleDistribution", "IsSecondSendForApproval");
            DropColumn("FleetManagement.VehicleDistribution", "SecondOnlineApprovedTime");
            DropColumn("FleetManagement.VehicleDistribution", "SecondOnlineApprovedBy");
            DropColumn("FleetManagement.VehicleDistribution", "IsSecondOnlineApproved");
            DropColumn("FleetManagement.VehicleDemand", "Location");
            DropColumn("FleetManagement.VehicleDemand", "RequestTime");
            DropColumn("FleetManagement.SitesVehicleTransfer", "AcceptedTime");
            DropColumn("FleetManagement.SitesVehicleTransfer", "AcceptedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsAccepted");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DistributionApprovedDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DistributionApprovedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsDistributionApproved");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DistributionCheckedDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DistributionCheckedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsDistributionChecked");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestReceivedDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestReceivedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsRequestReceived");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestApprovedDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestApprovedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsRequestApproved");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestCheckedDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestCheckedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsRequestChecked");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "RequestNumber");
            DropColumn("FleetManagement.RentFromExternalEquipment", "DayType");
            DropColumn("FleetManagement.RentFromExternal", "DepartmentId");
            DropColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "IsAdvanceSummary");
            DropColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "DateTo");
            DropColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "DateFrom");
            DropColumn("FleetManagement.RentedEquipmentMonthlyPaymentSummary", "AdvancePaymentId");
            DropColumn("FleetManagement.RentedEquipmentDailyOperation", "IsDetailUpdated");
            DropColumn("FleetManagement.RentalEquipmentAdvancePayment", "DateTo");
            DropColumn("FleetManagement.RentalEquipmentAdvancePayment", "DateFrom");
            DropColumn("FleetManagement.RentalEquipmentAdvancePayment", "IsSummaryCalculated");
            DropColumn("FleetManagement.RentalEquipmentAdvancePayment", "IsCreatedAfterUpdate");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "ExcavatorWorkType");
            DropColumn("FleetManagement.DriverAssignment", "DepartmentId");
            DropColumn("FleetManagement.AssignedEquipment", "ReturnedTime");
            DropColumn("FleetManagement.AssignedEquipment", "AssignedTime");
            DropColumn("FleetManagement.FleetInsurances", "InlandTransitPVT");
            DropColumn("FleetManagement.FleetInsurances", "InlandTransitOD");
            DropColumn("FleetManagement.FleetInsurances", "PVTInsurance");
            DropColumn("FleetManagement.FleetInsurances", "ThirdPartyInsuranceExpDate");
            DropColumn("FleetManagement.FleetInsurances", "ThirdPartyInsuranceLocation");
            DropColumn("FleetManagement.FleetInsurances", "OnDamageExpDate");
            DropColumn("FleetManagement.FleetInsurances", "OnDamageInsLocation");
            DropColumn("FleetManagement.FleetInsurances", "SumInsuBirr");
            DropColumn("FleetManagement.Fleets", "IsExcavator");
            DropColumn("FleetManagement.Fleets", "RentalInspectionRef");
            DropColumn("FleetManagement.Accident", "CoveredByOtherBodies");
            DropTable("FleetManagement.VehicleDistributionSecondValidation");
            DropTable("FleetManagement.VehicleDistributionSecondApprovalSetting");
            DropTable("FleetManagement.FuelRequest");
            DropTable("FleetManagement.FuelRequestDocument");
            DropTable("FleetManagement.FuelRequestDetail");
            DropTable("FleetManagement.DriverAssignmentDocument");
            CreateIndex("FleetManagement.SitesVehicleTransfer", "FleetsID");
            AddForeignKey("FleetManagement.SitesVehicleTransfer", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
        }
    }
}
