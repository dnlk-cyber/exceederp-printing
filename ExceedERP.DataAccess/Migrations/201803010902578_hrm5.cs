namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.RecruitmentPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.RecruitmentPlan", new[] { "GlFiscalYearId" });
        }
        
        public override void Down()
        {
            CreateIndex("HRM.RecruitmentPlan", "GlFiscalYearId");
            AddForeignKey("HRM.RecruitmentPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
        }
    }
}
