namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleet2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "FleetManagement.EquipmentPlanandEvaluations", name: "Fleets_FleetsID", newName: "Fleet_FleetsID");
            RenameIndex(table: "FleetManagement.EquipmentPlanandEvaluations", name: "IX_Fleets_FleetsID", newName: "IX_Fleet_FleetsID");
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "FleetId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "PlateNo", c => c.String());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "SerialNo", c => c.String());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "ChasisNo", c => c.String());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "AssetCode", c => c.String());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "WorkingVolume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "VolumeUnit", c => c.String());
            AddColumn("FleetManagement.TimeSheet", "PlateSerialNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FleetManagement.TimeSheet", "PlateSerialNo");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "VolumeUnit");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "WorkingVolume");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "AssetCode");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "ChasisNo");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "SerialNo");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "PlateNo");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "FleetId");
            RenameIndex(table: "FleetManagement.EquipmentPlanandEvaluations", name: "IX_Fleet_FleetsID", newName: "IX_Fleets_FleetsID");
            RenameColumn(table: "FleetManagement.EquipmentPlanandEvaluations", name: "Fleet_FleetsID", newName: "Fleets_FleetsID");
        }
    }
}
