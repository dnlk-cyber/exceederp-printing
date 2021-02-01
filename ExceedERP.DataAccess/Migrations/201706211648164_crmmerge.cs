namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmmerge : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "common.AccountMap", newSchema: "CRM");
            MoveTable(name: "common.AccountRequirement", newSchema: "CRM");
            MoveTable(name: "common.Account", newSchema: "CRM");
            MoveTable(name: "common.ActivityDefinition", newSchema: "CRM");
            MoveTable(name: "common.Activity", newSchema: "CRM");
            MoveTable(name: "common.BankAccountDetail", newSchema: "CRM");
            MoveTable(name: "common.Beginning", newSchema: "CRM");
            MoveTable(name: "Common.Cart", newSchema: "CRM");
            MoveTable(name: "common.CartTransaction", newSchema: "CRM");
            MoveTable(name: "Sales.CashSalesInvoice", newSchema: "CRM");
            MoveTable(name: "common.CommunicationSource", newSchema: "CRM");
            MoveTable(name: "common.ControlAccount", newSchema: "CRM");
            MoveTable(name: "Common.CreditSalesInvoice", newSchema: "CRM");
            MoveTable(name: "common.Currency", newSchema: "CRM");
            MoveTable(name: "common.CurrencyPreference", newSchema: "CRM");
            MoveTable(name: "common.GlobalTypeList", newSchema: "CRM");
            MoveTable(name: "common.GlobalTypeTax", newSchema: "CRM");
            MoveTable(name: "common.JournalDetail", newSchema: "CRM");
            MoveTable(name: "common.JournalTemplate", newSchema: "CRM");
            MoveTable(name: "Common.LifeSpan", newSchema: "CRM");
            MoveTable(name: "common.LifeTime", newSchema: "CRM");
            MoveTable(name: "Common.NonCashTransaction", newSchema: "CRM");
            MoveTable(name: "Common.ObjectAccountRequirement", newSchema: "CRM");
            MoveTable(name: "Common.ObjectStateDefinition", newSchema: "CRM");
            MoveTable(name: "common.ObjectState", newSchema: "CRM");
            MoveTable(name: "Common.OrganizationCategory", newSchema: "CRM");
            MoveTable(name: "Common.OrganizationCustomer", newSchema: "CRM");
            MoveTable(name: "Common.PersonCategory", newSchema: "CRM");
            MoveTable(name: "common.Price", newSchema: "CRM");
            MoveTable(name: "Common.Product", newSchema: "CRM");
            MoveTable(name: "common.Religion", newSchema: "EXCEED");
            MoveTable(name: "Common.Role", newSchema: "CRM");
            MoveTable(name: "Common.SalesOrderVoucher", newSchema: "CRM");
            MoveTable(name: "common.SalesQuoteVoucher", newSchema: "CRM");
            MoveTable(name: "common.SerialCode", newSchema: "CRM");
            MoveTable(name: "common.SerialDefinition", newSchema: "CRM");
            MoveTable(name: "Common.Specification", newSchema: "CRM");
            MoveTable(name: "common.StockBalance", newSchema: "CRM");
            MoveTable(name: "common.StockLevel", newSchema: "CRM");
            MoveTable(name: "Common.Store", newSchema: "CRM");
            MoveTable(name: "common.StoreTransaction", newSchema: "CRM");
            MoveTable(name: "common.Tax", newSchema: "CRM");
            MoveTable(name: "common.TaxTransaction", newSchema: "CRM");
            MoveTable(name: "common.Term", newSchema: "CRM");
            MoveTable(name: "common.VoucherDefinition", newSchema: "CRM");
            MoveTable(name: "Common.ThingCategory", newSchema: "CRM");
            MoveTable(name: "Common.ThingCategoryDefaultAccount", newSchema: "CRM");
            MoveTable(name: "common.TransactionReference", newSchema: "CRM");
            MoveTable(name: "common.User", newSchema: "CRM");
            MoveTable(name: "common.ValueFactorDefinition", newSchema: "EXCEED");
            MoveTable(name: "common.ValueFactor", newSchema: "CRM");
            MoveTable(name: "common.VoucherExtension", newSchema: "CRM");
            MoveTable(name: "common.VoucherExtensionTransaction", newSchema: "CRM");
            MoveTable(name: "common.VoucherTerm", newSchema: "CRM");
            MoveTable(name: "common.VoucherValue", newSchema: "CRM");
        }
        
        public override void Down()
        {
            MoveTable(name: "CRM.VoucherValue", newSchema: "common");
            MoveTable(name: "CRM.VoucherTerm", newSchema: "common");
            MoveTable(name: "CRM.VoucherExtensionTransaction", newSchema: "common");
            MoveTable(name: "CRM.VoucherExtension", newSchema: "common");
            MoveTable(name: "CRM.ValueFactor", newSchema: "common");
            MoveTable(name: "EXCEED.ValueFactorDefinition", newSchema: "common");
            MoveTable(name: "CRM.User", newSchema: "common");
            MoveTable(name: "CRM.TransactionReference", newSchema: "common");
            MoveTable(name: "CRM.ThingCategoryDefaultAccount", newSchema: "Common");
            MoveTable(name: "CRM.ThingCategory", newSchema: "Common");
            MoveTable(name: "CRM.VoucherDefinition", newSchema: "common");
            MoveTable(name: "CRM.Term", newSchema: "common");
            MoveTable(name: "CRM.TaxTransaction", newSchema: "common");
            MoveTable(name: "CRM.Tax", newSchema: "common");
            MoveTable(name: "CRM.StoreTransaction", newSchema: "common");
            MoveTable(name: "CRM.Store", newSchema: "Common");
            MoveTable(name: "CRM.StockLevel", newSchema: "common");
            MoveTable(name: "CRM.StockBalance", newSchema: "common");
            MoveTable(name: "CRM.Specification", newSchema: "Common");
            MoveTable(name: "CRM.SerialDefinition", newSchema: "common");
            MoveTable(name: "CRM.SerialCode", newSchema: "common");
            MoveTable(name: "CRM.SalesQuoteVoucher", newSchema: "common");
            MoveTable(name: "CRM.SalesOrderVoucher", newSchema: "Common");
            MoveTable(name: "CRM.Role", newSchema: "Common");
            MoveTable(name: "EXCEED.Religion", newSchema: "common");
            MoveTable(name: "CRM.Product", newSchema: "Common");
            MoveTable(name: "CRM.Price", newSchema: "common");
            MoveTable(name: "CRM.PersonCategory", newSchema: "Common");
            MoveTable(name: "CRM.OrganizationCustomer", newSchema: "Common");
            MoveTable(name: "CRM.OrganizationCategory", newSchema: "Common");
            MoveTable(name: "CRM.ObjectState", newSchema: "common");
            MoveTable(name: "CRM.ObjectStateDefinition", newSchema: "Common");
            MoveTable(name: "CRM.ObjectAccountRequirement", newSchema: "Common");
            MoveTable(name: "CRM.NonCashTransaction", newSchema: "Common");
            MoveTable(name: "CRM.LifeTime", newSchema: "common");
            MoveTable(name: "CRM.LifeSpan", newSchema: "Common");
            MoveTable(name: "CRM.JournalTemplate", newSchema: "common");
            MoveTable(name: "CRM.JournalDetail", newSchema: "common");
            MoveTable(name: "CRM.GlobalTypeTax", newSchema: "common");
            MoveTable(name: "CRM.GlobalTypeList", newSchema: "common");
            MoveTable(name: "CRM.CurrencyPreference", newSchema: "common");
            MoveTable(name: "CRM.Currency", newSchema: "common");
            MoveTable(name: "CRM.CreditSalesInvoice", newSchema: "Common");
            MoveTable(name: "CRM.ControlAccount", newSchema: "common");
            MoveTable(name: "CRM.CommunicationSource", newSchema: "common");
            MoveTable(name: "CRM.CashSalesInvoice", newSchema: "Sales");
            MoveTable(name: "CRM.CartTransaction", newSchema: "common");
            MoveTable(name: "CRM.Cart", newSchema: "Common");
            MoveTable(name: "CRM.Beginning", newSchema: "common");
            MoveTable(name: "CRM.BankAccountDetail", newSchema: "common");
            MoveTable(name: "CRM.Activity", newSchema: "common");
            MoveTable(name: "CRM.ActivityDefinition", newSchema: "common");
            MoveTable(name: "CRM.Account", newSchema: "common");
            MoveTable(name: "CRM.AccountRequirement", newSchema: "common");
            MoveTable(name: "CRM.AccountMap", newSchema: "common");
        }
    }
}
