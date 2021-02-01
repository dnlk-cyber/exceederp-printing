namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procreporttemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReportTemplate", "Temp6", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReportTemplate", "Temp6");
        }
    }
}
