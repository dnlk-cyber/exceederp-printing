namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "Size", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetConditionId", c => c.Int(nullable: true));
            CreateIndex("EXCEED.FixedAssetReceiveItem", "FixedAssetConditionId");
            AddForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetConditionId", "EXCEED.FixedAssetCondition", "FixedAssetConditionId", cascadeDelete: true);
            DropColumn("EXCEED.FixedAssetReceiveItem", "Condition");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "Condition", c => c.String());
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetConditionId", "EXCEED.FixedAssetCondition");
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "FixedAssetConditionId" });
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetConditionId");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Size");
        }
    }
}
