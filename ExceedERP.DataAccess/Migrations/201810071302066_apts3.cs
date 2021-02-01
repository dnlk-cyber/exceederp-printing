namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("APTS.DespensaryStores", "LastUpdateDate", c => c.DateTime(nullable: false));
            AddColumn("APTS.PatientRegister", "DrugUnitId", c => c.Int(nullable: false));
            CreateIndex("APTS.PatientRegister", "DrugListsId");
            CreateIndex("APTS.PatientRegister", "DrugUnitId");
            AddForeignKey("APTS.PatientRegister", "DrugListsId", "APTS.DrugLists", "DrugListsId", cascadeDelete: true);
            AddForeignKey("APTS.PatientRegister", "DrugUnitId", "APTS.DrugUnit", "DrugUnitId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("APTS.PatientRegister", "DrugUnitId", "APTS.DrugUnit");
            DropForeignKey("APTS.PatientRegister", "DrugListsId", "APTS.DrugLists");
            DropIndex("APTS.PatientRegister", new[] { "DrugUnitId" });
            DropIndex("APTS.PatientRegister", new[] { "DrugListsId" });
            DropColumn("APTS.PatientRegister", "DrugUnitId");
            DropColumn("APTS.DespensaryStores", "LastUpdateDate");
        }
    }
}
