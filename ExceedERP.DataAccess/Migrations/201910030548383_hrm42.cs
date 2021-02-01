namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm42 : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.ComplainResponse", newSchema: "HRM");
        }
        
        public override void Down()
        {
            MoveTable(name: "HRM.ComplainResponse", newSchema: "EXCEED");
        }
    }
}
