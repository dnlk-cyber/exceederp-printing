namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travel3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.ApTravelExpenseClaim", "Name");
            DropColumn("EXCEED.ApTravelOrder", "Name");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelOrder", "Name", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "Name", c => c.String());
        }
    }
}
