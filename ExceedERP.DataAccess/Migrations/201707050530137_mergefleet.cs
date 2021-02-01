namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mergefleet : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.JobOrder", "FleetsID", "EXCEED.Fleets");
            DropIndex("EXCEED.JobOrder", new[] { "FleetsID" });
            CreateTable(
                "CRM.Accounts",
                c => new
                    {
                        AccountsID = c.Int(nullable: false, identity: true),
                        ControlAccount = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Parent = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccountsID);
            
            CreateTable(
                "EXCEED.BudgetPlan",
                c => new
                    {
                        BudgetPlanID = c.Int(nullable: false, identity: true),
                        BudgetYear = c.String(),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Unit = c.String(),
                        TotalYearlyBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanID);
            
            CreateTable(
                "EXCEED.BudgetPlanMonthly",
                c => new
                    {
                        BudgetPlanMonthlyID = c.Int(nullable: false, identity: true),
                        BudgetDefineID = c.Int(nullable: false),
                        BudgetMonth = c.String(),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Unit = c.String(),
                        MonthlyBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        BudgetPlan_BudgetPlanID = c.Int(),
                        BudgetTransfer_BudgetTransferID = c.Int(),
                    })
                .PrimaryKey(t => t.BudgetPlanMonthlyID)
                .ForeignKey("EXCEED.BudgetPlan", t => t.BudgetPlan_BudgetPlanID)
                .ForeignKey("EXCEED.BudgetTransfer", t => t.BudgetTransfer_BudgetTransferID)
                .Index(t => t.BudgetPlan_BudgetPlanID)
                .Index(t => t.BudgetTransfer_BudgetTransferID);
            
            CreateTable(
                "EXCEED.BudgetTransfer",
                c => new
                    {
                        BudgetTransferID = c.Int(nullable: false, identity: true),
                        FromBudgetYear = c.String(nullable: false),
                        ToBudgetYear = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Unit = c.String(nullable: false),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetTransferID);
            
            CreateTable(
                "EXCEED.BudgetUsage",
                c => new
                    {
                        BudgetUsageID = c.Int(nullable: false, identity: true),
                        BudgetYear = c.String(nullable: false),
                        BudgetMonth = c.String(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Unit = c.String(nullable: false),
                        MonthlyActuals = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetUsageID);
            
            CreateTable(
                "EXCEED.ExternalCrew",
                c => new
                    {
                        ExternalCrewID = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        Name = c.String(),
                        ApprovalStatuses = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalCrewID);
            
            CreateTable(
                "EXCEED.ExternalMaterial",
                c => new
                    {
                        ExternalMaterialID = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        MaterialName = c.String(),
                        ApprovalStatuses = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalMaterialID);
            
            AddColumn("EXCEED.AssignedEquipment", "CategoryRef", c => c.String());
            AddColumn("EXCEED.AssignedEquipment", "EquipmentStatus", c => c.String());
            AddColumn("EXCEED.AssignedEquipment", "RequestDesc", c => c.String());
            AddColumn("EXCEED.JobOrder", "EquipmentName", c => c.String());
            AddColumn("EXCEED.JobOrder", "PlateNum", c => c.String());
            AddColumn("EXCEED.JobOrder", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.JobOrder", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.JobOrder", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.JobOrderDetail", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.ServiceTool", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.ServiceCrew", "Type", c => c.String());
            AddColumn("EXCEED.OtherCompanyMaintenance", "EquipmentName", c => c.String(nullable: false));
            AddColumn("EXCEED.RentRequest", "EquipmnetStatus", c => c.String());
            AddColumn("EXCEED.ServiceSpare", "Type", c => c.String());
            AddColumn("EXCEED.ServiceSpare", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.VehicleDemand", "EquipmnetStatus", c => c.String());
            AddColumn("EXCEED.VehicleReturn", "ReturnStatus", c => c.String());
            AlterColumn("EXCEED.Fleets", "PurchasingCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanFuelConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanIncome", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanEquipmentsPittyCashExpenditure", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("EXCEED.Fuel", "Project", c => c.String());
            AlterColumn("EXCEED.Fuel", "FuelType", c => c.String());
            AlterColumn("EXCEED.Fuel", "FuelStandard", c => c.String());
            AlterColumn("EXCEED.Fuel", "InitialKmHr", c => c.Int());
            AlterColumn("EXCEED.Fuel", "FinalKmHr", c => c.Int());
            AlterColumn("EXCEED.Fuel", "PlanKmHr", c => c.Int());
            AlterColumn("EXCEED.Fuel", "FuelFilledAmount", c => c.Int());
            AlterColumn("EXCEED.Fuel", "FuelSource", c => c.String());
            AlterColumn("EXCEED.Fuel", "UnitPrice", c => c.Int());
            AlterColumn("EXCEED.Fuel", "ReceiptNo", c => c.String());
            AlterColumn("EXCEED.DriverVehicleTransfer", "FromOperatorId", c => c.String());
            AlterColumn("EXCEED.DriverVehicleTransfer", "ToOperatorId", c => c.String());
            AlterColumn("EXCEED.Service", "PartCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("EXCEED.Service", "LabourCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("EXCEED.RentFromExternal", "CompanyName", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "CompanyAddress", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "CompanyPhno", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "ProjectLocation", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "Description", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "NameBankBranch", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "BankFullAddress", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "BankSlipNo", c => c.String());
            AlterColumn("EXCEED.RentFromExternal", "AdvancePayment", c => c.Int());
            AlterColumn("EXCEED.TimeSheet", "RentalRate", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("EXCEED.Fuel", "Date");
            DropColumn("EXCEED.OtherCompanyMaintenance", "MaintenanceName");
            DropColumn("EXCEED.RentRequest", "EquipmentStatus");
            DropColumn("EXCEED.VehicleDemand", "EquipmentStatus");
            DropTable("CRM.Account");
        }
        
        public override void Down()
        {
            CreateTable(
                "CRM.Account",
                c => new
                    {
                        AccountID = c.Int(nullable: false, identity: true),
                        ControlAccount = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Parent = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccountID);
            
            AddColumn("EXCEED.VehicleDemand", "EquipmentStatus", c => c.String());
            AddColumn("EXCEED.RentRequest", "EquipmentStatus", c => c.String(nullable: false));
            AddColumn("EXCEED.OtherCompanyMaintenance", "MaintenanceName", c => c.String(nullable: false));
            AddColumn("EXCEED.Fuel", "Date", c => c.DateTime());
            DropForeignKey("EXCEED.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID", "EXCEED.BudgetTransfer");
            DropForeignKey("EXCEED.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID", "EXCEED.BudgetPlan");
            DropIndex("EXCEED.BudgetPlanMonthly", new[] { "BudgetTransfer_BudgetTransferID" });
            DropIndex("EXCEED.BudgetPlanMonthly", new[] { "BudgetPlan_BudgetPlanID" });
            AlterColumn("EXCEED.TimeSheet", "RentalRate", c => c.Int());
            AlterColumn("EXCEED.RentFromExternal", "AdvancePayment", c => c.Int(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "BankSlipNo", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "BankFullAddress", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "NameBankBranch", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "Description", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "ProjectLocation", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "CompanyPhno", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "CompanyAddress", c => c.String(nullable: false));
            AlterColumn("EXCEED.RentFromExternal", "CompanyName", c => c.String(nullable: false));
            AlterColumn("EXCEED.Service", "LabourCost", c => c.Int());
            AlterColumn("EXCEED.Service", "PartCost", c => c.Int());
            AlterColumn("EXCEED.DriverVehicleTransfer", "ToOperatorId", c => c.Guid(nullable: false));
            AlterColumn("EXCEED.DriverVehicleTransfer", "FromOperatorId", c => c.Guid(nullable: false));
            AlterColumn("EXCEED.Fuel", "ReceiptNo", c => c.String(nullable: false));
            AlterColumn("EXCEED.Fuel", "UnitPrice", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fuel", "FuelSource", c => c.String(nullable: false));
            AlterColumn("EXCEED.Fuel", "FuelFilledAmount", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fuel", "PlanKmHr", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fuel", "FinalKmHr", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fuel", "InitialKmHr", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fuel", "FuelStandard", c => c.String(nullable: false));
            AlterColumn("EXCEED.Fuel", "FuelType", c => c.String(nullable: false));
            AlterColumn("EXCEED.Fuel", "Project", c => c.String(nullable: false));
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanEquipmentsPittyCashExpenditure", c => c.Int());
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanIncome", c => c.Int(nullable: false));
            AlterColumn("EXCEED.EquipmentPlanandEvaluations", "PlanFuelConsumption", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Fleets", "PurchasingCost", c => c.Int());
            DropColumn("EXCEED.VehicleReturn", "ReturnStatus");
            DropColumn("EXCEED.VehicleDemand", "EquipmnetStatus");
            DropColumn("EXCEED.ServiceSpare", "ApprovalStatus");
            DropColumn("EXCEED.ServiceSpare", "Type");
            DropColumn("EXCEED.RentRequest", "EquipmnetStatus");
            DropColumn("EXCEED.OtherCompanyMaintenance", "EquipmentName");
            DropColumn("EXCEED.ServiceCrew", "Type");
            DropColumn("EXCEED.ServiceTool", "ApprovalStatus");
            DropColumn("EXCEED.JobOrderDetail", "ApprovalStatus");
            DropColumn("EXCEED.JobOrder", "ApprovalStatus");
            DropColumn("EXCEED.JobOrder", "MaterialCost");
            DropColumn("EXCEED.JobOrder", "LabourCost");
            DropColumn("EXCEED.JobOrder", "PlateNum");
            DropColumn("EXCEED.JobOrder", "EquipmentName");
            DropColumn("EXCEED.AssignedEquipment", "RequestDesc");
            DropColumn("EXCEED.AssignedEquipment", "EquipmentStatus");
            DropColumn("EXCEED.AssignedEquipment", "CategoryRef");
            DropTable("EXCEED.ExternalMaterial");
            DropTable("EXCEED.ExternalCrew");
            DropTable("EXCEED.BudgetUsage");
            DropTable("EXCEED.BudgetTransfer");
            DropTable("EXCEED.BudgetPlanMonthly");
            DropTable("EXCEED.BudgetPlan");
            DropTable("CRM.Accounts");
            CreateIndex("EXCEED.JobOrder", "FleetsID");
            AddForeignKey("EXCEED.JobOrder", "FleetsID", "EXCEED.Fleets", "FleetsID", cascadeDelete: true);
        }
    }
}
