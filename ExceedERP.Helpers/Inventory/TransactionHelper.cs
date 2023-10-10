using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;

namespace ExceedERP.Helpers.Inventory
{
    public  class TransactionHelper
    {
       

        public static void CloseStoreRequisition()
        {
            ExceedDbContext db = new ExceedDbContext();
            var storeRequisitions = db.StoreRequisitions.Where(x => x.IsFullfilled == false).AsNoTracking().ToList();
            foreach (var req in storeRequisitions)
            {
                db = new ExceedDbContext();
                bool isfullfilled = IsSrFullFilled(req.StoreRequisitionID);
                if (isfullfilled)
                {
                    #region close req

                    req.IsFullfilled = true;
                    db.Entry(req).State = EntityState.Modified;
                    db.SaveChanges();

                    #endregion
                }
            }

        }

        //store requisition
        public static bool IsSrFullFilled(int  reqid)
        {
            ExceedDbContext db = new ExceedDbContext();
            bool isfullfilled = true;
            var req = db.StoreRequisitions.Find(reqid);
            var requisitionItems = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == reqid).ToList();

            foreach (var item in requisitionItems)
            {
                db = new ExceedDbContext();
                if (req != null && !req.IsTransfer)
                {
                    #region for issue

                    var issueItems = db.IssueItems.Where(x => x.Issue.StoreRequisitionID ==reqid)
                        .Where(x => x.ItemCode == item.ItemCode)
                        .Where(x => x.Issue.IsOnlineApproved && !x.Issue.IsVoid).ToList();
                    if (item.ApprovedQuantity < issueItems.Sum(x => x.IssuedQuantity))
                    {
                        isfullfilled = false;
                        break;
                    }

                    #endregion
                }
                else if (req != null && req.IsTransfer)
                {
                    #region for transfer

                    var transferItems = db.ItemTransferItems.Where(x => x.ItemTransfer.StoreRequisitionID == reqid)
                        .Where(x => x.ItemCode == item.ItemCode)
                        .Where(x => x.ItemTransfer.IsOnlineApproved && !x.ItemTransfer.IsVoid).ToList();
                    if (item.ApprovedQuantity < transferItems.Sum(x => x.ShippedQuantity))
                    {
                        isfullfilled = false;
                        break;
                    }

                    #endregion
                }
            }
            return isfullfilled;
        }

        public static void CloseTransferSend()
        {
            ExceedDbContext db = new ExceedDbContext();
            var itemTransfers = db.ItemTransfers.Where(x => x.IsFullfilled == false).ToList();
            foreach (var transfer in itemTransfers)
            {
                //db = new ExceedDbContext();
                bool isfullfilled = IsTsFullFilled(transfer.StoreRequisitionID);
                if (isfullfilled)
                {
                    #region close req

                    transfer.IsFullfilled = true;
                    db.Entry(transfer).State = EntityState.Modified;
                    db.SaveChanges();

                    #endregion
                }
            }

        }

        //transfer send
        public static bool IsTsFullFilled(int transferid)
        {
            ExceedDbContext db = new ExceedDbContext();
            bool isfullfilled = true;
            var transferItems = db.ItemTransferItems.Where(x => x.ItemTransferID == transferid).ToList();
            foreach (var item in transferItems)
            {
                db = new ExceedDbContext();
                #region check receive

                var issueItems = db.ItemTransferItemReceive.Where(x => x.ItemTransferReceive.ItemTransferID ==transferid)
                    .Where(x => x.ItemCode == item.ItemCode)
                    .Where(x => x.ItemTransferReceive.IsOnlineApproved && !x.ItemTransferReceive.IsVoid).ToList();
                if (item.ShippedQuantity < issueItems.Sum(x => x.ReceivedQuantity))
                {
                    isfullfilled = false;
                    break;
                }

                #endregion
            }
            return isfullfilled;
        }
    }
}
