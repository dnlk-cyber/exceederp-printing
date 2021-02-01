namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PaymentRequestLine", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequestLine", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequestLine", "CreatedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequestLine", "ModifiedBy", c => c.String());
            AlterColumn("EXCEED.PaymentRequest", "Purchaser", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PaymentRequest", "Purchaser", c => c.String());
            DropColumn("EXCEED.PaymentRequestLine", "ModifiedBy");
            DropColumn("EXCEED.PaymentRequestLine", "CreatedBy");
            DropColumn("EXCEED.PaymentRequestLine", "LastModified");
            DropColumn("EXCEED.PaymentRequestLine", "DateCreated");
        }
    }
}
