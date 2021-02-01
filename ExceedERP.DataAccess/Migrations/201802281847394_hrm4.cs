namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Vacancy", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentPlan", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "GlFiscalYearId", c => c.Int(nullable: false));
            CreateIndex("HRM.Vacancy", "GlFiscalYearId");
            CreateIndex("HRM.RecruitmentPlan", "GlFiscalYearId");
            CreateIndex("HRM.RecruitmentRequest", "GlFiscalYearId");
            AddForeignKey("HRM.Vacancy", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentRequest", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.RecruitmentRequest", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.RecruitmentPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.Vacancy", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.RecruitmentRequest", new[] { "GlFiscalYearId" });
            DropIndex("HRM.RecruitmentPlan", new[] { "GlFiscalYearId" });
            DropIndex("HRM.Vacancy", new[] { "GlFiscalYearId" });
            DropColumn("HRM.RecruitmentRequest", "GlFiscalYearId");
            DropColumn("HRM.RecruitmentPlan", "GlFiscalYearId");
            DropColumn("HRM.Vacancy", "GlFiscalYearId");
        }
    }
}
