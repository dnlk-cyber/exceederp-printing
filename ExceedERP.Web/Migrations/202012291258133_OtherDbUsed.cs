namespace ExceedERP.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherDbUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("User.ApplicationGroups", "GroupModule", c => c.Int(nullable: false));
            AddColumn("User.AspNetUsers", "OrgStructureId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("User.AspNetUsers", "OrgStructureId");
            DropColumn("User.ApplicationGroups", "GroupModule");
        }
    }
}
