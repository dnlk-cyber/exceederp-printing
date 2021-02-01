namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class earningbiz : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlEarningLedger", "BusinessUnitSegment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlEarningLedger", "BusinessUnitSegment");
        }
    }
}
