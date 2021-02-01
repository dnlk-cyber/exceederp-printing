namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankrecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReconciliationBookAccount", "RecordType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBookAccount", "RecordType");
        }
    }
}
