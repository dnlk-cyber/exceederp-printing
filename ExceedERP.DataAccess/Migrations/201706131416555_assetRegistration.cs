namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assetRegistration : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "Size", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetRegistration", "Size");
        }
    }
}
