namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.InternalMachinaryRent", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.MachinaryRent", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnEquipment", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEstmnIndexLabour", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropIndex("Construction.InternalMachinaryRent", new[] { "ItemCode" });
            DropIndex("Construction.MachinaryRent", new[] { "ItemCode" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "ItemCode" });
            DropIndex("Construction.ProjectEstmnIndexLabour", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstmnBoqSummary", new[] { "ProjectEstimationNameId" });
            RenameColumn(table: "Construction.ProjectEstimationBoq", name: "ProjectEstimationNameId", newName: "ProjectEstimationName_ProjectEstimationNameId");
            AddColumn("HRM.AllowanceDefination", "OrgBranchName", c => c.String());
            AddColumn("HRM.AllowanceDefination", "DateCreated", c => c.DateTime());
            AddColumn("HRM.AllowanceDefination", "LastModified", c => c.DateTime());
            AddColumn("HRM.AllowanceDefination", "CreatedBy", c => c.String());
            AddColumn("HRM.AllowanceDefination", "ModifiedBy", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "OrgBranchName", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "DateCreated", c => c.DateTime());
            AddColumn("HRM.AllowancePerEmployee", "LastModified", c => c.DateTime());
            AddColumn("HRM.AllowancePerEmployee", "CreatedBy", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "ModifiedBy", c => c.String());
            AddColumn("HRM.AllowanceTypeRule", "OrgBranchName", c => c.String());
            AddColumn("HRM.AllowanceTypeRule", "DateCreated", c => c.DateTime());
            AddColumn("HRM.AllowanceTypeRule", "LastModified", c => c.DateTime());
            AddColumn("HRM.AllowanceTypeRule", "CreatedBy", c => c.String());
            AddColumn("HRM.AllowanceTypeRule", "ModifiedBy", c => c.String());
            AddColumn("Construction.InternalMachinaryRent", "AssetNumber", c => c.String(nullable: false, maxLength: 128));
            AddColumn("Construction.MachinaryRent", "FleetEquipmentRentFromExternalID", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimationBoq", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstmnEquipment", "AssetNumber", c => c.String(nullable: false, maxLength: 100));
            AddColumn("Construction.ProjectEstmnEquipment", "EquipmentType", c => c.String());
            AddColumn("Construction.ProjectEstmnIndexLabour", "FormHr", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectEstmnIndexLabour", "Title", c => c.String(nullable: false, maxLength: 50));
            AddColumn("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", c => c.Int(nullable: false));
            AlterColumn("HRM.Employee", "EmployeeCode", c => c.String());
            AlterColumn("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", c => c.Int());
            CreateIndex("Construction.InternalMachinaryRent", "AssetNumber");
            CreateIndex("Construction.MachinaryRent", "FleetEquipmentRentFromExternalID");
            CreateIndex("Construction.ProjectEstimationBoq", "ProjectInformationId");
            CreateIndex("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId");
            CreateIndex("Construction.ProjectEstmnBoqSummary", "ProjectInformationId");
            AddForeignKey("Construction.InternalMachinaryRent", "AssetNumber", "EXCEED.FleetEquipmentRegistration", "AssetNumber", cascadeDelete: true);
            AddForeignKey("Construction.MachinaryRent", "FleetEquipmentRentFromExternalID", "EXCEED.FleetEquipmentRentFromExternal", "FleetEquipmentRentFromExternalID", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimationBoq", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour", "ProjectEstimationIndexLabourId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId");
            DropColumn("Construction.InternalMachinaryRent", "ItemCode");
            DropColumn("Construction.MachinaryRent", "ItemCode");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId");
            DropColumn("Construction.ProjectEstmnDirectLabour", "LabourName");
            DropColumn("Construction.ProjectEstmnEquipment", "ItemCode");
            DropColumn("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstmnEquipment", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("Construction.ProjectEstmnDirectLabour", "LabourName", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", c => c.Int(nullable: false));
            AddColumn("Construction.MachinaryRent", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("Construction.InternalMachinaryRent", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.MachinaryRent", "FleetEquipmentRentFromExternalID", "EXCEED.FleetEquipmentRentFromExternal");
            DropForeignKey("Construction.InternalMachinaryRent", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropIndex("Construction.ProjectEstmnBoqSummary", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectEstmnDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectEstimationName_ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectInformationId" });
            DropIndex("Construction.MachinaryRent", new[] { "FleetEquipmentRentFromExternalID" });
            DropIndex("Construction.InternalMachinaryRent", new[] { "AssetNumber" });
            AlterColumn("Construction.ProjectEstimationBoq", "ProjectEstimationName_ProjectEstimationNameId", c => c.Int(nullable: false));
            AlterColumn("HRM.Employee", "EmployeeCode", c => c.String(nullable: false));
            DropColumn("Construction.ProjectEstmnBoqSummary", "ProjectInformationId");
            DropColumn("Construction.ProjectEstmnIndexLabour", "Title");
            DropColumn("Construction.ProjectEstmnIndexLabour", "FormHr");
            DropColumn("Construction.ProjectEstmnEquipment", "EquipmentType");
            DropColumn("Construction.ProjectEstmnEquipment", "AssetNumber");
            DropColumn("Construction.ProjectEstmnDirectLabour", "ProjectEstimationIndexLabourId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId");
            DropColumn("Construction.ProjectEstimationBoq", "ProjectInformationId");
            DropColumn("Construction.MachinaryRent", "FleetEquipmentRentFromExternalID");
            DropColumn("Construction.InternalMachinaryRent", "AssetNumber");
            DropColumn("HRM.AllowanceTypeRule", "ModifiedBy");
            DropColumn("HRM.AllowanceTypeRule", "CreatedBy");
            DropColumn("HRM.AllowanceTypeRule", "LastModified");
            DropColumn("HRM.AllowanceTypeRule", "DateCreated");
            DropColumn("HRM.AllowanceTypeRule", "OrgBranchName");
            DropColumn("HRM.AllowancePerEmployee", "ModifiedBy");
            DropColumn("HRM.AllowancePerEmployee", "CreatedBy");
            DropColumn("HRM.AllowancePerEmployee", "LastModified");
            DropColumn("HRM.AllowancePerEmployee", "DateCreated");
            DropColumn("HRM.AllowancePerEmployee", "OrgBranchName");
            DropColumn("HRM.AllowanceDefination", "ModifiedBy");
            DropColumn("HRM.AllowanceDefination", "CreatedBy");
            DropColumn("HRM.AllowanceDefination", "LastModified");
            DropColumn("HRM.AllowanceDefination", "DateCreated");
            DropColumn("HRM.AllowanceDefination", "OrgBranchName");
            RenameColumn(table: "Construction.ProjectEstimationBoq", name: "ProjectEstimationName_ProjectEstimationNameId", newName: "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstmnIndexLabour", "LookupId");
            CreateIndex("Construction.ProjectEstmnEquipment", "ItemCode");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstimationBoq", "ProjectEstimationNameId");
            CreateIndex("Construction.MachinaryRent", "ItemCode");
            CreateIndex("Construction.InternalMachinaryRent", "ItemCode");
            AddForeignKey("Construction.ProjectEstimationBoq", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnIndexLabour", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnEquipment", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.MachinaryRent", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("Construction.InternalMachinaryRent", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
