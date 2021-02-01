namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetCondition",
                c => new
                    {
                        FixedAssetConditionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetConditionId);
            
            AddColumn("EXCEED.AssetRegistration", "FixedAssetConditionId", c => c.Int(nullable: true));
            CreateIndex("EXCEED.AssetRegistration", "FixedAssetConditionId");
            AddForeignKey("EXCEED.AssetRegistration", "FixedAssetConditionId", "EXCEED.FixedAssetCondition", "FixedAssetConditionId", cascadeDelete: true);
            DropColumn("EXCEED.AssetRegistration", "Condition");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "Condition");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "Condition", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "Condition", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.AssetRegistration", "FixedAssetConditionId", "EXCEED.FixedAssetCondition");
            DropIndex("EXCEED.AssetRegistration", new[] { "FixedAssetConditionId" });
            DropColumn("EXCEED.AssetRegistration", "FixedAssetConditionId");
            DropTable("EXCEED.FixedAssetCondition");
        }
    }
}
