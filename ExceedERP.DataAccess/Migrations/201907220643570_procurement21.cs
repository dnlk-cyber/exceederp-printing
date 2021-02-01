namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PassFailResult", "ModelNo", c => c.String());
            AddColumn("EXCEED.TenderTechnical", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationItemResult", "ModelNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaEvaluationItemResult", "ModelNo");
            DropColumn("EXCEED.TenderTechnical", "ModelNo");
            DropColumn("EXCEED.PassFailResult", "ModelNo");
        }
    }
}
