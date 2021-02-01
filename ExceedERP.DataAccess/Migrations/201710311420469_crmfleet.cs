namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.EquipmentLocation",
                c => new
                    {
                        EquipmentLocationId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        Name = c.String(),
                        Condition = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentLocationId);
            
            CreateTable(
                "EXCEED.ForeignCurrencyProcess",
                c => new
                    {
                        ForeignCurrencyProcessId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ForeignCurrencyProcessId);
            
            AddColumn("FleetManagement.Fleets", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Fleets", "RentId", c => c.Guid(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "ForeignCurrencyProcessId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Service", "STDTime", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternal", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.RepairType", "Code", c => c.String(nullable: false));
            AddColumn("FleetManagement.TimeSheet", "FleetTypeID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TimeSheet", "FleetCategoryID", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.TireReturn", "SerialNo", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetControl", "Percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.EquipmentTyre", "SerialNo", c => c.String(nullable: false));
            DropColumn("FleetManagement.Service", "PartCost");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.Service", "PartCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("FleetManagement.EquipmentTyre", "SerialNo", c => c.String());
            AlterColumn("Budget.BudgetControl", "Percentage", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TireReturn", "SerialNo", c => c.Int(nullable: false));
            DropColumn("FleetManagement.TimeSheet", "FleetCategoryID");
            DropColumn("FleetManagement.TimeSheet", "FleetTypeID");
            DropColumn("FleetManagement.RepairType", "Code");
            DropColumn("FleetManagement.RentFromExternal", "EndDate");
            DropColumn("FleetManagement.RentFromExternal", "StartDate");
            DropColumn("FleetManagement.Service", "STDTime");
            DropColumn("EXCEED.ForeignPurchase", "ForeignCurrencyProcessId");
            DropColumn("FleetManagement.Fleets", "RentId");
            DropColumn("FleetManagement.Fleets", "IsExternal");
            DropTable("EXCEED.ForeignCurrencyProcess");
            DropTable("FleetManagement.EquipmentLocation");
        }
    }
}
