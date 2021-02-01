namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serialmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisitionItem", "PartNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "ChasisNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisitionItem", "ChasisNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "SerialNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "ModelNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "PartNo");
        }
    }
}
