namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fa4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.FixedAssetReceiveItem", "Supplier");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "Supplier", c => c.String());
        }
    }
}
