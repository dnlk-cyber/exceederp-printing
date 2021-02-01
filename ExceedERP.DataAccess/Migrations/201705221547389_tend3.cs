namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tend3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.ProformaEvaluationCriteria", "CriteriaName");
            DropColumn("EXCEED.ProformaEvaluationResult", "CriteriaName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProformaEvaluationResult", "CriteriaName", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationCriteria", "CriteriaName", c => c.String());
        }
    }
}
