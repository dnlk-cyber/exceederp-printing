namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Issue", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemTransfer", "EmployeeId");
            DropColumn("EXCEED.Issue", "EmployeeId");
        }
    }
}
