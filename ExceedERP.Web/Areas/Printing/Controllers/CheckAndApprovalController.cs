using ExceedERP.DataAccess.Context;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class CheckAndApprovalController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/CheckAndApproval
        public ActionResult Index()
        {
            return View();
        }

        #region Followup Task
        public JsonResult CheckFollowupTask(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUpTasks.FirstOrDefault(x => x.PrintingProductionFollowUpTaskId == id);
            if (model != null)
            {
                model.IsSendForApproval = true;
                model.SendForApprovalBy = User.Identity.Name;
                model.SendForApprovalTime = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Checked" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveFollowupTask(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUpTasks.Find(id);
            if (model != null)
            {
                model.IsOnlineApproved = true;
                model.OnlineApprovedBy = User.Identity.Name;
                model.OnlineApprovedTime = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();            
                response = new { Success = true, Message = "Successfully Approved" };               
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        #endregion

        #region MyRegion
        public JsonResult CheckFollowup(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if (model != null)
            {
                model.IsSendForApproval = true;
                model.SendForApprovalBy = User.Identity.Name;
                model.SendForApprovalTime = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Checked" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveFollowup(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsOnlineApproved = true;
                model.OnlineApprovedBy = User.Identity.Name;
                model.OnlineApprovedTime = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Approved" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        #endregion
    }
}