using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common
{
    public static class CacheKeys
    {
        public const string IsMultipleBusinessUnit = "IsMultipleBusinessUnit";
        public const string IsPayrollModeBranchAndBusinessUnit = "IsPayrollModeBranchAndBusinessUnit";
        public const string IsProcessAllBranchPayroll = "IsProcessAllBranchPayroll";
        public const string Suppliers = "Suppliers";
        public const string FiscalYears = "FiscalYears";
        public const string Branchs = "Branchs";
        public const string Periods = "Periods";
        public const string WorkUnits = "WorkUnits";
        public const string Employees = "Employees";
        public const string Taxes = "Taxes";
        public const string APItemsSetups = "APItemsSetups";
        public const string PaymentMethods = "PaymentMethods";
        public const string ReceiveTypes = "ReceiveTypes";
        public const string InventoryCategory = "InventoryCategory";
        public const string InventoryItems = "InventoryItems";
        public const string InventoryPeriods = "InventoryPeriods";
        public const string Stores = "Stores";
        public const string BusinessUnits = "BusinessUnits";
        public const string GlSegmentSetups = "GlSegmentSetups";
        public const string GlChartOfAccountSegments = "GlChartOfAccountSegments";
        public const string GLChartOfAccountAccounts = "GLChartOfAccountAccounts";
        public const string SegmentList = "SegmentList";
        //ppe
        public const string PPEPeriods = "PPEPeriods";
        public const string PPECategories = "PPECategories";
        public const string PPESubCategories = "PPESubCategories";
        public const string PPESubCategoryNames = "PPESubCategoryNames";
        public const string PPEAsstes = "PPEAsstes";
        public const string PPEAsstConditions = "PPEAsstConditions";
        //financial
        public static string SegmentById(int id) => $"Segment:{id}";
        public static string UserRoles(string userId) => $"UserRoles:{userId}";
        public static string ConfigSetting(string category, string reference) => $"Config:{category}:{reference}";
        public const string JournalCategories = "JournalCategories";
        public const string JournalRecordCategories = "JournalRecordCategories";
        public const string JournalUsers = "JournalUsers";
        // CRM
        public const string CrmProducts = "CrmProducts";
        public const string SalesStores = "SalesStores";
        public const string CrmCustomers = "CrmCustomers";
        public const string CrmActiveCustomers = "CrmActiveCustomers";
        public const string CrmPeriods = "CrmPeriods";
        public const string RepairTypes = "RepairTypes";
        public const string BankAccounts = "BankAccounts";

        //hrm 
        public const string JobTitles = "JobTitles";
        public const string Steps = "Steps";
        public const string Ranks = "Ranks";
        //Fleet
        public const string EquipmentIdentity = "EquipmentIdentity";
        public const string EquipmentType = "EquipmentType";
        public const string EquipmentName = "EquipmentName";
        //maintenance
        public const string MaintenanceCenters = "MaintenanceCenters";
    }

}
