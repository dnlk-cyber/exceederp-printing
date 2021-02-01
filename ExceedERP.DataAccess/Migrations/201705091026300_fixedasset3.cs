namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ApplicantRegistration", "VacancyType", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantionForm", "VacancyType", c => c.Int(nullable: false));
            AddColumn("Payroll.OverTimeTransfer", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Payroll.OverTimeTransfer", "Remark");
            DropColumn("HRM.ApplicantionForm", "VacancyType");
            DropColumn("HRM.ApplicantRegistration", "VacancyType");
        }
    }
}
