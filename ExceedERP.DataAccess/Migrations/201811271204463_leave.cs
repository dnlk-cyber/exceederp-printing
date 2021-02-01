namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leave : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveEntitle", "GlFiscalYearId2", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveEntitle", "GlFiscalYearId2");
        }
    }
}
