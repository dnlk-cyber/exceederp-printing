namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetname : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisitionItem", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisitionItem", "Name");
        }
    }
}
