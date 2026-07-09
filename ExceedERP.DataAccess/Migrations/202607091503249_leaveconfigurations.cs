namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leaveconfigurations : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PaymentRequest", "Source", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "SourceId", c => c.String());
            AddColumn("HRM.LeaveEntitlementSettings", "LeaveIncrementAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "IsSendForPayment", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePayableDetails", "SendForPaymentBy", c => c.String());
            AddColumn("HRM.LeavePayableDetails", "SendForPaymentTime", c => c.DateTime());
            AddColumn("HRM.LeavePayableDetails", "PaymentRequestId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeavePayableDetails", "PaymentRequestId");
            DropColumn("HRM.LeavePayableDetails", "SendForPaymentTime");
            DropColumn("HRM.LeavePayableDetails", "SendForPaymentBy");
            DropColumn("HRM.LeavePayableDetails", "IsSendForPayment");
            DropColumn("HRM.LeaveEntitlementSettings", "LeaveIncrementAmount");
            DropColumn("EXCEED.PaymentRequest", "SourceId");
            DropColumn("EXCEED.PaymentRequest", "Source");
        }
    }
}
