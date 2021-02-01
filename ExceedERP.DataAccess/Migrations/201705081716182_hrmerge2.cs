namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.CourseTracking", "PeriodId", "common.Period");
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            DropIndex("HRM.CourseTracking", new[] { "PeriodId" });
            DropIndex("HRM.LeavePaid", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeavePlan", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeaveRequest", new[] { "GLFiscalYearId" });
            DropIndex("HRM.Leave", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeaveEntitle", new[] { "GLFiscalYearId" });
            DropIndex("Health.PatientsLeave", new[] { "GLFiscalYearId" });
            AddColumn("HRM.ApplicantRegistration", "VacancyCode", c => c.String(nullable: false));
            AddColumn("HRM.CourseTracking", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.Experience", "IsPrivate", c => c.Boolean(nullable: false));
            AddColumn("HRM.JobTitleMapping", "WorkUnitId", c => c.Int(nullable: false));
            AddColumn("HRM.JobTitleMapping", "BranchId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentPlan", "WorkUnitId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentPlan", "BranchId", c => c.Int(nullable: false));
            CreateIndex("HRM.CourseTracking", "GlFiscalYearId");
            CreateIndex("HRM.JobTitleMapping", "BranchId");
            CreateIndex("HRM.LeavePaid", "GlFiscalYearId");
            CreateIndex("HRM.LeavePlan", "GlFiscalYearId");
            CreateIndex("HRM.LeaveRequest", "GlFiscalYearId");
            CreateIndex("HRM.Leave", "GlFiscalYearId");
            CreateIndex("HRM.LeaveEntitle", "GlFiscalYearId");
            CreateIndex("Health.PatientsLeave", "GlFiscalYearId");
            CreateIndex("HRM.RecruitmentPlan", "BranchId");
            AddForeignKey("HRM.CourseTracking", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.JobTitleMapping", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentPlan", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            DropColumn("HRM.ApplicantRegistration", "VaccancyCode");
            DropColumn("HRM.CourseTracking", "PeriodId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.CourseTracking", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "VaccancyCode", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("HRM.RecruitmentPlan", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.JobTitleMapping", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.CourseTracking", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.RecruitmentPlan", new[] { "BranchId" });
            DropIndex("Health.PatientsLeave", new[] { "GlFiscalYearId" });
            DropIndex("HRM.LeaveEntitle", new[] { "GlFiscalYearId" });
            DropIndex("HRM.Leave", new[] { "GlFiscalYearId" });
            DropIndex("HRM.LeaveRequest", new[] { "GlFiscalYearId" });
            DropIndex("HRM.LeavePlan", new[] { "GlFiscalYearId" });
            DropIndex("HRM.LeavePaid", new[] { "GlFiscalYearId" });
            DropIndex("HRM.JobTitleMapping", new[] { "BranchId" });
            DropIndex("HRM.CourseTracking", new[] { "GlFiscalYearId" });
            DropColumn("HRM.RecruitmentPlan", "BranchId");
            DropColumn("HRM.RecruitmentPlan", "WorkUnitId");
            DropColumn("HRM.JobTitleMapping", "BranchId");
            DropColumn("HRM.JobTitleMapping", "WorkUnitId");
            DropColumn("HRM.Experience", "IsPrivate");
            DropColumn("HRM.CourseTracking", "GlFiscalYearId");
            DropColumn("HRM.ApplicantRegistration", "VacancyCode");
            CreateIndex("Health.PatientsLeave", "GLFiscalYearId");
            CreateIndex("HRM.LeaveEntitle", "GLFiscalYearId");
            CreateIndex("HRM.Leave", "GLFiscalYearId");
            CreateIndex("HRM.LeaveRequest", "GLFiscalYearId");
            CreateIndex("HRM.LeavePlan", "GLFiscalYearId");
            CreateIndex("HRM.LeavePaid", "GLFiscalYearId");
            CreateIndex("HRM.CourseTracking", "PeriodId");
            CreateIndex("HRM.ApplicantRegistration", "VaccancyCode");
            AddForeignKey("HRM.CourseTracking", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy", "VaccancyCode", cascadeDelete: true);
        }
    }
}
