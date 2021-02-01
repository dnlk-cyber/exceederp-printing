namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class associa1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Payroll.Association",
                c => new
                    {
                        AssociationId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMemeber = c.Boolean(nullable: false),
                        ApplyToAll = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssociationId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Payroll.Association", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Association", "EmployeeId", "HRM.Employee");
            DropIndex("Payroll.Association", new[] { "GLPeriodId" });
            DropIndex("Payroll.Association", new[] { "EmployeeId" });
            DropTable("Payroll.Association");
        }
    }
}
