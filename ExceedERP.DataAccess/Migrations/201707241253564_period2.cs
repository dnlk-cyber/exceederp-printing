namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class period2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.Addition", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Addition", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Addition", "LookupId", "Common.Lookup");
            DropForeignKey("Payroll.Advance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Advance", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.FixedContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("Payroll.FixedContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.FixedContribution", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Loan", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Loan", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.OverTime", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.OverTime", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.RegularContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("Payroll.RegularContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.RegularContribution", "GLPeriodId", "Payroll.PayrollPeriod");
            DropIndex("Payroll.Addition", new[] { "EmployeeId" });
            DropIndex("Payroll.Addition", new[] { "GLPeriodId" });
            DropIndex("Payroll.Addition", new[] { "LookupId" });
            DropIndex("Payroll.Advance", new[] { "EmployeeId" });
            DropIndex("Payroll.Advance", new[] { "GLPeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "EmployeeId" });
            DropIndex("Payroll.FixedContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "ContributionTypeId" });
            DropIndex("Payroll.Loan", new[] { "EmployeeId" });
            DropIndex("Payroll.Loan", new[] { "GLPeriodId" });
            DropIndex("Payroll.OverTime", new[] { "EmployeeId" });
            DropIndex("Payroll.OverTime", new[] { "GLPeriodId" });
            DropIndex("Payroll.RegularContribution", new[] { "EmployeeId" });
            DropIndex("Payroll.RegularContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.RegularContribution", new[] { "ContributionTypeId" });
            DropTable("Payroll.Addition");
            DropTable("Payroll.Advance");
            DropTable("Payroll.FixedContribution");
            DropTable("Payroll.Loan");
            DropTable("Payroll.OverTime");
            DropTable("Payroll.RegularContribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "Payroll.RegularContribution",
                c => new
                    {
                        RegContributionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        ContributionTypeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        ApplyToAll = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RegContributionId);
            
            CreateTable(
                "Payroll.OverTime",
                c => new
                    {
                        OverTimeId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        NormalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NightHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RestdayHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeId);
            
            CreateTable(
                "Payroll.Loan",
                c => new
                    {
                        LoanId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JumpHoliday = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanId);
            
            CreateTable(
                "Payroll.FixedContribution",
                c => new
                    {
                        FixedContributionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        ContributionTypeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetGross = c.Int(nullable: false),
                        AmtPercnet = c.Int(nullable: false),
                        ApplyToAll = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedContributionId);
            
            CreateTable(
                "Payroll.Advance",
                c => new
                    {
                        AdvanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JumpHoliday = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvanceId);
            
            CreateTable(
                "Payroll.Addition",
                c => new
                    {
                        AdditionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LookupId = c.Int(nullable: false),
                        AdditionTaxType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdditionId);
            
            CreateIndex("Payroll.RegularContribution", "ContributionTypeId");
            CreateIndex("Payroll.RegularContribution", "GLPeriodId");
            CreateIndex("Payroll.RegularContribution", "EmployeeId");
            CreateIndex("Payroll.OverTime", "GLPeriodId");
            CreateIndex("Payroll.OverTime", "EmployeeId");
            CreateIndex("Payroll.Loan", "GLPeriodId");
            CreateIndex("Payroll.Loan", "EmployeeId");
            CreateIndex("Payroll.FixedContribution", "ContributionTypeId");
            CreateIndex("Payroll.FixedContribution", "GLPeriodId");
            CreateIndex("Payroll.FixedContribution", "EmployeeId");
            CreateIndex("Payroll.Advance", "GLPeriodId");
            CreateIndex("Payroll.Advance", "EmployeeId");
            CreateIndex("Payroll.Addition", "LookupId");
            CreateIndex("Payroll.Addition", "GLPeriodId");
            CreateIndex("Payroll.Addition", "EmployeeId");
            AddForeignKey("Payroll.RegularContribution", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.RegularContribution", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Payroll.RegularContribution", "ContributionTypeId", "HRM.ContributionType", "ContributionTypeId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTime", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTime", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Payroll.Loan", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Loan", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "ContributionTypeId", "HRM.ContributionType", "ContributionTypeId", cascadeDelete: true);
            AddForeignKey("Payroll.Advance", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Advance", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Payroll.Addition", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Payroll.Addition", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Addition", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
