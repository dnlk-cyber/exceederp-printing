namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.TenderEvaluationItemResult", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.PassFailResult", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationItemResult", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "ItemSpecification", c => c.String());
            DropColumn("EXCEED.ProformaEvaluationItemResult", "ItemName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProformaEvaluationItemResult", "ItemName", c => c.String());
            DropColumn("EXCEED.ProformTechnical", "ItemSpecification");
            DropColumn("EXCEED.ProformPassFail", "ItemSpecification");
            DropColumn("EXCEED.ProformaEvaluationItemResult", "ItemSpecification");
            DropColumn("EXCEED.TenderTechnical", "ItemSpecification");
            DropColumn("EXCEED.PassFailResult", "ItemSpecification");
            DropColumn("EXCEED.TenderEvaluationItemResult", "ItemSpecification");
        }
    }
}
