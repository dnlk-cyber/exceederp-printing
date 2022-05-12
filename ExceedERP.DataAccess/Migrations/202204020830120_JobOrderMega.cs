namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobOrderMega : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "IsMaga", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "IsMaga");
        }
    }
}
