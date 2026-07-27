using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;

using ExceedERP.Helpers.Common;

namespace ExceedERP.Helpers.Procurement
{

     public  class GuaranteeHelper
    {

        public static List<int> GetExpiredList()
        {
            List<int> list = new List<int>();
            ExceedDbContext db = new ExceedDbContext();
            var guaranteeIds = db.Guarantees.Select(x => x.GuaranteeId).ToList();
            foreach (var id in guaranteeIds)
            {
                if(IsGuaranteeExpired(id))
                    list.Add(id);
            }
            return list;
        }

        public static List<int> GetRenewalList()
        {
            List<int> list = new List<int>();
            ExceedDbContext db = new ExceedDbContext();
            var guaranteeIds = db.Guarantees.Select(x => x.GuaranteeId).ToList();
            foreach (var id in guaranteeIds)
            {
                if (IsGuaranteeExpired(id)&& IsRenewalSent(id))
                    list.Add(id);
            }
            return list;
        }

        public static bool IsGuaranteeExpired(int id)
        {
             ExceedDbContext db = new ExceedDbContext();
             bool isGuaranteeExpired = false;
            var guarantee = db.Guarantees.Where(x=>x.GuaranteeId==id).AsNoTracking().FirstOrDefault();
            DateTime today = DateTime.Now;
            decimal days = 100000000;
            int alarmday = 0;


            if (guarantee != null)
            {
                if (guarantee.ValidityStatus == GuaranteeValidityStatus.Forfeited ||
                    guarantee.ValidityStatus == GuaranteeValidityStatus.Released)
                    return false;

                #region get rule

                var rule = db.GlobalRuleValues.Where(x => x.Module == Modules.Procurement).FirstOrDefault(x => x.Key == KeyType.GuaranteeExpiryAlarmDays);
                if (rule != null)
                    try
                    {
                        alarmday = int.Parse(rule.Value);
                    }
                    catch (Exception e)
                    {
                        Logger.Debug(e.Message);
                    }

                #endregion
                var renewal = guarantee.GuaranteeRenewals;
                if (renewal.Count > 0)
                {
                    int totalRenewDays = 0;
                    foreach (var r in renewal)
                    {
                        totalRenewDays = totalRenewDays + r.ValidityPeriod;
                    }
                    var newExpireDate = guarantee.ExpireDate.AddDays(totalRenewDays);
                    days = ((newExpireDate - today)).Days;
                    if (days <= alarmday)
                        isGuaranteeExpired = true;
                }
                else
                {
                    days = (guarantee.ExpireDate - today).Days;
                    if (days <= alarmday)
                        isGuaranteeExpired = true;
                }


            }
            

            return isGuaranteeExpired;
        }

        public static bool IsRenewalSent(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
           
            var guarantee = db.Guarantees.Where(x => x.GuaranteeId == id).AsNoTracking().FirstOrDefault();
           
            if (guarantee != null)
            {
                if (guarantee.ValidityStatus == GuaranteeValidityStatus.Forfeited ||
                    guarantee.ValidityStatus == GuaranteeValidityStatus.Released)
                    return false;

            
                var renewal = guarantee.RenewalRequests;
                if (renewal.Any())
                {
                    return true;
                }
               

            }


            return false;
        }
    }
}
