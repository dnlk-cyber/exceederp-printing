namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractpayment1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractPayment", "Paid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractPaymentGrn", "ItemType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ContractPaymentGrn", "GrnNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ContractPaymentGrn", "GrnNo", c => c.String());
            DropColumn("EXCEED.ContractPaymentGrn", "ItemType");
            DropColumn("EXCEED.ContractPayment", "Paid");
        }
    }
}
