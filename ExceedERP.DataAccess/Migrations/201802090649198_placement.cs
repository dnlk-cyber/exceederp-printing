namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class placement : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Placement", "ImmediateSupervisor", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Placement", "ImmediateSupervisor");
        }
    }
}
