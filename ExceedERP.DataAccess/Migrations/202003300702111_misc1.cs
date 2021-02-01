namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class misc1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "BalanceQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Inventory_Archive.BinCardLineArchive", "ReceivedQTY", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
