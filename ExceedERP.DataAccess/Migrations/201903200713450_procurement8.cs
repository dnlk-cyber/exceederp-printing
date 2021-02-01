namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.TenderEvaluationItemResult", "Name", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "Name", c => c.String());
            AddColumn("EXCEED.PassFailResult", "Name", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.TenderTechnical", "Name");
            DropColumn("EXCEED.PassFailResult", "Name");
            DropColumn("EXCEED.ParticipantsItem", "Name");
            DropColumn("EXCEED.TenderEvaluationItemResult", "Name");
        }
    }
}
