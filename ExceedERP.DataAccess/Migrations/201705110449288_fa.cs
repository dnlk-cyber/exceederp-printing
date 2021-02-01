namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fa : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "FixedAseetCategoryCode", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "FixedAseetSubCategoryCode", c => c.String());
        }

        public override void Down()
        {
            DropColumn("EXCEED.AssetRegistration", "FixedAseetSubCategoryCode");
            DropColumn("EXCEED.AssetRegistration", "FixedAseetCategoryCode");
        }
    }
}
