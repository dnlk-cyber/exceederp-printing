namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetgroupqty : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "Quanity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetRegistration", "Quanity");
        }
    }
}
