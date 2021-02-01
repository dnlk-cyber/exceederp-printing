namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class docapproval : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelOrder", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "ForwardedTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApTravelOrder", "ForwardedTo");
            DropColumn("EXCEED.ApTravelOrder", "ForwardedFrom");
        }
    }
}
