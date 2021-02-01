namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Archive.OutgoingLetter", "OutgoingLetterDate", c => c.DateTime(nullable: false));
            AddColumn("Archive.OutgoingLetter", "OutgoingLetterDateAm", c => c.String());
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "IssuedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLineSecondary", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Archive.OutgoingLetter", "OutgoingLetterDateAm");
            DropColumn("Archive.OutgoingLetter", "OutgoingLetterDate");
        }
    }
}
