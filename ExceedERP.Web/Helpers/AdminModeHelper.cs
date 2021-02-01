using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Helpers
{
    public class AdminModeHelper
    {
        public static bool IsModuleMode()
        {
            ExceedDbContext db = new ExceedDbContext();
            string reff = SecurityHelper.Encrypt("AdminMode", XAPI.Common.KEY);
            string att = SecurityHelper.Encrypt("Module", XAPI.Common.KEY);

            
            var adminMode = db.Configurations
                   .Where(x => x.Category == "S")
                   .Where(x=>x.Reference == reff)
                   .Where(x => x.Attribute == att).FirstOrDefault();

            if (adminMode != null)
                return true;
            return false;
        }

        public static List<string> GetModuleForAdmin(string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            List<string> list =new List<string>();
            string reff = SecurityHelper.Encrypt("ModuleAdmin", XAPI.Common.KEY);
            string usr = SecurityHelper.Encrypt(user, XAPI.Common.KEY);


            var moduleList = db.Configurations
                   .Where(x => x.Category == "S")
                   .Where(x => x.Reference == reff)
                   .Where(x => x.CurrentValue == usr).Select(x=>x.Attribute).ToList();

            //x => 

            foreach (var module in moduleList)
                list.Add(SecurityHelper.Decrypt(module, XAPI.Common.KEY));

            return list;
        }
    }
}