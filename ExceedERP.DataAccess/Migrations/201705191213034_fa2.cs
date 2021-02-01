namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fa2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetReceiveItem", "Name");
        }
    }
}
