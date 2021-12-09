namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmleaventitle : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("JobCosting.IdleTimes", "Period_GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("JobCosting.IdleTimes", "Year_GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("JobCosting.IdleTimes", new[] { "Period_GLPeriodId" });
            DropIndex("JobCosting.IdleTimes", new[] { "Year_GlFiscalYearId" });
            CreateTable(
                "HRM.LeaveEntitleStartingDate",
                c => new
                    {
                        LeaveEntitleStartingDateId = c.Int(nullable: false, identity: true),
                        Start = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeaveEntitleStartingDateId);
            
            DropColumn("JobCosting.IdleTimes", "Period_GLPeriodId");
            DropColumn("JobCosting.IdleTimes", "Year_GlFiscalYearId");
        }
        
        public override void Down()
        {
            AddColumn("JobCosting.IdleTimes", "Year_GlFiscalYearId", c => c.Int());
            AddColumn("JobCosting.IdleTimes", "Period_GLPeriodId", c => c.Int());
            DropTable("HRM.LeaveEntitleStartingDate");
            CreateIndex("JobCosting.IdleTimes", "Year_GlFiscalYearId");
            CreateIndex("JobCosting.IdleTimes", "Period_GLPeriodId");
            AddForeignKey("JobCosting.IdleTimes", "Year_GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId");
            AddForeignKey("JobCosting.IdleTimes", "Period_GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId");
        }
    }
}
