using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {
      
        // GET: CMD

        public ActionResult GenIssuePeriod()
        {
            var records = db.Issues.Where(x => x.GLPeriodId == 0).ToList();
            foreach (var r in records)
            {
                var period = db.InventoryPeriods.FirstOrDefault(x => x.DateFrom <= r.IssueDate && x.DateTo >= r.IssueDate);
                if (period != null)
                {
                    r.GLPeriodId = period.InventoryPeriodId;
                    db.Entry(r).State = EntityState.Modified;
                   
                }
            }
            db.SaveChanges();
            return View("Success");
        }
        public ActionResult GenGrnPeriod()
        {
            var records = db.GoodReceives.Where(x => x.GLPeriodId == 0).ToList();
            foreach (var r in records)
            {
                var period = db.InventoryPeriods.FirstOrDefault(x => x.DateFrom <= r.ReceiveDate && x.DateTo >= r.ReceiveDate);
                if (period != null)
                {
                    r.GLPeriodId = period.InventoryPeriodId;
                    db.Entry(r).State = EntityState.Modified;

                }
            }
            db.SaveChanges();
            return View("Success");
        }
        public ActionResult GenReturnPeriod()
        {
            var records = db.StoreReturns.Where(x => x.GLPeriodId == 0).ToList();
            foreach (var r in records)
            {
                var period = db.InventoryPeriods.FirstOrDefault(x => x.DateFrom <= r.ReturnDate && x.DateTo >= r.ReturnDate);
                if (period != null)
                {
                    r.GLPeriodId = period.InventoryPeriodId;
                    db.Entry(r).State = EntityState.Modified;

                }
            }
            db.SaveChanges();
            return View("Success");
        }
        public ActionResult GenTransferSendPeriod()
        {
            var records = db.ItemTransfers.Where(x => x.GLPeriodId == 0).ToList();
            foreach (var r in records)
            {
                var period = db.InventoryPeriods.FirstOrDefault(x => x.DateFrom <= r.TransferDate && x.DateTo >= r.TransferDate);
                if (period != null)
                {
                    r.GLPeriodId = period.InventoryPeriodId;
                    db.Entry(r).State = EntityState.Modified;

                }
            }
            db.SaveChanges();
            return View("Success");
        }
        public ActionResult GenTransferReceivePeriod()
        {
            var records = db.ItemTransferReceive.Where(x => x.GLPeriodId == 0).ToList();
            foreach (var r in records)
            {
                var period = db.InventoryPeriods.FirstOrDefault(x => x.DateFrom <= r.ReceiveDate && x.DateTo >= r.ReceiveDate);
                if (period != null)
                {
                    r.GLPeriodId = period.InventoryPeriodId;
                    db.Entry(r).State = EntityState.Modified;

                }
            }
            db.SaveChanges();
            return View("Success");
        }
    }
}