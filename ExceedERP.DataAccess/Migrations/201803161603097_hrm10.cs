namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Vacancy", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.Vacancy", "IsComplete", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantionForm", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentVacancy", "From", c => c.DateTime());
            AddColumn("HRM.RecruitmentVacancy", "To", c => c.DateTime());
            AddColumn("HRM.RecruitmentVacancy", "FromAmh", c => c.String());
            AddColumn("HRM.RecruitmentVacancy", "ToAmh", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.RecruitmentVacancy", "ToAmh");
            DropColumn("HRM.RecruitmentVacancy", "FromAmh");
            DropColumn("HRM.RecruitmentVacancy", "To");
            DropColumn("HRM.RecruitmentVacancy", "From");
            DropColumn("HRM.ApplicantionForm", "OrgStructureId");
            DropColumn("HRM.Vacancy", "IsComplete");
            DropColumn("HRM.Vacancy", "IsActive");
        }
    }
}
