namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goodreceive1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "IsManual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GoodReceive", "IsManual");
        }
    }
}
