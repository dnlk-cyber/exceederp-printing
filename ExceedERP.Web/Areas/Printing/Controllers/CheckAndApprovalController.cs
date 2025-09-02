using ExceedERP.Core.Domain.Printing;
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
                var printingFollowup = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == model.PrintingProductionFollowUpId);
                if (printingFollowup == null)
                {
                    response = new { Success = false, Message = "Something's Wrong!" };
                }
                var tasks = db.PrintingProductionFollowUpTasks
                        .Where(x => x.PrintingProductionFollowUpId == printingFollowup.PrintingProductionFollowUpId)
                        .AsNoTracking()
                        .ToList();
                if (model.PressCategory == PressCategory.PrePress)
                {

                    //printingFollowup.PrePressProgress == 0 ? 0 : (100 * tasks.Count)
                }
                else if (model.PressCategory == PressCategory.Press)
                {
                    //printingFollowup.PressProgress
                }
                else
                {
                    //printingFollowup.PostPressProgress
                }

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
                model.IsPrePressChecked = false;
                model.IsPrePressApproved = false;
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
                model.IsPressChecked = false;
                model.IsPressApproved = false;
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

        #region PostPress
        public JsonResult CheckPostPress(int id)
        {
            Object response = null;
            var model = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if (model != null)
            {
                model.IsPostPressChecked = true;
                model.PostPressCheckedBy = User.Identity.Name;
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
                model.IsPostPressChecked = false;
                model.IsPostPressApproved = false;
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