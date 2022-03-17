namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobOrderUnitAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "UnitId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "UnitId");
        }
    }
}
