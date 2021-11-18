using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
   public class FinishedGoodReceivalODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FinishedGoodReceivalVM> GetFinishedItems(string id)
        {
            List<FinishedGoodReceivalVM> finishedReceivalList = new List<FinishedGoodReceivalVM>();
            if(id != null)
            {
                int.TryParse(id,out int finishedReceivalId);
                var finishedReceivals = db.PrintingProductionFinishedGoodsStoreReceives.FirstOrDefault(x=>x.FinishedGoodsStoreReceiveId == finishedReceivalId);
                if(finishedReceivals != null)
                {
                    var receival = new FinishedGoodReceivalVM();

                    receival.Store = finishedReceivals.StoreCode;
                    receival.VoucherNo = finishedReceivals.VoucherNo;
                    receival.DeliveredBy = finishedReceivals.DeliveredBy;
                    receival.ReceivedBy = finishedReceivals.ReceivedBy;
                    receival.Supervisor = finishedReceivals.Supervisor;

                    var job = db.Jobs.FirstOrDefault(x=>x.JobId == finishedReceivals.JobId);
                    if(job != null)
                    {
                        receival.JobOrderNo = job.JobNo;
                        var CustomerName = db.OrganizationCustomers.Find(job.CustomerId)?.TradeName;
                        receival.CustomerName = CustomerName;
                    }

                    receival.FinishedGoods = new List<FinishedGoodsVM>();

                    var receivalItems = db.FinishedGoods.Where(x=>x.FinishedGoodReceivingId == finishedReceivals.FinishedGoodsStoreReceiveId).ToList();
                    foreach(var item in receivalItems)
                    {
                        var finished = new FinishedGoodsVM();

                        finished.CodeNo = item.CodeNo;
                        finished.Description = item.Description;
                        finished.Unit = item.Unit;
                        finished.Quantity = item.Quantity;

                        receival.FinishedGoods.Add(finished);
                    }

                    finishedReceivalList.Add(receival);
                }
            }

           return finishedReceivalList;
        }
    }
}
