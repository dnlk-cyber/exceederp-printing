namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ApplicantRegistration", "FirstName", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "MiddleName", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "LastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.ApplicantRegistration", "LastName");
            DropColumn("HRM.ApplicantRegistration", "MiddleName");
            DropColumn("HRM.ApplicantRegistration", "FirstName");
        }
    }
}
