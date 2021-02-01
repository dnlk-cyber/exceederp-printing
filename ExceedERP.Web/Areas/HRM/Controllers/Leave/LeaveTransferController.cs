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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
    public class LeaveTransferController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }

        public ActionResult TransferLeave(int period, int toPeriod)
        {
            string messages = "";
            if (period == toPeriod)
            {
                messages = "From and To period values are the same. please check them!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }
            var leaveEntitle = db.LeavesEntitle.Where(x => x.GlFiscalYearId == period).ToList();
            var paid = db.LeavePaid.Where(x => x.GlFiscalYearId == period).ToList();
            var taken = db.Leaves.Where(x => x.GlFiscalYearId == period && x.LeaveType == LeaveType.ANNUAL).ToList();
            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            LeaveTransfer transfer = new LeaveTransfer();
            List<LeaveTransfer> transferList = new List<LeaveTransfer>();
            db = new ExceedDbContext();
            var transfredLeave = db.LeaveTransfer.Where(x => x.From == period && x.To == toPeriod).ToList();
            if (leaveEntitle.Any())
            {

                foreach (var l in transfredLeave)
                {
                    //db = new ExceedDbContext();
                    db.LeaveTransfer.Attach(l);
                    db.LeaveTransfer.Remove(l);
                    db.SaveChanges();
                }
                foreach (var emp in employees)
                {
                    var e = leaveEntitle.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();

                    var pay = Convert.ToDecimal(paid.Where(x => x.EmployeeId == emp.EmployeeId).Sum(x => x.Amount));
                    var take = Convert.ToDecimal(taken.Where(x => x.EmployeeId == emp.EmployeeId).Sum(x => x.Amount));
                    transfer = new LeaveTransfer();

                    transfer.From = period;
                    transfer.To = toPeriod;
                    if (e != null)
                    {
                        transfer.Amount = e.Amount - (pay + take);
                    }
                    else
                    {
                        transfer.Amount = pay + take;
                    }
                    transfer.DateCreated = DateTime.Now;
                    transfer.LastModified = null;
                    transfer.CreatedBy = User.Identity.Name;
                    transfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    transfer.ModifiedBy = null;
                    transfer.EmployeeId = emp.EmployeeId;
                    transfer.EmployeeCode = emp.EmployeeCode;
                    transferList.Add(transfer);



                }
                db = new ExceedDbContext();
                db.LeaveTransfer.AddRange(transferList);

                db.SaveChanges();
                //messages = " Successfully Transferred!!";
                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                return null;
            }
            else
            {
                messages = "There is no entitled amount under selected Leave period!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }


        }
        public ActionResult SendForApproval()
        {
            var status =
                      db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
                        return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        public ActionResult LeaveTransfer_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<LeaveTransfer> leavetransfer = db.LeaveTransfer.Where(x => employees.Contains(x.EmployeeId.ToString())&& !x.Check );
            DataSourceResult result = leavetransfer.ToDataSourceResult(request, leaveTransfer => new
            {
                LeaveTransferId = leaveTransfer.LeaveTransferId,
                EmployeeId = leaveTransfer.EmployeeId,
                From = leaveTransfer.From,
                To = leaveTransfer.To,
                Amount = leaveTransfer.Amount,
                Remark = leaveTransfer.Remark,
                DateCreated = leaveTransfer.DateCreated,
                LastModified = leaveTransfer.LastModified,
                CreatedBy = leaveTransfer.CreatedBy,
                ModifiedBy = leaveTransfer.ModifiedBy,
                EmployeeCode = leaveTransfer.EmployeeCode,
                Check = leaveTransfer.Check,
                OrgBranchName = leaveTransfer.OrgBranchName,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<LeaveTransfer> products)
        {
            var results = new List<LeaveTransfer>();
            LeaveTransfer leaveTransfer = new LeaveTransfer();
            ExceedDbContext dbs = new ExceedDbContext();






            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    var req = new ExceedDbContext();
                    var list = req.RequestesStatus.Where(x => x.RequestId == product.LeaveTransferId && x.ReferenceType == "LeaveTransfer").ToList();
                    if (list.Any())
                    {
                        req.RequestesStatus.RemoveRange(list);
                        req.SaveChanges();
                    }

                    db = new ExceedDbContext();
                    var leave = db.LeaveTransfer.Where(x => x.LeaveTransferId == product.LeaveTransferId).FirstOrDefault();
                    leaveTransfer = new LeaveTransfer();
                    leaveTransfer = leave;
                    if (leave != null)
                    {
                        db.LeaveTransfer.Remove(leave);
                        db.SaveChanges();
                    }
                    db = new ExceedDbContext();
                    leaveTransfer.Amount = product.Amount;
                    leaveTransfer.ModifiedBy = User.Identity.Name;
                    leaveTransfer.LastModified = DateTime.Now;
                    leaveTransfer.Check = product.Check;
                    leaveTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    db.LeaveTransfer.Add(leaveTransfer);


                    db.SaveChanges();
                    leave.LeaveTransferId = leaveTransfer.LeaveTransferId;

                    var entity = new RequestesStatus();
                    req = new ExceedDbContext();
                    entity.RequestId = leave.LeaveTransferId;
                    entity.ReferenceType = "LeaveTransfer";
                    if (product.Check)
                    {
                        entity.Status = RequestStatus.APPROVE;
                    }
                    else
                    {
                        entity.Status = RequestStatus.WAITING_FOR_APPROVAL;
                    }
                    entity.Date = DateTime.Now;
                    entity.Remark = "";
                    req.RequestesStatus.Add(entity);
                    req.SaveChanges();
                }
            }


            return Json(results.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTransfer_Create([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeaveTransfer
                {
                    EmployeeId = leaveTransfer.EmployeeId,
                    From = leaveTransfer.From,
                    To = leaveTransfer.To,
                    Amount = leaveTransfer.Amount,
                    Remark = leaveTransfer.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = leaveTransfer.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = leaveTransfer.ModifiedBy,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leaveTransfer.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.LeaveTransfer.Add(entity);
                db.SaveChanges();
                leaveTransfer.LeaveTransferId = entity.LeaveTransferId;
            }

            return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTransfer_Update([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeaveTransfer
                {
                    LeaveTransferId = leaveTransfer.LeaveTransferId,
                    EmployeeId = leaveTransfer.EmployeeId,
                    From = leaveTransfer.From,
                    To = leaveTransfer.To,
                    Amount = leaveTransfer.Amount,
                    Remark = leaveTransfer.Remark,
                    DateCreated = leaveTransfer.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = leaveTransfer.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leaveTransfer.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = leaveTransfer.OrgBranchName,
                };

                db.LeaveTransfer.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTransfer_Destroy([DataSourceRequest]DataSourceRequest request, LeaveTransfer leaveTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeaveTransfer
                {
                    LeaveTransferId = leaveTransfer.LeaveTransferId,
                    EmployeeId = leaveTransfer.EmployeeId,
                    From = leaveTransfer.From,
                    To = leaveTransfer.To,
                    Amount = leaveTransfer.Amount,
                    Remark = leaveTransfer.Remark,
                    DateCreated = leaveTransfer.DateCreated,
                    LastModified = leaveTransfer.LastModified,
                    CreatedBy = leaveTransfer.CreatedBy,
                    ModifiedBy = leaveTransfer.ModifiedBy,
                    EmployeeCode = leaveTransfer.EmployeeCode,
                    OrgBranchName = leaveTransfer.OrgBranchName,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave transfer " + leaveTransfer.EmployeeCode);
                #endregion
                db.LeaveTransfer.Attach(entity);
                db.LeaveTransfer.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leaveTransfer }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave transfer");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave transfer");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
