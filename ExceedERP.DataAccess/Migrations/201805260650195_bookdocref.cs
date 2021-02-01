namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bookdocref : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReconciliationBookAccount", "ReferenceDoc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBookAccount", "ReferenceDoc");
        }
    }
}
