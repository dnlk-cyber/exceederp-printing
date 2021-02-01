namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemnameinevaluation : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.TenderEvaluationResult", "CatCode", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "CategoryName", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "ItemName", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "CatCode", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "CategoryName", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "ItemName", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "CatCode", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "CategoryName", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "ItemName", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "CatCode", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "CategoryName", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "ItemName", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "CatCode", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "CategoryName", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformTechnical", "ItemName");
            DropColumn("EXCEED.ProformTechnical", "CategoryName");
            DropColumn("EXCEED.ProformTechnical", "CatCode");
            DropColumn("EXCEED.ProformPassFail", "ItemName");
            DropColumn("EXCEED.ProformPassFail", "CategoryName");
            DropColumn("EXCEED.ProformPassFail", "CatCode");
            DropColumn("EXCEED.ProformaEvaluationResult", "ItemName");
            DropColumn("EXCEED.ProformaEvaluationResult", "CategoryName");
            DropColumn("EXCEED.ProformaEvaluationResult", "CatCode");
            DropColumn("EXCEED.TenderTechnical", "ItemName");
            DropColumn("EXCEED.TenderTechnical", "CategoryName");
            DropColumn("EXCEED.TenderTechnical", "CatCode");
            DropColumn("EXCEED.TenderEvaluationResult", "ItemName");
            DropColumn("EXCEED.TenderEvaluationResult", "CategoryName");
            DropColumn("EXCEED.TenderEvaluationResult", "CatCode");
        }
    }
}
