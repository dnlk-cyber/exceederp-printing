using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Procurement
{

     public static class ItemHelper
    {
         private static ExceedDbContext db = new ExceedDbContext();

        #region general

         public static List<string> GetUserBranchs(string user)
         {
             var branchesUserBelongsTo = db.OrgBranchUserMappings.Where(x => x.User == user).Select(x => x.Branch).ToList();
             return branchesUserBelongsTo;
         }
         public static string GetItemDesc(string itemcode)
         {
             string desc = "";
             var itemObj = db.InventoryItems.Where(x => x.ItemCode == itemcode).ToList();
             desc = itemObj.Any() ? itemObj.Select(x => x.Name).FirstOrDefault() : "";
             return desc;
         }
         public static string GetCategoryDesc(string categorycode)
         {
             string desc = "";
             var catObj = db.ItemCategorys.Where(x => x.ItemCategoryCode == categorycode).ToList();
             desc = catObj.Any() ? catObj.Select(x => x.Name).FirstOrDefault() : "";
             return desc;
         }
        public static string UserCanManipulateRecord(string currentuser, string recorduser, string recordbranch)
        {
            string errOrSuccesMsg = "";//edit and delet
            //check if the data was created by the user
            if (currentuser != recorduser)
            {
                errOrSuccesMsg = "You can not manipulate this record";
            }
            else
            {
                var branchObj = db.OrgBranchUserMappings.Where(x => x.User == currentuser)
                                                    .Where(x => x.Branch == recordbranch).ToList();
                if (branchObj == null)
                    errOrSuccesMsg = "You can not manipulate this record";
            }

            return errOrSuccesMsg;
        }

        public static bool SetDefaultUserBranch(string userid, string branchId)
        {
            DefaultBranch userDefaultBranch = db.DefaultBranches.Find(userid);
            if (branchId != null && userDefaultBranch != null)
            {

                userDefaultBranch.BranchId = branchId;
                db.Entry(userDefaultBranch).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }

            else if (branchId != null && userDefaultBranch == null)
            {
                userDefaultBranch = new DefaultBranch();
                userDefaultBranch.UserId = userid;
                userDefaultBranch.BranchId = branchId;
                db.DefaultBranches.Add(userDefaultBranch);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public static void AccessLog(string ip, string type, string status, string user, string description)
        {
            GlobalUserAccessLog log = new GlobalUserAccessLog
            {
                IPAddress = ip,
                UserName = user,
                TimeAccessed = DateTime.Now,
                Type = type,
                Status = status,
                Description = description
            };
            db.GlobalUserAccessLog.Add(log);
            db.SaveChanges();
        }

        public static void DeleteLog(string ip, string type, string status, string user, string description)
        {
            GlobalDeleteLog log = new GlobalDeleteLog
            {
                IpAddress = ip,
                UserName = user,
                TimeAccessed = DateTime.Now,
                Type = type,
                Status = status,
                Description = description
            };
            db.GlobalDeleteLogs.Add(log);
            db.SaveChanges();
        }

        public static void ExportLog(string ip, string type, string status, string user, string description)
        {
            GlobalExportLog log = new GlobalExportLog
            {
                IpAddress = ip,
                UserName = user,
                TimeAccessed = DateTime.Now,
                Type = type,
                Status = status,
                Description = description
            };
            db.GlobalExportLog.Add(log);
            db.SaveChanges();
        }

        public static void MiscLog(string ip, string type, string status, string user, string description)
        {
           
        }
        #endregion
    }
}
