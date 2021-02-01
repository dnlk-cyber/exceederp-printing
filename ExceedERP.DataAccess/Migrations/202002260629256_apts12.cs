namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "APTS.DrugStoreTransfer",
                c => new
                    {
                        DrugStoreTransferId = c.Int(nullable: false, identity: true),
                        FromDrugStoreId = c.Int(nullable: false),
                        ToDrugStoreId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DrugStoreTransferId);
            
            CreateTable(
                "APTS.DrugToTransferTemporaryLists",
                c => new
                    {
                        DrugToTransferTemporaryListsId = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        DrugStoreTransferId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsNew = c.Boolean(nullable: false),
                        HaveNoBalance = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DrugToTransferTemporaryListsId);
            
            AddColumn("APTS.DrugStoreItems", "IsInnerTransfered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("APTS.DrugStoreItems", "IsInnerTransfered");
            DropTable("APTS.DrugToTransferTemporaryLists");
            DropTable("APTS.DrugStoreTransfer");
        }
    }
}
