namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("APTS.DespensaryStoreS", "BegStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DespensaryStoreS", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DespensaryStoreS", "Unit", c => c.String());
            AddColumn("APTS.DrugStores", "BegStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "Unit", c => c.String());
            AddColumn("APTS.RequisitionModel20", "Unit", c => c.String());
            AddColumn("APTS.RequisitionModel20", "QuantityIssued", c => c.Int(nullable: false));
            AddColumn("APTS.RequisitionModel20", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.RequisitionModel20", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.RequisitionModel20", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.RequisitionModel20", "RetailPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.RequisitionModel20", "DispensaryEntryDate", c => c.DateTime());
            AddColumn("APTS.RequisitionModel20", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("APTS.PatientRegister", "Unit", c => c.String());
            DropColumn("APTS.DespensaryStoreS", "CurrentAmount");
            DropColumn("APTS.DespensaryStoreS", "TotalAmount");
            DropColumn("APTS.DrugStores", "CurrentAmount");
            DropColumn("APTS.DrugStores", "TotalAmount");
        }
        
        public override void Down()
        {
            AddColumn("APTS.DrugStores", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "CurrentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DespensaryStoreS", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DespensaryStoreS", "CurrentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("APTS.PatientRegister", "Unit");
            DropColumn("APTS.RequisitionModel20", "IsClosed");
            DropColumn("APTS.RequisitionModel20", "DispensaryEntryDate");
            DropColumn("APTS.RequisitionModel20", "RetailPrice");
            DropColumn("APTS.RequisitionModel20", "UnitPrice");
            DropColumn("APTS.RequisitionModel20", "TotalCost");
            DropColumn("APTS.RequisitionModel20", "UnitCost");
            DropColumn("APTS.RequisitionModel20", "QuantityIssued");
            DropColumn("APTS.RequisitionModel20", "Unit");
            DropColumn("APTS.DrugStores", "Unit");
            DropColumn("APTS.DrugStores", "Balance");
            DropColumn("APTS.DrugStores", "BegStock");
            DropColumn("APTS.DespensaryStoreS", "Unit");
            DropColumn("APTS.DespensaryStoreS", "Balance");
            DropColumn("APTS.DespensaryStoreS", "BegStock");
        }
    }
}
