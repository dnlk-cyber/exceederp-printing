using System;
using System.Collections.Generic;
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
            BusinessUnit

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
    }
}
