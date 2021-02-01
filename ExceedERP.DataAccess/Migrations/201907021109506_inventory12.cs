namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BinCardLine", "StatusType", c => c.Int(nullable: false));
            AddColumn("EXCEED.Issue", "AssetRegistrationId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Issue", "AssetRegistrationId");
            DropColumn("EXCEED.BinCardLine", "StatusType");
        }
    }
}
