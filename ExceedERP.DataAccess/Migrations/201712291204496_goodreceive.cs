namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goodreceive : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "SupplierId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GoodReceive", "SupplierId");
        }
    }
}
