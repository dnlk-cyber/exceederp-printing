namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedIsCurrent : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.LocationTracking", "IsCurrent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.LocationTracking", "IsCurrent");
        }
    }
}
