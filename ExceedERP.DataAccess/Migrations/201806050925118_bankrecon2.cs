namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankrecon2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ReconciliationBookAccount", "SourceID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ReconciliationBookAccount", "SourceID", c => c.Int(nullable: false));
        }
    }
}
