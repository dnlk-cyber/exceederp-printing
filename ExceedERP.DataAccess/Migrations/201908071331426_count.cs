namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class count : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCountItem", "ItemCodeReadOnly", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PropertyCountItem", "ItemCodeReadOnly");
        }
    }
}
