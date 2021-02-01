namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "Category", c => c.String());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "Type", c => c.String());
            AddColumn("FleetManagement.EquipmentTyre", "Duration", c => c.String());
            AlterColumn("FleetManagement.EquipmentTyre", "StoreCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("FleetManagement.EquipmentTyre", "StoreCode", c => c.String(nullable: false));
            DropColumn("FleetManagement.EquipmentTyre", "Duration");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "Type");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "Category");
        }
    }
}
