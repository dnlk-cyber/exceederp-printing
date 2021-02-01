namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ApplicantRegistration", "Rank", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "Check", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantScreeningResult", "Rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.ApplicantScreeningResult", "Rank");
            DropColumn("HRM.ApplicantRegistration", "Check");
            DropColumn("HRM.ApplicantRegistration", "Rank");
        }
    }
}
