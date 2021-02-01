namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction21 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.PromotionRequest", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.PromotionRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.PromotionRequest", "RankId", "HRM.Rank");
            DropIndex("HRM.PromotionRequest", new[] { "OrgStructureId" });
            DropIndex("HRM.PromotionRequest", new[] { "LookupId" });
            DropIndex("HRM.PromotionRequest", new[] { "RankId" });
            AddColumn("Construction.Task", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "EquipmentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("HRM.PromotionRequest");
        }
        
        public override void Down()
        {
            CreateTable(
                "HRM.PromotionRequest",
                c => new
                    {
                        PromotionRequestId = c.Int(nullable: false, identity: true),
                        PlanType = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfPostion = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EduLevel = c.Int(nullable: false),
                        RelevantExp = c.String(),
                        Experience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialSkill = c.String(),
                        PlannedSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RankId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PromotionRequestId);
            
            DropColumn("Construction.Task", "EquipmentCost");
            DropColumn("Construction.Task", "LabourCost");
            DropColumn("Construction.Task", "MaterialCost");
            DropColumn("Construction.Task", "DirectCost");
            DropColumn("Construction.Task", "TotalQuantityOfWork");
            CreateIndex("HRM.PromotionRequest", "RankId");
            CreateIndex("HRM.PromotionRequest", "LookupId");
            CreateIndex("HRM.PromotionRequest", "OrgStructureId");
            AddForeignKey("HRM.PromotionRequest", "RankId", "HRM.Rank", "RankId", cascadeDelete: true);
            AddForeignKey("HRM.PromotionRequest", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("HRM.PromotionRequest", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
        }
    }
}
