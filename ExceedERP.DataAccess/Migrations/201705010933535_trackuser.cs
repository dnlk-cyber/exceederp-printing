namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class trackuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceive", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.GoodReceiveDonation", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceiveDonation", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLending", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodSellingAndLending", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.Issue", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.IssueFuel", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.IssueFuel", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PurchaseReturn", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseReturn", "OrgBranchName", c => c.String());
            //AddColumn("EXCEED.Revaluation", "IsSendForApproval", c => c.Boolean(nullable: false));
            //AddColumn("EXCEED.Revaluation", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StockAdjustment", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StockAdjustment", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StoreReturn", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.TempGoodReceive", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.TempGoodReceive", "OrgBranchName", c => c.String());
        }

        public override void Down()
        {
            DropColumn("EXCEED.TempGoodReceive", "OrgBranchName");
            DropColumn("EXCEED.TempGoodReceive", "IsSendForApproval");
            DropColumn("EXCEED.StoreReturn", "OrgBranchName");
            DropColumn("EXCEED.StoreReturn", "IsSendForApproval");
            DropColumn("EXCEED.StockAdjustment", "OrgBranchName");
            DropColumn("EXCEED.StockAdjustment", "IsSendForApproval");
            DropColumn("EXCEED.Revaluation", "OrgBranchName");
            DropColumn("EXCEED.Revaluation", "IsSendForApproval");
            DropColumn("EXCEED.PurchaseReturn", "OrgBranchName");
            DropColumn("EXCEED.PurchaseReturn", "IsSendForApproval");
            DropColumn("EXCEED.PurchaseRequisition", "OrgBranchName");
            DropColumn("EXCEED.PurchaseRequisition", "IsSendForApproval");
            DropColumn("EXCEED.ItemTransfer", "OrgBranchName");
            DropColumn("EXCEED.ItemTransfer", "IsSendForApproval");
            DropColumn("EXCEED.IssueFuel", "OrgBranchName");
            DropColumn("EXCEED.IssueFuel", "IsSendForApproval");
            DropColumn("EXCEED.StoreRequisition", "OrgBranchName");
            DropColumn("EXCEED.StoreRequisition", "IsSendForApproval");
            DropColumn("EXCEED.Issue", "OrgBranchName");
            DropColumn("EXCEED.Issue", "IsSendForApproval");
            DropColumn("EXCEED.GoodSellingAndLending", "OrgBranchName");
            DropColumn("EXCEED.GoodSellingAndLending", "IsSendForApproval");
            DropColumn("EXCEED.GoodReceiveDonation", "OrgBranchName");
            DropColumn("EXCEED.GoodReceiveDonation", "IsSendForApproval");
            DropColumn("EXCEED.ItemInspection", "OrgBranchName");
            DropColumn("EXCEED.ItemInspection", "IsSendForApproval");
            DropColumn("EXCEED.GoodReceive", "OrgBranchName");
            DropColumn("EXCEED.GoodReceive", "IsSendForApproval");
        }
    }
}
