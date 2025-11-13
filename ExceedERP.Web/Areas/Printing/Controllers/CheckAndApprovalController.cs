using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Printing;
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
        private void UpdateStatus(int id, PrintingOperationStatus printingOperationStatus)
        {
            var entity = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            entity.PrintingOperationStatus = printingOperationStatus;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
        public JsonResult ApproveFollowupTask(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUpTasks.Find(id);
            if (model != null)
            {
                //var printingFollowup = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == model.PrintingProductionFollowUpId);
                //if (printingFollowup == null)
                //{
                //    response = new { Success = false, Message = "Something's Wrong!" };
                //}
                //var tasks = db.PrintingProductionFollowUpTasks
                //        .Where(x => x.PrintingProductionFollowUpId == printingFollowup.PrintingProductionFollowUpId)
                //        .AsNoTracking()
                //        .ToList();
                //if (model.PressCategory == PressCategory.PrePress)
                //{

                //    //printingFollowup.PrePressProgress == 0 ? 0 : (100 * tasks.Count)
                //}
                //else if (model.PressCategory == PressCategory.Press)
                //{
                //    //printingFollowup.PressProgress
                //}
                //else
                //{
                //    //printingFollowup.PostPressProgress
                //}

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
        public JsonResult RejectFollowupTask(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUpTasks.Find(id);
            if (model != null)
            {
                model.IsOnlineApproved = false;
                model.IsSendForApproval = false;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Rejected" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        #endregion

        #region PrePress
        public JsonResult CheckPrePress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if (model != null)
            {
                model.IsPrePressChecked = true;
                model.PrePressCheckedBy = User.Identity.Name;
                model.Status = "Checked";
                model.PrePressCheckedTime = DateTime.Today;
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

        public JsonResult ApprovePrePress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsPrePressApproved = true;
                model.Status = "Approved";
                model.PrintingOperationStatus = PrintingOperationStatus.PrePressCompleted;
                model.PrePressApprovedBy = User.Identity.Name;
                model.PrePressApprovedTime = DateTime.Now;
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

        public JsonResult RejectPrePress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsVoid = true;
                model.IsPrePressChecked = false;
                model.IsPrePressApproved = false;
                model.Status = "Rejected";
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Rejected!" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        #endregion

        #region Press
        public JsonResult CheckPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if (model != null)
            {
                model.IsPressChecked = true;
                model.PressCheckedBy = User.Identity.Name;
                model.PressCheckedTime = DateTime.Today;
                model.Status = "Checked";
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

        public JsonResult ApprovePress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsPressApproved = true;
                model.Status = "Approved";
                model.PrintingOperationStatus = PrintingOperationStatus.PressCompleted;
                model.PressApprovedBy = User.Identity.Name;
                model.PressApprovedTime = DateTime.Now;
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

        public JsonResult RejectPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsVoid = true;
                model.IsPressChecked = false;
                model.IsPressApproved = false;
                model.Status = "Rejected";
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Rejected" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        #endregion

        #region PostPress
        public JsonResult CheckPostPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if (model != null)
            {
                model.IsPostPressChecked = true;
                model.PostPressCheckedBy = User.Identity.Name;
                model.Status = "Checked";
                model.PostPressCheckedTime = DateTime.Today;
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

        public JsonResult ApprovePostPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsPostPressApproved = true;
                model.Status = "Approved";
                model.PrintingOperationStatus = PrintingOperationStatus.PostPressCompleted;
                model.PostPressApprovedBy = User.Identity.Name;
                model.PostPressApprovedTime = DateTime.Now;
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

        public JsonResult RejectPostPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsVoid = true;
                model.IsPostPressChecked = false;
                model.IsPostPressApproved = false;
              
                model.Status = "Rejected";
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Rejected" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }

        public JsonResult RevertProcess(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model != null)
            {
                model.IsVoid = true;
                model.IsPostPressChecked = false;
                model.IsPostPressApproved = false;
                model.IsPressApproved = false;
                model.IsPressChecked = false;
                model.Status = "Process Reverted!";
                model.PrintingOperationStatus = PrintingOperationStatus.Reverted;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Reverted" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!, Not Found!" };
            }
            return Json(response);
        }
        public JsonResult CloseJob(int id)
        {
            var model = db.PrintingProductionFollowUps.Find(id);
            if (model == null)
            {
                return Json(new { Success = false, Message = "Job not found." });
            }

            var result = PrintCloseJobHelper.GenerateItems(model, User.Identity.Name);
            if (!result.Success)
            {
                return Json(new { Success = false, Message = result.Message });
            }

            model.IsClosed = true;
            model.Status = "Closed";
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            var jobOrderItem = db.PrintingJobOrderItems.FirstOrDefault(x => x.PrintingJobOrderItemId == model.PrintingJobOrderItemId);
            jobOrderItem.IsClosed = true;
            db.Entry(jobOrderItem).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Success = true, Message = "Job closed successfully." });
        }

        #endregion

    }
}