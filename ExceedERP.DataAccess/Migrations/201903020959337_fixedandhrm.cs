namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedandhrm : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "Unit", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "Unit", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "Remark", c => c.String());
            AlterColumn("HRM.NeedAssessment", "RequiredTraining", c => c.Int(nullable: false));
            DropColumn("EXCEED.FixedAssetReceiveItem", "ItemCategoryCode");
            DropColumn("EXCEED.FixedAssetReceiveItem", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "ItemCode", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "ItemCategoryCode", c => c.String());
            AlterColumn("HRM.NeedAssessment", "RequiredTraining", c => c.String());
            DropColumn("EXCEED.FixedAssetReceiveItem", "Remark");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Unit");
            DropColumn("EXCEED.AssetRegistration", "Unit");
        }
    }
}
