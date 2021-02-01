namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrmbudget : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetRequisitionSecondValidation",
                c => new
                    {
                        FixedAssetRequisitionSecondValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetRequisitionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetRequisitionSecondValidationId)
                .ForeignKey("EXCEED.FixedAssetRequisition", t => t.FixedAssetRequisitionId, cascadeDelete: true)
                .Index(t => t.FixedAssetRequisitionId);
            
            CreateTable(
                "CRM.InventoryCateoryMap",
                c => new
                    {
                        InventoryCateoryMapId = c.Guid(nullable: false),
                        InventoryCategoryId = c.String(),
                        Category = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventoryCateoryMapId);
            
            AddColumn("EXCEED.FixedAssetRequisitionValidation", "OrgBranchName", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "ComponentName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.ExternalComponent", "EnterpriseName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.ExternalComponent", "Manufacturer", c => c.String(nullable: false));
            AlterColumn("FleetManagement.ExternalComponent", "Model", c => c.String(nullable: false));
            AlterColumn("FleetManagement.ExternalComponent", "MaintaingCompany", c => c.String(nullable: false));
            AlterColumn("FleetManagement.ExternalComponentDetail", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetRequisitionSecondValidation", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropIndex("EXCEED.FixedAssetRequisitionSecondValidation", new[] { "FixedAssetRequisitionId" });
            AlterColumn("FleetManagement.ExternalComponentDetail", "Name", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "MaintaingCompany", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "Model", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "Manufacturer", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "EnterpriseName", c => c.String());
            AlterColumn("FleetManagement.ExternalComponent", "ComponentName", c => c.String());
            DropColumn("EXCEED.FixedAssetRequisitionValidation", "OrgBranchName");
            DropTable("CRM.InventoryCateoryMap");
            DropTable("EXCEED.FixedAssetRequisitionSecondValidation");
        }
    }
}
