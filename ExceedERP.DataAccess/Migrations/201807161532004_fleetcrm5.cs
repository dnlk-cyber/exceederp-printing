namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.FleetPettyCash",
                c => new
                    {
                        FleetPettyCashId = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.FleetPettyCashId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "FleetManagement.SiteVechicleTransferTyreDetail",
                c => new
                    {
                        SiteVechicleTransferTyreDetailId = c.Int(nullable: false, identity: true),
                        SiteVechicleTransferId = c.Guid(nullable: false),
                        Type = c.String(),
                        SerialNo = c.String(nullable: false),
                        Description = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.SiteVechicleTransferTyreDetailId);
            
            AddColumn("FleetManagement.Accident", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.Accident", "IsHornHeared", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "PersonWasAtAccident", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "VehicleSpeed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Accident", "VehicleDistanceFromRoadEdge", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentTyreLog", "Equipment", c => c.String());
            AddColumn("FleetManagement.RentFromExternal", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.SitesVehicleTransfer", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.SitesVehicleTransfer", "FuelAmount", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "BatteryStatus", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "EquipmentCondition", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "TransferMode", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "Hydrolic", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "BatteryAmpere", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "BatteryQuantity", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "TransferedOperator", c => c.String());
            AddColumn("FleetManagement.SiteVechicleTransferDetail", "SerialNo", c => c.String());
            AddColumn("FleetManagement.SiteVechicleTransferDetail", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SiteVechicleTransferDetail", "DamagedParts", c => c.String());
            AddColumn("FleetManagement.SiteVechicleTransferDetail", "MissingParts", c => c.String());
            DropTable("FleetManagement.FleetInsuranceType");
        }
        
        public override void Down()
        {
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
            
            DropForeignKey("FleetManagement.FleetPettyCash", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.FleetPettyCash", new[] { "FleetsID" });
            DropColumn("FleetManagement.SiteVechicleTransferDetail", "MissingParts");
            DropColumn("FleetManagement.SiteVechicleTransferDetail", "DamagedParts");
            DropColumn("FleetManagement.SiteVechicleTransferDetail", "Quantity");
            DropColumn("FleetManagement.SiteVechicleTransferDetail", "SerialNo");
            DropColumn("FleetManagement.SitesVehicleTransfer", "TransferedOperator");
            DropColumn("FleetManagement.SitesVehicleTransfer", "BatteryQuantity");
            DropColumn("FleetManagement.SitesVehicleTransfer", "BatteryAmpere");
            DropColumn("FleetManagement.SitesVehicleTransfer", "Hydrolic");
            DropColumn("FleetManagement.SitesVehicleTransfer", "TransferMode");
            DropColumn("FleetManagement.SitesVehicleTransfer", "EquipmentCondition");
            DropColumn("FleetManagement.SitesVehicleTransfer", "BatteryStatus");
            DropColumn("FleetManagement.SitesVehicleTransfer", "FuelAmount");
            DropColumn("FleetManagement.SitesVehicleTransfer", "Num");
            DropColumn("FleetManagement.RentFromExternal", "Num");
            DropColumn("FleetManagement.EquipmentTyreLog", "Equipment");
            DropColumn("FleetManagement.Accident", "VehicleDistanceFromRoadEdge");
            DropColumn("FleetManagement.Accident", "VehicleSpeed");
            DropColumn("FleetManagement.Accident", "PersonWasAtAccident");
            DropColumn("FleetManagement.Accident", "IsHornHeared");
            DropColumn("FleetManagement.Accident", "Num");
            DropTable("FleetManagement.SiteVechicleTransferTyreDetail");
            DropTable("FleetManagement.FleetPettyCash");
        }
    }
}
