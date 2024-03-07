namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufacfurniture : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.ManufacturingMaterialCategoryItem",
                c => new
                    {
                        ManufacturingMaterialCategoryItemId = c.Int(nullable: false, identity: true),
                        InventorySubCategoryCode = c.String(),
                        ItemName = c.String(),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        ManufacturingMaterialCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ManufacturingMaterialCategoryItemId);
            
            AddColumn("Manufacturing.FurnitureBOQLabor", "EstimatedMinute", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureBOQMaterial", "ManufacturingMaterialCategoryId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureBOQMaterial", "ManufacturingMaterialCategoryItemId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "FurnitureBillOfQuantityId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "Status", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "Remark", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "VoidBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "VoidTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "OnlineApprovedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "OrgBranchName", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "SendForApprovalBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureMaterialCosts", "ManufacturingMaterialCategoryItemId", c => c.Int(nullable: false));
            AlterColumn("Manufacturing.FurnitureBOQMaterial", "ItemCode", c => c.String());
            DropColumn("Manufacturing.FurnitureBOQLabor", "EstimatedHour");
            DropColumn("Manufacturing.FurnitureCostEstimations", "Date");
            DropColumn("Manufacturing.FurnitureCostEstimations", "JT");
            DropColumn("Manufacturing.FurnitureCostEstimations", "JobNumber");
            DropColumn("Manufacturing.FurnitureCostEstimations", "Others");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsChecked");
            DropColumn("Manufacturing.FurnitureCostEstimations", "CheckedBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "ApprovedBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsApproved");
            DropTable("Manufacturing.ManufacturingMaterialCategoryDetail");
        }
        
        public override void Down()
        {
            CreateTable(
                "Manufacturing.ManufacturingMaterialCategoryDetail",
                c => new
                    {
                        ManufacturingMaterialCategoryDetailId = c.Int(nullable: false, identity: true),
                        InventorySubCategoryCode = c.String(),
                        ItemName = c.String(),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        ManufacturingMaterialCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ManufacturingMaterialCategoryDetailId);
            
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "ApprovedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "CheckedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsChecked", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "Others", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "JobNumber", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "JT", c => c.String());
            AddColumn("Manufacturing.FurnitureCostEstimations", "Date", c => c.DateTime(nullable: false));
            AddColumn("Manufacturing.FurnitureBOQLabor", "EstimatedHour", c => c.Single(nullable: false));
            AlterColumn("Manufacturing.FurnitureBOQMaterial", "ItemCode", c => c.Int(nullable: false));
            DropColumn("Manufacturing.FurnitureMaterialCosts", "ManufacturingMaterialCategoryItemId");
            DropColumn("Manufacturing.FurnitureCostEstimations", "SendForApprovalTime");
            DropColumn("Manufacturing.FurnitureCostEstimations", "SendForApprovalBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "OrgBranchName");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsSendForApproval");
            DropColumn("Manufacturing.FurnitureCostEstimations", "OnlineTransferredTime");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineTransferredBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineTransferred");
            DropColumn("Manufacturing.FurnitureCostEstimations", "OnlineApprovedTime");
            DropColumn("Manufacturing.FurnitureCostEstimations", "OnlineApprovedBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsOnlineApproved");
            DropColumn("Manufacturing.FurnitureCostEstimations", "VoidTime");
            DropColumn("Manufacturing.FurnitureCostEstimations", "VoidBy");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsVoid");
            DropColumn("Manufacturing.FurnitureCostEstimations", "Remark");
            DropColumn("Manufacturing.FurnitureCostEstimations", "Status");
            DropColumn("Manufacturing.FurnitureCostEstimations", "FurnitureBillOfQuantityId");
            DropColumn("Manufacturing.FurnitureBOQMaterial", "ManufacturingMaterialCategoryItemId");
            DropColumn("Manufacturing.FurnitureBOQMaterial", "ManufacturingMaterialCategoryId");
            DropColumn("Manufacturing.FurnitureBOQLabor", "EstimatedMinute");
            DropTable("Manufacturing.ManufacturingMaterialCategoryItem");
        }
    }
}
