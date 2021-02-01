namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Insurance.AccidentRecord", "EmployeeCode", c => c.String());
            AddColumn("Insurance.AccidentRecord", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Insurance.AccidentRecord", "EmployeeId");
            DropColumn("Insurance.AccidentRecord", "EmployeeCode");
        }
    }
}
