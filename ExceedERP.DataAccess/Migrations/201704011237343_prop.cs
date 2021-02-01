namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prop : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ParticipantsItem", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropForeignKey("EXCEED.Lot", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.Lot", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.ParticipantsItem", new[] { "GLTaxRateID" });
            CreateTable(
                "EXCEED.PassFailResult",
                c => new
                    {
                        PassFailResultId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        SupplierCode = c.Int(nullable: false),
                        FinancialResult = c.Single(nullable: false),
                        LotID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PassFailResultId)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.TenderTechnical",
                c => new
                    {
                        TenderTechnicalId = c.Int(nullable: false, identity: true),
                        SupplierId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        Result = c.Single(nullable: false),
                        LotID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderTechnicalId)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            AddColumn("EXCEED.AnnualDepreciation", "GlPeriod", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidPParticipatingSuppliers", "IsQualified", c => c.Int(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "Technical", c => c.Single(nullable: false));
            AddColumn("EXCEED.Tender", "TenderType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Lot", "PurchaseRequisitionID", c => c.String());
            DropColumn("EXCEED.TenderEvaluationCriteria", "CriteriaName");
            DropColumn("EXCEED.TenderEvaluationResult", "CriteriaName");
            DropColumn("EXCEED.LotItems", "TaxType");
            DropColumn("EXCEED.LotItems", "TaxAmount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.LotItems", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.LotItems", "TaxType", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "CriteriaName", c => c.String(nullable: false));
            AddColumn("EXCEED.TenderEvaluationCriteria", "CriteriaName", c => c.String());
            DropForeignKey("EXCEED.TenderTechnical", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.PassFailResult", "LotID", "EXCEED.Lot");
            DropIndex("EXCEED.TenderTechnical", new[] { "LotID" });
            DropIndex("EXCEED.PassFailResult", new[] { "LotID" });
            AlterColumn("EXCEED.Lot", "PurchaseRequisitionID", c => c.Int(nullable: false));
            DropColumn("EXCEED.Tender", "TenderType");
            DropColumn("EXCEED.ParticipantsItem", "Technical");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "IsQualified");
            DropColumn("EXCEED.AnnualDepreciation", "GlPeriod");
            DropTable("EXCEED.TenderTechnical");
            DropTable("EXCEED.PassFailResult");
            CreateIndex("EXCEED.ParticipantsItem", "GLTaxRateID");
            CreateIndex("EXCEED.Lot", "PurchaseRequisitionID");
            AddForeignKey("EXCEED.Lot", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition", "PurchaseRequisitionID", cascadeDelete: true);
            AddForeignKey("EXCEED.ParticipantsItem", "GLTaxRateID", "EXCEED.GLTaxRate", "GLTaxRateID", cascadeDelete: true);
        }
    }
}
