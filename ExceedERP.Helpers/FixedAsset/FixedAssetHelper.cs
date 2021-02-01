using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Helpers.FixedAsset
{
    public static class FixedAssetHelper
    {
       
        public static bool IsAssetCurrentLocationStore(int regId,string storecode)
        {
            ExceedDbContext Db = new ExceedDbContext();
            bool isStore = false;

            var record =Db.LocationTrackings.Where(x => x.AssetRegistrationId == regId).ToList();
           
            if (record.Any())
            {
                var maxid = record.Max(x => x.LocationTrackingId);
                var locationTracking = Db.LocationTrackings.Find(maxid);
                if (locationTracking != null && locationTracking.StoreCode == storecode)
                {
                    isStore = true;
                }
            }
            return isStore;
        }

        public static void UpdateAssetLocation(int assetid,int branchid, int employeeid,string storecode,LocationType type,string user,string ttype,string tno)
        {
            ExceedDbContext Db = new ExceedDbContext();
            var asset = Db.AssetRegistrations.Find(assetid);
            var newLocation = new LocationTracking();
            var previosLocation = Db.LocationTrackings.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId);
            if (previosLocation.Any())
            {
                #region update prev location
                int maxid = previosLocation.Max(x => x.LocationTrackingId);
                var prevRecord = previosLocation.FirstOrDefault(x => x.LocationTrackingId == maxid);
                if (prevRecord != null)
                {
                    prevRecord.IsCurrent = false;
                    prevRecord.EndDateDate = DateTime.Today;
                    prevRecord.LastModified = DateTime.Now;
                    prevRecord.ModifiedBy = user;
                    Db.Entry(prevRecord).State = EntityState.Modified;
                }
                Db.SaveChanges();
                #endregion
                
            }
            #region add new location
            if (asset != null)
            {
                newLocation.AssetRegistrationId = asset.AssetRegistrationId;
                newLocation.TransactionType = ttype;
                newLocation.TransactionNo =tno;
                newLocation.LocationType = type;
                newLocation.EmployeeId = employeeid.ToString();
                newLocation.StartDate = DateTime.Today;
                newLocation.BranchId = branchid;
                newLocation.StoreCode = storecode;
                newLocation.IsCurrent = true;
                newLocation.DateCreated = DateTime.Now;
                newLocation.CreatedBy = user;
                Db.LocationTrackings.Add(newLocation);
                Db.SaveChanges();
            }

            #endregion
           
            
        }

        public static int GetDepreciationMonths(DateTime? grnDate, DateTime quarterEndDate)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int months = 0;
            var periods = Db.GLPeriods.Where(x => x.DateFrom >= grnDate)
                .Where(x => x.DateTo <= quarterEndDate).ToList();
            months = periods.Count();
            if (grnDate?.Day < 15)//if bought before the 15th of the month,it is counted
                months = months + 1;
            return months;
        }
    }
}
