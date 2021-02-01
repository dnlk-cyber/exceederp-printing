namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tendersuppidname : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidAwardedSupplier", "SupplierId", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "SupplierName", c => c.String());
            AddColumn("EXCEED.BidPParticipatingSuppliers", "SupplierId", c => c.String());
            AddColumn("EXCEED.BidPParticipatingSuppliers", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidPParticipatingSuppliers", "SupplierName", c => c.String());
            AddColumn("EXCEED.BidPurchasingSuppliers", "SupplierId", c => c.String());
            AddColumn("EXCEED.BidPurchasingSuppliers", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidPurchasingSuppliers", "SupplierName", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "SupplierId", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.TenderEvaluationResult", "SupplierName", c => c.String());
            AddColumn("EXCEED.LotEvaluationByDocument", "SupplierId", c => c.String());
            AddColumn("EXCEED.LotEvaluationByDocument", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.LotEvaluationByDocument", "SupplierName", c => c.String());
            AddColumn("EXCEED.LotFinancialOffer", "SupplierId", c => c.String());
            AddColumn("EXCEED.LotFinancialOffer", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.LotFinancialOffer", "SupplierName", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "SupplierId", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "SupplierName", c => c.String());
            AddColumn("EXCEED.PassFailResult", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PassFailResult", "SupplierName", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.TenderTechnical", "SupplierName", c => c.String());
            AddColumn("EXCEED.TenderMessages", "ReceiverDepartment", c => c.String());
            AddColumn("EXCEED.TenderMessages", "Receiver", c => c.String());
            AddColumn("EXCEED.TenderMessages", "ReceiveDate", c => c.DateTime(nullable: false));
            AlterColumn("EXCEED.PassFailResult", "SupplierId", c => c.String());
            AlterColumn("EXCEED.TenderTechnical", "SupplierId", c => c.String());
            DropColumn("EXCEED.BidAwardedSupplier", "Supplier");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "Supplier");
            DropColumn("EXCEED.BidPurchasingSuppliers", "Supplier");
            DropColumn("EXCEED.TenderEvaluationResult", "Supplier");
            DropColumn("EXCEED.LotEvaluationByDocument", "Supplier");
            DropColumn("EXCEED.LotFinancialOffer", "Supplier");
            DropColumn("EXCEED.ParticipantsItem", "Supplier");
            DropColumn("EXCEED.TenderMessages", "ReciverDepartment");
            DropColumn("EXCEED.TenderMessages", "Reciver");
            DropColumn("EXCEED.TenderMessages", "ReciveDate");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TenderMessages", "ReciveDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.TenderMessages", "Reciver", c => c.String());
            AddColumn("EXCEED.TenderMessages", "ReciverDepartment", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "Supplier", c => c.String());
            AddColumn("EXCEED.LotFinancialOffer", "Supplier", c => c.String());
            AddColumn("EXCEED.LotEvaluationByDocument", "Supplier", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "Supplier", c => c.String());
            AddColumn("EXCEED.BidPurchasingSuppliers", "Supplier", c => c.String(nullable: false));
            AddColumn("EXCEED.BidPParticipatingSuppliers", "Supplier", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "Supplier", c => c.String());
            AlterColumn("EXCEED.TenderTechnical", "SupplierId", c => c.String(nullable: false));
            AlterColumn("EXCEED.PassFailResult", "SupplierId", c => c.String(nullable: false));
            DropColumn("EXCEED.TenderMessages", "ReceiveDate");
            DropColumn("EXCEED.TenderMessages", "Receiver");
            DropColumn("EXCEED.TenderMessages", "ReceiverDepartment");
            DropColumn("EXCEED.TenderTechnical", "SupplierName");
            DropColumn("EXCEED.TenderTechnical", "IsNewSupplier");
            DropColumn("EXCEED.PassFailResult", "SupplierName");
            DropColumn("EXCEED.PassFailResult", "IsNewSupplier");
            DropColumn("EXCEED.ParticipantsItem", "SupplierName");
            DropColumn("EXCEED.ParticipantsItem", "IsNewSupplier");
            DropColumn("EXCEED.ParticipantsItem", "SupplierId");
            DropColumn("EXCEED.LotFinancialOffer", "SupplierName");
            DropColumn("EXCEED.LotFinancialOffer", "IsNewSupplier");
            DropColumn("EXCEED.LotFinancialOffer", "SupplierId");
            DropColumn("EXCEED.LotEvaluationByDocument", "SupplierName");
            DropColumn("EXCEED.LotEvaluationByDocument", "IsNewSupplier");
            DropColumn("EXCEED.LotEvaluationByDocument", "SupplierId");
            DropColumn("EXCEED.TenderEvaluationResult", "SupplierName");
            DropColumn("EXCEED.TenderEvaluationResult", "IsNewSupplier");
            DropColumn("EXCEED.TenderEvaluationResult", "SupplierId");
            DropColumn("EXCEED.BidPurchasingSuppliers", "SupplierName");
            DropColumn("EXCEED.BidPurchasingSuppliers", "IsNewSupplier");
            DropColumn("EXCEED.BidPurchasingSuppliers", "SupplierId");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "SupplierName");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "IsNewSupplier");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "SupplierId");
            DropColumn("EXCEED.BidAwardedSupplier", "SupplierName");
            DropColumn("EXCEED.BidAwardedSupplier", "IsNewSupplier");
            DropColumn("EXCEED.BidAwardedSupplier", "SupplierId");
        }
    }
}
