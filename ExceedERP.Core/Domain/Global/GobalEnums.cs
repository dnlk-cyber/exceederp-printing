using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Global
{
    public class GlobalEnum
    {


        public enum GlobalSettingCategories
        {
            BACKUP_AND_RESTORE

        }

        public enum GlobalSettingBackup
        {
            DATABASE_BACKUP_PATH

        }

        public enum SuperSettingCategories
        {
            AdminMode,
            ModuleAdmin,
            BusinessUnit,
            PayrollProcessMode,
            [Description("Fiscal Year Type")]
            FiscalYearType
        }

        public enum SuperSettingAdminMode
        {
            Global,
            Module
        }

        public enum SuperSettingBusinessUnit
        {
            Single,
            Multiple
        }

        public enum PayrollProcessMode
        {
            Branch,
            BranchAndBusinessUnit
        }
        public enum FiscalYearTypeAttribute
        {
            Standard,
            Ethiopian,
            Unknown
        }
        public enum BatteryStatus
        {
            New, Old
        }
    }
}
