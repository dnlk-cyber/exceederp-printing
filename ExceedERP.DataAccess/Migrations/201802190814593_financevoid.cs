namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financevoid : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApPayment", "IsOnlineTransferredBy", c => c.String());
            AlterColumn("EXCEED.ApPaybleInvoices", "EmployeeId", c => c.Int());
            DropColumn("EXCEED.ApMiscellaneousPayment", "OnlineTransferredBy");
            DropColumn("EXCEED.ApPayment", "OnlineTransferredBy");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApPayment", "OnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "OnlineTransferredBy", c => c.String());
            AlterColumn("EXCEED.ApPaybleInvoices", "EmployeeId", c => c.Int(nullable: false));
            DropColumn("EXCEED.ApPayment", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ApMiscellaneousPayment", "IsOnlineTransferredBy");
        }
    }
}
