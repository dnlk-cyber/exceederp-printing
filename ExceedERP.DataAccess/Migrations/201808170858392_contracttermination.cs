namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contracttermination : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ContractTerminationAttachment",
                c => new
                    {
                        ContractTerminationAttachmentId = c.Int(nullable: false, identity: true),
                        ContractTerminationId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractTerminationAttachmentId)
                .ForeignKey("EXCEED.ContractTermination", t => t.ContractTerminationId, cascadeDelete: true)
                .Index(t => t.ContractTerminationId);
            
            CreateTable(
                "EXCEED.ContractTermination",
                c => new
                    {
                        ContractTerminationId = c.Int(nullable: false, identity: true),
                        ContractManagementID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                        GiveAwardToNextSupplier = c.Boolean(nullable: false),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractTerminationId)
                .ForeignKey("EXCEED.ContractManagement", t => t.ContractManagementID, cascadeDelete: true)
                .Index(t => t.ContractManagementID);
            
            CreateTable(
                "EXCEED.ContractTerminationValidation",
                c => new
                    {
                        ContractTerminationValidationId = c.Int(nullable: false, identity: true),
                        ContractTerminationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractTerminationValidationId)
                .ForeignKey("EXCEED.ContractTermination", t => t.ContractTerminationId, cascadeDelete: true)
                .Index(t => t.ContractTerminationId);
            
            AddColumn("EXCEED.ContractPayment", "WithOutGRN", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ContractTerminationValidation", "ContractTerminationId", "EXCEED.ContractTermination");
            DropForeignKey("EXCEED.ContractTerminationAttachment", "ContractTerminationId", "EXCEED.ContractTermination");
            DropForeignKey("EXCEED.ContractTermination", "ContractManagementID", "EXCEED.ContractManagement");
            DropIndex("EXCEED.ContractTerminationValidation", new[] { "ContractTerminationId" });
            DropIndex("EXCEED.ContractTermination", new[] { "ContractManagementID" });
            DropIndex("EXCEED.ContractTerminationAttachment", new[] { "ContractTerminationId" });
            DropColumn("EXCEED.ContractPayment", "WithOutGRN");
            DropTable("EXCEED.ContractTerminationValidation");
            DropTable("EXCEED.ContractTermination");
            DropTable("EXCEED.ContractTerminationAttachment");
        }
    }
}
