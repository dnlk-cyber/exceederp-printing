namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("APTS.DrugReceivalModel19", "DrugStoreId", "APTS.DrugStores");
            DropForeignKey("APTS.PatientRegister", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.PatientRegister", "DrugUnitId", "APTS.DrugUnit");
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugStoreId" });
            DropIndex("APTS.PatientRegister", new[] { "DrugListsId" });
            DropIndex("APTS.PatientRegister", new[] { "DrugUnitId" });
            CreateTable(
                "APTS.DrugPurchaseRequisitions",
                c => new
                    {
                        DrugPurchaseRequisitionId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(),
                        StoreCode = c.String(),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DrugPurchaseRequisitionId);
            
            CreateTable(
                "APTS.PatientRegisterDetail",
                c => new
                    {
                        PatientRegisterDetailId = c.Int(nullable: false, identity: true),
                        PatientRegisterId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        InternalCode = c.String(),
                        TotalQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PatientRegisterDetailId);
            
            AddColumn("APTS.DrugRequestItems", "QtyAllowed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.PatientRegister", "IsCredit", c => c.Boolean(nullable: false));
            AddColumn("APTS.PatientRegister", "DrugistId", c => c.Int(nullable: false));
            AddColumn("APTS.PatientRegister", "EmployeeId", c => c.Int(nullable: false));
            DropColumn("APTS.PatientRegister", "DrugListsId");
            DropColumn("APTS.PatientRegister", "DrugUnitId");
            DropColumn("APTS.PatientRegister", "InternalCode");
            DropColumn("APTS.PatientRegister", "TotalQty");
            DropColumn("APTS.PatientRegister", "RetailPrice");
            DropColumn("APTS.PatientRegister", "TotalRetailPrice");
        }
        
        public override void Down()
        {
            AddColumn("APTS.PatientRegister", "TotalRetailPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.PatientRegister", "RetailPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.PatientRegister", "TotalQty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.PatientRegister", "InternalCode", c => c.String());
            AddColumn("APTS.PatientRegister", "DrugUnitId", c => c.Int(nullable: false));
            AddColumn("APTS.PatientRegister", "DrugListsId", c => c.Int(nullable: false));
            DropColumn("APTS.PatientRegister", "EmployeeId");
            DropColumn("APTS.PatientRegister", "DrugistId");
            DropColumn("APTS.PatientRegister", "IsCredit");
            DropColumn("APTS.DrugRequestItems", "QtyAllowed");
            DropTable("APTS.PatientRegisterDetail");
            DropTable("APTS.DrugPurchaseRequisitions");
            CreateIndex("APTS.PatientRegister", "DrugUnitId");
            CreateIndex("APTS.PatientRegister", "DrugListsId");
            CreateIndex("APTS.DrugReceivalModel19", "DrugStoreId");
            AddForeignKey("APTS.PatientRegister", "DrugUnitId", "APTS.DrugUnit", "DrugUnitId", cascadeDelete: true);
            AddForeignKey("APTS.PatientRegister", "DrugListsId", "APTS.DrugLists", "DrugListsId", cascadeDelete: true);
            AddForeignKey("APTS.DrugReceivalModel19", "DrugStoreId", "APTS.DrugStores", "DrugStoreId", cascadeDelete: true);
        }
    }
}
