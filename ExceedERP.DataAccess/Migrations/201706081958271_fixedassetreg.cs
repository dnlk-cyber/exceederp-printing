namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetreg : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Sales", "GLTaxRateID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Sales", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.Sales", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.Sales", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.Sales", "TotalAmount", c => c.String(nullable: false));
            AlterColumn("EXCEED.Sales", "TaxAmount", c => c.String());
            AlterColumn("EXCEED.Sales", "Amount", c => c.String(nullable: false));
            DropColumn("EXCEED.Sales", "GLTaxRateID");
        }
    }
}
