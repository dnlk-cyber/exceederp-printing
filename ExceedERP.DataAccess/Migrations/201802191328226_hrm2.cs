namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.NeedAssessment", "Check", c => c.Boolean(nullable: false));
            AddColumn("HRM.NeedAssessment", "GlFiscalYearId", c => c.Int(nullable: false));
            CreateIndex("HRM.NeedAssessment", "GlFiscalYearId");
            AddForeignKey("HRM.NeedAssessment", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.NeedAssessment", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.NeedAssessment", new[] { "GlFiscalYearId" });
            DropColumn("HRM.NeedAssessment", "GlFiscalYearId");
            DropColumn("HRM.NeedAssessment", "Check");
        }
    }
}
