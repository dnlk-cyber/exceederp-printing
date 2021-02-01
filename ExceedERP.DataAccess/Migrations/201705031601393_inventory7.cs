namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory7 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.IssueItem", "FromStoreReturn");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueItem", "FromStoreReturn", c => c.Boolean(nullable: false));
        }
    }
}
