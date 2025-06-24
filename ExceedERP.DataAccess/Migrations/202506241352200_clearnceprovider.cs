namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clearnceprovider : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ClearanceProviderDetail",
                c => new
                    {
                        ClearanceProviderDetailId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(),
                        HeadId = c.Int(),
                        ClearanceProviderId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ClearanceProviderDetailId)
                .ForeignKey("HRM.ClearanceProvider", t => t.ClearanceProviderId, cascadeDelete: true)
                .Index(t => t.ClearanceProviderId);
            
            CreateTable(
                "EXCEED.ClearanceRequestApprover",
                c => new
                    {
                        ClearanceRequestApproverId = c.Int(nullable: false, identity: true),
                        ApproverOrgStructureId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        ClearanceRequestId = c.Int(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.ClearanceRequestApproverId)
                .ForeignKey("EXCEED.ClearanceRequest", t => t.ClearanceRequestId, cascadeDelete: true)
                .Index(t => t.ClearanceRequestId);
            
            CreateTable(
                "EXCEED.ClearanceRequest",
                c => new
                    {
                        ClearanceRequestId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.ClearanceRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.ClearanceRequestStatus",
                c => new
                    {
                        ClearanceRequestStatusId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        IsAproved = c.Boolean(nullable: false),
                        ClearanceRequestId = c.Int(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.ClearanceRequestStatusId)
                .ForeignKey("EXCEED.ClearanceRequest", t => t.ClearanceRequestId, cascadeDelete: true)
                .Index(t => t.ClearanceRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ClearanceRequestStatus", "ClearanceRequestId", "EXCEED.ClearanceRequest");
            DropForeignKey("EXCEED.ClearanceRequestApprover", "ClearanceRequestId", "EXCEED.ClearanceRequest");
            DropForeignKey("EXCEED.ClearanceRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.ClearanceProviderDetail", "ClearanceProviderId", "HRM.ClearanceProvider");
            DropIndex("EXCEED.ClearanceRequestStatus", new[] { "ClearanceRequestId" });
            DropIndex("EXCEED.ClearanceRequest", new[] { "EmployeeId" });
            DropIndex("EXCEED.ClearanceRequestApprover", new[] { "ClearanceRequestId" });
            DropIndex("EXCEED.ClearanceProviderDetail", new[] { "ClearanceProviderId" });
            DropTable("EXCEED.ClearanceRequestStatus");
            DropTable("EXCEED.ClearanceRequest");
            DropTable("EXCEED.ClearanceRequestApprover");
            DropTable("EXCEED.ClearanceProviderDetail");
        }
    }
}
