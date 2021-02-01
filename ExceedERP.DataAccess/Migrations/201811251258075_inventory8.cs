namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "IsFreshBazaar", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GoodReceive", "IsFreshBazaar");
        }
    }
}
