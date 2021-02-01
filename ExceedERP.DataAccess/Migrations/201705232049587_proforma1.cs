namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaEvaluationResult", "Financial", c => c.Single(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Technical", c => c.Single(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Total", c => c.Single(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Pass", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Supplier", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "ItemCode", c => c.String());
            DropColumn("EXCEED.ProformaEvaluationResult", "SupplierName");
            DropColumn("EXCEED.ProformaEvaluationResult", "Type");
            DropColumn("EXCEED.ProformaEvaluationResult", "Score");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProformaEvaluationResult", "Score", c => c.Single(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "SupplierName", c => c.String(nullable: false));
            DropColumn("EXCEED.ProformaEvaluationResult", "ItemCode");
            DropColumn("EXCEED.ProformaEvaluationResult", "Supplier");
            DropColumn("EXCEED.ProformaEvaluationResult", "Pass");
            DropColumn("EXCEED.ProformaEvaluationResult", "Total");
            DropColumn("EXCEED.ProformaEvaluationResult", "Technical");
            DropColumn("EXCEED.ProformaEvaluationResult", "Financial");
        }
    }
}
