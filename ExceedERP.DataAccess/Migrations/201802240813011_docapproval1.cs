namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class docapproval1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelOrder", "IsFinalApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApTravelOrder", "IsFinalApproval");
        }
    }
}
