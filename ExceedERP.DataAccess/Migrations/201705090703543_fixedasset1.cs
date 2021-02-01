namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetInspection", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.FixedAssetInspection", "FixedAssetReceive_FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropIndex("EXCEED.FixedAssetInspection", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.FixedAssetInspection", new[] { "FixedAssetReceive_FixedAssetReceiveId" });
            RenameColumn(table: "EXCEED.FixedAssetInspection", name: "FixedAssetReceive_FixedAssetReceiveId", newName: "FixedAssetReceiveId");
            AlterColumn("EXCEED.FixedAssetInspection", "FixedAssetReceiveId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetInspection", "FixedAssetReceiveId");
            AddForeignKey("EXCEED.FixedAssetInspection", "FixedAssetReceiveId", "EXCEED.FixedAssetReceive", "FixedAssetReceiveId", cascadeDelete: true);
            DropColumn("EXCEED.FixedAssetInspection", "GoodReceiveId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetInspection", "GoodReceiveId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.FixedAssetInspection", "FixedAssetReceiveId", "EXCEED.FixedAssetReceive");
            DropIndex("EXCEED.FixedAssetInspection", new[] { "FixedAssetReceiveId" });
            AlterColumn("EXCEED.FixedAssetInspection", "FixedAssetReceiveId", c => c.Int());
            RenameColumn(table: "EXCEED.FixedAssetInspection", name: "FixedAssetReceiveId", newName: "FixedAssetReceive_FixedAssetReceiveId");
            CreateIndex("EXCEED.FixedAssetInspection", "FixedAssetReceive_FixedAssetReceiveId");
            CreateIndex("EXCEED.FixedAssetInspection", "GoodReceiveId");
            AddForeignKey("EXCEED.FixedAssetInspection", "FixedAssetReceive_FixedAssetReceiveId", "EXCEED.FixedAssetReceive", "FixedAssetReceiveId");
            AddForeignKey("EXCEED.FixedAssetInspection", "GoodReceiveId", "EXCEED.GoodReceive", "GoodReceiveId", cascadeDelete: true);
        }
    }
}
