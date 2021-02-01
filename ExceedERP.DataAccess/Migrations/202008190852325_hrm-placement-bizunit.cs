namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmplacementbizunit : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Placement", "BusinessUnitId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Placement", "BusinessUnitId");
        }
    }
}
