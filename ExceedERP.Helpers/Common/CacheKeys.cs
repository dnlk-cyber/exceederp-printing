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
        public const string Branchs = "Branchs";
        public const string WorkUnits = "WorkUnits";
        public const string Employees = "Employees";
        public const string Taxes = "Taxes";
        public const string BusinessUnits = "BusinessUnits";
        public const string GlSegmentSetups = "GlSegmentSetups";
        public const string GlChartOfAccountSegments = "GlChartOfAccountSegments";
        public const string GLChartOfAccountAccounts = "GLChartOfAccountAccounts";
        public const string SegmentList = "SegmentList";
        public static string SegmentById(int id) => $"Segment:{id}";
        public static string UserRoles(string userId) => $"UserRoles:{userId}";
        public static string ConfigSetting(string category, string reference) => $"Config:{category}:{reference}";
    }

}
