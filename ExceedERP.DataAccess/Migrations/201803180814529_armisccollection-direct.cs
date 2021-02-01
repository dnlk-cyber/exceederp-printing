namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class armisccollectiondirect : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArMiscelaneousCollectionLine",
                c => new
                    {
                        ArMiscelaneousCollectionLineId = c.Int(nullable: false, identity: true),
                        ARMiscelaneousCollectionID = c.Int(nullable: false),
                        Activity = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevenueAccount = c.String(nullable: false),
                        RevenueAccountDesc = c.String(),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.String(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetReceipt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArMiscelaneousCollectionLineId)
                .ForeignKey("EXCEED.ARMiscelaneousCollection", t => t.ARMiscelaneousCollectionID, cascadeDelete: true)
                .Index(t => t.ARMiscelaneousCollectionID);
            
            AddColumn("EXCEED.ARMiscelaneousCollection", "CustomerId", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollection", "IsNewCustomer", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "CustomerName", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollection", "IsDirectCollection", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ARMiscelaneousCollection", "Activity");
            DropColumn("EXCEED.ARMiscelaneousCollection", "Amount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "RevenueAccount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "RevenueAccountDesc");
            DropColumn("EXCEED.ARMiscelaneousCollection", "TaxType");
            DropColumn("EXCEED.ARMiscelaneousCollection", "TaxAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "WithholdingType");
            DropColumn("EXCEED.ARMiscelaneousCollection", "WithholdingAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "WithholdTax");
            DropColumn("EXCEED.ARMiscelaneousCollection", "WithholdTaxAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "ReceiptAmount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ARMiscelaneousCollection", "ReceiptAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "WithholdingType", c => c.String(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "TaxType", c => c.String(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "RevenueAccountDesc", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollection", "RevenueAccount", c => c.String(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "Activity", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ArMiscelaneousCollectionLine", "ARMiscelaneousCollectionID", "EXCEED.ARMiscelaneousCollection");
            DropIndex("EXCEED.ArMiscelaneousCollectionLine", new[] { "ARMiscelaneousCollectionID" });
            DropColumn("EXCEED.ARMiscelaneousCollection", "IsDirectCollection");
            DropColumn("EXCEED.ARMiscelaneousCollection", "CustomerName");
            DropColumn("EXCEED.ARMiscelaneousCollection", "IsNewCustomer");
            DropColumn("EXCEED.ARMiscelaneousCollection", "CustomerId");
            DropTable("EXCEED.ArMiscelaneousCollectionLine");
        }
    }
}
