namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.FleetIFuelConsumption",
                c => new
                    {
                        FleetIFuelConsumptionId = c.Int(nullable: false, identity: true),
                        EquipmentTypeId = c.Guid(nullable: false),
                        MinConsupmtion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxConsupmtion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetIFuelConsumptionId)
                .ForeignKey("FleetManagement.EquipmentType", t => t.EquipmentTypeId, cascadeDelete: true)
                .Index(t => t.EquipmentTypeId);
            
            AddColumn("FleetManagement.CheckList", "EquipmentCategory", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Accident", "CostcoveredbyInsurance", c => c.Int());
            AlterColumn("FleetManagement.Accident", "CostcoveredbyCompany", c => c.Int());
            AlterColumn("FleetManagement.Accident", "CostcoveredbyOperator", c => c.Int());
            AlterColumn("FleetManagement.Accident", "CostcoveredbyOtherBodies", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("FleetManagement.FleetIFuelConsumption", "EquipmentTypeId", "FleetManagement.EquipmentType");
            DropIndex("FleetManagement.FleetIFuelConsumption", new[] { "EquipmentTypeId" });
            AlterColumn("FleetManagement.Accident", "CostcoveredbyOtherBodies", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Accident", "CostcoveredbyOperator", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Accident", "CostcoveredbyCompany", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Accident", "CostcoveredbyInsurance", c => c.Int(nullable: false));
            DropColumn("FleetManagement.CheckList", "EquipmentCategory");
            DropTable("FleetManagement.FleetIFuelConsumption");
        }
    }
}
