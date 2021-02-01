namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm29 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Probation", "Decision", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Probation", "Decision");
        }
    }
}
