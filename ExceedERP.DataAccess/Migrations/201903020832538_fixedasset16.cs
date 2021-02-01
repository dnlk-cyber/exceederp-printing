namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "Remark", c => c.String());
            AddColumn("EXCEED.FixedAssetSubCategory", "IsGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetSubCategory", "IsGroup");
            DropColumn("EXCEED.AssetRegistration", "Remark");
        }
    }
}
