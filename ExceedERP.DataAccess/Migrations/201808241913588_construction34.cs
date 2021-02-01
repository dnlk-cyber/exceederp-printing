namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction34 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ContractClaimValidation",
                c => new
                    {
                        ContractClaimValidationId = c.Int(nullable: false, identity: true),
                        ContractClaimId = c.Int(nullable: false),
                        RespondedDate = c.DateTime(),
                        ResponseRefNumber = c.String(),
                        ResponseReferanceLatter = c.String(),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractClaimValidationId)
                .ForeignKey("Construction.ContractClaim", t => t.ContractClaimId, cascadeDelete: true)
                .Index(t => t.ContractClaimId);
            
            CreateTable(
                "Construction.ProjectPaymentCertificateValidation",
                c => new
                    {
                        ProjectPaymentCertificateValidationId = c.Int(nullable: false, identity: true),
                        ProjectPaymentCertificateId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPaymentCertificateValidationId)
                .ForeignKey("Construction.PaymentCertificate", t => t.ProjectPaymentCertificateId, cascadeDelete: true)
                .Index(t => t.ProjectPaymentCertificateId);
            
            CreateTable(
                "Construction.RepresentativeElementCostInBill",
                c => new
                    {
                        RepresentativeElementCostInBillId = c.Int(nullable: false, identity: true),
                        BoQOfExecutedWorkId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        ItemName = c.String(),
                        Unit = c.String(),
                        BasicPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RepresentativeElementCostInBillId)
                .ForeignKey("Construction.BoQOfExecutedWork", t => t.BoQOfExecutedWorkId, cascadeDelete: true)
                .Index(t => t.BoQOfExecutedWorkId);
            
            AddColumn("Construction.BoQOfExecutedWork", "OrgBranchName", c => c.String());
            AddColumn("Construction.BoQOfExecutedWork", "DateCreated", c => c.DateTime());
            AddColumn("Construction.BoQOfExecutedWork", "LastModified", c => c.DateTime());
            AddColumn("Construction.BoQOfExecutedWork", "CreatedBy", c => c.String());
            AddColumn("Construction.BoQOfExecutedWork", "ModifiedBy", c => c.String());
            AddColumn("Construction.ContractClaim", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "Status", c => c.String());
            AddColumn("Construction.ContractClaim", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "VoidBy", c => c.String());
            AddColumn("Construction.ContractClaim", "VoidTime", c => c.DateTime());
            AddColumn("Construction.ContractClaim", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "OnlineApprovedBy", c => c.String());
            AddColumn("Construction.ContractClaim", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Construction.ContractClaim", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Construction.ContractClaim", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Construction.ContractClaim", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "SendForApprovalBy", c => c.String());
            AddColumn("Construction.ContractClaim", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "Status", c => c.String());
            AddColumn("Construction.PaymentCertificate", "Remark", c => c.String());
            AddColumn("Construction.PaymentCertificate", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "VoidBy", c => c.String());
            AddColumn("Construction.PaymentCertificate", "VoidTime", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "OnlineApprovedBy", c => c.String());
            AddColumn("Construction.PaymentCertificate", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Construction.PaymentCertificate", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "SendForApprovalBy", c => c.String());
            AddColumn("Construction.PaymentCertificate", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Construction.ProjectRepresentativeElement", "BasicPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectRepresentativeElement", "RepresentedItems", c => c.String());
            AddColumn("Construction.ProjectRepresentativeElement", "Source", c => c.String());
            AlterColumn("Construction.ContractClaim", "RespondedDate", c => c.DateTime());
            DropColumn("Construction.MaterialPriceIndex", "LoadingUnloading");
            DropColumn("Construction.ProjectRepresentativeElement", "PreviousPrice");
            DropColumn("Construction.ProjectRepresentativeElement", "CurrentPrice");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ProjectRepresentativeElement", "CurrentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectRepresentativeElement", "PreviousPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MaterialPriceIndex", "LoadingUnloading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("Construction.RepresentativeElementCostInBill", "BoQOfExecutedWorkId", "Construction.BoQOfExecutedWork");
            DropForeignKey("Construction.ProjectPaymentCertificateValidation", "ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.ContractClaimValidation", "ContractClaimId", "Construction.ContractClaim");
            DropIndex("Construction.RepresentativeElementCostInBill", new[] { "BoQOfExecutedWorkId" });
            DropIndex("Construction.ProjectPaymentCertificateValidation", new[] { "ProjectPaymentCertificateId" });
            DropIndex("Construction.ContractClaimValidation", new[] { "ContractClaimId" });
            AlterColumn("Construction.ContractClaim", "RespondedDate", c => c.DateTime(nullable: false));
            DropColumn("Construction.ProjectRepresentativeElement", "Source");
            DropColumn("Construction.ProjectRepresentativeElement", "RepresentedItems");
            DropColumn("Construction.ProjectRepresentativeElement", "BasicPrice");
            DropColumn("Construction.PaymentCertificate", "SendForApprovalTime");
            DropColumn("Construction.PaymentCertificate", "SendForApprovalBy");
            DropColumn("Construction.PaymentCertificate", "IsSendForApproval");
            DropColumn("Construction.PaymentCertificate", "OnlineTransferredTime");
            DropColumn("Construction.PaymentCertificate", "IsOnlineTransferredBy");
            DropColumn("Construction.PaymentCertificate", "IsOnlineTransferred");
            DropColumn("Construction.PaymentCertificate", "OnlineApprovedTime");
            DropColumn("Construction.PaymentCertificate", "OnlineApprovedBy");
            DropColumn("Construction.PaymentCertificate", "IsOnlineApproved");
            DropColumn("Construction.PaymentCertificate", "VoidTime");
            DropColumn("Construction.PaymentCertificate", "VoidBy");
            DropColumn("Construction.PaymentCertificate", "IsVoid");
            DropColumn("Construction.PaymentCertificate", "Remark");
            DropColumn("Construction.PaymentCertificate", "Status");
            DropColumn("Construction.PaymentCertificate", "IsPostedToSecondLedger");
            DropColumn("Construction.ContractClaim", "SendForApprovalTime");
            DropColumn("Construction.ContractClaim", "SendForApprovalBy");
            DropColumn("Construction.ContractClaim", "IsSendForApproval");
            DropColumn("Construction.ContractClaim", "OnlineTransferredTime");
            DropColumn("Construction.ContractClaim", "IsOnlineTransferredBy");
            DropColumn("Construction.ContractClaim", "IsOnlineTransferred");
            DropColumn("Construction.ContractClaim", "OnlineApprovedTime");
            DropColumn("Construction.ContractClaim", "OnlineApprovedBy");
            DropColumn("Construction.ContractClaim", "IsOnlineApproved");
            DropColumn("Construction.ContractClaim", "VoidTime");
            DropColumn("Construction.ContractClaim", "VoidBy");
            DropColumn("Construction.ContractClaim", "IsVoid");
            DropColumn("Construction.ContractClaim", "Status");
            DropColumn("Construction.ContractClaim", "IsPostedToSecondLedger");
            DropColumn("Construction.BoQOfExecutedWork", "ModifiedBy");
            DropColumn("Construction.BoQOfExecutedWork", "CreatedBy");
            DropColumn("Construction.BoQOfExecutedWork", "LastModified");
            DropColumn("Construction.BoQOfExecutedWork", "DateCreated");
            DropColumn("Construction.BoQOfExecutedWork", "OrgBranchName");
            DropTable("Construction.RepresentativeElementCostInBill");
            DropTable("Construction.ProjectPaymentCertificateValidation");
            DropTable("Construction.ContractClaimValidation");
        }
    }
}
