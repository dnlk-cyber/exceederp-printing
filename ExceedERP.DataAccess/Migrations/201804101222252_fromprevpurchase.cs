namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fromprevpurchase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ServiceRequisitionItem", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.ServiceRequisitionItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.ServiceRequisitionItem", "ServiceItemCode", "EXCEED.ServiceItem");
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "ServiceItemCode" });
            CreateTable(
                "EXCEED.PurchaseRequisitionReassignmentValidation",
                c => new
                    {
                        PurchaseRequisitionReassignmentValidationId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionAssignmentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionReassignmentValidationId)
                .ForeignKey("EXCEED.PurchaseRequisitionAssignment", t => t.PurchaseRequisitionAssignmentId, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionAssignmentId);
            
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "Status", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "Remark", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "VoidBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "SendForApprovalTime", c => c.DateTime());
            DropTable("EXCEED.ServiceRequisitionItem");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ServiceRequisitionItem",
                c => new
                    {
                        ServiceRequisitionItemId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        ServiceCategoryCode = c.String(nullable: false, maxLength: 128),
                        ServiceItemCode = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceRequisitionItemId);
            
            DropForeignKey("EXCEED.PurchaseRequisitionReassignmentValidation", "PurchaseRequisitionAssignmentId", "EXCEED.PurchaseRequisitionAssignment");
            DropIndex("EXCEED.PurchaseRequisitionReassignmentValidation", new[] { "PurchaseRequisitionAssignmentId" });
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "SendForApprovalTime");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "SendForApprovalBy");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "OrgBranchName");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsSendForApproval");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineTransferredTime");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineTransferredBy");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineTransferred");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineApprovedTime");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "OnlineApprovedBy");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsOnlineApproved");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "VoidTime");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "VoidBy");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsVoid");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "Remark");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "Status");
            DropTable("EXCEED.PurchaseRequisitionReassignmentValidation");
            CreateIndex("EXCEED.ServiceRequisitionItem", "ServiceItemCode");
            CreateIndex("EXCEED.ServiceRequisitionItem", "ServiceCategoryCode");
            CreateIndex("EXCEED.ServiceRequisitionItem", "PurchaseRequisitionID");
            AddForeignKey("EXCEED.ServiceRequisitionItem", "ServiceItemCode", "EXCEED.ServiceItem", "ServiceItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ServiceRequisitionItem", "ServiceCategoryCode", "EXCEED.ServiceCategory", "ServiceCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ServiceRequisitionItem", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition", "PurchaseRequisitionID", cascadeDelete: true);
        }
    }
}
