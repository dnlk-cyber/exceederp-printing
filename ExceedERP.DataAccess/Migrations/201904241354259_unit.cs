namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unit : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.UoM", "Default", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.UoM", "Default");
        }
    }
}
