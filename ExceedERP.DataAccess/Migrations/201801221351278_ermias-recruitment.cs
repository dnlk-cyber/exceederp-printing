namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ermiasrecruitment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.RecruitmentVacancy", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy");
            DropIndex("HRM.ApplicantionForm", new[] { "VaccancyCode" });
            DropIndex("HRM.RecruitmentVacancy", new[] { "VaccancyCode" });
            RenameColumn(table: "HRM.ApplicantionForm", name: "VaccancyCode", newName: "VacancyId");
            RenameColumn(table: "HRM.RecruitmentVacancy", name: "VaccancyCode", newName: "VacancyId");
            DropPrimaryKey("HRM.Vacancy");
            AddColumn("HRM.ApplicantRegistration", "VacancyId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "VacancyId", c => c.Int(nullable: false, identity: true));
            AlterColumn("HRM.ApplicantionForm", "VacancyId", c => c.Int(nullable: false));
            AlterColumn("HRM.Vacancy", "VaccancyCode", c => c.String());
            AlterColumn("HRM.RecruitmentVacancy", "VacancyId", c => c.Int(nullable: false));
            AddPrimaryKey("HRM.Vacancy", "VacancyId");
            CreateIndex("HRM.ApplicantionForm", "VacancyId");
            CreateIndex("HRM.RecruitmentVacancy", "VacancyId");
            AddForeignKey("HRM.RecruitmentVacancy", "VacancyId", "HRM.Vacancy", "VacancyId", cascadeDelete: false);
            AddForeignKey("HRM.ApplicantionForm", "VacancyId", "HRM.Vacancy", "VacancyId", cascadeDelete: false);
            DropColumn("HRM.ApplicantRegistration", "VacancyCode");
        }
        
        public override void Down()
        {
            AddColumn("HRM.ApplicantRegistration", "VacancyCode", c => c.String(nullable: false));
            DropForeignKey("HRM.ApplicantionForm", "VacancyId", "HRM.Vacancy");
            DropForeignKey("HRM.RecruitmentVacancy", "VacancyId", "HRM.Vacancy");
            DropIndex("HRM.RecruitmentVacancy", new[] { "VacancyId" });
            DropIndex("HRM.ApplicantionForm", new[] { "VacancyId" });
            DropPrimaryKey("HRM.Vacancy");
            AlterColumn("HRM.RecruitmentVacancy", "VacancyId", c => c.String(maxLength: 128));
            AlterColumn("HRM.Vacancy", "VaccancyCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("HRM.ApplicantionForm", "VacancyId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("HRM.Vacancy", "VacancyId");
            DropColumn("HRM.ApplicantRegistration", "VacancyId");
            AddPrimaryKey("HRM.Vacancy", "VaccancyCode");
            RenameColumn(table: "HRM.RecruitmentVacancy", name: "VacancyId", newName: "VaccancyCode");
            RenameColumn(table: "HRM.ApplicantionForm", name: "VacancyId", newName: "VaccancyCode");
            CreateIndex("HRM.RecruitmentVacancy", "VaccancyCode");
            CreateIndex("HRM.ApplicantionForm", "VaccancyCode");
            AddForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy", "VaccancyCode", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentVacancy", "VaccancyCode", "HRM.Vacancy", "VaccancyCode");
        }
    }
}
