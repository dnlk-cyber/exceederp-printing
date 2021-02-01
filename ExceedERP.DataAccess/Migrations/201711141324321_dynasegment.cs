namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dynasegment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPosting", "Segment1", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment2", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment3", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment4", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment5", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment6", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment7", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "Segment8", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLLedgerPosting", "Segment8");
            DropColumn("EXCEED.GLLedgerPosting", "Segment7");
            DropColumn("EXCEED.GLLedgerPosting", "Segment6");
            DropColumn("EXCEED.GLLedgerPosting", "Segment5");
            DropColumn("EXCEED.GLLedgerPosting", "Segment4");
            DropColumn("EXCEED.GLLedgerPosting", "Segment3");
            DropColumn("EXCEED.GLLedgerPosting", "Segment2");
            DropColumn("EXCEED.GLLedgerPosting", "Segment1");
        }
    }
}
