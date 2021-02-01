namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dayleave : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Leave", "Day", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Leave", "Day");
        }
    }
}
