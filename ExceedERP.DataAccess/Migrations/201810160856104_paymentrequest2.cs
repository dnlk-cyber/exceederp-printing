namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PaymentRequestValidation",
                c => new
                    {
                        PaymentRequestValidationId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestValidationId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PaymentRequestValidation", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropIndex("EXCEED.PaymentRequestValidation", new[] { "PaymentRequestId" });
            DropTable("EXCEED.PaymentRequestValidation");
        }
    }
}
