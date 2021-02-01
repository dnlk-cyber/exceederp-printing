namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "APTS.DespensaryStoreS",
                c => new
                    {
                        DespensaryStoreId = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        CurrentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DespensaryStoreId)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .Index(t => t.DrugListsId);
            
            CreateTable(
                "APTS.DrugLists",
                c => new
                    {
                        DrugListsId = c.Int(nullable: false, identity: true),
                        DrugCode = c.String(),
                        InternalDrugCode = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsOur = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DrugListsId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "APTS.DrugReceivalModel19",
                c => new
                    {
                        DrugReceivalModel19Id = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        ReceiverName = c.String(),
                        ReceivingDate = c.DateTime(nullable: false),
                        ReceivingDateAmh = c.String(),
                        Name = c.String(),
                        ReceiptNo = c.String(),
                        Unit = c.String(),
                        QtyReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BatchNo = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        ExpiryDateAmh = c.String(),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        IsCurrent = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DrugReceivalModel19Id)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .Index(t => t.DrugListsId);
            
            CreateTable(
                "APTS.DrugStores",
                c => new
                    {
                        DrugStoreId = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        StoreCode = c.String(maxLength: 128),
                        CurrentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DrugStoreId)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode)
                .Index(t => t.DrugListsId)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "APTS.DrugUnit",
                c => new
                    {
                        DrugUnitId = c.Int(nullable: false, identity: true),
                        UnitName = c.String(),
                        UoMName = c.String(maxLength: 128),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DrugUnitId)
                .ForeignKey("EXCEED.UoM", t => t.UoMName)
                .Index(t => t.UoMName);
            
            CreateTable(
                "APTS.Model22",
                c => new
                    {
                        Model22Id = c.Int(nullable: false, identity: true),
                        RequisitionModel20Id = c.Int(nullable: false),
                        Unit = c.String(),
                        QuantityIssued = c.Int(nullable: false),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsCompleted = c.Boolean(nullable: false),
                        IsOut = c.Boolean(nullable: false),
                        IsNew = c.Boolean(nullable: false),
                        DispensaryEntryDate = c.DateTime(),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Model22Id)
                .ForeignKey("APTS.RequisitionModel20", t => t.RequisitionModel20Id, cascadeDelete: true)
                .Index(t => t.RequisitionModel20Id);
            
            CreateTable(
                "APTS.RequisitionModel20",
                c => new
                    {
                        RequisitionModel20Id = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        Description = c.String(),
                        InternalCode = c.String(),
                        BegStock = c.Int(nullable: false),
                        ReceivedQty = c.Int(nullable: false),
                        Loss = c.Int(nullable: false),
                        EndStock = c.Int(nullable: false),
                        CalculatedConsumption = c.Int(nullable: false),
                        Max = c.Int(nullable: false),
                        QtyToMax = c.Int(nullable: false),
                        QtyToBeApproved = c.Int(nullable: false),
                        RequestedBy = c.String(),
                        RequestDate = c.DateTime(),
                        RequestDateAm = c.String(),
                        ApproverName = c.String(),
                        ApprovedDate = c.DateTime(),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        IsOut = c.Boolean(nullable: false),
                        IsNew = c.Boolean(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Requester = c.String(),
                        Remark = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RequisitionModel20Id)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .Index(t => t.DrugListsId);
            
            CreateTable(
                "APTS.PatientRegister",
                c => new
                    {
                        PatientRegisterId = c.Int(nullable: false, identity: true),
                        DrugListsId = c.Int(nullable: false),
                        InternalCode = c.String(),
                        PatientName = c.String(),
                        CardNumber = c.String(),
                        Age = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        NameOfOrg = c.String(),
                        RegisterDate = c.DateTime(nullable: false),
                        RegisterDateAmh = c.String(),
                        TotalQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PatientSignature = c.String(),
                        SurviceProviderName = c.String(),
                        SurviceProviderDepartment = c.String(),
                        SurviceProviderSign = c.String(),
                        AccountantName = c.String(),
                        AccountantSign = c.String(),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.PatientRegisterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("APTS.Model22", "RequisitionModel20Id", "APTS.RequisitionModel20");
            DropForeignKey("APTS.RequisitionModel20", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugUnit", "UoMName", "EXCEED.UoM");
            DropForeignKey("APTS.DrugStores", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("APTS.DrugStores", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugReceivalModel19", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DespensaryStoreS", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugLists", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("APTS.DrugLists", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("APTS.RequisitionModel20", new[] { "DrugListsId" });
            DropIndex("APTS.Model22", new[] { "RequisitionModel20Id" });
            DropIndex("APTS.DrugUnit", new[] { "UoMName" });
            DropIndex("APTS.DrugStores", new[] { "StoreCode" });
            DropIndex("APTS.DrugStores", new[] { "DrugListsId" });
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugListsId" });
            DropIndex("APTS.DrugLists", new[] { "ItemCode" });
            DropIndex("APTS.DrugLists", new[] { "ItemCategoryCode" });
            DropIndex("APTS.DespensaryStoreS", new[] { "DrugListsId" });
            DropTable("APTS.PatientRegister");
            DropTable("APTS.RequisitionModel20");
            DropTable("APTS.Model22");
            DropTable("APTS.DrugUnit");
            DropTable("APTS.DrugStores");
            DropTable("APTS.DrugReceivalModel19");
            DropTable("APTS.DrugLists");
            DropTable("APTS.DespensaryStoreS");
        }
    }
}
