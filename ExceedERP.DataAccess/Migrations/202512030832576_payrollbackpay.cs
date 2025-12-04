namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollbackpay : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.BackPaymentLine", newSchema: "Payroll");
            MoveTable(name: "EXCEED.BackPaymentDetail", newSchema: "Payroll");
            DropForeignKey("EXCEED.BackPayment", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.BackPayment", "EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.BackPayment", new[] { "GLPeriodId" });
            DropIndex("EXCEED.BackPayment", new[] { "EmployeeId" });
            CreateTable(
                "Payroll.BackPaymentDistribution",
                c => new
                    {
                        BackPaymentDistributionId = c.Int(nullable: false, identity: true),
                        BackPayId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BackPaymentDistributionId)
                .ForeignKey("Payroll.BackPay", t => t.BackPayId, cascadeDelete: true)
                .Index(t => t.BackPayId);
            
            AddColumn("Payroll.PayrollProcessViewModel", "OriginalBasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.BackPay", "ToGLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.BackPay", "ProcessedGLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.BackPaymentLine", "PensionComp", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.BackPaymentLine", "Allowances", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.BackPaymentLine", "IncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.BackPaymentLine", "Pension", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.BackPaymentLine", "Net", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("Payroll.BackPaymentDetail", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            DropTable("EXCEED.BackPayment");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.BackPayment",
                c => new
                    {
                        BackPaymentId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Allowances = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pension = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Net = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsClosed = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BackPaymentId);
            
            DropForeignKey("Payroll.BackPaymentDistribution", "BackPayId", "Payroll.BackPay");
            DropIndex("Payroll.BackPaymentDistribution", new[] { "BackPayId" });
            AlterColumn("Payroll.BackPaymentDetail", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "Net", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "Pension", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "IncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "Allowances", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.BackPaymentLine", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Payroll.BackPaymentLine", "PensionComp");
            DropColumn("Payroll.BackPay", "ProcessedGLPeriodId");
            DropColumn("Payroll.BackPay", "ToGLPeriodId");
            DropColumn("Payroll.PayrollProcessViewModel", "OriginalBasicSalary");
            DropTable("Payroll.BackPaymentDistribution");
            CreateIndex("EXCEED.BackPayment", "EmployeeId");
            CreateIndex("EXCEED.BackPayment", "GLPeriodId");
            AddForeignKey("EXCEED.BackPayment", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("EXCEED.BackPayment", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            MoveTable(name: "Payroll.BackPaymentDetail", newSchema: "EXCEED");
            MoveTable(name: "Payroll.BackPaymentLine", newSchema: "EXCEED");
        }
    }
}
