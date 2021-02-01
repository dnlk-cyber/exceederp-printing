namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transferreceivefix : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.ItemTransfer", "IsSecondOnlineApproved");
            DropColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedBy");
            DropColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedTime");
            DropColumn("EXCEED.ItemTransfer", "IsSecondSendForApproval");
            DropColumn("EXCEED.ItemTransfer", "SecondSendForApprovalBy");
            DropColumn("EXCEED.ItemTransfer", "SecondSendForApprovalTime");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemTransfer", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ItemTransfer", "SecondSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
        }
    }
}
