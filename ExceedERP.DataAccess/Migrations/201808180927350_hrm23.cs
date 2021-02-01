namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.HealthRankAndValue",
                c => new
                    {
                        HealthRankAndValueId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAmh = c.String(),
                        ClinicName = c.String(),
                        AllowedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.HealthRankAndValueId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: false)
                .Index(t => t.EmployeeId)
                .Index(t => t.BranchId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.HealthRankAndValueSetting",
                c => new
                    {
                        HealthRankAndValueSettingId = c.Int(nullable: false, identity: true),
                        RankId = c.Int(nullable: false),
                        CurrentValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.HealthRankAndValueSettingId)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.RankId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.HealthRankAndValueSetting", "RankId", "HRM.Rank");
            DropForeignKey("EXCEED.HealthRankAndValue", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.HealthRankAndValue", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.HealthRankAndValue", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.HealthRankAndValue", "BranchId", "HRM.Branch");
            DropIndex("EXCEED.HealthRankAndValueSetting", new[] { "RankId" });
            DropIndex("EXCEED.HealthRankAndValue", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.HealthRankAndValue", new[] { "GLPeriodId" });
            DropIndex("EXCEED.HealthRankAndValue", new[] { "BranchId" });
            DropIndex("EXCEED.HealthRankAndValue", new[] { "EmployeeId" });
            DropTable("EXCEED.HealthRankAndValueSetting");
            DropTable("EXCEED.HealthRankAndValue");
        }
    }
}
