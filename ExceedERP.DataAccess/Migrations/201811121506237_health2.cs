namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Health.PatientDrugOrder",
                c => new
                    {
                        PatientDrugOrderId = c.Int(nullable: false, identity: true),
                        PatientResultId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PatientDrugOrderId);
            
            AddColumn("Health.PatientResult", "IsRefer", c => c.Boolean(nullable: false));
            AddColumn("Health.PatientResult", "OnChecking", c => c.Boolean(nullable: false));
            DropColumn("Health.PatientResultDetail", "DrugListsId");
            DropColumn("Health.PatientResultDetail", "DrugUnitId");
            DropColumn("Health.PatientResultDetail", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("Health.PatientResultDetail", "Quantity", c => c.Int(nullable: false));
            AddColumn("Health.PatientResultDetail", "DrugUnitId", c => c.Int(nullable: false));
            AddColumn("Health.PatientResultDetail", "DrugListsId", c => c.Int(nullable: false));
            DropColumn("Health.PatientResult", "OnChecking");
            DropColumn("Health.PatientResult", "IsRefer");
            DropTable("Health.PatientDrugOrder");
        }
    }
}
