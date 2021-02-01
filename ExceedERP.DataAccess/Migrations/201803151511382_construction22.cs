namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.EarnedValueManagementTermFormulaConstants",
                c => new
                    {
                        EarnedValueManagementTermFormulaConstantId = c.Int(nullable: false, identity: true),
                        EarnedValueManagementTermDetailId = c.Int(nullable: false),
                        Operator = c.Int(nullable: false),
                        ConstantValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FormulaValue = c.String(),
                        HiddenFormulaValue = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValueManagementTermFormulaConstantId)
                .ForeignKey("Construction.EarnedValueManagementTermDetail", t => t.EarnedValueManagementTermDetailId, cascadeDelete: true)
                .Index(t => t.EarnedValueManagementTermDetailId);
            
            CreateTable(
                "Construction.TaskDetail",
                c => new
                    {
                        TaskDetailId = c.Int(nullable: false, identity: true),
                        ID = c.Int(nullable: false),
                        ConstructionPeriodId = c.Int(nullable: false),
                        PlannedQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedMaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedEquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedOverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskDetailId)
                .ForeignKey("Construction.Task", t => t.ID, cascadeDelete: true)
                .Index(t => t.ID);
            
            AlterColumn("Construction.EarnedValueManagementTermDetail", "ConstantValue", c => c.String());
            AlterColumn("Construction.EVMTermOperandComponent", "ConstantValue", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.TaskDetail", "ID", "Construction.Task");
            DropForeignKey("Construction.EarnedValueManagementTermFormulaConstants", "EarnedValueManagementTermDetailId", "Construction.EarnedValueManagementTermDetail");
            DropIndex("Construction.TaskDetail", new[] { "ID" });
            DropIndex("Construction.EarnedValueManagementTermFormulaConstants", new[] { "EarnedValueManagementTermDetailId" });
            AlterColumn("Construction.EVMTermOperandComponent", "ConstantValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValueManagementTermDetail", "ConstantValue", c => c.Decimal(precision: 18, scale: 2));
            DropTable("Construction.TaskDetail");
            DropTable("Construction.EarnedValueManagementTermFormulaConstants");
        }
    }
}
