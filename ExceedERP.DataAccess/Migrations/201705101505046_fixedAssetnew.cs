namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedAssetnew : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetGroup", "StraightLinePercentage", c => c.Single(nullable: false));
            AddColumn("EXCEED.FixedAssetGroup", "PoolingPercentage", c => c.Single(nullable: false));
            DropColumn("EXCEED.FixedAssetGroup", "DepreciationMethod");
            DropColumn("EXCEED.FixedAssetGroup", "YearlyDepreciationPercentage");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetGroup", "YearlyDepreciationPercentage", c => c.Single(nullable: false));
            AddColumn("EXCEED.FixedAssetGroup", "DepreciationMethod", c => c.Int(nullable: false));
            DropColumn("EXCEED.FixedAssetGroup", "PoolingPercentage");
            DropColumn("EXCEED.FixedAssetGroup", "StraightLinePercentage");
        }
    }
}
