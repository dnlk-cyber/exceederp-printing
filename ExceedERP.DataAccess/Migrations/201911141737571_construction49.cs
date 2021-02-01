namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction49 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject");
            DropIndex("Construction.WorkItemDuration", new[] { "GenericWorkItemPerProjectId" });
            CreateTable(
                "Construction.ResourceEstimationEquipment",
                c => new
                    {
                        ResourceEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        WorkItemDurationId = c.Int(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        Number = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactor = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactorAndNumberPerCrew = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NumberPerDay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HourlyCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCostPerDay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceEstimationEquipmentId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemDuration", t => t.WorkItemDurationId, cascadeDelete: true)
                .Index(t => t.WorkItemDurationId)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "Construction.ResourceEstimationLabour",
                c => new
                    {
                        ResourceEstimationLabourId = c.Int(nullable: false, identity: true),
                        WorkItemDurationId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Number = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactor = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactorAndNumberPerCrew = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NumberPerDay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCostPerDay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceEstimationLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemDuration", t => t.WorkItemDurationId, cascadeDelete: true)
                .Index(t => t.WorkItemDurationId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.ResourceEstimationMaterial",
                c => new
                    {
                        ResourceEstimationMaterialId = c.Int(nullable: false, identity: true),
                        WorkItemDurationId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(maxLength: 128),
                        Unit = c.String(),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceEstimationMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("Construction.WorkItemDuration", t => t.WorkItemDurationId, cascadeDelete: true)
                .Index(t => t.WorkItemDurationId)
                .Index(t => t.ItemCode);
            
            AddColumn("Construction.WorkItemDuration", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "WorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "Parent", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "Source", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "BlockName", c => c.String());
            DropColumn("Construction.WorkItemDuration", "GenericWorkItemPerProjectId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", c => c.Int(nullable: false));
            DropForeignKey("Construction.ResourceEstimationMaterial", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropForeignKey("Construction.ResourceEstimationMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ResourceEstimationLabour", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropForeignKey("Construction.ResourceEstimationLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ResourceEstimationEquipment", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropForeignKey("Construction.ResourceEstimationEquipment", "FleetsID", "FleetManagement.Fleets");
            DropIndex("Construction.ResourceEstimationMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ResourceEstimationMaterial", new[] { "WorkItemDurationId" });
            DropIndex("Construction.ResourceEstimationLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ResourceEstimationLabour", new[] { "WorkItemDurationId" });
            DropIndex("Construction.ResourceEstimationEquipment", new[] { "FleetsID" });
            DropIndex("Construction.ResourceEstimationEquipment", new[] { "WorkItemDurationId" });
            DropColumn("Construction.WorkItemDuration", "BlockName");
            DropColumn("Construction.WorkItemDuration", "Source");
            DropColumn("Construction.WorkItemDuration", "Parent");
            DropColumn("Construction.WorkItemDuration", "WorkItemPerProjectId");
            DropColumn("Construction.WorkItemDuration", "ProjectInformationId");
            DropTable("Construction.ResourceEstimationMaterial");
            DropTable("Construction.ResourceEstimationLabour");
            DropTable("Construction.ResourceEstimationEquipment");
            CreateIndex("Construction.WorkItemDuration", "GenericWorkItemPerProjectId");
            AddForeignKey("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject", "GenericWorkItemPerProjectId", cascadeDelete: true);
        }
    }
}
