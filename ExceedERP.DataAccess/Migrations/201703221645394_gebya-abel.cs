namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gebyaabel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ParticipantsItem",
                c => new
                    {
                        ParticipantsItemId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        Supplier = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LotID = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ParticipantsItemId)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .ForeignKey("EXCEED.GLTaxRate", t => t.GLTaxRateID, cascadeDelete: true)
                .Index(t => t.LotID)
                .Index(t => t.GLTaxRateID);
            
            CreateTable(
                "EXCEED.TenderMessages",
                c => new
                    {
                        TenderMessagesId = c.Int(nullable: false, identity: true),
                        ReciverDepartment = c.String(),
                        Sender = c.String(),
                        Reciver = c.String(),
                        Message = c.String(),
                        FileName = c.String(),
                        PhysicalPath = c.String(),
                        MessageStatus = c.Int(nullable: false),
                        TenderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderMessagesId)
                .ForeignKey("EXCEED.Tender", t => t.TenderId, cascadeDelete: true)
                .Index(t => t.TenderId);
            
            AddColumn("EXCEED.InventoryItem", "Abbrevation", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItemCode", c => c.String(nullable: false));
            AddColumn("EXCEED.TenderEvaluationResult", "ItemCode", c => c.String(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsTransfer", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "ToStore", c => c.String());
            AddColumn("EXCEED.Tender", "TenderMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodReceive", "TenderLot", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "SupplierReference", c => c.String());
            AlterColumn("EXCEED.BidAwardedSupplier", "Supplier", c => c.Int(nullable: false));
            AlterColumn("EXCEED.BidPParticipatingSuppliers", "Supplier", c => c.Int(nullable: false));
            AlterColumn("EXCEED.TenderEvaluationResult", "SupplierName", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ProcurementSupplier", "Category", c => c.String());
            AlterColumn("EXCEED.ProcurementSupplier", "TradeLicense", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.TenderMessages", "TenderId", "EXCEED.Tender");
            DropForeignKey("EXCEED.ParticipantsItem", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropForeignKey("EXCEED.ParticipantsItem", "LotID", "EXCEED.Lot");
            DropIndex("EXCEED.TenderMessages", new[] { "TenderId" });
            DropIndex("EXCEED.ParticipantsItem", new[] { "GLTaxRateID" });
            DropIndex("EXCEED.ParticipantsItem", new[] { "LotID" });
            AlterColumn("EXCEED.ProcurementSupplier", "TradeLicense", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProcurementSupplier", "Category", c => c.String(nullable: false));
            AlterColumn("EXCEED.TenderEvaluationResult", "SupplierName", c => c.String(nullable: false));
            AlterColumn("EXCEED.BidPParticipatingSuppliers", "Supplier", c => c.String());
            AlterColumn("EXCEED.BidAwardedSupplier", "Supplier", c => c.String());
            DropColumn("EXCEED.ProcurementSupplier", "SupplierReference");
            DropColumn("EXCEED.GoodReceive", "TenderLot");
            DropColumn("EXCEED.Tender", "TenderMethod");
            DropColumn("EXCEED.StoreRequisition", "ToStore");
            DropColumn("EXCEED.StoreRequisition", "IsTransfer");
            DropColumn("EXCEED.TenderEvaluationResult", "ItemCode");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemCode");
            DropColumn("EXCEED.InventoryItem", "Abbrevation");
            DropTable("EXCEED.TenderMessages");
            DropTable("EXCEED.ParticipantsItem");
        }
    }
}
