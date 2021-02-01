namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmergeermi : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.RecruitmentVacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropIndex("HRM.RecruitmentVacancy", new[] { "RecruitmentPlanId" });
            AddColumn("HRM.ApplicantRegistration", "VacancyCode", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "JobtitleName", c => c.String());
            //AddColumn("HRM.Vacancy", "RecruitmentPlanId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "Check", c => c.Boolean(nullable: false));
            AddColumn("HRM.Vacancy", "WorkUnitId", c => c.Int(nullable: false));
            CreateIndex("HRM.ApplicantRegistration", "VacancyId");
            CreateIndex("HRM.Vacancy", "RecruitmentPlanId");
            CreateIndex("HRM.ApplicantScreeningResult", "ApplicantRegistrationId");
            //AddForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan", "RecruitmentPlanId", cascadeDelete: false);
            //AddForeignKey("HRM.ApplicantRegistration", "VacancyId", "HRM.Vacancy", "VacancyId", cascadeDelete: false);
            //AddForeignKey("HRM.ApplicantScreeningResult", "ApplicantRegistrationId", "HRM.ApplicantRegistration", "ApplicantRegistrationId", cascadeDelete: true);
            DropColumn("HRM.RecruitmentVacancy", "RecruitmentPlanId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.RecruitmentVacancy", "RecruitmentPlanId", c => c.Int(nullable: false));
            DropForeignKey("HRM.ApplicantScreeningResult", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.ApplicantRegistration", "VacancyId", "HRM.Vacancy");
            DropForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropIndex("HRM.ApplicantScreeningResult", new[] { "ApplicantRegistrationId" });
            DropIndex("HRM.Vacancy", new[] { "RecruitmentPlanId" });
            DropIndex("HRM.ApplicantRegistration", new[] { "VacancyId" });
            DropColumn("HRM.Vacancy", "WorkUnitId");
            DropColumn("HRM.Vacancy", "Check");
            DropColumn("HRM.Vacancy", "RecruitmentPlanId");
            DropColumn("HRM.ApplicantRegistration", "JobtitleName");
            DropColumn("HRM.ApplicantRegistration", "VacancyCode");
            CreateIndex("HRM.RecruitmentVacancy", "RecruitmentPlanId");
            AddForeignKey("HRM.RecruitmentVacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan", "RecruitmentPlanId", cascadeDelete: true);
        }
    }
}
