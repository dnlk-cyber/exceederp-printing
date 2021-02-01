namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts6 : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.APTSValidation", newSchema: "APTS");
        }
        
        public override void Down()
        {
            MoveTable(name: "APTS.APTSValidation", newSchema: "EXCEED");
        }
    }
}
