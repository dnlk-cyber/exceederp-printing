namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class healthapts : DbMigration
    {
        public override void Up()
        {
            AddColumn("APTS.LaboratoryTestPatientCard", "IsSentBackToDoctor", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCardDetail", "IsSentToLaboratory", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCardDetail", "IsSentDespensary", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientDrugOrder", "PatientCardDetailId", c => c.Int(nullable: false));
            AddColumn("Health.PatientDrugOrder", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.PatientDrugOrder", "IsSentToPatientRegiser", c => c.Boolean(nullable: false));
            AlterColumn("Health.PatientResult", "MainCompanyId", c => c.Int());
            DropColumn("Health.PatientCardDetail", "IsSent");
            DropColumn("Health.PatientDrugOrder", "PatientResultId");
        }
        
        public override void Down()
        {
            AddColumn("Health.PatientDrugOrder", "PatientResultId", c => c.Int(nullable: false));
            AddColumn("Health.PatientCardDetail", "IsSent", c => c.Boolean(nullable: false));
            AlterColumn("Health.PatientResult", "MainCompanyId", c => c.Int(nullable: false));
            DropColumn("Health.PatientDrugOrder", "IsSentToPatientRegiser");
            DropColumn("Health.PatientDrugOrder", "PatientId");
            DropColumn("Health.PatientDrugOrder", "PatientCardDetailId");
            DropColumn("Health.PatientCardDetail", "IsSentDespensary");
            DropColumn("Health.PatientCardDetail", "IsSentToLaboratory");
            DropColumn("APTS.LaboratoryTestPatientCard", "IsSentBackToDoctor");
        }
    }
}
