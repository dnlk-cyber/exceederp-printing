namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmadditional : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Payroll.BackPayment", newSchema: "EXCEED");
            MoveTable(name: "Performance.EmployeeMeasurementDefinition", newSchema: "EXCEED");
            MoveTable(name: "Performance.EmployeePerformanceAction", newSchema: "EXCEED");
            CreateTable(
                "Payroll.BackPay",
                c => new
                    {
                        BackPayId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        Remark = c.String(),
                        IsClosed = c.Boolean(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BackPayId)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "EXCEED.BackPaymentLine",
                c => new
                    {
                        BackPaymentLineId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Allowances = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pension = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Net = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BackPayId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BackPaymentLineId)
                .ForeignKey("Payroll.BackPay", t => t.BackPayId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.BackPayId);
            
            CreateTable(
                "EXCEED.BackPaymentDetail",
                c => new
                    {
                        BackPaymentDetailId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BackPaymentLineId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BackPaymentDetailId)
                .ForeignKey("EXCEED.BackPaymentLine", t => t.BackPaymentLineId, cascadeDelete: true)
                .Index(t => t.BackPaymentLineId);
            
            AlterColumn("EXCEED.BackPayment", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BackPayment", "Allowances", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BackPayment", "IncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BackPayment", "Pension", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BackPayment", "Net", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.PayrollProcessAllowances", "Taxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.PayrollProcessAllowances", "NonTaxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.PayrollProcessAllowances", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("Payroll.BackPay", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.BackPaymentLine", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.BackPaymentDetail", "BackPaymentLineId", "EXCEED.BackPaymentLine");
            DropForeignKey("EXCEED.BackPaymentLine", "BackPayId", "Payroll.BackPay");
            DropIndex("EXCEED.BackPaymentDetail", new[] { "BackPaymentLineId" });
            DropIndex("EXCEED.BackPaymentLine", new[] { "BackPayId" });
            DropIndex("EXCEED.BackPaymentLine", new[] { "EmployeeId" });
            DropIndex("Payroll.BackPay", new[] { "GLPeriodId" });
            AlterColumn("Payroll.PayrollProcessAllowances", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.PayrollProcessAllowances", "NonTaxable", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.PayrollProcessAllowances", "Taxable", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.BackPayment", "Net", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.BackPayment", "Pension", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.BackPayment", "IncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.BackPayment", "Allowances", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.BackPayment", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            DropTable("EXCEED.BackPaymentDetail");
            DropTable("EXCEED.BackPaymentLine");
            DropTable("Payroll.BackPay");
            MoveTable(name: "EXCEED.EmployeePerformanceAction", newSchema: "Performance");
            MoveTable(name: "EXCEED.EmployeeMeasurementDefinition", newSchema: "Performance");
            MoveTable(name: "EXCEED.BackPayment", newSchema: "Payroll");
        }
    }
}
