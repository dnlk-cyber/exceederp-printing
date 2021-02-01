namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCountItem", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.PropertyCountItem", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.PropertyCountItem", "CreatedBy", c => c.String());
            AddColumn("EXCEED.PropertyCountItem", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PropertyCountItem", "ModifiedBy");
            DropColumn("EXCEED.PropertyCountItem", "CreatedBy");
            DropColumn("EXCEED.PropertyCountItem", "LastModified");
            DropColumn("EXCEED.PropertyCountItem", "DateCreated");
        }
    }
}
