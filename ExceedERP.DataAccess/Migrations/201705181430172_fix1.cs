namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "CategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "SubCategoryCode", c => c.String(nullable: false));
            DropColumn("EXCEED.AssetRegistration", "FixedAseetCategoryCode");
            DropColumn("EXCEED.AssetRegistration", "FixedAseetSubCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AssetRegistration", "FixedAseetSubCategoryCode", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "FixedAseetCategoryCode", c => c.String());
            DropColumn("EXCEED.AssetRegistration", "SubCategoryCode");
            DropColumn("EXCEED.AssetRegistration", "CategoryCode");
        }
    }
}
