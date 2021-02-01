namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budgetfleet : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FleetManagement.TimeSheet", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.TimeSheet", new[] { "FleetsID" });
            CreateTable(
                "Budget.BudgetControl",
                c => new
                    {
                        BudgetControlId = c.Guid(nullable: false),
                        Name = c.String(),
                        ReserveAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Percentage = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetControlId);
            
            CreateTable(
                "FleetManagement.DriverAssignment",
                c => new
                    {
                        DriverAssignmentId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        FromOperatorName = c.String(),
                        ToOperatorName = c.String(nullable: false),
                        StartingDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.DriverAssignmentId);
            
            CreateTable(
                "FleetManagement.EquipmentTyre",
                c => new
                    {
                        EquipmentTyreId = c.Guid(nullable: false),
                        Type = c.String(),
                        SerialNo = c.String(),
                        Make = c.String(),
                        Size = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fleetId = c.Guid(nullable: false),
                        Isattached = c.Boolean(nullable: false),
                        Condition = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentTyreId);
            
            CreateTable(
                "FleetManagement.FuelInventory",
                c => new
                    {
                        FuelInventoryId = c.Guid(nullable: false),
                        SiteName = c.String(nullable: false),
                        FuelType = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.FuelInventoryId);
            
            CreateTable(
                "FleetManagement.FuelType",
                c => new
                    {
                        FuelTypeId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FuelTypeId);
            
            CreateTable(
                "FleetManagement.TyreTransferApproval",
                c => new
                    {
                        TyreTransferApprovalId = c.Int(nullable: false, identity: true),
                        TyreId = c.Guid(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        TyreTransfers_TyreTransferId = c.Guid(),
                    })
                .PrimaryKey(t => t.TyreTransferApprovalId)
                .ForeignKey("FleetManagement.TyreTransfer", t => t.TyreTransfers_TyreTransferId)
                .Index(t => t.TyreTransfers_TyreTransferId);
            
            CreateTable(
                "FleetManagement.TyreTransfer",
                c => new
                    {
                        TyreTransferId = c.Guid(nullable: false),
                        TyreSerialNo = c.String(),
                        FromEquipment = c.String(nullable: false),
                        ToEquipment = c.String(nullable: false),
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
                        EquipmentTyres_EquipmentTyreId = c.Guid(),
                    })
                .PrimaryKey(t => t.TyreTransferId)
                .ForeignKey("FleetManagement.EquipmentTyre", t => t.EquipmentTyres_EquipmentTyreId)
                .Index(t => t.EquipmentTyres_EquipmentTyreId);
            
            AddColumn("FleetManagement.Fleets", "Condition", c => c.String());
            AddColumn("FleetManagement.Fleets", "EngineModel", c => c.String());
            AddColumn("FleetManagement.Fleets", "OperatorName", c => c.String());
            AddColumn("FleetManagement.Fleets", "EngineManufacturer", c => c.String());
            AddColumn("FleetManagement.Fleets", "Location", c => c.String());
            AddColumn("FleetManagement.Fleets", "HistoricalCost", c => c.String());
            AddColumn("FleetManagement.Fleets", "Battery", c => c.String());
            AddColumn("FleetManagement.Fleets", "Tyre", c => c.String());
            AddColumn("FleetManagement.Operator", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetCategory", "BudgetControl", c => c.String(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "FromEquipment", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "ToEquipment", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.JobOrder", "Odometer", c => c.Int());
            AddColumn("FleetManagement.JobOrder", "Site", c => c.String());
            AddColumn("FleetManagement.JobOrder", "EquipmentNo", c => c.String());
            AddColumn("FleetManagement.MechanicCrew", "HourlyWage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ServiceCrew", "HourlyWage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ServiceSpare", "UnitPrice", c => c.Int());
            AddColumn("FleetManagement.TimeSheet", "Type", c => c.String());
            AddColumn("FleetManagement.TimeSheet", "EquipmentName", c => c.String());
            AddColumn("FleetManagement.TimeSheet", "IsExternal", c => c.Boolean(nullable: false));
            AlterColumn("FleetManagement.Fleets", "EquipmentName", c => c.String());
            AlterColumn("FleetManagement.Fleets", "UnitNum", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "Manufacturer", c => c.String());
            AlterColumn("FleetManagement.Fleets", "ManufacturerYear", c => c.String());
            AlterColumn("FleetManagement.Fleets", "Model", c => c.String());
            AlterColumn("FleetManagement.Fleets", "Capacity", c => c.String());
            AlterColumn("FleetManagement.Fleets", "Color", c => c.String());
            AlterColumn("FleetManagement.Fleets", "TyreSize", c => c.String());
            AlterColumn("FleetManagement.Fleets", "PlateNumber", c => c.String());
            AlterColumn("FleetManagement.Fleets", "EngineNumber", c => c.String());
            AlterColumn("FleetManagement.Fleets", "ChasisNumber", c => c.String());
            AlterColumn("FleetManagement.Fleets", "WayOfSupply", c => c.String());
            AlterColumn("FleetManagement.Fleets", "CurentOdoMeter", c => c.Int());
            AlterColumn("FleetManagement.Fleets", "SerialNo", c => c.String());
            AlterColumn("FleetManagement.Fleets", "TypeOfFuel", c => c.String());
            AlterColumn("FleetManagement.TireIssue", "Quantity", c => c.Int());
            AlterColumn("FleetManagement.SparePart", "SparePartName", c => c.String());
            AlterColumn("FleetManagement.SparePart", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TimeSheet", "EquipmentAdministratorName", c => c.String());
            DropColumn("FleetManagement.TimeSheet", "Location");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.TimeSheet", "Location", c => c.String(nullable: false));
            DropForeignKey("FleetManagement.TyreTransferApproval", "TyreTransfers_TyreTransferId", "FleetManagement.TyreTransfer");
            DropForeignKey("FleetManagement.TyreTransfer", "EquipmentTyres_EquipmentTyreId", "FleetManagement.EquipmentTyre");
            DropIndex("FleetManagement.TyreTransfer", new[] { "EquipmentTyres_EquipmentTyreId" });
            DropIndex("FleetManagement.TyreTransferApproval", new[] { "TyreTransfers_TyreTransferId" });
            AlterColumn("FleetManagement.TimeSheet", "EquipmentAdministratorName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.SparePart", "UnitPrice", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.SparePart", "SparePartName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "Quantity", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "TypeOfFuel", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "SerialNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "CurentOdoMeter", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "WayOfSupply", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "ChasisNumber", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "EngineNumber", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "PlateNumber", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "TyreSize", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "Color", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "Capacity", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "Model", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "ManufacturerYear", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "Manufacturer", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Fleets", "UnitNum", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "EquipmentName", c => c.String(nullable: false));
            DropColumn("FleetManagement.TimeSheet", "IsExternal");
            DropColumn("FleetManagement.TimeSheet", "EquipmentName");
            DropColumn("FleetManagement.TimeSheet", "Type");
            DropColumn("FleetManagement.ServiceSpare", "UnitPrice");
            DropColumn("FleetManagement.ServiceCrew", "HourlyWage");
            DropColumn("FleetManagement.MechanicCrew", "HourlyWage");
            DropColumn("FleetManagement.JobOrder", "EquipmentNo");
            DropColumn("FleetManagement.JobOrder", "Site");
            DropColumn("FleetManagement.JobOrder", "Odometer");
            DropColumn("FleetManagement.DriverVehicleTransfer", "ToEquipment");
            DropColumn("FleetManagement.DriverVehicleTransfer", "FromEquipment");
            DropColumn("Budget.BudgetCategory", "BudgetControl");
            DropColumn("FleetManagement.Operator", "IsActive");
            DropColumn("FleetManagement.Fleets", "Tyre");
            DropColumn("FleetManagement.Fleets", "Battery");
            DropColumn("FleetManagement.Fleets", "HistoricalCost");
            DropColumn("FleetManagement.Fleets", "Location");
            DropColumn("FleetManagement.Fleets", "EngineManufacturer");
            DropColumn("FleetManagement.Fleets", "OperatorName");
            DropColumn("FleetManagement.Fleets", "EngineModel");
            DropColumn("FleetManagement.Fleets", "Condition");
            DropTable("FleetManagement.TyreTransfer");
            DropTable("FleetManagement.TyreTransferApproval");
            DropTable("FleetManagement.FuelType");
            DropTable("FleetManagement.FuelInventory");
            DropTable("FleetManagement.EquipmentTyre");
            DropTable("FleetManagement.DriverAssignment");
            DropTable("Budget.BudgetControl");
            CreateIndex("FleetManagement.TimeSheet", "FleetsID");
            AddForeignKey("FleetManagement.TimeSheet", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
        }
    }
}
