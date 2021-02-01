namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class collection_features : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.CompanyEmail",
                c => new
                    {
                        CompanyEmailId = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        ServerType = c.String(),
                        MainCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyEmailId)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId, cascadeDelete: true)
                .Index(t => t.MainCompanyId);
            
            CreateTable(
                "FleetManagement.EquipmentPlanEvaluationApproval",
                c => new
                    {
                        EquipmentPlanEvaluationApprovalId = c.Int(nullable: false, identity: true),
                        EquipmentPlanandEvaluationsId = c.Guid(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentPlanEvaluationApprovalId)
                .ForeignKey("FleetManagement.EquipmentPlanandEvaluations", t => t.EquipmentPlanandEvaluationsId, cascadeDelete: true)
                .Index(t => t.EquipmentPlanandEvaluationsId);
            
            CreateTable(
                "EXCEED.JournalSourceMap",
                c => new
                    {
                        JournalSourceMapId = c.Int(nullable: false, identity: true),
                        GLJournalCategoriersID = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JournalSourceMapId)
                .ForeignKey("EXCEED.GLJournalCategoriers", t => t.GLJournalCategoriersID, cascadeDelete: true)
                .Index(t => t.GLJournalCategoriersID);
            
            CreateTable(
                "FleetManagement.RequestedEquipment",
                c => new
                    {
                        RequestedEquipmentId = c.Guid(nullable: false),
                        RequestNumber = c.Guid(nullable: false),
                        EquipmentID = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AssignedDate = c.DateTime(),
                        RequestedDate = c.DateTime(),
                        Type = c.String(nullable: false),
                        CategoryRef = c.String(),
                        EquipmentStatus = c.String(),
                        RequestDesc = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.RequestedEquipmentId);
            
            CreateTable(
                "FleetManagement.TimeSheetApproval",
                c => new
                    {
                        TimeSheetApprovalId = c.Int(nullable: false, identity: true),
                        TimeSheetId = c.Guid(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TimeSheetApprovalId)
                .ForeignKey("FleetManagement.TimeSheet", t => t.TimeSheetId, cascadeDelete: true)
                .Index(t => t.TimeSheetId);
            
            AddColumn("EXCEED.BidAwardedSupplier", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "CategoryName", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItemName", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "DeliveryDate", c => c.DateTime());
            AddColumn("FleetManagement.RentFromExternal", "EquipmentIdentity", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.GoodRecivingNoteApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.OtherCompanyMaintenaceApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.OutsourceMaintenanceApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.RentForInternalApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.RentFromExternal", "EquipmentType", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.RentFromExternalApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.SalvageStoreApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.SiteVehicleApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.VehicleDemandApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.VehicleDistributionApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.VehicleReturnApproval", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("FleetManagement.TimeSheetApproval", "TimeSheetId", "FleetManagement.TimeSheet");
            DropForeignKey("EXCEED.JournalSourceMap", "GLJournalCategoriersID", "EXCEED.GLJournalCategoriers");
            DropForeignKey("FleetManagement.EquipmentPlanEvaluationApproval", "EquipmentPlanandEvaluationsId", "FleetManagement.EquipmentPlanandEvaluations");
            DropForeignKey("HRM.CompanyEmail", "MainCompanyId", "HRM.MainCompany");
            DropIndex("FleetManagement.TimeSheetApproval", new[] { "TimeSheetId" });
            DropIndex("EXCEED.JournalSourceMap", new[] { "GLJournalCategoriersID" });
            DropIndex("FleetManagement.EquipmentPlanEvaluationApproval", new[] { "EquipmentPlanandEvaluationsId" });
            DropIndex("HRM.CompanyEmail", new[] { "MainCompanyId" });
            AlterColumn("FleetManagement.VehicleReturnApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.VehicleDistributionApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.VehicleDemandApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.SiteVehicleApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.SalvageStoreApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.RentFromExternalApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.RentFromExternal", "EquipmentType", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentForInternalApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.OutsourceMaintenanceApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.OtherCompanyMaintenaceApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.GoodRecivingNoteApproval", "Status", c => c.Int(nullable: false));
            DropColumn("FleetManagement.RentFromExternal", "EquipmentIdentity");
            DropColumn("EXCEED.BidAwardedSupplier", "DeliveryDate");
            DropColumn("EXCEED.BidAwardedSupplier", "TPrice");
            DropColumn("EXCEED.BidAwardedSupplier", "TaxAmount");
            DropColumn("EXCEED.BidAwardedSupplier", "GLTaxRateID");
            DropColumn("EXCEED.BidAwardedSupplier", "UPrice");
            DropColumn("EXCEED.BidAwardedSupplier", "Quanity");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemType");
            DropColumn("EXCEED.BidAwardedSupplier", "UoM");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemSpecification");
            DropColumn("EXCEED.BidAwardedSupplier", "IsApproved");
            DropColumn("EXCEED.BidAwardedSupplier", "PartNo");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemName");
            DropColumn("EXCEED.BidAwardedSupplier", "CategoryName");
            DropColumn("EXCEED.BidAwardedSupplier", "CatCode");
            DropTable("FleetManagement.TimeSheetApproval");
            DropTable("FleetManagement.RequestedEquipment");
            DropTable("EXCEED.JournalSourceMap");
            DropTable("FleetManagement.EquipmentPlanEvaluationApproval");
            DropTable("HRM.CompanyEmail");
        }
    }
}
