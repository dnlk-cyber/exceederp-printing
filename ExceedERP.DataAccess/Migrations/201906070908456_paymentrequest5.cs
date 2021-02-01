namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PaymentRequestDepartmentValidation",
                c => new
                    {
                        PaymentRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestDepartmentValidationId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
            AddColumn("EXCEED.PaymentRequest", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "DepartmentSendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PaymentRequestDepartmentValidation", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropIndex("EXCEED.PaymentRequestDepartmentValidation", new[] { "PaymentRequestId" });
            DropColumn("EXCEED.PaymentRequest", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.PaymentRequest", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.PaymentRequest", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.PaymentRequest", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.PaymentRequest", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.PaymentRequest", "IsDepartmentOnlineApproved");
            DropTable("EXCEED.PaymentRequestDepartmentValidation");
        }
    }
}
