namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class associa : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.Association", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Association", "GLPeriodId", "Payroll.PayrollPeriod");
            DropIndex("Payroll.Association", new[] { "EmployeeId" });
            DropIndex("Payroll.Association", new[] { "GLPeriodId" });
            DropTable("Payroll.Association");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.AssociationId);
            
            CreateIndex("Payroll.Association", "GLPeriodId");
            CreateIndex("Payroll.Association", "EmployeeId");
            AddForeignKey("Payroll.Association", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Association", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
