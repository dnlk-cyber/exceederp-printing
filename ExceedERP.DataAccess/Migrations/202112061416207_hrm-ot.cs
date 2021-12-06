namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmot : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.OverTimeRequest", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.OverTimeRequest", "Type");
        }
    }
}
