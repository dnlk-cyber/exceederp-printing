namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("APTS.DrugStores", "DrugListsId", "APTS.DrugLists");
            DropIndex("APTS.DrugStores", new[] { "DrugListsId" });
            CreateTable(
                "APTS.APTSRequestStatus",
                c => new
                    {
                        APTSRequestStatusId = c.Int(nullable: false, identity: true),
                        RequestId = c.Int(nullable: false),
                        ReferenceType = c.String(),
                        Status = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateReqStatAm = c.String(),
                        ChangedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.APTSRequestStatusId);
            
            CreateTable(
                "APTS.DrugRequestItems",
                c => new
                    {
                        DrugRequestItemId = c.Int(nullable: false, identity: true),
                        DrugRequestId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        BegStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loss = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyRequested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyApproved = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DrugRequestItemId)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .ForeignKey("APTS.DrugRequests", t => t.DrugRequestId, cascadeDelete: true)
                .ForeignKey("APTS.DrugUnit", t => t.DrugUnitId, cascadeDelete: true)
                .Index(t => t.DrugRequestId)
                .Index(t => t.DrugListsId)
                .Index(t => t.DrugUnitId);
            
            CreateTable(
                "APTS.DrugRequests",
                c => new
                    {
                        DrugRequestId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(),
                        RequestedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        Remark = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DrugRequestId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "APTS.DrugStoreItems",
                c => new
                    {
                        DrugStoreItemId = c.Int(nullable: false, identity: true),
                        DrugStoreId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DrugStoreItemId)
                .ForeignKey("APTS.DrugLists", t => t.DrugListsId, cascadeDelete: true)
                .ForeignKey("APTS.DrugStores", t => t.DrugStoreId, cascadeDelete: true)
                .ForeignKey("APTS.DrugUnit", t => t.DrugUnitId, cascadeDelete: true)
                .Index(t => t.DrugStoreId)
                .Index(t => t.DrugListsId)
                .Index(t => t.DrugUnitId);
            
            AddColumn("APTS.DespensaryStores", "DrugUnitId", c => c.Int(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "DrugStoreId", c => c.Int(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "StoreCode", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "DrugUnitId", c => c.Int(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "BranchId", c => c.Int(nullable: false));
            AlterColumn("APTS.RequisitionModel20", "QuantityIssued", c => c.Int());
            AlterColumn("APTS.RequisitionModel20", "UnitCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "TotalCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "RetailPrice", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("APTS.DespensaryStores", "DrugUnitId");
            CreateIndex("APTS.DrugReceivalModel19", "DrugStoreId");
            CreateIndex("APTS.DrugReceivalModel19", "DrugUnitId");
            CreateIndex("APTS.DrugStores", "BranchId");
            AddForeignKey("APTS.DespensaryStores", "DrugUnitId", "APTS.DrugUnit", "DrugUnitId", cascadeDelete: true);
            AddForeignKey("APTS.DrugStores", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("APTS.DrugReceivalModel19", "DrugStoreId", "APTS.DrugStores", "DrugStoreId", cascadeDelete: true);
            AddForeignKey("APTS.DrugReceivalModel19", "DrugUnitId", "APTS.DrugUnit", "DrugUnitId", cascadeDelete: true);
            DropColumn("APTS.DespensaryStores", "Unit");
            DropColumn("APTS.DrugReceivalModel19", "Unit");
            DropColumn("APTS.DrugStores", "DrugListsId");
            DropColumn("APTS.DrugStores", "BegStock");
            DropColumn("APTS.DrugStores", "NewReceived");
            DropColumn("APTS.DrugStores", "Balance");
            DropColumn("APTS.DrugStores", "Unit");
        }
        
        public override void Down()
        {
            AddColumn("APTS.DrugStores", "Unit", c => c.String());
            AddColumn("APTS.DrugStores", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "NewReceived", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "BegStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugStores", "DrugListsId", c => c.Int(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "Unit", c => c.String());
            AddColumn("APTS.DespensaryStores", "Unit", c => c.String());
            DropForeignKey("APTS.DrugStoreItems", "DrugUnitId", "APTS.DrugUnit");
            DropForeignKey("APTS.DrugStoreItems", "DrugStoreId", "APTS.DrugStores");
            DropForeignKey("APTS.DrugStoreItems", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugRequestItems", "DrugUnitId", "APTS.DrugUnit");
            DropForeignKey("APTS.DrugRequestItems", "DrugRequestId", "APTS.DrugRequests");
            DropForeignKey("APTS.DrugRequests", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("APTS.DrugRequests", "BranchId", "HRM.Branch");
            DropForeignKey("APTS.DrugRequestItems", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugReceivalModel19", "DrugUnitId", "APTS.DrugUnit");
            DropForeignKey("APTS.DrugReceivalModel19", "DrugStoreId", "APTS.DrugStores");
            DropForeignKey("APTS.DrugStores", "BranchId", "HRM.Branch");
            DropForeignKey("APTS.DespensaryStores", "DrugUnitId", "APTS.DrugUnit");
            DropIndex("APTS.DrugStoreItems", new[] { "DrugUnitId" });
            DropIndex("APTS.DrugStoreItems", new[] { "DrugListsId" });
            DropIndex("APTS.DrugStoreItems", new[] { "DrugStoreId" });
            DropIndex("APTS.DrugRequests", new[] { "GlFiscalYearId" });
            DropIndex("APTS.DrugRequests", new[] { "BranchId" });
            DropIndex("APTS.DrugRequestItems", new[] { "DrugUnitId" });
            DropIndex("APTS.DrugRequestItems", new[] { "DrugListsId" });
            DropIndex("APTS.DrugRequestItems", new[] { "DrugRequestId" });
            DropIndex("APTS.DrugStores", new[] { "BranchId" });
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugUnitId" });
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugStoreId" });
            DropIndex("APTS.DespensaryStores", new[] { "DrugUnitId" });
            AlterColumn("APTS.RequisitionModel20", "RetailPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("APTS.RequisitionModel20", "QuantityIssued", c => c.Int(nullable: false));
            DropColumn("APTS.DrugStores", "BranchId");
            DropColumn("APTS.DrugReceivalModel19", "Balance");
            DropColumn("APTS.DrugReceivalModel19", "DrugUnitId");
            DropColumn("APTS.DrugReceivalModel19", "StoreCode");
            DropColumn("APTS.DrugReceivalModel19", "DrugStoreId");
            DropColumn("APTS.DespensaryStores", "DrugUnitId");
            DropTable("APTS.DrugStoreItems");
            DropTable("APTS.DrugRequests");
            DropTable("APTS.DrugRequestItems");
            DropTable("APTS.APTSRequestStatus");
            CreateIndex("APTS.DrugStores", "DrugListsId");
            AddForeignKey("APTS.DrugStores", "DrugListsId", "APTS.DrugLists", "DrugListsId", cascadeDelete: true);
        }
    }
}
