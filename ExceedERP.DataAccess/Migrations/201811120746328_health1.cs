namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Health.PatientResultDetail",
                c => new
                    {
                        PatientResultDetailId = c.Int(nullable: false, identity: true),
                        PatientResultId = c.Int(nullable: false),
                        Prescription = c.String(),
                        TestType = c.String(),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PatientResultDetailId);
            
            CreateTable(
                "Health.PatientResult",
                c => new
                    {
                        PatientResultId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        DoctorId = c.Int(nullable: false),
                        Description = c.String(),
                        IsForDespensary = c.Boolean(nullable: false),
                        IsForLaboratory = c.Boolean(nullable: false),
                        RegisterDate = c.DateTime(nullable: false),
                        RegisterDateAm = c.String(),
                    })
                .PrimaryKey(t => t.PatientResultId);
            
        }
        
        public override void Down()
        {
            DropTable("Health.PatientResult");
            DropTable("Health.PatientResultDetail");
        }
    }
}
