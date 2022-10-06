namespace ExceedERP.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("User.AspNetUsers", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("User.AspNetUsers", "EmployeeId", c => c.String());
        }
    }
}
