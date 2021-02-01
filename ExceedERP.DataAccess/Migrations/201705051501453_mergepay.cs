namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mergepay : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.Delegation",
                c => new
                    {
                        DelegationId = c.Int(nullable: false, identity: true),
                        FromEmployeeId = c.Int(nullable: false),
                        FromDateTime = c.DateTime(nullable: false),
                        ToDateTime = c.DateTime(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DelegationId);
            
            CreateTable(
                "HRM.DisciplineMeasuerByMoney",
                c => new
                    {
                        DisciplineMeasuerByMoneyId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        DisciplinaryCaseId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DisciplineMeasuerByMoneyId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            AddColumn("Payroll.PayrollProcessViewModel", "NonTaxableResponsiblityAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TaxableResponsiblityAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalResponsiblityAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "NonTaxableProjectAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TaxableProjectAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalProjectAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalPenality", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "SportFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessDeductions", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.DisciplineMeasuerByMoney", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.DisciplineMeasuerByMoney", new[] { "EmployeeId" });
            DropColumn("Payroll.PayrollProcessDeductions", "Remark");
            DropColumn("Payroll.PayrollProcessViewModel", "SportFee");
            DropColumn("Payroll.PayrollProcessViewModel", "TotalPenality");
            DropColumn("Payroll.PayrollProcessViewModel", "TotalProjectAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "TaxableProjectAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "NonTaxableProjectAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "TotalResponsiblityAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "TaxableResponsiblityAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "NonTaxableResponsiblityAllowance");
            DropTable("HRM.DisciplineMeasuerByMoney");
            DropTable("HRM.Delegation");
        }
    }
}
