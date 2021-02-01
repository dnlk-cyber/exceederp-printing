namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gaapreportrule : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GaapBalanceSheet", "ReportType", c => c.Int(nullable: false));
            AddColumn("EXCEED.GaapIncomeStatement", "ReportType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GaapIncomeStatement", "ReportType");
            DropColumn("EXCEED.GaapBalanceSheet", "ReportType");
        }
    }
}
