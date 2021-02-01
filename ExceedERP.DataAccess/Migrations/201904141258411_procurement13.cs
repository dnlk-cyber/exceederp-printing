namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReportTemplate", "Temp7", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReportTemplate", "Temp7");
        }
    }
}
