namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guarantee1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Guarantee", "Status", c => c.String());
            AddColumn("EXCEED.Guarantee", "isOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Guarantee", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.Guarantee", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.Guarantee", "isVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Guarantee", "VoidBy", c => c.String());
            AddColumn("EXCEED.Guarantee", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.Guarantee", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Guarantee", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.Guarantee", "SendForApprovalTime", c => c.DateTime());
            DropColumn("EXCEED.Guarantee", "GuaranteePresenterName");
            DropColumn("EXCEED.Guarantee", "GuaranteePresenterAddress");
            DropColumn("EXCEED.Guarantee", "Remark");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.Guarantee", "Remark", c => c.String());
            AddColumn("EXCEED.Guarantee", "GuaranteePresenterAddress", c => c.String());
            AddColumn("EXCEED.Guarantee", "GuaranteePresenterName", c => c.String());
            DropColumn("EXCEED.Guarantee", "SendForApprovalTime");
            DropColumn("EXCEED.Guarantee", "SendForApprovalBy");
            DropColumn("EXCEED.Guarantee", "IsSendForApproval");
            DropColumn("EXCEED.Guarantee", "VoidTime");
            DropColumn("EXCEED.Guarantee", "VoidBy");
            DropColumn("EXCEED.Guarantee", "isVoid");
            DropColumn("EXCEED.Guarantee", "OnlineApprovedTime");
            DropColumn("EXCEED.Guarantee", "OnlineApprovedBy");
            DropColumn("EXCEED.Guarantee", "isOnlineApproved");
            DropColumn("EXCEED.Guarantee", "Status");
        }
    }
}
