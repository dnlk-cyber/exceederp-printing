using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
 
    [AuthorizeRoles(HrmConstants.HRLeaveApprover)]
    public class LeaveTransferStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var periodList = db.GLPeriods.AsQueryable();

            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GLPeriod>();
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        //public ActionResult TransferLeave(int period, int toPeriod)
        //{
        //    string messages = "";
        //    var leaveEntitle = db.LeavesEntitle.Where(x => x.GlFiscalYearId == period).ToList();
        //    LeaveTransfer transfer = new LeaveTransfer();
        //    List<LeaveTransfer> transferList = new List<LeaveTransfer>();
        //    db = new ExceedDbContext();
        //    var transfredLeave = db.LeaveTransfer.Where(x => x.From == period && x.To == toPeriod).ToList();
        //    if (leaveEntitle.Any())
        //    {

        //        foreach (var l in transfredLeave)
        //        {
        //            db = new ExceedDbContext();
        //            db.LeaveTransfer.Attach(l);
        //            db.LeaveTransfer.Remove(l);
        //            db.SaveChanges();
        //        }
        //        foreach (var e in leaveEntitle)
        //        {
        //            transfer = new LeaveTransfer();

        //            transfer.From = period;
        //            transfer.To = toPeriod;
        //            transfer.Amount = e.Amount;
        //            transfer.DateCreated = DateTime.Now;
        //            transfer.LastModified = null;
        //            transfer.CreatedBy = User.Identity.Name;
        //            transfer.ModifiedBy = null;
        //            transfer.EmployeeId = e.EmployeeId;
        //            transferList.Add(transfer);

        //        }
        //        db = new ExceedDbContext();
        //        db.LeaveTransfer.AddRange(transferList);

        //        db.SaveChanges();
        //    }
        //    messages = " Successfully Transferred!!";
        //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //}
        //public ActionResult LeaveTransfer_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<LeaveTransfer> leavetransfer = db.LeaveTransfer;
        //    DataSourceResult result = leavetransfer.ToDataSourceResult(request, leaveTransfer => new
        //    {
        //        LeaveTransferId = leaveTransfer.LeaveTransferId,
        //        EmployeeId = leaveTransfer.EmployeeId,
        //        From = leaveTransfer.From,
        //        To = leaveTransfer.To,
        //        Amount = leaveTransfer.Amount,
        //        Remark = leaveTransfer.Remark,
        //        DateCreated = leaveTransfer.DateCreated,
        //        LastModified = leaveTransfer.LastModified,
        //        CreatedBy = leaveTransfer.CreatedBy,
        //        ModifiedBy = leaveTransfer.ModifiedBy
        //    });

        //    return Json(result);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveTransfer_Create([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new LeaveTransfer
        //        {
        //            EmployeeId = leaveTransfer.EmployeeId,
        //            From = leaveTransfer.From,
        //            To = leaveTransfer.To,
        //            Amount = leaveTransfer.Amount,
        //            Remark = leaveTransfer.Remark,
        //            DateCreated = DateTime.Now,
        //            LastModified = leaveTransfer.LastModified,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = leaveTransfer.ModifiedBy
        //        };

        //        db.LeaveTransfer.Add(entity);
        //        db.SaveChanges();
        //        leaveTransfer.LeaveTransferId = entity.LeaveTransferId;
        //    }

        //    return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveTransfer_Update([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new LeaveTransfer
        //        {
        //            LeaveTransferId = leaveTransfer.LeaveTransferId,
        //            EmployeeId = leaveTransfer.EmployeeId,
        //            From = leaveTransfer.From,
        //            To = leaveTransfer.To,
        //            Amount = leaveTransfer.Amount,
        //            Remark = leaveTransfer.Remark,
        //            DateCreated = leaveTransfer.DateCreated,
        //            LastModified = DateTime.Now,
        //            CreatedBy = leaveTransfer.CreatedBy,
        //            ModifiedBy = User.Identity.Name
        //        };

        //        db.LeaveTransfer.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveTransfer_Destroy([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new LeaveTransfer
        //        {
        //            LeaveTransferId = leaveTransfer.LeaveTransferId,
        //            EmployeeId = leaveTransfer.EmployeeId,
        //            From = leaveTransfer.From,
        //            To = leaveTransfer.To,
        //            Amount = leaveTransfer.Amount,
        //            Remark = leaveTransfer.Remark,
        //            DateCreated = leaveTransfer.DateCreated,
        //            LastModified = leaveTransfer.LastModified,
        //            CreatedBy = leaveTransfer.CreatedBy,
        //            ModifiedBy = leaveTransfer.ModifiedBy
        //        };

        //        #region delete log
        //        var name = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == leaveTransfer.EmployeeId).FullNameaEnglish;
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Leave Transfer of" + name);
        //        #endregion

        //        db.LeaveTransfer.Attach(entity);
        //        db.LeaveTransfer.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
