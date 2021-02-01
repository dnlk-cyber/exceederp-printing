namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "Name", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "Name", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformPassFail", "Name");
            DropColumn("EXCEED.ProformaEvaluationResult", "Name");
            DropColumn("EXCEED.ParticipatingSupplier", "Name");
        }
    }
}
