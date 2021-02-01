namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrandpayroll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.Addition", "PeriodId", "common.Period");
            DropForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropForeignKey("Payroll.Association", "PeriodId", "common.Period");
            DropForeignKey("Payroll.EmpPayrollNoDaysWorked", "PeriodId", "common.Period");
            DropForeignKey("Payroll.FixedContribution", "PeriodId", "common.Period");
            DropForeignKey("Payroll.Loan", "PeriodId", "common.Period");
            DropForeignKey("Payroll.OverTimePerDay", "PeriodId", "common.Period");
            DropForeignKey("Payroll.OverTime", "PeriodId", "common.Period");
            DropForeignKey("Payroll.PayrollDistribution", "PeriodId", "common.Period");
            DropForeignKey("Payroll.PayrollProcessViewModel", "PeriodId", "common.Period");
            DropForeignKey("Payroll.PeriodStatus", "PeriodId", "common.Period");
            DropForeignKey("Payroll.RegularContribution", "PeriodId", "common.Period");
            DropIndex("Payroll.Addition", new[] { "PeriodId" });
            DropIndex("HRM.Vacancy", new[] { "RecruitmentPlanId" });
            DropIndex("Payroll.Association", new[] { "PeriodId" });
            DropIndex("Payroll.EmpPayrollNoDaysWorked", new[] { "PeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "PeriodId" });
            DropIndex("Payroll.Loan", new[] { "PeriodId" });
            DropIndex("Payroll.OverTimePerDay", new[] { "PeriodId" });
            DropIndex("Payroll.OverTime", new[] { "PeriodId" });
            DropIndex("Payroll.PayrollDistribution", new[] { "PeriodId" });
            DropIndex("Payroll.PayrollProcessViewModel", new[] { "PeriodId" });
            DropIndex("Payroll.PeriodStatus", new[] { "PeriodId" });
            DropIndex("Payroll.RegularContribution", new[] { "PeriodId" });
            RenameColumn(table: "Payroll.Advance", name: "PeriodId", newName: "GLPeriodId");
            RenameColumn(table: "Payroll.OverTimeTransfer", name: "PeriodId", newName: "GLPeriodId");
            RenameIndex(table: "Payroll.Advance", name: "IX_PeriodId", newName: "IX_GLPeriodId");
            RenameIndex(table: "Payroll.OverTimeTransfer", name: "IX_PeriodId", newName: "IX_GLPeriodId");
            CreateTable(
                "HRM.RecruitmentVacancy",
                c => new
                    {
                        RecruitmentVacancyId = c.Int(nullable: false, identity: true),
                        VaccancyCode = c.String(maxLength: 128),
                        RecruitmentPlanId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentVacancyId)
                .ForeignKey("HRM.RecruitmentPlan", t => t.RecruitmentPlanId, cascadeDelete: true)
                .ForeignKey("HRM.Vacancy", t => t.VaccancyCode)
                .Index(t => t.VaccancyCode)
                .Index(t => t.RecruitmentPlanId);
            
            AddColumn("Payroll.Addition", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentPlan", "Check", c => c.Boolean(nullable: false));
            AddColumn("Payroll.Association", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.EmpPayrollDeduction", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.EmpPayrollLongTermDeduction", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.EmpPayrollNoDaysWorked", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.EmpPayrollServiceChargeViewModel", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.FixedContribution", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.Loan", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.OverTimePerDay", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.OverTime", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollDistribution", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollProcessAdditions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessAdvances", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessAllowances", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessDeductions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessFixedContribution", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessLoans", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessLongTermDeductions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessRegularContribution", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PeriodStatus", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.RegularContribution", "GLPeriodId", c => c.Int(nullable: false));
            CreateIndex("Payroll.Addition", "GLPeriodId");
            CreateIndex("Payroll.Association", "GLPeriodId");
            CreateIndex("Payroll.EmpPayrollNoDaysWorked", "GLPeriodId");
            CreateIndex("Payroll.FixedContribution", "GLPeriodId");
            CreateIndex("Payroll.Loan", "GLPeriodId");
            CreateIndex("Payroll.OverTimePerDay", "GLPeriodId");
            CreateIndex("Payroll.OverTime", "GLPeriodId");
            CreateIndex("Payroll.PayrollDistribution", "GLPeriodId");
            CreateIndex("Payroll.PayrollProcessViewModel", "GLPeriodId");
            CreateIndex("Payroll.PeriodStatus", "GLPeriodId");
            CreateIndex("Payroll.RegularContribution", "GLPeriodId");
            AddForeignKey("Payroll.Addition", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Association", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            //AddForeignKey("Payroll.EmpPayrollNoDaysWorked", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            //AddForeignKey("Payroll.FixedContribution", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            //AddForeignKey("Payroll.Loan", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTimePerDay", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTime", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.PayrollDistribution", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            //AddForeignKey("Payroll.PayrollProcessViewModel", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            //AddForeignKey("Payroll.PeriodStatus", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.RegularContribution", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            DropColumn("Payroll.Addition", "PeriodId");
            DropColumn("HRM.Vacancy", "RecruitmentPlanId");
            DropColumn("Payroll.Association", "PeriodId");
            DropColumn("Payroll.EmpPayrollNoDaysWorked", "PeriodId");
            DropColumn("Payroll.FixedContribution", "PeriodId");
            DropColumn("Payroll.Loan", "PeriodId");
            DropColumn("Payroll.OverTimePerDay", "PeriodId");
            DropColumn("Payroll.OverTime", "PeriodId");
            DropColumn("Payroll.PayrollDistribution", "PeriodId");
            DropColumn("Payroll.PayrollProcessViewModel", "PeriodId");
            DropColumn("Payroll.PeriodStatus", "PeriodId");
            DropColumn("Payroll.RegularContribution", "PeriodId");
        }
        
        public override void Down()
        {
            AddColumn("Payroll.RegularContribution", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PeriodStatus", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollDistribution", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.OverTime", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.OverTimePerDay", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.Loan", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.FixedContribution", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.EmpPayrollNoDaysWorked", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.Association", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "RecruitmentPlanId", c => c.Int(nullable: false));
            AddColumn("Payroll.Addition", "PeriodId", c => c.Int(nullable: false));
            DropForeignKey("Payroll.RegularContribution", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("HRM.RecruitmentVacancy", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.RecruitmentVacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropForeignKey("Payroll.PeriodStatus", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.PayrollProcessViewModel", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.PayrollDistribution", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.OverTime", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.OverTimePerDay", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.Loan", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.FixedContribution", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.EmpPayrollNoDaysWorked", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.Association", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Payroll.Addition", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("Payroll.RegularContribution", new[] { "GLPeriodId" });
            DropIndex("HRM.RecruitmentVacancy", new[] { "RecruitmentPlanId" });
            DropIndex("HRM.RecruitmentVacancy", new[] { "VaccancyCode" });
            DropIndex("Payroll.PeriodStatus", new[] { "GLPeriodId" });
            DropIndex("Payroll.PayrollProcessViewModel", new[] { "GLPeriodId" });
            DropIndex("Payroll.PayrollDistribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.OverTime", new[] { "GLPeriodId" });
            DropIndex("Payroll.OverTimePerDay", new[] { "GLPeriodId" });
            DropIndex("Payroll.Loan", new[] { "GLPeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.EmpPayrollNoDaysWorked", new[] { "GLPeriodId" });
            DropIndex("Payroll.Association", new[] { "GLPeriodId" });
            DropIndex("Payroll.Addition", new[] { "GLPeriodId" });
            DropColumn("Payroll.RegularContribution", "GLPeriodId");
            DropColumn("Payroll.PeriodStatus", "GLPeriodId");
            DropColumn("Payroll.PayrollProcessRegularContribution", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessLongTermDeductions", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessLoans", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessFixedContribution", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessDeductions", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessAllowances", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessAdvances", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessViewModel", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessViewModel", "GLPeriodId");
            DropColumn("Payroll.PayrollProcessAdditions", "isSendForApproval");
            DropColumn("Payroll.PayrollDistribution", "GLPeriodId");
            DropColumn("Payroll.OverTime", "GLPeriodId");
            DropColumn("Payroll.OverTimePerDay", "GLPeriodId");
            DropColumn("Payroll.Loan", "GLPeriodId");
            DropColumn("Payroll.FixedContribution", "GLPeriodId");
            DropColumn("Payroll.EmpPayrollServiceChargeViewModel", "isSendForApproval");
            DropColumn("Payroll.EmpPayrollNoDaysWorked", "GLPeriodId");
            DropColumn("Payroll.EmpPayrollLongTermDeduction", "isSendForApproval");
            DropColumn("Payroll.EmpPayrollDeduction", "isSendForApproval");
            DropColumn("Payroll.Association", "GLPeriodId");
            DropColumn("HRM.RecruitmentPlan", "Check");
            DropColumn("Payroll.Addition", "GLPeriodId");
            DropTable("HRM.RecruitmentVacancy");
            RenameIndex(table: "Payroll.OverTimeTransfer", name: "IX_GLPeriodId", newName: "IX_PeriodId");
            RenameIndex(table: "Payroll.Advance", name: "IX_GLPeriodId", newName: "IX_PeriodId");
            RenameColumn(table: "Payroll.OverTimeTransfer", name: "GLPeriodId", newName: "PeriodId");
            RenameColumn(table: "Payroll.Advance", name: "GLPeriodId", newName: "PeriodId");
            CreateIndex("Payroll.RegularContribution", "PeriodId");
            CreateIndex("Payroll.PeriodStatus", "PeriodId");
            CreateIndex("Payroll.PayrollProcessViewModel", "PeriodId");
            CreateIndex("Payroll.PayrollDistribution", "PeriodId");
            CreateIndex("Payroll.OverTime", "PeriodId");
            CreateIndex("Payroll.OverTimePerDay", "PeriodId");
            CreateIndex("Payroll.Loan", "PeriodId");
            CreateIndex("Payroll.FixedContribution", "PeriodId");
            CreateIndex("Payroll.EmpPayrollNoDaysWorked", "PeriodId");
            CreateIndex("Payroll.Association", "PeriodId");
            CreateIndex("HRM.Vacancy", "RecruitmentPlanId");
            CreateIndex("Payroll.Addition", "PeriodId");
            AddForeignKey("Payroll.RegularContribution", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.PeriodStatus", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.PayrollProcessViewModel", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.PayrollDistribution", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTime", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTimePerDay", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Loan", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.EmpPayrollNoDaysWorked", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Association", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan", "RecruitmentPlanId", cascadeDelete: true);
            AddForeignKey("Payroll.Addition", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
        }
    }
}
