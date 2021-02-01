namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedtransfer1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetTransfer", "RecipientType", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "RecipientEmployee", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "RecipientSupplier", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetTransfer", "RecipientSupplier");
            DropColumn("EXCEED.AssetTransfer", "RecipientEmployee");
            DropColumn("EXCEED.AssetTransfer", "RecipientType");
        }
    }
}
