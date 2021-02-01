namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm32 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Experience", "TotalExp", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Experience", "TotalExp");
        }
    }
}
