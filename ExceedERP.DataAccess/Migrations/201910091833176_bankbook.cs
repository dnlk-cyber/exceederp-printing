namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankbook : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReconciliationBookAccount", "AccountSegment", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "BranchSegment", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "CostCenterSegment", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "SubAccountSegment", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "ChannelSegment", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment1", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment2", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment3", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment4", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment5", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment6", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment7", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "Segment8", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment8");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment7");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment6");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment5");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment4");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment3");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment2");
            DropColumn("EXCEED.ReconciliationBookAccount", "Segment1");
            DropColumn("EXCEED.ReconciliationBookAccount", "ChannelSegment");
            DropColumn("EXCEED.ReconciliationBookAccount", "SubAccountSegment");
            DropColumn("EXCEED.ReconciliationBookAccount", "CostCenterSegment");
            DropColumn("EXCEED.ReconciliationBookAccount", "BranchSegment");
            DropColumn("EXCEED.ReconciliationBookAccount", "AccountSegment");
        }
    }
}
