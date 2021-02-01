namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ForeignPurchaseValidation",
                c => new
                    {
                        ForeignPurchaseValidationId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseValidationId)
                .ForeignKey("EXCEED.ForeignPurchase", t => t.ForeignPurchaseId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseId);
            
            AddColumn("EXCEED.ForeignPurchase", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "Status", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "VoidBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "SendForApprovalTime", c => c.DateTime());
            AlterColumn("EXCEED.ContractManagement", "ProcurementRefNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ForeignPurchaseValidation", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropIndex("EXCEED.ForeignPurchaseValidation", new[] { "ForeignPurchaseId" });
            AlterColumn("EXCEED.ContractManagement", "ProcurementRefNo", c => c.String());
            DropColumn("EXCEED.ForeignPurchase", "SendForApprovalTime");
            DropColumn("EXCEED.ForeignPurchase", "SendForApprovalBy");
            DropColumn("EXCEED.ForeignPurchase", "IsSendForApproval");
            DropColumn("EXCEED.ForeignPurchase", "OnlineTransferredTime");
            DropColumn("EXCEED.ForeignPurchase", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ForeignPurchase", "IsOnlineTransferred");
            DropColumn("EXCEED.ForeignPurchase", "OnlineApprovedTime");
            DropColumn("EXCEED.ForeignPurchase", "OnlineApprovedBy");
            DropColumn("EXCEED.ForeignPurchase", "IsOnlineApproved");
            DropColumn("EXCEED.ForeignPurchase", "VoidTime");
            DropColumn("EXCEED.ForeignPurchase", "VoidBy");
            DropColumn("EXCEED.ForeignPurchase", "IsVoid");
            DropColumn("EXCEED.ForeignPurchase", "Status");
            DropColumn("EXCEED.ForeignPurchase", "IsPostedToSecondLedger");
            DropTable("EXCEED.ForeignPurchaseValidation");
        }
    }
}
