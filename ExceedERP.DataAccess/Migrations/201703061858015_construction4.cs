namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction4 : DbMigration
    {
        public override void Up()
        {
           // DropForeignKey("EXCEED.LetterSender", "BookNo", "Archive.IncommingLetter");
            //DropForeignKey("Archive.LetterStatus", "LetterSenderId", "EXCEED.LetterSender");
            DropForeignKey("Construction.ProjectEstimationBoq", "ProjectInformationId", "Construction.ProjectInformation");
            //DropIndex("Archive.LetterStatus", new[] { "LetterSenderId" });
           // DropIndex("EXCEED.LetterSender", new[] { "BookNo" });
            DropIndex("Construction.ProjectEstimationBoq", new[] { "ProjectInformationId" });
            CreateTable(
                "Construction.ProjectDirectLabour",
                c => new
                    {
                        ProjectDirectLabourId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectDirectLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.WorkItemPerProject",
                c => new
                    {
                        WorkItemPerProjectId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        TotalQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.Int(nullable: false),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkItemPerProjectId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.ProjectEquipment",
                c => new
                    {
                        ProjectEquipmentId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        AssetNumber = c.String(nullable: false, maxLength: 100),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentId)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId);
            
            CreateTable(
                "Construction.ProjectMaterial",
                c => new
                    {
                        ProjectMaterialId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId)
                .Index(t => t.ItemCode);
            
            AddColumn("Archive.LetterStatus", "BookNo", c => c.String(nullable: false));
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId");
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            //DropColumn("Archive.LetterStatus", "LetterSenderId");
            //DropTable("EXCEED.LetterSender");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.LetterSender",
                c => new
                    {
                        LetterSenderId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Receiver = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Sender = c.String(),
                        BookNo = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LetterSenderId);
            
            AddColumn("Archive.LetterStatus", "LetterSenderId", c => c.Int(nullable: false));
            DropForeignKey("Construction.WorkItemPerProject", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.WorkItemPerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterial", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectEquipment", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectDirectLabour", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.ProjectDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ProjectMaterial", new[] { "ItemCode" });
            DropIndex("Construction.ProjectMaterial", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectEquipment", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "WorkCategoryId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.ProjectDirectLabour", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "ProjectInformationId" });
            DropColumn("Archive.LetterStatus", "BookNo");
            DropTable("Construction.ProjectMaterial");
            DropTable("Construction.ProjectEquipment");
            DropTable("Construction.WorkItemPerProject");
            DropTable("Construction.ProjectDirectLabour");
            CreateIndex("Construction.ProjectEstimationBoq", "ProjectInformationId");
            CreateIndex("EXCEED.LetterSender", "BookNo");
            CreateIndex("Archive.LetterStatus", "LetterSenderId");
            AddForeignKey("Construction.ProjectEstimationBoq", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Archive.LetterStatus", "LetterSenderId", "EXCEED.LetterSender", "LetterSenderId", cascadeDelete: true);
            AddForeignKey("EXCEED.LetterSender", "BookNo", "Archive.IncommingLetter", "BookNo");
        }
    }
}
