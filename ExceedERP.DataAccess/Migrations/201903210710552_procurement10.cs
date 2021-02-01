namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.TenderEvaluationItemResult", "LotItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "LotItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.PassFailResult", "LotItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.TenderTechnical", "LotItemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.TenderTechnical", "LotItemID");
            DropColumn("EXCEED.PassFailResult", "LotItemID");
            DropColumn("EXCEED.ParticipantsItem", "LotItemID");
            DropColumn("EXCEED.TenderEvaluationItemResult", "LotItemID");
        }
    }
}
