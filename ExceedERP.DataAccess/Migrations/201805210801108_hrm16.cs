namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.LateComingSummary",
                c => new
                    {
                        LateComingSummaryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LateComingSummaryId)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.LateComingSummary", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LateComingSummary", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("HRM.LateComingSummary", new[] { "GLPeriodId" });
            DropIndex("HRM.LateComingSummary", new[] { "EmployeeId" });
            DropTable("HRM.LateComingSummary");
        }
    }
}
