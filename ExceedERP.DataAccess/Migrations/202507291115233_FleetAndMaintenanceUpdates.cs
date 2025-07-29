namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FleetAndMaintenanceUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Maintenance.AdministrativeJobDepartmentApproval",
                c => new
                    {
                        AdministrativeJobDepartmentApprovalID = c.Int(nullable: false, identity: true),
                        AdministrativeJobId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdministrativeJobDepartmentApprovalID);
            
            CreateTable(
                "Maintenance.CanibalizationApprovals",
                c => new
                    {
                        CanibalizationApprovalId = c.Int(nullable: false, identity: true),
                        CanibalizationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CanibalizationApprovalId);
            
            CreateTable(
                "Maintenance.CanibalizationDepartmentApproval",
                c => new
                    {
                        CanibalizationDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        CanibalizationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CanibalizationDepartmentApprovalId);
            
            CreateTable(
                "Maintenance.Canibalizations",
                c => new
                    {
                        CanibalizationId = c.Int(nullable: false, identity: true),
                        ServiceCenterJobId = c.String(),
                        ProvidingMachinModel = c.String(),
                        ProvidingMachinePlateNumber = c.String(),
                        ProvidingMachineEngineModel = c.String(),
                        ProvidingMachineEngineNumber = c.String(),
                        RequestingMachinModel = c.String(),
                        RequestingMachinePlateNumber = c.String(),
                        RequestingMachineEngineModel = c.String(),
                        RequestingMachineEngineNumber = c.String(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.CanibalizationId);
            
            CreateTable(
                "Maintenance.CanibalizedParts",
                c => new
                    {
                        CanibalizedPartsId = c.Int(nullable: false, identity: true),
                        CanibalizationId = c.Int(nullable: false),
                        Description = c.String(),
                        VehiclePartId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CanibalizedPartsId);
            
            CreateTable(
                "Maintenance.CloseWorkShopWithRemarks",
                c => new
                    {
                        CloseWorkShopWithRemarkId = c.Int(nullable: false, identity: true),
                        JobId = c.String(),
                        JobOrderId = c.String(),
                        ServiceName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CloseWorkShopWithRemarkId);
            
            CreateTable(
                "Maintenance.ExternalMaintenancePaymentRequestDepValidations",
                c => new
                    {
                        ExternalMaintenancePaymentRequestDepValidationId = c.Int(nullable: false, identity: true),
                        ExternalMaintenancePaymenntrequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalMaintenancePaymentRequestDepValidationId);
            
            CreateTable(
                "Maintenance.ExternalMaintenancePaymentRequestValidations",
                c => new
                    {
                        ExternalMaintenancePaymentRequestValidationId = c.Int(nullable: false, identity: true),
                        ExternalMaintenancePaymenntrequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalMaintenancePaymentRequestValidationId);
            
            CreateTable(
                "Maintenance.ExternalMaintenancePaymentRequestHeaders",
                c => new
                    {
                        ExternalPaymentRequestJobId = c.Int(nullable: false, identity: true),
                        ExternalMaintenancePaymenntrequestId = c.Int(nullable: false),
                        ServiceCenterJobId = c.String(),
                        Desctiption = c.String(),
                        FleetId = c.Guid(),
                        EquipmentName = c.String(),
                        EnteranceDate = c.DateTime(),
                        ExitDate = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalPaymentRequestJobId);
            
            CreateTable(
                "Maintenance.ExternalServiceCenterDetail",
                c => new
                    {
                        ExternalServiceCenterDetailId = c.Int(nullable: false, identity: true),
                        ServiceCenterJobId = c.String(),
                        Description = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExternalServiceCenterDetailId);
            
            CreateTable(
                "Maintenance.InternalJobToExternal",
                c => new
                    {
                        InternalJobToExternalId = c.Int(nullable: false, identity: true),
                        ServiceCenterJobId = c.String(),
                        Description = c.String(),
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
                .PrimaryKey(t => t.InternalJobToExternalId);
            
            CreateTable(
                "Maintenance.InternalJobToExternalValidation",
                c => new
                    {
                        InternalJobToExternalValidationId = c.Int(nullable: false, identity: true),
                        InternalJobToExternalId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalJobToExternalValidationId);
            
            CreateTable(
                "Maintenance.PmSchedulers",
                c => new
                    {
                        PmSchedulerId = c.Int(nullable: false, identity: true),
                        EquipmentNameId = c.String(),
                        PlateNumber = c.String(),
                        Location = c.String(),
                        TaskId = c.String(),
                        PmSettingId = c.Int(nullable: false),
                        DateFrequency = c.Int(nullable: false),
                        KMFrequency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastPmDate = c.DateTime(nullable: false),
                        LastPmKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NextPmDate = c.DateTime(nullable: false),
                        NextPmKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsExpired = c.Boolean(nullable: false),
                        WorkDelay = c.Boolean(nullable: false),
                        DelayReason = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PmSchedulerId);
            
            CreateTable(
                "Maintenance.PmSetting",
                c => new
                    {
                        PmSettingId = c.Int(nullable: false, identity: true),
                        PmName = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PmSettingId);
            
            CreateTable(
                "Maintenance.ServiceGroupSettings",
                c => new
                    {
                        ServiceGroupSettingId = c.Int(nullable: false, identity: true),
                        ServiceGroupName = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceGroupSettingId);
            
            CreateTable(
                "Maintenance.SOPDocuments",
                c => new
                    {
                        SOPDocumentId = c.Int(nullable: false, identity: true),
                        PmSchedulerId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.SOPDocumentId);
            
            CreateTable(
                "Maintenance.VehicleReceptionApprovals",
                c => new
                    {
                        VehicleReceptionApprovalId = c.Int(nullable: false, identity: true),
                        VehicleReceptionFormId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleReceptionApprovalId);
            
            AddColumn("FleetManagement.DryLoadingTimeSheet", "IdleRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCenterJob", "IsSentToExternal", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ServiceCenter", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrewWorkLog", "RecordedDate", c => c.DateTime());
            AddColumn("Maintenance.ServiceCrewWorkLog", "RepiarTypeId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Maintenance.ServiceCrewWorkLog", "RepiarTypeId");
            DropColumn("Maintenance.ServiceCrewWorkLog", "RecordedDate");
            DropColumn("Maintenance.ServiceCenter", "Weight");
            DropColumn("Maintenance.ServiceCenterJob", "IsSentToExternal");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "IdleRate");
            DropTable("Maintenance.VehicleReceptionApprovals");
            DropTable("Maintenance.SOPDocuments");
            DropTable("Maintenance.ServiceGroupSettings");
            DropTable("Maintenance.PmSetting");
            DropTable("Maintenance.PmSchedulers");
            DropTable("Maintenance.InternalJobToExternalValidation");
            DropTable("Maintenance.InternalJobToExternal");
            DropTable("Maintenance.ExternalServiceCenterDetail");
            DropTable("Maintenance.ExternalMaintenancePaymentRequestHeaders");
            DropTable("Maintenance.ExternalMaintenancePaymentRequestValidations");
            DropTable("Maintenance.ExternalMaintenancePaymentRequestDepValidations");
            DropTable("Maintenance.CloseWorkShopWithRemarks");
            DropTable("Maintenance.CanibalizedParts");
            DropTable("Maintenance.Canibalizations");
            DropTable("Maintenance.CanibalizationDepartmentApproval");
            DropTable("Maintenance.CanibalizationApprovals");
            DropTable("Maintenance.AdministrativeJobDepartmentApproval");
        }
    }
}
