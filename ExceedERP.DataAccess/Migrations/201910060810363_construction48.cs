namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction48 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate");
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "UnitRateId" });
            AddColumn("Construction.ProjectInformation", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectInformation", "DeletedOn", c => c.DateTime());
            AddColumn("Construction.ProjectInformation", "DeletedBy", c => c.String());
            AddColumn("Construction.WorkItemPerProject", "HasAdditionalTransportationCost", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "TransportCostRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.WorkItemPerProject", "CalculatedTransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.WorkItemPerProject", "TotalUnitCostIncludingTransport", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.WorkItemPerProject", "ThicknessFactor", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "HasAdditionalTransportationCost", c => c.Boolean(nullable: false));
            AddColumn("Construction.GenericWorkItemPerProject", "TransportCostRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "CalculatedTransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "TotalUnitCostIncludingTransport", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "ThicknessFactor", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOff", "IsReinforcementbarTakeOff", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectTakeOffLineForKg", "Location", c => c.String());
            AddColumn("Construction.ProjectTakeOffLineForKg", "DeformedBarId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOffLineForKg", "Diameter", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLineForKg", "TotalWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLineForKg", "DateOfInformation", c => c.DateTime(nullable: false));
            AddColumn("Construction.SubProjectType", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("Construction.SubProjectType", "DeletedOn", c => c.DateTime());
            AddColumn("Construction.SubProjectType", "DeletedBy", c => c.String());
            AddColumn("Construction.TakeOffPerProject", "IsReinforcementbarTakeOff", c => c.Boolean(nullable: false));
            AlterColumn("Construction.ProjectTakeOff", "TakeOffNo", c => c.String());
            AlterColumn("Construction.ProjectTakeOffLineForKg", "Description", c => c.String());
            AlterColumn("Construction.TakeOffLineForKgPerProject", "Description", c => c.String());
            AlterColumn("Construction.TakeOffLinePerProject", "Description", c => c.String());
            CreateIndex("Construction.ProjectTakeOffLineForKg", "DeformedBarId");
            AddForeignKey("Construction.ProjectTakeOffLineForKg", "DeformedBarId", "Construction.DeformedBar", "DeformedBarId", cascadeDelete: true);
            DropColumn("Construction.ProjectTakeOffLineForKg", "UnitRateId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ProjectTakeOffLineForKg", "UnitRateId", c => c.Int(nullable: false));
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "DeformedBarId", "Construction.DeformedBar");
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "DeformedBarId" });
            AlterColumn("Construction.TakeOffLinePerProject", "Description", c => c.String(maxLength: 400));
            AlterColumn("Construction.TakeOffLineForKgPerProject", "Description", c => c.String(maxLength: 200));
            AlterColumn("Construction.ProjectTakeOffLineForKg", "Description", c => c.String(maxLength: 200));
            AlterColumn("Construction.ProjectTakeOff", "TakeOffNo", c => c.String(maxLength: 100));
            DropColumn("Construction.TakeOffPerProject", "IsReinforcementbarTakeOff");
            DropColumn("Construction.SubProjectType", "DeletedBy");
            DropColumn("Construction.SubProjectType", "DeletedOn");
            DropColumn("Construction.SubProjectType", "IsDeleted");
            DropColumn("Construction.ProjectTakeOffLineForKg", "DateOfInformation");
            DropColumn("Construction.ProjectTakeOffLineForKg", "TotalWeight");
            DropColumn("Construction.ProjectTakeOffLineForKg", "Diameter");
            DropColumn("Construction.ProjectTakeOffLineForKg", "DeformedBarId");
            DropColumn("Construction.ProjectTakeOffLineForKg", "Location");
            DropColumn("Construction.ProjectTakeOff", "IsReinforcementbarTakeOff");
            DropColumn("Construction.GenericWorkItemPerProject", "ThicknessFactor");
            DropColumn("Construction.GenericWorkItemPerProject", "TotalUnitCostIncludingTransport");
            DropColumn("Construction.GenericWorkItemPerProject", "CalculatedTransportCost");
            DropColumn("Construction.GenericWorkItemPerProject", "TransportCostRate");
            DropColumn("Construction.GenericWorkItemPerProject", "HasAdditionalTransportationCost");
            DropColumn("Construction.WorkItemPerProject", "ThicknessFactor");
            DropColumn("Construction.WorkItemPerProject", "TotalUnitCostIncludingTransport");
            DropColumn("Construction.WorkItemPerProject", "CalculatedTransportCost");
            DropColumn("Construction.WorkItemPerProject", "TransportCostRate");
            DropColumn("Construction.WorkItemPerProject", "HasAdditionalTransportationCost");
            DropColumn("Construction.ProjectInformation", "DeletedBy");
            DropColumn("Construction.ProjectInformation", "DeletedOn");
            DropColumn("Construction.ProjectInformation", "IsDeleted");
            CreateIndex("Construction.ProjectTakeOffLineForKg", "UnitRateId");
            AddForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate", "UnitRateId", cascadeDelete: true);
        }
    }
}
