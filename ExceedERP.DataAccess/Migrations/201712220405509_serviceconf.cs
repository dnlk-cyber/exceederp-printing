namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceconf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ServiceReceiveDistribution",
                c => new
                    {
                        ServiceReceiveDistributionId = c.Int(nullable: false, identity: true),
                        ServiceReceiveId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ServiceReceiveDistributionId)
                .ForeignKey("EXCEED.ServiceReceive", t => t.ServiceReceiveId, cascadeDelete: true)
                .Index(t => t.ServiceReceiveId);
            
            CreateTable(
                "EXCEED.ServiceReceive",
                c => new
                    {
                        ServiceReceiveId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        ReferenceNo = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        ItemType = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ServiceReceiveId);
            
            CreateTable(
                "EXCEED.ServiceReceiveItem",
                c => new
                    {
                        ServiceReceiveItemId = c.Int(nullable: false, identity: true),
                        ServiceReceiveId = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceReceiveItemId)
                .ForeignKey("EXCEED.ServiceReceive", t => t.ServiceReceiveId, cascadeDelete: true)
                .Index(t => t.ServiceReceiveId);
            
            CreateTable(
                "EXCEED.ServiceReceiveValidation",
                c => new
                    {
                        ServiceReceiveValidationId = c.Int(nullable: false, identity: true),
                        ServiceReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceReceiveValidationId)
                .ForeignKey("EXCEED.ServiceReceive", t => t.ServiceReceiveId, cascadeDelete: true)
                .Index(t => t.ServiceReceiveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ServiceReceiveValidation", "ServiceReceiveId", "EXCEED.ServiceReceive");
            DropForeignKey("EXCEED.ServiceReceiveItem", "ServiceReceiveId", "EXCEED.ServiceReceive");
            DropForeignKey("EXCEED.ServiceReceiveDistribution", "ServiceReceiveId", "EXCEED.ServiceReceive");
            DropIndex("EXCEED.ServiceReceiveValidation", new[] { "ServiceReceiveId" });
            DropIndex("EXCEED.ServiceReceiveItem", new[] { "ServiceReceiveId" });
            DropIndex("EXCEED.ServiceReceiveDistribution", new[] { "ServiceReceiveId" });
            DropTable("EXCEED.ServiceReceiveValidation");
            DropTable("EXCEED.ServiceReceiveItem");
            DropTable("EXCEED.ServiceReceive");
            DropTable("EXCEED.ServiceReceiveDistribution");
        }
    }
}
