namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApSetupReceiptBalanceAccount", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApSetupReceiptBalanceAccount", "Type");
        }
    }
}
