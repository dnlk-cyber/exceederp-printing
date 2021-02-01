namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assetrequi : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId");
            AddForeignKey("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
