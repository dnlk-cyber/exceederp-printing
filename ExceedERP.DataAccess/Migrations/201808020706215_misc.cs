namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class misc : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreItemAssignment", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreItemAssignment", "ItemName");
        }
    }
}
