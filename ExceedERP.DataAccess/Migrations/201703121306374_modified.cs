namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modified : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApSetupReceiptBalanceAccount", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ApSetupReceiptBalanceAccount", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ApSetupReceiptBalanceAccount", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ApSetupReceiptBalanceAccount", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ARSetupReceiptBalanceAccount", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ARSetupReceiptBalanceAccount", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ARSetupReceiptBalanceAccount", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ARSetupReceiptBalanceAccount", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ARSetupReceiptBalanceAccount", "ModifiedBy");
            DropColumn("EXCEED.ARSetupReceiptBalanceAccount", "CreatedBy");
            DropColumn("EXCEED.ARSetupReceiptBalanceAccount", "LastModified");
            DropColumn("EXCEED.ARSetupReceiptBalanceAccount", "DateCreated");
            DropColumn("EXCEED.ApSetupReceiptBalanceAccount", "ModifiedBy");
            DropColumn("EXCEED.ApSetupReceiptBalanceAccount", "CreatedBy");
            DropColumn("EXCEED.ApSetupReceiptBalanceAccount", "LastModified");
            DropColumn("EXCEED.ApSetupReceiptBalanceAccount", "DateCreated");
        }
    }
}
