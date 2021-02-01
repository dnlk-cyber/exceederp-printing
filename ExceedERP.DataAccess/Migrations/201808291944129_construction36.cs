namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction36 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.WorkItemPerProject", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.WorkItemPerProject", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.WorkItemPerProject", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ConsReceivableInvoiceDetail", "Correction", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ConsReceivableInvoiceDetail", "Others", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectEstimnWorkItem", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectEstimnWorkItem", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectEstimnWorkItem", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "MaterialCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "EquipmentCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.GenericWorkItemPerProject", "LaborCostPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.GenericWorkItemPerProject", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("Construction.GenericWorkItemPerProject", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.GenericWorkItemPerProject", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "EquipmentOutput", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "LabourOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.GenericWorkItemPerProject", "LaborCostPerUnit");
            DropColumn("Construction.GenericWorkItemPerProject", "EquipmentCostPerUnit");
            DropColumn("Construction.GenericWorkItemPerProject", "MaterialCostPerUnit");
            DropColumn("Construction.ProjectEstimnWorkItem", "LaborCostPerUnit");
            DropColumn("Construction.ProjectEstimnWorkItem", "EquipmentCostPerUnit");
            DropColumn("Construction.ProjectEstimnWorkItem", "MaterialCostPerUnit");
            DropColumn("Construction.ConsReceivableInvoiceDetail", "Others");
            DropColumn("Construction.ConsReceivableInvoiceDetail", "Correction");
            DropColumn("Construction.WorkItemPerProject", "LaborCostPerUnit");
            DropColumn("Construction.WorkItemPerProject", "EquipmentCostPerUnit");
            DropColumn("Construction.WorkItemPerProject", "MaterialCostPerUnit");
        }
    }
}
