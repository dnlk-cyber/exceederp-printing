namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class digit4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BinCardLine", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "IssuedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.GLLedgerPostingLines", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "IssuedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
