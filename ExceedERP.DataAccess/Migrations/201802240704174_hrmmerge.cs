namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmmerge : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ApplicantRegistration", "IsTransfered", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "MediaVacancyCode", c => c.String());
            AddColumn("HRM.ApplicantionForm", "IDAndTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.ApplicantionForm", "IDAndTitle");
            DropColumn("HRM.ApplicantRegistration", "MediaVacancyCode");
            DropColumn("HRM.ApplicantRegistration", "IsTransfered");
        }
    }
}
