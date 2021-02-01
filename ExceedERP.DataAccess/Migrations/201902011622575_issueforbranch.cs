namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issueforbranch : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Issue", "ForBranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Issue", "ForBranchId");
        }
    }
}
