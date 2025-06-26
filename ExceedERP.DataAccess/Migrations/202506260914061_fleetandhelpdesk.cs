namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetandhelpdesk : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.MaterialsUsed", newName: "MaterialsUseds");
            MoveTable(name: "EXCEED.MaterialItems", newSchema: "InfoDesk");
            MoveTable(name: "EXCEED.MaterialsUseds", newSchema: "InfoDesk");
            DropForeignKey("FleetManagement.VehicleDemandApproval", "VehicleDemandId", "FleetManagement.VehicleDemand");
            DropIndex("FleetManagement.AnnualInventoryInspectionApproval", new[] { "AnnualEquipmentInventoryInspectionId" });
            DropIndex("FleetManagement.VehicleDemandApproval", new[] { "VehicleDemandId" });
            CreateTable(
                "EXCEED.BoloRegistrationDocument",
                c => new
                    {
                        BoloRegistrationDocumentId = c.Int(nullable: false, identity: true),
                        FleetBoloRegistrationId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.BoloRegistrationDocumentId);
            
            CreateTable(
                "InfoDesk.ChangeApprovals",
                c => new
                    {
                        ChangeApprovalId = c.Int(nullable: false, identity: true),
                        ApproverId = c.Int(),
                        IsApproved = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        ChangeRequestId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ChangeApprovalId)
                .ForeignKey("InfoDesk.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .Index(t => t.ChangeRequestId);
            
            CreateTable(
                "InfoDesk.ChangeRequests",
                c => new
                    {
                        ChangeRequestId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        ChangeType = c.Int(nullable: false),
                        ChangeTypeDetail = c.String(),
                        ChangeStatus = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        PlannedStartDate = c.DateTime(),
                        PlannedEndDate = c.DateTime(),
                        RequestorId = c.Int(),
                        ApproverId = c.Int(),
                        ImplementationTeam = c.String(),
                        RiskAssessment = c.String(),
                        RollbackPlan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImpactAnalysis = c.String(),
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
                .PrimaryKey(t => t.ChangeRequestId);
            
            CreateTable(
                "InfoDesk.ChangeTasks",
                c => new
                    {
                        ChangeTaskId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        OrgStructureId = c.Int(),
                        EmployeeId = c.Int(),
                        Responsiblity = c.String(),
                        IsCompleted = c.Boolean(nullable: false),
                        CompletionDate = c.DateTime(),
                        ChangeRequestId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ChangeTaskId)
                .ForeignKey("InfoDesk.ChangeRequests", t => t.ChangeRequestId, cascadeDelete: true)
                .Index(t => t.ChangeRequestId);
            
            CreateTable(
                "InfoDesk.ChatSessions",
                c => new
                    {
                        ChatSessionId = c.Int(nullable: false, identity: true),
                        User1Id = c.String(nullable: false),
                        User2Id = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ChatSessionId);
            
            CreateTable(
                "EXCEED.FleetBoloRegistrationHistory",
                c => new
                    {
                        FleetBoloRegistrationHistoryId = c.Int(nullable: false, identity: true),
                        FleetBoloRegistrationId = c.Int(nullable: false),
                        EquipmentCategoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        EquipmentNameId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        BoloRegistrationDate = c.DateTime(nullable: false),
                        BoloExpireDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetBoloRegistrationHistoryId);
            
            CreateTable(
                "FleetManagement.FleetInsuranceDocument",
                c => new
                    {
                        FleetInsuranceDocumentId = c.Int(nullable: false, identity: true),
                        FleetInsuranceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceDocumentId);
            
            CreateTable(
                "EXCEED.FleetInsuranceHistory",
                c => new
                    {
                        FleetInsuranceHistoryId = c.Int(nullable: false, identity: true),
                        FleetInsuranceId = c.Int(nullable: false),
                        EquipmentCategoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        EquipmentNameId = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        InsuranceCompanyId = c.Int(nullable: false),
                        PremiumCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(),
                        ExpireDate = c.DateTime(),
                        Attachment = c.String(),
                        Description = c.String(),
                        PolicyNo = c.String(),
                        HazardOfExpiryDate = c.DateTime(),
                        ThirdParty = c.String(),
                        InsuranceType = c.Int(nullable: false),
                        ThirdPartyInsurance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ComprehensiveInsurance = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.FleetInsuranceHistoryId);
            
            CreateTable(
                "FleetManagement.HeavyORDumpTruck",
                c => new
                    {
                        HeavyORDumpTruckId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HeavyORDumpTruckId);
            
            CreateTable(
                "FleetManagement.InspectionStatusKeys",
                c => new
                    {
                        InspectionStatusKeyId = c.Int(nullable: false, identity: true),
                        StatusKey = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InspectionStatusKeyId);
            
            CreateTable(
                "InfoDesk.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        SenderId = c.String(nullable: false),
                        ReceiverId = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        ChatSessionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("InfoDesk.ChatSessions", t => t.ChatSessionId, cascadeDelete: true)
                .Index(t => t.ChatSessionId);
            
            CreateTable(
                "FleetManagement.RateOfRoadORLoad",
                c => new
                    {
                        RateOfRoadORLoadId = c.Int(nullable: false, identity: true),
                        RoadOrLoadId = c.Int(nullable: false),
                        Name = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RateOfRoadORLoadId);
            
            CreateTable(
                "FleetManagement.RentFromExternalEquipmentRate",
                c => new
                    {
                        RentFromExternalEquipmentRateId = c.Int(nullable: false, identity: true),
                        RentFromExternalEquipmentId = c.Int(nullable: false),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManufacturingYearFrom = c.DateTime(nullable: false),
                        ManufacturingYearTo = c.DateTime(nullable: false),
                        DistanceFrom = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DistanceTo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DumpTruckTypeOfWork = c.Int(),
                        RoadType = c.Int(),
                        RentType = c.Int(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentFromExternalEquipmentRateId);
            
            CreateTable(
                "FleetManagement.RoadORLoad",
                c => new
                    {
                        RoadOrLoadId = c.Int(nullable: false, identity: true),
                        HeavyORDumpTruckId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RoadOrLoadId);
            
            CreateTable(
                "FleetManagement.TimeSheetException",
                c => new
                    {
                        TimeSheetExceptionId = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        BranchId = c.String(),
                        TimeSheetSettingId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TimeSheetExceptionId);
            
            CreateTable(
                "FleetManagement.TimeSheetSetting",
                c => new
                    {
                        TimeSheetSettingId = c.Int(nullable: false, identity: true),
                        IsSet = c.Boolean(nullable: false),
                        SettedHours = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TimeSheetSettingId);
            
            CreateTable(
                "FleetManagement.TireIssueDetails",
                c => new
                    {
                        TireIssueDetailId = c.Int(nullable: false, identity: true),
                        TireIssueId = c.String(),
                        SerialNo = c.String(),
                        TireSize = c.String(),
                        IssueType = c.Int(nullable: false),
                        TireType = c.String(),
                        TireTube = c.String(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Brand = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TireIssueDetailId);
            
            CreateTable(
                "FleetManagement.RequestToInspection",
                c => new
                    {
                        RequestToInspectionId = c.Int(nullable: false, identity: true),
                        Remark = c.String(),
                        GoToInspection = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RequestToInspectionId);
            
            AddColumn("FleetManagement.Fleets", "OperatorId", c => c.Guid());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "Make", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "ModelNumber", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "SerialNumber", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "YearOfManufacturing", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EngineModel", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EngineSerailNumber", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "CountryOfOrigin", c => c.String());
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "Operator", c => c.String());
            AddColumn("FleetManagement.Fuel", "RequestedQty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.Operator", "EmployeeCode", c => c.String());
            AddColumn("FleetManagement.TireIssue", "IssueDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.TireIssue", "IssueReference", c => c.String());
            AddColumn("FleetManagement.TireIssue", "SerialNoB", c => c.String());
            AddColumn("FleetManagement.TireIssue", "SerialNoC", c => c.String());
            AddColumn("FleetManagement.TireIssue", "SerialNoD", c => c.String());
            AddColumn("FleetManagement.TireReturn", "IssueDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.TireReturn", "ReturnDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.TireReturn", "ReturnedKMReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelOrder", "BankAccount", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "ArrivalPlace", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "AsphaltKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "GravelKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "IsSendToKB", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "Handler", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "FixedAssetCategoryId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "FixedAssetSubCategoryId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "AssetRegistrationId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "ItemName", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "TagNo", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "PendingReason", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Handler", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "RequesterIPAddress", c => c.String());
            AddColumn("InfoDesk.MaterialItems", "CategoryId", c => c.String());
            AddColumn("InfoDesk.MaterialItems", "InventoryItemId", c => c.String());
            AddColumn("InfoDesk.TicketType_HelpDesk", "EstimatedCost", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.DriverAssignment", "OperatorId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "SecondOperatorId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "DownReason", c => c.String());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "OilConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentArrival", "ArrivalPlace", c => c.String());
            AddColumn("FleetManagement.EquipmentRate", "IdleRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentRate", "IsDump", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.EquipmentTyreLog", "InstalledDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.EquipmentTyreLog", "ReturnedDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.EquipmentTyreLog", "ReturnedKmReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequestDetail", "ExternalPaymentRequestJobId", c => c.Int(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "Paid", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedFrom", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedTo", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "CurrentApprovalStep", c => c.Int());
            AddColumn("Maintenance.ExternalServiceCenterPayment", "Invoice", c => c.String());
            AddColumn("FleetManagement.FleetBoloRegistration", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetBoloRegistration", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetBoloRegistration", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetLineCheckList", "SystemName", c => c.String(nullable: false));
            AddColumn("FleetManagement.FleetLineCheckList", "CheckList", c => c.String());
            AddColumn("InfoDesk.InfoDeskKnowledgeBases", "DocumentType", c => c.Int(nullable: false));
            AddColumn("InfoDesk.InfoDeskKnowledgeBases", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.InfoDeskKnowledgeBases", "OnlineApprovedBy", c => c.String());
            AddColumn("InfoDesk.InfoDeskKnowledgeBases", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.InspectionApproval", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.InspectionLog", "AnnualEquipmentInventoryInspectionID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.InspectionLog", "SystemName", c => c.String());
            AddColumn("FleetManagement.InspectionLog", "CheckList", c => c.String());
            AddColumn("FleetManagement.InspectionLog", "StatusKey", c => c.Int(nullable: false));
            AddColumn("FleetManagement.InspectionLog", "StatusKeyId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobCauses", "ShowOnReception", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobCauses", "FromReceptionCount", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "ToExternalJobOrderId", c => c.String());
            AddColumn("Maintenance.JobOrder", "ServiceCenterJobId", c => c.String());
            AddColumn("Maintenance.JobOrder", "KMReading", c => c.String());
            AddColumn("Maintenance.JobOrder", "GPSInfo", c => c.String());
            AddColumn("Maintenance.JobOrder", "IsSentToWorkShopTime", c => c.DateTime());
            AddColumn("Maintenance.JobOrderDetail", "ServiceGroupSettingId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "IsFromWorkShop", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "DescriptionOfJobs", c => c.String());
            AddColumn("Maintenance.JobOrderDetail", "IsFromWorkShopRejected", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "IsFromWorkShopApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.RepairType", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.JobRequests", "FromBranchId", c => c.String());
            AddColumn("Maintenance.JobRequests", "KMReading", c => c.String());
            AddColumn("Maintenance.JobRequests", "GPSInfo", c => c.String());
            AddColumn("Maintenance.JobRequests", "EstmatedCostAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.JobRequests", "IsSentBackToReception", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "CreateJobOrder", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("Maintenance.JobRequests", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("Maintenance.JobRequests", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "SecondSendForApprovalBy", c => c.String());
            AddColumn("Maintenance.JobRequests", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "OilConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.MaintenancePlanDetail", "CMMaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.MaintenancePlanDetail", "CMLaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.MaintenancePlanDetail", "PMMaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.MaintenancePlanDetail", "PMLaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.MaintenancePlan", "MaintenanceCenterId", c => c.Guid());
            AddColumn("Maintenance.MechanicCrew", "MaintenanceCenterId", c => c.String());
            AddColumn("Maintenance.MechanicCrew", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.MechanicCrew", "Status", c => c.String());
            AddColumn("Maintenance.MechanicCrew", "ServiceCenterId", c => c.String());
            AddColumn("FleetManagement.RentalInspection", "OperatorName", c => c.String());
            AddColumn("FleetManagement.RentalPaymentRequestLine", "EquipmentCategoryId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "ExpKey", c => c.Guid());
            AddColumn("FleetManagement.RentFromExternal", "IsExpired", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "IsReplaced", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "OperatorId", c => c.Guid());
            AddColumn("FleetManagement.RentFromExternal", "Operator", c => c.String());
            AddColumn("FleetManagement.RentFromExternalEquipment", "IsDumpTruck", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "ReturnDate", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "ArrivalDate", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "SentDate", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "ArrivalPlace", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "AsphaltKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RequestedEquipment", "GravelKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RequestedEquipment", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "ForwardedFrom", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "ForwardedTo", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "CurrentApprovalStep", c => c.Int());
            AddColumn("Maintenance.ServiceCenterJob", "IsTransferedToGarage", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ServiceCrewWorkLog", "AdditionalWork", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "SecondOperatorId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.TechnicalJobApproval", "IsFromWorkShop", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.TechnicalJobApproval", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.VehicleDemand", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.VehicleDemand", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.VehicleDemand", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.VehicleDemand", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "CurrentApprovalStep", c => c.Int());
            AddColumn("FleetManagement.VehicleDistribution", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "ReferenceNumber", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "FromRequest", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "JobRequestId", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "SitesVehicleTransferID", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "IssentToBackToJobRequest", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "Status", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "VoidBy", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "VoidTime", c => c.DateTime());
            AddColumn("Maintenance.VehicleReceptionForm", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "OnlineApprovedBy", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Maintenance.VehicleReceptionForm", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Maintenance.VehicleReceptionForm", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "OrgBranchName", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "SendForApprovalBy", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "SendForApprovalTime", c => c.DateTime());
            AlterColumn("FleetManagement.DriverAssignment", "ToOperatorName", c => c.String());
            AlterColumn("FleetManagement.EquipmentRate", "DumpTruckTypeOfWork", c => c.Int());
            AlterColumn("FleetManagement.EquipmentRate", "RoadType", c => c.Int());
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "FleetId", c => c.Guid());
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "BranchId", c => c.Int());
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "BusinessUnitId", c => c.Int());
            AlterColumn("FleetManagement.FleetCheckList", "EquipmentTypeID", c => c.Guid());
            AlterColumn("FleetManagement.LightVehicleTimesheet", "OperatorId", c => c.Guid());
            AlterColumn("FleetManagement.RentalPaymentRequestLine", "GLTaxRateID", c => c.Int());
            AlterColumn("FleetManagement.VehicleDemandApproval", "VehicleDemandId", c => c.Guid());
            CreateIndex("FleetManagement.AnnualInventoryInspectionApproval", "AnnualEquipmentInventoryInspectionID");
            CreateIndex("FleetManagement.VehicleDemandApproval", "VehicleDemandId");
            AddForeignKey("FleetManagement.VehicleDemandApproval", "VehicleDemandId", "FleetManagement.VehicleDemand", "VehicleDemandId");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "FindingsOutcome");
            DropColumn("FleetManagement.FleetCheckList", "Type");
            DropColumn("FleetManagement.FleetCheckList", "IsMaintenace");
            DropColumn("FleetManagement.FleetLineCheckList", "Name");
            DropColumn("FleetManagement.InspectionLog", "InspectionId");
            DropColumn("FleetManagement.InspectionLog", "CategoryName");
            DropColumn("FleetManagement.InspectionLog", "LineName");
            DropColumn("FleetManagement.InspectionLog", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.InspectionLog", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.InspectionLog", "LineName", c => c.String());
            AddColumn("FleetManagement.InspectionLog", "CategoryName", c => c.String());
            AddColumn("FleetManagement.InspectionLog", "InspectionId", c => c.String());
            AddColumn("FleetManagement.FleetLineCheckList", "Name", c => c.String(nullable: false));
            AddColumn("FleetManagement.FleetCheckList", "IsMaintenace", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetCheckList", "Type", c => c.Int(nullable: false));
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "FindingsOutcome", c => c.String(nullable: false));
            DropForeignKey("FleetManagement.VehicleDemandApproval", "VehicleDemandId", "FleetManagement.VehicleDemand");
            DropForeignKey("InfoDesk.Messages", "ChatSessionId", "InfoDesk.ChatSessions");
            DropForeignKey("InfoDesk.ChangeTasks", "ChangeRequestId", "InfoDesk.ChangeRequests");
            DropForeignKey("InfoDesk.ChangeApprovals", "ChangeRequestId", "InfoDesk.ChangeRequests");
            DropIndex("FleetManagement.VehicleDemandApproval", new[] { "VehicleDemandId" });
            DropIndex("InfoDesk.Messages", new[] { "ChatSessionId" });
            DropIndex("InfoDesk.ChangeTasks", new[] { "ChangeRequestId" });
            DropIndex("InfoDesk.ChangeApprovals", new[] { "ChangeRequestId" });
            DropIndex("FleetManagement.AnnualInventoryInspectionApproval", new[] { "AnnualEquipmentInventoryInspectionID" });
            AlterColumn("FleetManagement.VehicleDemandApproval", "VehicleDemandId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.RentalPaymentRequestLine", "GLTaxRateID", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.LightVehicleTimesheet", "OperatorId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.FleetCheckList", "EquipmentTypeID", c => c.Guid(nullable: false));
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "BusinessUnitId", c => c.Int(nullable: false));
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "BranchId", c => c.Int(nullable: false));
            AlterColumn("Maintenance.ExternalMaintenancePaymentRequest", "FleetId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.EquipmentRate", "RoadType", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.EquipmentRate", "DumpTruckTypeOfWork", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.DriverAssignment", "ToOperatorName", c => c.String(nullable: false));
            DropColumn("Maintenance.VehicleReceptionForm", "SendForApprovalTime");
            DropColumn("Maintenance.VehicleReceptionForm", "SendForApprovalBy");
            DropColumn("Maintenance.VehicleReceptionForm", "OrgBranchName");
            DropColumn("Maintenance.VehicleReceptionForm", "IsSendForApproval");
            DropColumn("Maintenance.VehicleReceptionForm", "OnlineTransferredTime");
            DropColumn("Maintenance.VehicleReceptionForm", "IsOnlineTransferredBy");
            DropColumn("Maintenance.VehicleReceptionForm", "IsOnlineTransferred");
            DropColumn("Maintenance.VehicleReceptionForm", "OnlineApprovedTime");
            DropColumn("Maintenance.VehicleReceptionForm", "OnlineApprovedBy");
            DropColumn("Maintenance.VehicleReceptionForm", "IsOnlineApproved");
            DropColumn("Maintenance.VehicleReceptionForm", "VoidTime");
            DropColumn("Maintenance.VehicleReceptionForm", "VoidBy");
            DropColumn("Maintenance.VehicleReceptionForm", "IsVoid");
            DropColumn("Maintenance.VehicleReceptionForm", "Status");
            DropColumn("Maintenance.VehicleReceptionForm", "IssentToBackToJobRequest");
            DropColumn("Maintenance.VehicleReceptionForm", "SitesVehicleTransferID");
            DropColumn("Maintenance.VehicleReceptionForm", "JobRequestId");
            DropColumn("Maintenance.VehicleReceptionForm", "FromRequest");
            DropColumn("Maintenance.VehicleReceptionForm", "ReferenceNumber");
            DropColumn("FleetManagement.VehicleDistribution", "IsExternal");
            DropColumn("FleetManagement.VehicleDemand", "CurrentApprovalStep");
            DropColumn("FleetManagement.VehicleDemand", "IsFinalApproval");
            DropColumn("FleetManagement.VehicleDemand", "DepartmentSendForApprovalTime");
            DropColumn("FleetManagement.VehicleDemand", "DepartmentSendForApprovalBy");
            DropColumn("FleetManagement.VehicleDemand", "IsDepartmentSendForApproval");
            DropColumn("FleetManagement.VehicleDemand", "DepartmentOnlineApprovedTime");
            DropColumn("FleetManagement.VehicleDemand", "DepartmentOnlineApprovedBy");
            DropColumn("FleetManagement.VehicleDemand", "IsDepartmentOnlineApproved");
            DropColumn("Maintenance.TechnicalJobApproval", "IsApproved");
            DropColumn("Maintenance.TechnicalJobApproval", "IsFromWorkShop");
            DropColumn("FleetManagement.SitesVehicleTransfer", "SecondOperatorId");
            DropColumn("Maintenance.ServiceCrewWorkLog", "AdditionalWork");
            DropColumn("Maintenance.ServiceCenterJob", "IsTransferedToGarage");
            DropColumn("FleetManagement.RequestedEquipment", "CurrentApprovalStep");
            DropColumn("FleetManagement.RequestedEquipment", "IsFinalApproval");
            DropColumn("FleetManagement.RequestedEquipment", "ForwardedTo");
            DropColumn("FleetManagement.RequestedEquipment", "ForwardedFrom");
            DropColumn("FleetManagement.RequestedEquipment", "DepartmentSendForApprovalTime");
            DropColumn("FleetManagement.RequestedEquipment", "DepartmentSendForApprovalBy");
            DropColumn("FleetManagement.RequestedEquipment", "IsDepartmentSendForApproval");
            DropColumn("FleetManagement.RequestedEquipment", "DepartmentOnlineApprovedTime");
            DropColumn("FleetManagement.RequestedEquipment", "DepartmentOnlineApprovedBy");
            DropColumn("FleetManagement.RequestedEquipment", "IsDepartmentOnlineApproved");
            DropColumn("FleetManagement.RequestedEquipment", "GravelKm");
            DropColumn("FleetManagement.RequestedEquipment", "AsphaltKm");
            DropColumn("FleetManagement.RequestedEquipment", "ArrivalPlace");
            DropColumn("FleetManagement.RequestedEquipment", "SentDate");
            DropColumn("FleetManagement.RequestedEquipment", "ArrivalDate");
            DropColumn("FleetManagement.RequestedEquipment", "ReturnDate");
            DropColumn("FleetManagement.RentFromExternalEquipment", "IsDumpTruck");
            DropColumn("FleetManagement.RentFromExternal", "Operator");
            DropColumn("FleetManagement.RentFromExternal", "OperatorId");
            DropColumn("FleetManagement.RentFromExternal", "IsReplaced");
            DropColumn("FleetManagement.RentFromExternal", "IsExpired");
            DropColumn("FleetManagement.RentFromExternal", "ExpKey");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "EquipmentCategoryId");
            DropColumn("FleetManagement.RentalInspection", "OperatorName");
            DropColumn("Maintenance.MechanicCrew", "ServiceCenterId");
            DropColumn("Maintenance.MechanicCrew", "Status");
            DropColumn("Maintenance.MechanicCrew", "IsActive");
            DropColumn("Maintenance.MechanicCrew", "MaintenanceCenterId");
            DropColumn("Maintenance.MaintenancePlan", "MaintenanceCenterId");
            DropColumn("Maintenance.MaintenancePlanDetail", "PMLaborCost");
            DropColumn("Maintenance.MaintenancePlanDetail", "PMMaterialCost");
            DropColumn("Maintenance.MaintenancePlanDetail", "CMLaborCost");
            DropColumn("Maintenance.MaintenancePlanDetail", "CMMaterialCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "OilConsumption");
            DropColumn("Maintenance.JobRequests", "SecondSendForApprovalTime");
            DropColumn("Maintenance.JobRequests", "SecondSendForApprovalBy");
            DropColumn("Maintenance.JobRequests", "IsSecondSendForApproval");
            DropColumn("Maintenance.JobRequests", "SecondOnlineApprovedTime");
            DropColumn("Maintenance.JobRequests", "SecondOnlineApprovedBy");
            DropColumn("Maintenance.JobRequests", "IsSecondOnlineApproved");
            DropColumn("Maintenance.JobRequests", "CreateJobOrder");
            DropColumn("Maintenance.JobRequests", "IsSentBackToReception");
            DropColumn("Maintenance.JobRequests", "EstmatedCostAmount");
            DropColumn("Maintenance.JobRequests", "GPSInfo");
            DropColumn("Maintenance.JobRequests", "KMReading");
            DropColumn("Maintenance.JobRequests", "FromBranchId");
            DropColumn("Maintenance.RepairType", "Weight");
            DropColumn("Maintenance.JobOrderDetail", "IsFromWorkShopApproved");
            DropColumn("Maintenance.JobOrderDetail", "IsFromWorkShopRejected");
            DropColumn("Maintenance.JobOrderDetail", "DescriptionOfJobs");
            DropColumn("Maintenance.JobOrderDetail", "IsFromWorkShop");
            DropColumn("Maintenance.JobOrderDetail", "ServiceGroupSettingId");
            DropColumn("Maintenance.JobOrder", "IsSentToWorkShopTime");
            DropColumn("Maintenance.JobOrder", "GPSInfo");
            DropColumn("Maintenance.JobOrder", "KMReading");
            DropColumn("Maintenance.JobOrder", "ServiceCenterJobId");
            DropColumn("Maintenance.JobOrder", "ToExternalJobOrderId");
            DropColumn("Maintenance.JobCauses", "FromReceptionCount");
            DropColumn("Maintenance.JobCauses", "ShowOnReception");
            DropColumn("FleetManagement.InspectionLog", "StatusKeyId");
            DropColumn("FleetManagement.InspectionLog", "StatusKey");
            DropColumn("FleetManagement.InspectionLog", "CheckList");
            DropColumn("FleetManagement.InspectionLog", "SystemName");
            DropColumn("FleetManagement.InspectionLog", "AnnualEquipmentInventoryInspectionID");
            DropColumn("FleetManagement.InspectionApproval", "IsApproved");
            DropColumn("InfoDesk.InfoDeskKnowledgeBases", "OnlineApprovedTime");
            DropColumn("InfoDesk.InfoDeskKnowledgeBases", "OnlineApprovedBy");
            DropColumn("InfoDesk.InfoDeskKnowledgeBases", "IsOnlineApproved");
            DropColumn("InfoDesk.InfoDeskKnowledgeBases", "DocumentType");
            DropColumn("FleetManagement.FleetLineCheckList", "CheckList");
            DropColumn("FleetManagement.FleetLineCheckList", "SystemName");
            DropColumn("FleetManagement.FleetBoloRegistration", "EquipmentNameId");
            DropColumn("FleetManagement.FleetBoloRegistration", "EquipmentTypeId");
            DropColumn("FleetManagement.FleetBoloRegistration", "EquipmentCategoryId");
            DropColumn("Maintenance.ExternalServiceCenterPayment", "Invoice");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "CurrentApprovalStep");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinalApproval");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedTo");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedFrom");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentSendForApprovalTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentSendForApprovalBy");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsDepartmentSendForApproval");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentOnlineApprovedTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "DepartmentOnlineApprovedBy");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsDepartmentOnlineApproved");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "AmountDue");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "TotalAmount");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "Paid");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequestDetail", "ExternalPaymentRequestJobId");
            DropColumn("FleetManagement.EquipmentTyreLog", "ReturnedKmReading");
            DropColumn("FleetManagement.EquipmentTyreLog", "ReturnedDate");
            DropColumn("FleetManagement.EquipmentTyreLog", "InstalledDate");
            DropColumn("FleetManagement.EquipmentRate", "IsDump");
            DropColumn("FleetManagement.EquipmentRate", "IdleRate");
            DropColumn("FleetManagement.EquipmentArrival", "ArrivalPlace");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "OilConsumption");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "DownReason");
            DropColumn("FleetManagement.DriverVehicleTransfer", "EquipmentNameId");
            DropColumn("FleetManagement.DriverVehicleTransfer", "EquipmentTypeId");
            DropColumn("FleetManagement.DriverVehicleTransfer", "EquipmentCategoryId");
            DropColumn("FleetManagement.DriverAssignment", "EquipmentNameId");
            DropColumn("FleetManagement.DriverAssignment", "EquipmentTypeId");
            DropColumn("FleetManagement.DriverAssignment", "EquipmentCategoryId");
            DropColumn("FleetManagement.DriverAssignment", "SecondOperatorId");
            DropColumn("FleetManagement.DriverAssignment", "OperatorId");
            DropColumn("InfoDesk.TicketType_HelpDesk", "EstimatedCost");
            DropColumn("InfoDesk.MaterialItems", "InventoryItemId");
            DropColumn("InfoDesk.MaterialItems", "CategoryId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "RequesterIPAddress");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Handler");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "PendingReason");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "TagNo");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "ItemName");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "AssetRegistrationId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "FixedAssetSubCategoryId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "FixedAssetCategoryId");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "Handler");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "IsSendToKB");
            DropColumn("FleetManagement.AssignedEquipment", "GravelKm");
            DropColumn("FleetManagement.AssignedEquipment", "AsphaltKm");
            DropColumn("FleetManagement.AssignedEquipment", "ArrivalPlace");
            DropColumn("EXCEED.ApTravelOrder", "BankAccount");
            DropColumn("FleetManagement.TireReturn", "ReturnedKMReading");
            DropColumn("FleetManagement.TireReturn", "ReturnDate");
            DropColumn("FleetManagement.TireReturn", "IssueDate");
            DropColumn("FleetManagement.TireIssue", "SerialNoD");
            DropColumn("FleetManagement.TireIssue", "SerialNoC");
            DropColumn("FleetManagement.TireIssue", "SerialNoB");
            DropColumn("FleetManagement.TireIssue", "IssueReference");
            DropColumn("FleetManagement.TireIssue", "IssueDate");
            DropColumn("FleetManagement.Operator", "EmployeeCode");
            DropColumn("FleetManagement.FleetInsurances", "EquipmentNameId");
            DropColumn("FleetManagement.FleetInsurances", "EquipmentTypeId");
            DropColumn("FleetManagement.FleetInsurances", "EquipmentCategoryId");
            DropColumn("FleetManagement.Fuel", "RequestedQty");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "Operator");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "CountryOfOrigin");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EngineSerailNumber");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EngineModel");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "YearOfManufacturing");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "SerialNumber");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "ModelNumber");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "Make");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentNameId");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentTypeId");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "EquipmentCategoryId");
            DropColumn("FleetManagement.Fleets", "OperatorId");
            DropTable("FleetManagement.RequestToInspection");
            DropTable("FleetManagement.TireIssueDetails");
            DropTable("FleetManagement.TimeSheetSetting");
            DropTable("FleetManagement.TimeSheetException");
            DropTable("FleetManagement.RoadORLoad");
            DropTable("FleetManagement.RentFromExternalEquipmentRate");
            DropTable("FleetManagement.RateOfRoadORLoad");
            DropTable("InfoDesk.Messages");
            DropTable("FleetManagement.InspectionStatusKeys");
            DropTable("FleetManagement.HeavyORDumpTruck");
            DropTable("EXCEED.FleetInsuranceHistory");
            DropTable("FleetManagement.FleetInsuranceDocument");
            DropTable("EXCEED.FleetBoloRegistrationHistory");
            DropTable("InfoDesk.ChatSessions");
            DropTable("InfoDesk.ChangeTasks");
            DropTable("InfoDesk.ChangeRequests");
            DropTable("InfoDesk.ChangeApprovals");
            DropTable("EXCEED.BoloRegistrationDocument");
            CreateIndex("FleetManagement.VehicleDemandApproval", "VehicleDemandId");
            CreateIndex("FleetManagement.AnnualInventoryInspectionApproval", "AnnualEquipmentInventoryInspectionId");
            AddForeignKey("FleetManagement.VehicleDemandApproval", "VehicleDemandId", "FleetManagement.VehicleDemand", "VehicleDemandId", cascadeDelete: true);
            MoveTable(name: "InfoDesk.MaterialsUseds", newSchema: "EXCEED");
            MoveTable(name: "InfoDesk.MaterialItems", newSchema: "EXCEED");
            RenameTable(name: "EXCEED.MaterialsUseds", newName: "MaterialsUsed");
        }
    }
}
