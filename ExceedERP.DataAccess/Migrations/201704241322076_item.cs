namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class item : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemPriceIndex", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "InventoryItemDesc", c => c.String());
            DropColumn("Payroll.EmpPayrollDeduction", "isSendForApproval");
            DropColumn("Payroll.EmpPayrollLongTermDeduction", "isSendForApproval");
            DropColumn("Payroll.EmpPayrollServiceChargeViewModel", "isSendForApproval");
            DropColumn("EXCEED.GoodReceive", "IsSendForApproval");
            DropColumn("EXCEED.ItemInspection", "IsSendForApproval");
            DropColumn("EXCEED.GoodReceiveDonation", "IsSendForApproval");
            DropColumn("EXCEED.GoodSellingAndLending", "IsSendForApproval");
            DropColumn("EXCEED.Issue", "IsSendForApproval");
            DropColumn("EXCEED.StoreRequisition", "IsSendForApproval");
            DropColumn("EXCEED.IssueFuel", "IsSendForApproval");
            DropColumn("EXCEED.ItemTransfer", "IsSendForApproval");
            DropColumn("EXCEED.PurchaseRequisition", "IsSendForApproval");
            DropColumn("Payroll.PayrollProcessAdditions", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessViewModel", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessAdvances", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessAllowances", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessDeductions", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessFixedContribution", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessLoans", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessLongTermDeductions", "isSendForApproval");
            DropColumn("Payroll.PayrollProcessRegularContribution", "isSendForApproval");
            DropColumn("EXCEED.PurchaseReturn", "IsSendForApproval");
            //DropColumn("EXCEED.Revaluation", "IsSendForApproval");
            DropColumn("EXCEED.StockAdjustment", "IsSendForApproval");
            DropColumn("EXCEED.StoreReturn", "IsSendForApproval");
            DropColumn("EXCEED.TempGoodReceive", "IsSendForApproval");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TempGoodReceive", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StockAdjustment", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Revaluation", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseReturn", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessRegularContribution", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessLongTermDeductions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessLoans", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessFixedContribution", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessDeductions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessAllowances", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessAdvances", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.PayrollProcessAdditions", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.IssueFuel", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodSellingAndLending", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceiveDonation", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceive", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.EmpPayrollServiceChargeViewModel", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.EmpPayrollLongTermDeduction", "isSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Payroll.EmpPayrollDeduction", "isSendForApproval", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ItemPriceIndex", "InventoryItemDesc");
            DropColumn("EXCEED.ItemPriceIndex", "ItemCategoryDesc");
        }
    }
}
