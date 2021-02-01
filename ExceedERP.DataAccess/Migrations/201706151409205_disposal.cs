namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Disposal", "Status", c => c.String());
            AddColumn("EXCEED.Disposal", "Remark", c => c.String());
            AddColumn("EXCEED.Disposal", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Disposal", "VoidBy", c => c.String());
            AddColumn("EXCEED.Disposal", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.Disposal", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Disposal", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.Disposal", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.Disposal", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Disposal", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.Disposal", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.Disposal", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Disposal", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.Disposal", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Disposal", "SendForApprovalTime");
            DropColumn("EXCEED.Disposal", "SendForApprovalBy");
            DropColumn("EXCEED.Disposal", "IsSendForApproval");
            DropColumn("EXCEED.Disposal", "OnlineTransferredTime");
            DropColumn("EXCEED.Disposal", "IsOnlineTransferredBy");
            DropColumn("EXCEED.Disposal", "IsOnlineTransferred");
            DropColumn("EXCEED.Disposal", "OnlineApprovedTime");
            DropColumn("EXCEED.Disposal", "OnlineApprovedBy");
            DropColumn("EXCEED.Disposal", "IsOnlineApproved");
            DropColumn("EXCEED.Disposal", "VoidTime");
            DropColumn("EXCEED.Disposal", "VoidBy");
            DropColumn("EXCEED.Disposal", "IsVoid");
            DropColumn("EXCEED.Disposal", "Remark");
            DropColumn("EXCEED.Disposal", "Status");
        }
    }
}
