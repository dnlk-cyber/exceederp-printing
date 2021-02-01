namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "ReceiveRef", c => c.String());
            DropColumn("EXCEED.GoodReceive", "ReciveRef");
            DropColumn("EXCEED.GoodReceive", "FiscalYear");
            DropColumn("EXCEED.GoodReceive", "Period");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodReceive", "Period", c => c.String());
            AddColumn("EXCEED.GoodReceive", "FiscalYear", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.GoodReceive", "ReciveRef", c => c.String());
            DropColumn("EXCEED.GoodReceive", "ReceiveRef");
        }
    }
}
