namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemcount : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCountItem", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PropertyCountItem", "ItemName");
        }
    }
}
