namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profeval : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ProformaEvaluationResult", "Pass", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ProformPassFail", "SupplierId", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProformTechnical", "SupplierId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ProformTechnical", "SupplierId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ProformPassFail", "SupplierId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ProformaEvaluationResult", "Pass", c => c.Boolean(nullable: false));
        }
    }
}
