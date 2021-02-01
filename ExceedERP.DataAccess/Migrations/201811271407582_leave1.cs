namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leave1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("HRM.LeaveEntitle", "GlFiscalYearId2");
        }
        
        public override void Down()
        {
            AddColumn("HRM.LeaveEntitle", "GlFiscalYearId2", c => c.Int(nullable: false));
        }
    }
}
