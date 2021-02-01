namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Health.PatientCardDetail", "IsSent", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCardDetail", "IsTested", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCardDetail", "IsClosed", c => c.Boolean(nullable: false));
            DropColumn("Health.PatientCard", "IsSent");
            DropColumn("Health.PatientCard", "IsTested");
        }
        
        public override void Down()
        {
            AddColumn("Health.PatientCard", "IsTested", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCard", "IsSent", c => c.Boolean(nullable: false));
            DropColumn("Health.PatientCardDetail", "IsClosed");
            DropColumn("Health.PatientCardDetail", "IsTested");
            DropColumn("Health.PatientCardDetail", "IsSent");
        }
    }
}
