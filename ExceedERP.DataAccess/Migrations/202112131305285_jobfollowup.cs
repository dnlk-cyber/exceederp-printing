namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobfollowup : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "ReceivedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "ReceivedDate");
        }
    }
}
