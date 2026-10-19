namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverTimechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.OverTime", "IsFromOverTimeRequest", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.OverTime", "OverTimeRequestId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.OverTime", "OverTimeRequestId");
            DropColumn("EXCEED.OverTime", "IsFromOverTimeRequest");
        }
    }
}
