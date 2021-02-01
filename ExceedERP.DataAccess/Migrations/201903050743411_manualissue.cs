namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manualissue : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Issue", "IsManual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Issue", "IsManual");
        }
    }
}
