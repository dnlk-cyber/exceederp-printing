namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiveEntitlementChangesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.LeaveEntitlementHistories",
                c => new
                    {
                        LeaveEntitlementHistoryId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(),
                        Entitlement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveEntitlementHistoryId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.LeaveEntitlementHistories", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.LeaveEntitlementHistories", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.LeaveEntitlementHistories", new[] { "EmployeeId" });
            DropIndex("HRM.LeaveEntitlementHistories", new[] { "GlFiscalYearId" });
            DropTable("HRM.LeaveEntitlementHistories");
        }
    }
}
