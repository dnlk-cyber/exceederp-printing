namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidAwardedSupplier", "AwardGiven", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Lot", "EvaluateTotalFinance", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "TenderRefNo", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "AwardGiven", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.WinningSupplier", "AwardGiven");
            DropColumn("EXCEED.ContractManagement", "TenderRefNo");
            DropColumn("EXCEED.Lot", "EvaluateTotalFinance");
            DropColumn("EXCEED.BidAwardedSupplier", "AwardGiven");
        }
    }
}
