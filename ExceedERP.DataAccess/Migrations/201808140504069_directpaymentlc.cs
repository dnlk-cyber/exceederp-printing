namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class directpaymentlc : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApDirectPaymentLine", "Currency", c => c.String());
            AddColumn("EXCEED.ApDirectPaymentLine", "ExchangeRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApDirectPaymentLine", "ForexAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApDirectPaymentLine", "ForexAmount");
            DropColumn("EXCEED.ApDirectPaymentLine", "ExchangeRate");
            DropColumn("EXCEED.ApDirectPaymentLine", "Currency");
        }
    }
}
