namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetgroupmodif : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetGroup", "DepreciationMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetGroup", "IfrsPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FixedAssetGroup", "SalvageValueType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetGroup", "StraightLinePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetGroup", "PoolingPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.FixedAssetGroup", "PoolingPercentage", c => c.Single(nullable: false));
            AlterColumn("EXCEED.FixedAssetGroup", "StraightLinePercentage", c => c.Single(nullable: false));
            DropColumn("EXCEED.FixedAssetGroup", "SalvageValueType");
            DropColumn("EXCEED.FixedAssetGroup", "IfrsPercentage");
            DropColumn("EXCEED.FixedAssetGroup", "DepreciationMethod");
        }
    }
}
