namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdleTiemUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("JobCosting.IdleTimes", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("JobCosting.IdleTimes", "GLPeriodId", c => c.Int(nullable: false));
            DropColumn("JobCosting.IdleTimes", "YearId");
            DropColumn("JobCosting.IdleTimes", "PeriodId");
        }
        
        public override void Down()
        {
            AddColumn("JobCosting.IdleTimes", "PeriodId", c => c.Int(nullable: false));
            AddColumn("JobCosting.IdleTimes", "YearId", c => c.Int(nullable: false));
            DropColumn("JobCosting.IdleTimes", "GLPeriodId");
            DropColumn("JobCosting.IdleTimes", "GlFiscalYearId");
        }
    }
}
