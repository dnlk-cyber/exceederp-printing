namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedAssetnew1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.DepreciationRule", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.DepreciationRule", new[] { "ItemCategoryCode" });
            AddColumn("EXCEED.DepreciationRule", "CategoryCode", c => c.String());
            AddColumn("EXCEED.DepreciationRule", "DepreciationCode", c => c.String());
            AddColumn("EXCEED.DepreciationRule", "AccumulatedAccount", c => c.String());
            DropColumn("EXCEED.DepreciationRule", "ItemCategoryCode");
           // DropColumn("EXCEED.DepreciationRule", "DepretiationCode");
           // DropColumn("EXCEED.DepreciationRule", "AccomolatedAccount");
        }

        public override void Down()
        {
            AddColumn("EXCEED.DepreciationRule", "AccomolatedAccount", c => c.String());
            AddColumn("EXCEED.DepreciationRule", "DepretiationCode", c => c.String());
            AddColumn("EXCEED.DepreciationRule", "ItemCategoryCode", c => c.String(maxLength: 128));
            DropColumn("EXCEED.DepreciationRule", "AccumulatedAccount");
            DropColumn("EXCEED.DepreciationRule", "DepreciationCode");
            DropColumn("EXCEED.DepreciationRule", "CategoryCode");
            CreateIndex("EXCEED.DepreciationRule", "ItemCategoryCode");
            AddForeignKey("EXCEED.DepreciationRule", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
        }
    }
}
