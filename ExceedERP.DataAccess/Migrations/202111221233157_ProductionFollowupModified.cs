namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionFollowupModified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("JobCosting.IdleTimes", "Period_GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("JobCosting.IdleTimes", "Year_GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("JobCosting.IdleTimes", new[] { "Period_GLPeriodId" });
            DropIndex("JobCosting.IdleTimes", new[] { "Year_GlFiscalYearId" });
            AddColumn("ProductFollowUp.Job", "JT", c => c.String());
            AddColumn("ProductFollowUp.Job", "DateCreated", c => c.DateTime());
            AddColumn("ProductFollowUp.Job", "LastModified", c => c.DateTime());
            AddColumn("ProductFollowUp.Job", "CreatedBy", c => c.String());
            AddColumn("ProductFollowUp.Job", "ModifiedBy", c => c.String());
            DropColumn("JobCosting.IdleTimes", "Period_GLPeriodId");
            DropColumn("JobCosting.IdleTimes", "Year_GlFiscalYearId");
        }
        
        public override void Down()
        {
            AddColumn("JobCosting.IdleTimes", "Year_GlFiscalYearId", c => c.Int());
            AddColumn("JobCosting.IdleTimes", "Period_GLPeriodId", c => c.Int());
            DropColumn("ProductFollowUp.Job", "ModifiedBy");
            DropColumn("ProductFollowUp.Job", "CreatedBy");
            DropColumn("ProductFollowUp.Job", "LastModified");
            DropColumn("ProductFollowUp.Job", "DateCreated");
            DropColumn("ProductFollowUp.Job", "JT");
            CreateIndex("JobCosting.IdleTimes", "Year_GlFiscalYearId");
            CreateIndex("JobCosting.IdleTimes", "Period_GLPeriodId");
            AddForeignKey("JobCosting.IdleTimes", "Year_GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId");
            AddForeignKey("JobCosting.IdleTimes", "Period_GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId");
        }
    }
}
