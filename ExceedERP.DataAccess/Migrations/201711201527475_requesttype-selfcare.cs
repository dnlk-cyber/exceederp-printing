namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requesttypeselfcare : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ExperienceRequest", "RequestType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.ExperienceRequest", "RequestType");
        }
    }
}
