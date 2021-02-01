namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaPurchase", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ProformaPurchase", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ProformaPurchase", "IsFinalApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaPurchase", "IsFinalApproval");
            DropColumn("EXCEED.ProformaPurchase", "ForwardedTo");
            DropColumn("EXCEED.ProformaPurchase", "ForwardedFrom");
        }
    }
}
