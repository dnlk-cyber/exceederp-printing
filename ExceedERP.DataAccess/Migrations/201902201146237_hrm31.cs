namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Employee", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Employee", "BranchId");
        }
    }
}
