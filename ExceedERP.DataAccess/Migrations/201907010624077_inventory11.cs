namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BinCardLine", "TransactionType", c => c.Int(nullable: false));
            AddColumn("EXCEED.BinCardLineSecondary", "TransactionType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BinCardLineSecondary", "TransactionType");
            DropColumn("EXCEED.BinCardLine", "TransactionType");
        }
    }
}
