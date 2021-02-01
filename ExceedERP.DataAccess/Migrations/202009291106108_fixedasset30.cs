namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AnnualDepreciation", "FixedAssetGroupID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AnnualDepreciation", "FixedAssetGroupID");
        }
    }
}
