namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetmaintenanceupg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Maintenance.ExternalMaintenancePaymentRequestDetail",
                c => new
                    {
                        ExternalMaintenancePaymentRequestDetailId = c.Int(nullable: false, identity: true),
                        CostType = c.String(),
                        Date = c.DateTime(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlTaxRateId = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RepairDescribtion = c.String(),
                        ExternalMaintenancePaymenntrequestId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalMaintenancePaymentRequestDetailId);
            
            CreateTable(
                "Maintenance.ExternalMaintenancePaymentRequest",
                c => new
                    {
                        ExternalMaintenancePaymenntrequestId = c.Int(nullable: false, identity: true),
                        FleetId = c.Guid(nullable: false),
                        EquipmentName = c.String(),
                        EnteranceDate = c.DateTime(),
                        ExitDate = c.DateTime(),
                        ServiceCenterId = c.Int(),
                        MaintenanceCenterId = c.Guid(),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        JobNum = c.Int(nullable: false),
                        RequestorId = c.Int(),
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
                .PrimaryKey(t => t.ExternalMaintenancePaymenntrequestId);
            
            CreateTable(
                "Maintenance.MaintenanceCompanyDocument",
                c => new
                    {
                        MaintenanceCompanyDocumentId = c.Int(nullable: false, identity: true),
                        MaintenanceCompanyDocumentTypeId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceCompanyDocumentId);
            
            CreateTable(
                "Maintenance.MaintenanceCompanyDocumentType",
                c => new
                    {
                        MaintenanceCompanyDocumentTypeId = c.Int(nullable: false, identity: true),
                        DocumentType = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceCompanyDocumentTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("Maintenance.MaintenanceCompanyDocumentType");
            DropTable("Maintenance.MaintenanceCompanyDocument");
            DropTable("Maintenance.ExternalMaintenancePaymentRequest");
            DropTable("Maintenance.ExternalMaintenancePaymentRequestDetail");
        }
    }
}
