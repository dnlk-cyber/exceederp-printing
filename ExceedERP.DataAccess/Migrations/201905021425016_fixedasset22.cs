namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset22 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.AssetTransferItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.AssetTransferItem", new[] { "AssetRegistrationId" });
            AddColumn("EXCEED.AssetTransferItem", "ItemName", c => c.String());
            AddColumn("EXCEED.AssetTransferItem", "Name", c => c.String());
            AddColumn("EXCEED.AssetTransferItem", "Unit", c => c.String());
            AddColumn("EXCEED.AssetTransferItem", "GrnNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetTransferItem", "GrnNo");
            DropColumn("EXCEED.AssetTransferItem", "Unit");
            DropColumn("EXCEED.AssetTransferItem", "Name");
            DropColumn("EXCEED.AssetTransferItem", "ItemName");
            CreateIndex("EXCEED.AssetTransferItem", "AssetRegistrationId");
            AddForeignKey("EXCEED.AssetTransferItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
