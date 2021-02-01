namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class immediatesup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("HRM.Placement", "ImmediateSupervisor", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Placement", "ImmediateSupervisor", c => c.Int(nullable: false));
        }
    }
}
