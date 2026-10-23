using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    public class TravelOrderBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
       // private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult ApTravelOrderBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderBudgetValidation> ApTravelOrderBudgetValidations = _db.ApTravelOrderBudgetValidations.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = ApTravelOrderBudgetValidations.ToDataSourceResult(request, ApTravelOrderBudgetValidation => new {
                ApTravelOrderBudgetValidationId = ApTravelOrderBudgetValidation.ApTravelOrderBudgetValidationId,
                ApTravelOrderId=id,
                Status = ApTravelOrderBudgetValidation.Status,
                Remark = ApTravelOrderBudgetValidation.Remark,
                OrgBranchName = ApTravelOrderBudgetValidation.OrgBranchName,
                DateCreated = ApTravelOrderBudgetValidation.DateCreated,
                LastModified = ApTravelOrderBudgetValidation.LastModified,
                CreatedBy = ApTravelOrderBudgetValidation.CreatedBy,
                ModifiedBy = ApTravelOrderBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderBudgetValidation ApTravelOrderBudgetValidation)
        {
            var record = _db.ApTravelOrders.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");

            if (record != null && !record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("notdepapproved", "The record should be approved by requestor department first.");

            var lines = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add departure lines");
          
          

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (ApTravelOrderBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        #region approval notification
                       
                        #region notify creator
                        NotifModels notif1 = new NotifModels
                        {
                            UserId = record.CreatedBy,
                            Title = "Approval of Travel request at Budget",
                            Message = $"Travel payment document {record.ApTravelOrderId} is Approved by User: {User.Identity.Name} at {DateTime.Now}",
                            URL = "/Finance/TravelOrderRequest/"
                        };

                        Notify.NotifyUser(notif1);
                        #endregion
                        #region notify requestor
                        NotifModels notif2 = new NotifModels
                        {
                            UserId = UserInformationHelper.GetEmployeeUserId(record.EmployeeId),
                            Title = "Approval of Travel request at Budget",
                            Message = $"Travel payment document {record.ApTravelOrderId} is Approved by User: {User.Identity.Name} at {DateTime.Now}",
                            URL = "/Finance/TravelOrderRequest/"
                        };

                        Notify.NotifyUser(notif2);
                        #endregion

                        #endregion

                    }

                    if (ApTravelOrderBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        #region approval notification

                        #region notify creator
                        NotifModels notif1 = new NotifModels
                        {
                            UserId = record.CreatedBy,
                            Title = "Approval of Travel request at Budget",
                            Message = $"Travel payment document {record.ApTravelOrderId} is rejected by User: {User.Identity.Name} at {DateTime.Now}",
                            URL = "/Finance/TravelOrderRequest/"
                        };

                        Notify.NotifyUser(notif1);
                        #endregion
                        #region notify requestor
                        NotifModels notif2 = new NotifModels
                        {
                            UserId = UserInformationHelper.GetEmployeeUserId(record.EmployeeId),
                            Title = "Approval of Travel request at Budget",
                            Message = $"Travel payment document {record.ApTravelOrderId} is rejected by User: {User.Identity.Name} at {DateTime.Now}",
                            URL = "/Finance/TravelOrderRequest/"
                        };

                        Notify.NotifyUser(notif2);
                        #endregion

                        #endregion
                    }

                    var entity = new ApTravelOrderBudgetValidation
                    {
                        ApTravelOrderId = id,
                        Status = ApTravelOrderBudgetValidation.Status,
                        Remark = ApTravelOrderBudgetValidation.Remark,
                        OrgBranchName = ApTravelOrderBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ApTravelOrderBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ApTravelOrderBudgetValidation.ModifiedBy
                    };

                    _db.ApTravelOrderBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    ApTravelOrderBudgetValidation.ApTravelOrderBudgetValidationId = entity.ApTravelOrderBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request budget approval", "Success", User.Identity.Name, $"approved request {entity.ApTravelOrderId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { ApTravelOrderBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderBudgetValidation ApTravelOrderBudgetValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PaymentRequests.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (ApTravelOrderBudgetValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (ApTravelOrderBudgetValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new ApTravelOrderBudgetValidation
                    {
                        ApTravelOrderId = id,
                        ApTravelOrderBudgetValidationId = ApTravelOrderBudgetValidation.ApTravelOrderBudgetValidationId,
                        Status = ApTravelOrderBudgetValidation.Status,
                        Remark = ApTravelOrderBudgetValidation.Remark,
                        OrgBranchName = ApTravelOrderBudgetValidation.OrgBranchName,
                        DateCreated = ApTravelOrderBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = ApTravelOrderBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApTravelOrderBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { ApTravelOrderBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
