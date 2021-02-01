namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ermiashr : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.RecruitmentVacancy", "MediaVacancyCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.RecruitmentVacancy", "MediaVacancyCode");
        }
    }
}
