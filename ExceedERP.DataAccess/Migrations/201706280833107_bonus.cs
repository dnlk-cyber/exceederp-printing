namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bonus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Payroll.Bonus",
                c => new
                    {
                        BonusId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        BounsFactor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerformanceRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusType = c.Int(nullable: false),
                        TotalBonus = c.Double(nullable: false),
                        TotalIncomeTax = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BonusId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.EmployeeId);
            
            AddColumn("Payroll.PerformanceRank", "BonusRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("Payroll.Bonus", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Bonus", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("Payroll.Bonus", new[] { "EmployeeId" });
            DropIndex("Payroll.Bonus", new[] { "GlFiscalYearId" });
            DropColumn("Payroll.PerformanceRank", "BonusRate");
            DropTable("Payroll.Bonus");
        }
    }
}
