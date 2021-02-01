namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Health.Diagnosis",
                c => new
                    {
                        DiagnosisId = c.Int(nullable: false, identity: true),
                        PatientResultId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.DiagnosisId);
            
            AddColumn("Health.PatientDrugOrder", "Prescription", c => c.String());
            AddColumn("Health.PatientResultDetail", "Description", c => c.String());
            AddColumn("Health.PatientResult", "MainCompanyId", c => c.Int(nullable: false));
            DropColumn("Health.PatientResultDetail", "Prescription");
        }
        
        public override void Down()
        {
            AddColumn("Health.PatientResultDetail", "Prescription", c => c.String());
            DropColumn("Health.PatientResult", "MainCompanyId");
            DropColumn("Health.PatientResultDetail", "Description");
            DropColumn("Health.PatientDrugOrder", "Prescription");
            DropTable("Health.Diagnosis");
        }
    }
}
