namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeaverAndTermniationUpdateMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Experience", "LookupId", c => c.Int());
            DropColumn("HRM.Experience", "JobTitleId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Experience", "JobTitleId", c => c.Int(nullable: false));
            DropColumn("HRM.Experience", "LookupId");
        }
    }
}
