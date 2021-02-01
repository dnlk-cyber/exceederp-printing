namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicereceive1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ForeignPurchase", "FOBMethod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ForeignPurchase", "FOBMethod");
        }
    }
}
