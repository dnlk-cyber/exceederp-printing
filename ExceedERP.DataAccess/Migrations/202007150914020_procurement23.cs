namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaEvaluationItemResult", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaEvaluationItemResult", "ItemName");
        }
    }
}
