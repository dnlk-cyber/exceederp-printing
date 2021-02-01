namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PaymentRequest", "SupplierId", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "IsNewSupplier", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.PaymentRequest", "Purchaser", c => c.Int());
            AlterColumn("EXCEED.PaymentRequest", "EmployeeId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PaymentRequest", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PaymentRequest", "Purchaser", c => c.Int(nullable: false));
            DropColumn("EXCEED.PaymentRequest", "IsNewSupplier");
            DropColumn("EXCEED.PaymentRequest", "SupplierId");
        }
    }
}
