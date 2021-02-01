namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class placement1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Placement", "IsHead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Placement", "IsHead");
        }
    }
}
