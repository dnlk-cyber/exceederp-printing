namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrmbudget1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FleetManagement.Fleets", "IFRSCondition", c => c.String());
            AddColumn("Budget.BudgetRequest", "FiscalYearId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetRequest", "FromDate", c => c.DateTime(nullable: false));
            AddColumn("Budget.BudgetRequest", "ToDate", c => c.DateTime(nullable: false));
            AddColumn("Budget.BudgetRequestLine", "AccountCode", c => c.String(nullable: false));
            AddColumn("Budget.BudgetRequestTransferLog", "FromDate", c => c.DateTime(nullable: false));
            AddColumn("Budget.BudgetRequestTransferLog", "ToDate", c => c.DateTime(nullable: false));
            DropColumn("Budget.BudgetRequest", "FromMonth");
            DropColumn("Budget.BudgetRequest", "ToMonth");
            DropColumn("Budget.BudgetRequestTransferLog", "FromMonth");
            DropColumn("Budget.BudgetRequestTransferLog", "ToMonth");
        }
        
        public override void Down()
        {
            AddColumn("Budget.BudgetRequestTransferLog", "ToMonth", c => c.String());
            AddColumn("Budget.BudgetRequestTransferLog", "FromMonth", c => c.String());
            AddColumn("Budget.BudgetRequest", "ToMonth", c => c.String());
            AddColumn("Budget.BudgetRequest", "FromMonth", c => c.String());
            DropColumn("Budget.BudgetRequestTransferLog", "ToDate");
            DropColumn("Budget.BudgetRequestTransferLog", "FromDate");
            DropColumn("Budget.BudgetRequestLine", "AccountCode");
            DropColumn("Budget.BudgetRequest", "ToDate");
            DropColumn("Budget.BudgetRequest", "FromDate");
            DropColumn("Budget.BudgetRequest", "FiscalYearId");
            DropColumn("FleetManagement.Fleets", "IFRSCondition");
        }
    }
}
