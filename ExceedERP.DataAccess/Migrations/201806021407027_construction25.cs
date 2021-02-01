namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction25 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConsReceivableInvoiceDistribution",
                c => new
                    {
                        ConsReceivableInvoiceDistributionId = c.Int(nullable: false, identity: true),
                        ConsReceivableInvoiceId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsReceivableInvoiceDistributionId)
                .ForeignKey("Construction.ConsReceivableInvoice", t => t.ConsReceivableInvoiceId, cascadeDelete: true)
                .Index(t => t.ConsReceivableInvoiceId);
            
            CreateTable(
                "Construction.ConsStandardCollectionDistribution",
                c => new
                    {
                        ConsStandardCollectionDistributionId = c.Int(nullable: false, identity: true),
                        ConsARStandardCollectionId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsStandardCollectionDistributionId)
                .ForeignKey("Construction.ConsARStandardCollection", t => t.ConsARStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ConsARStandardCollectionId);
            
            CreateTable(
                "Construction.WorkInProgressDistribution",
                c => new
                    {
                        WorkInProgressDistributionId = c.Int(nullable: false, identity: true),
                        WorkInProgressId = c.Int(nullable: false),
                        WorkInProgressWorkItemId = c.Int(),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkInProgressDistributionId)
                .ForeignKey("Construction.WorkInProgress", t => t.WorkInProgressId, cascadeDelete: true)
                .Index(t => t.WorkInProgressId);
            
            AddColumn("Construction.ConsARStandardCollectionDetail", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("Construction.ConsARStandardCollectionDetail", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ConsARStandardCollectionDetail", "VatWithholding", c => c.Boolean(nullable: false));
            AddColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxRateId", c => c.Int(nullable: false));
            AddColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ConsARStandardCollection", "PaymentMethodID", c => c.Int(nullable: false));
            DropColumn("Construction.ConsARStandardCollectionDetail", "WithholdingType");
            DropColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTax");
            DropColumn("Construction.ConsARStandardCollection", "ReceiptMethodId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ConsARStandardCollection", "ReceiptMethodId", c => c.Int(nullable: false));
            AddColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ConsARStandardCollectionDetail", "WithholdingType", c => c.Int(nullable: false));
            DropForeignKey("Construction.WorkInProgressDistribution", "WorkInProgressId", "Construction.WorkInProgress");
            DropForeignKey("Construction.ConsStandardCollectionDistribution", "ConsARStandardCollectionId", "Construction.ConsARStandardCollection");
            DropForeignKey("Construction.ConsReceivableInvoiceDistribution", "ConsReceivableInvoiceId", "Construction.ConsReceivableInvoice");
            DropIndex("Construction.WorkInProgressDistribution", new[] { "WorkInProgressId" });
            DropIndex("Construction.ConsStandardCollectionDistribution", new[] { "ConsARStandardCollectionId" });
            DropIndex("Construction.ConsReceivableInvoiceDistribution", new[] { "ConsReceivableInvoiceId" });
            DropColumn("Construction.ConsARStandardCollection", "PaymentMethodID");
            DropColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxAmount");
            DropColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxRateId");
            DropColumn("Construction.ConsARStandardCollectionDetail", "VatWithholding");
            DropColumn("Construction.ConsARStandardCollectionDetail", "TaxAmount");
            DropColumn("Construction.ConsARStandardCollectionDetail", "GLTaxRateID");
            DropTable("Construction.WorkInProgressDistribution");
            DropTable("Construction.ConsStandardCollectionDistribution");
            DropTable("Construction.ConsReceivableInvoiceDistribution");
        }
    }
}
