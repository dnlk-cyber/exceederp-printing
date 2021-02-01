namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CommitteeMember", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.CommitteeMember", "IsActive");
        }
    }
}
