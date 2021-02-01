namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction9 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "Construction.ProjectAdvancePayment", newName: "ProjectAdvancePayments");
            DropForeignKey("Construction.MaterialPriceIndex", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.MaterialPriceIndex", new[] { "GLPeriodId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectInformationId" });
            CreateTable(
                "Construction.ProjectCustomWorkItemContent",
                c => new
                    {
                        ProjectCustomWorkItemContentId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectCustomWorkItemId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCustomWorkItemContentId)
                .ForeignKey("Construction.ProjectCustomWorkItem", t => t.ProjectCustomWorkItemId, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId)
                .Index(t => t.ProjectCustomWorkItemId);
            
            CreateTable(
                "Construction.ProjectCustomWorkItem",
                c => new
                    {
                        ProjectCustomWorkItemId = c.Int(nullable: false, identity: true),
                        WorkItemName = c.String(maxLength: 200),
                        ProjectInformationId = c.Int(nullable: false),
                        TotalUnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectType = c.Int(nullable: false),
                        BlockId = c.Int(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCustomWorkItemId);
            
            AddColumn("Global.DocumentApprovalRule", "DocumentType", c => c.Int(nullable: false));
            AddColumn("Construction.MachinaryRent", "ItemCode", c => c.String());
            DropColumn("Construction.MachinaryRent", "FuelType");
            DropTable("Construction.ProjectPaymenAdvance");
        }
        
        public override void Down()
        {
            CreateTable(
                "Construction.ProjectPaymenAdvance",
                c => new
                    {
                        ProjectPaymenAdvanceId = c.Int(nullable: false, identity: true),
                        AdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPaymenAdvanceId);
            
            AddColumn("Construction.MachinaryRent", "FuelType", c => c.Int(nullable: false));
            DropForeignKey("Construction.ProjectCustomWorkItemContent", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectCustomWorkItemContent", "ProjectCustomWorkItemId", "Construction.ProjectCustomWorkItem");
            DropIndex("Construction.ProjectCustomWorkItemContent", new[] { "ProjectCustomWorkItemId" });
            DropIndex("Construction.ProjectCustomWorkItemContent", new[] { "WorkItemPerProjectId" });
            DropColumn("Construction.MachinaryRent", "ItemCode");
            DropColumn("Global.DocumentApprovalRule", "DocumentType");
            DropTable("Construction.ProjectCustomWorkItem");
            DropTable("Construction.ProjectCustomWorkItemContent");
            CreateIndex("Construction.ProjectPaymenAdvance", "ProjectInformationId");
            CreateIndex("Construction.MaterialPriceIndex", "GLPeriodId");
            AddForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.MaterialPriceIndex", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            RenameTable(name: "Construction.ProjectAdvancePayments", newName: "ProjectAdvancePayment");
        }
    }
}
