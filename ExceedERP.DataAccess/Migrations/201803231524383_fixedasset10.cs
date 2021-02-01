namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisition", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "ForBranchId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisition", "ForBranchId");
            DropColumn("EXCEED.FixedAssetRequisition", "IsForOther");
        }
    }
}
