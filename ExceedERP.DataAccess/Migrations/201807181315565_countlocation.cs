namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countlocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCountItem", "Location", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PropertyCountItem", "Location");
        }
    }
}
