namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bizunibook : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReconciliationBookAccount", "BusinessUnitSegment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBookAccount", "BusinessUnitSegment");
        }
    }
}
