namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchasedisruption : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaEvaluationResult", "ItmCode", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformTechnical", "ItmCode", c => c.String(nullable: false));
            DropColumn("EXCEED.ProformaEvaluationResult", "ItemCode");
            DropColumn("EXCEED.ProformPassFail", "ItemCode");
            DropColumn("EXCEED.ProformTechnical", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProformTechnical", "ItemCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformPassFail", "ItemCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "ItemCode", c => c.String());
            DropColumn("EXCEED.ProformTechnical", "ItmCode");
            DropColumn("EXCEED.ProformPassFail", "ItmCode");
            DropColumn("EXCEED.ProformaEvaluationResult", "ItmCode");
        }
    }
}
