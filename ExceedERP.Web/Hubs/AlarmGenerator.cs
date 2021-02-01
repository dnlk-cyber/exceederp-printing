using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Hubs
{
    public  class AlarmGenerator
    {
        private static ExceedDbContext _db = new ExceedDbContext();
        public void GenerateGuaranteeAlarms()
        {
            DateTime today = DateTime.Now;
            decimal days = 100000000;
            int alarmday = 0;

            #region get rule

            var rule = _db.GlobalRuleValues.Where(x=>x.Module==Modules.Procurement).FirstOrDefault(x => x.Key==KeyType.GuaranteeExpiryAlarmDays);
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
            var guarantees = _db.Guarantees.ToList();
           
            foreach (var i in guarantees)
            {
                var renewal = i.GuaranteeRenewals;
                if (renewal.Count > 0)
                {
                    int maxid = renewal.Max(x => x.GuaranteeRenewalId);
                    var lastRecord = renewal.SingleOrDefault(x => x.GuaranteeRenewalId == maxid);
                    if (lastRecord != null)
                    {
                        var newExpireDate = i.ExpireDate.AddDays(lastRecord.ValidityPeriod);
                        days = ((newExpireDate - today)).Days;
                    }
                    if (days <= alarmday)
                    {

                        #region send notification

                        List<UserVM> users = new List<UserVM>();
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Bond_User"));
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Bond_Approver"));
                        var uniqueusers=users.DistinctByField(x=>x.UserId);

                        string message = "Guarantee of supplier : " + i.SupplierName + " reference " + i.GuaranteeRefNo + " is going to expire in " + days + " days.";
                        foreach (var user in uniqueusers)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserName,
                                Title = "Guarantee expiring alarm",
                                Message = message,
                                DeadLine = i.ExpireDate,
                                URL="~/Procurement/Guarantee"
                            };
                            Notify.NotifyUser(notif);
                        }
                       
                        #endregion
                    }
                }
                else
                {
                    days = (i.ExpireDate - today).Days;
                    if (days <= alarmday)
                    {
                        #region send notification
                        List<UserVM> users = new List<UserVM>();
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Bond_User"));
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Bond_Approver"));
                        var uniqueusers = users.DistinctByField(x => x.UserId);

                        string message = "Guarantee of supplier : " + i.SupplierName + " reference " + i.GuaranteeRefNo + " is going to expire in " + days + " days.";
                        foreach (var user in uniqueusers)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId =user.UserName,
                                Title = "Guarantee expiring alarm",
                                Message = message,
                                DeadLine = i.ExpireDate
                            };
                            Notify.NotifyUser(notif);
                        }

                        #endregion
                    }
                }
            }
            _db.SaveChanges();
        }
        public void ContractAlarms()
        {
            DateTime today = DateTime.Now;
            var contractManagements = _db.ContractManagements.ToList();
            int alarmday = 0;

            #region get rule

            var rule = _db.GlobalRuleValues.Where(x => x.Module == Modules.Procurement).FirstOrDefault(x => x.Key == KeyType.ContractExpiryAlarmDays);
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

            foreach (var i in contractManagements)
            {
                bool notify = false;
                #region check delivery
                var contractItems = _db.ContractManagementItems
                    .Where(x => x.ContractManagementID == i.ContractManagementID).ToList();
                foreach (var item in contractItems)
                {
                    if (notify == false)
                    {
                        switch (item.ItemType)
                        {
                            case PurchaseItemType.Stock:
                                var receiveItem = _db.GoodReceiveItems
                                    .Where(x => x.GoodReceive.IsOnlineApproved && x.GoodReceive.IsVoid==false)
                                    .Where(x => x.GoodReceive.ReceiveType.Type == GoodReceiveType.Tender)
                                    .Where(x => x.GoodReceive.ReceiveRef == i.ProcurementRefNo.ToString())
                                    .Where(x=>x.ItemCode==item.ItmCode).ToList();
                                if (receiveItem.Sum(x => x.Quantity) < item.Quantity)
                                    notify = true;
                                break;
                            case PurchaseItemType.FixedAsset:
                                var assetReceiveItems = _db.FixedAssetReceiveItems
                                    .Where(x => x.FixedAssetReceive.IsOnlineApproved && x.FixedAssetReceive.IsVoid == false)
                                    .Where(x => x.FixedAssetReceive.ReceiveType.Type == GoodReceiveType.Tender)
                                    .Where(x => x.FixedAssetReceive.ReceiveRef == i.ProcurementRefNo.ToString())
                                    .Where(x => x.FixedAssetSubCategoryID.ToString() == item.ItmCode)
                                    .Where(x => x.Name == item.Name).ToList();
                                if (assetReceiveItems.Sum(x => x.Quantity) < item.Quantity)
                                    notify = true;
                                break;
                            case PurchaseItemType.Service:
                                var serviceReceiveItems = _db.ServiceReceiveItems
                                    .Where(x => x.ServiceReceive.IsOnlineApproved && x.ServiceReceive.IsVoid == false)
                                    .Where(x => x.ServiceReceive.ReceiveType.Type == GoodReceiveType.Tender)
                                    .Where(x => x.ServiceReceive.ReceiveRef == i.ProcurementRefNo.ToString())
                                    .Where(x => x.ServiceCategoryCode == item.CatCode)
                                    .Where(x => x.ServiceItemCode == item.ItmCode)
                                    .ToList();
                                if (serviceReceiveItems.Sum(x => x.Quantity) < item.Quantity)
                                    notify = true;
                                break;
                        }
                    }
                    
                }
                #endregion

                if (notify)
                {
                    decimal days = (i.DeliveryDate - today).Days;
                    if (days <= alarmday)
                    {
                        #region send notification

                        List<UserVM> users = new List<UserVM>();
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Contract_User"));
                        users.AddRange(UserInformationHelper.GetUsersInRole("Procurement_Contract_Approver"));
                        var uniqueusers = users.DistinctByField(x => x.UserId);

                        string message = "Contract of supplier : " + i.SupplierName + " reference " + i.ContractRefNo +
                                         " is going to expire in " + days + " days.";
                        foreach (var user in uniqueusers)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserId,
                                Title = "Contract expiring alarm",
                                Message = message,
                                DeadLine = i.DeliveryDate
                            };
                            Notify.NotifyUser(notif);
                        }

                        #endregion

                    }
                }

            }
            _db.SaveChanges();
        }

    }
}