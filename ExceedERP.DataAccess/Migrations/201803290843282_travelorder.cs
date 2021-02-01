namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travelorder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ApTravelExpenseClaim", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApTravelExpenseClaim", "Name", c => c.String(nullable: false));
        }
    }
}
