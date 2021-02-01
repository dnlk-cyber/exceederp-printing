namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm57 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.MonthlyLeavePlanDetail",
                c => new
                    {
                        MonthlyLeavePlanDetailId = c.Int(nullable: false, identity: true),
                        MonthlyLeavePlanId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyLeavePlanDetailId);
            
            CreateTable(
                "HRM.MonthlyLeavePlan",
                c => new
                    {
                        MonthlyLeavePlanId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyLeavePlanId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.MonthlyLeavePlan");
            DropTable("HRM.MonthlyLeavePlanDetail");
        }
    }
}
