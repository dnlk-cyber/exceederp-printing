namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subtaxtotal : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ARMiscelaneousCollection", "LineAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ARMiscelaneousCollection", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ArStandardCollection", "LineAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ArStandardCollection", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ArStandardCollection", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            DropColumn("EXCEED.ARMiscelaneousCollection", "ReceiptAmount");
            DropColumn("EXCEED.ArStandardCollection", "ReceiptAmount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArStandardCollection", "ReceiptAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ARMiscelaneousCollection", "ReceiptAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.ArStandardCollection", "TotalAmount");
            DropColumn("EXCEED.ArStandardCollection", "TaxAmount");
            DropColumn("EXCEED.ArStandardCollection", "LineAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "TotalAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "TaxAmount");
            DropColumn("EXCEED.ARMiscelaneousCollection", "LineAmount");
        }
    }
}
