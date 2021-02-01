namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class miscpaymentdirect : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApMiscellaneousPaymentLine",
                c => new
                    {
                        ApMiscellaneousPaymentLineId = c.Int(nullable: false, identity: true),
                        ApMiscellaneousPaymentId = c.Int(nullable: false),
                        Purpose = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountCode = c.String(),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.Int(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApMiscellaneousPaymentLineId)
                .ForeignKey("EXCEED.ApMiscellaneousPayment", t => t.ApMiscellaneousPaymentId, cascadeDelete: true)
                .Index(t => t.ApMiscellaneousPaymentId);
            
            AddColumn("EXCEED.ApMiscellaneousPayment", "SupplierId", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "SupplierName", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "Purchaser", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "Grn", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "Po", c => c.String());
            DropColumn("EXCEED.ApMiscellaneousPayment", "Purpose");
            DropColumn("EXCEED.ApMiscellaneousPayment", "Amount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "AccountCode");
            DropColumn("EXCEED.ApMiscellaneousPayment", "GLTaxRateID");
            DropColumn("EXCEED.ApMiscellaneousPayment", "TaxAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdingType");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdingAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdTax");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdTaxAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "NetPayable");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdingType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "AccountCode", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "Purpose", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ApMiscellaneousPaymentLine", "ApMiscellaneousPaymentId", "EXCEED.ApMiscellaneousPayment");
            DropIndex("EXCEED.ApMiscellaneousPaymentLine", new[] { "ApMiscellaneousPaymentId" });
            DropColumn("EXCEED.ApMiscellaneousPayment", "Po");
            DropColumn("EXCEED.ApMiscellaneousPayment", "Grn");
            DropColumn("EXCEED.ApMiscellaneousPayment", "Purchaser");
            DropColumn("EXCEED.ApMiscellaneousPayment", "SupplierName");
            DropColumn("EXCEED.ApMiscellaneousPayment", "IsNewSupplier");
            DropColumn("EXCEED.ApMiscellaneousPayment", "SupplierId");
            DropTable("EXCEED.ApMiscellaneousPaymentLine");
        }
    }
}
