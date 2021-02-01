namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed8 : DbMigration
    {
        public override void Up()
        {
            DropIndex("FleetManagement.EquipmentType", new[] { "EquipmentIdentityID" });
            CreateTable(
                "BudgetGAAP.BudgetSheetCategory",
                c => new
                    {
                        BalanceSheetCategoryId = c.Int(nullable: false, identity: true),
                        BalanceSheetType = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BalanceSheetCategoryId);
            
            CreateTable(
                "BudgetGAAP.BalanceSheet",
                c => new
                    {
                        BalanceSheetId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BalanceSheetCategoryId = c.Int(nullable: false),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Second = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Third = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fourth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromAccCode = c.String(),
                        ToAccCode = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BalanceSheetId)
                .ForeignKey("BudgetGAAP.BudgetSheetCategory", t => t.BalanceSheetCategoryId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BalanceSheetCategoryId)
                .Index(t => t.GlFiscalYearId);
            
            AddColumn("HRM.EducationRequest", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.EquipmentType", "EquipmentIdentityID", c => c.Guid(nullable: false));
            CreateIndex("FleetManagement.EquipmentType", "EquipmentIdentityID");
        }
        
        public override void Down()
        {
            DropForeignKey("BudgetGAAP.BalanceSheet", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("BudgetGAAP.BalanceSheet", "BalanceSheetCategoryId", "BudgetGAAP.BudgetSheetCategory");
            DropIndex("BudgetGAAP.BalanceSheet", new[] { "GlFiscalYearId" });
            DropIndex("BudgetGAAP.BalanceSheet", new[] { "BalanceSheetCategoryId" });
            DropIndex("FleetManagement.EquipmentType", new[] { "EquipmentIdentityID" });
            AlterColumn("FleetManagement.EquipmentType", "EquipmentIdentityID", c => c.Guid());
            DropColumn("HRM.EducationRequest", "EmployeeId");
            DropTable("BudgetGAAP.BalanceSheet");
            DropTable("BudgetGAAP.BudgetSheetCategory");
            CreateIndex("FleetManagement.EquipmentType", "EquipmentIdentityID");
        }
    }
}
