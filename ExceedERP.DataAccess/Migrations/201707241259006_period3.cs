namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class period3 : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.AdditionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.LookupId);
            
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
                .PrimaryKey(t => t.AdvanceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
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
                .PrimaryKey(t => t.FixedContributionId)
                .ForeignKey("HRM.ContributionType", t => t.ContributionTypeId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.ContributionTypeId);
            
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
                .PrimaryKey(t => t.LoanId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
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
                .PrimaryKey(t => t.OverTimeId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
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
                .PrimaryKey(t => t.RegContributionId)
                .ForeignKey("HRM.ContributionType", t => t.ContributionTypeId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Payroll.PayrollPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.ContributionTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Payroll.RegularContribution", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.RegularContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.RegularContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("Payroll.OverTime", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.OverTime", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Loan", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Loan", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.FixedContribution", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.FixedContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.FixedContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("Payroll.Advance", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Advance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Addition", "LookupId", "Common.Lookup");
            DropForeignKey("Payroll.Addition", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.Addition", "EmployeeId", "HRM.Employee");
            DropIndex("Payroll.RegularContribution", new[] { "ContributionTypeId" });
            DropIndex("Payroll.RegularContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.RegularContribution", new[] { "EmployeeId" });
            DropIndex("Payroll.OverTime", new[] { "GLPeriodId" });
            DropIndex("Payroll.OverTime", new[] { "EmployeeId" });
            DropIndex("Payroll.Loan", new[] { "GLPeriodId" });
            DropIndex("Payroll.Loan", new[] { "EmployeeId" });
            DropIndex("Payroll.FixedContribution", new[] { "ContributionTypeId" });
            DropIndex("Payroll.FixedContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "EmployeeId" });
            DropIndex("Payroll.Advance", new[] { "GLPeriodId" });
            DropIndex("Payroll.Advance", new[] { "EmployeeId" });
            DropIndex("Payroll.Addition", new[] { "LookupId" });
            DropIndex("Payroll.Addition", new[] { "GLPeriodId" });
            DropIndex("Payroll.Addition", new[] { "EmployeeId" });
            DropTable("Payroll.RegularContribution");
            DropTable("Payroll.OverTime");
            DropTable("Payroll.Loan");
            DropTable("Payroll.FixedContribution");
            DropTable("Payroll.Advance");
            DropTable("Payroll.Addition");
        }
    }
}
