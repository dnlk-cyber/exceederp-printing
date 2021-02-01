namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "APTS.LaboratoryCatagory",
                c => new
                    {
                        LaboratoryCatagoryId = c.Int(nullable: false, identity: true),
                        Catagory = c.String(),
                        DateCreated = c.DateTime(),
                        DateModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LaboratoryCatagoryId);
            
            CreateTable(
                "APTS.LaboratoryCatagoryPatientCard",
                c => new
                    {
                        LaboratoryCatagoryPatientCardId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        PatientCardDetailId = c.Int(nullable: false),
                        Catagory = c.String(),
                    })
                .PrimaryKey(t => t.LaboratoryCatagoryPatientCardId);
            
            CreateTable(
                "APTS.LaboratoryTestPatientCard",
                c => new
                    {
                        LaboratoryTestPatientCardId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        PatientCardDetailId = c.Int(nullable: false),
                        LaboratoryCatagoryPatientCardId = c.Int(nullable: false),
                        Test = c.String(),
                        Reference = c.String(),
                        Result = c.String(),
                    })
                .PrimaryKey(t => t.LaboratoryTestPatientCardId);
            
            CreateTable(
                "APTS.LaboratoryTests",
                c => new
                    {
                        LaboratoryTestsId = c.Int(nullable: false, identity: true),
                        LaboratoryCatagoryId = c.Int(nullable: false),
                        Test = c.String(),
                        Reference = c.String(),
                        Result = c.String(),
                        DateCreated = c.DateTime(),
                        DateModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LaboratoryTestsId);
            
            AddColumn("Health.PatientCard", "IsSent", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCard", "IsTested", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCard", "IsNew", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientCard", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Health.PatientCard", "LastModifiedDate");
            DropColumn("Health.PatientCard", "IsNew");
            DropColumn("Health.PatientCard", "IsTested");
            DropColumn("Health.PatientCard", "IsSent");
            DropTable("APTS.LaboratoryTests");
            DropTable("APTS.LaboratoryTestPatientCard");
            DropTable("APTS.LaboratoryCatagoryPatientCard");
            DropTable("APTS.LaboratoryCatagory");
        }
    }
}
