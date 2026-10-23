using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.RentalPayment
{
    public class RentalPaymentRequestBudgetValidationController: Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        //[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult Index()
        {
            return View();
        }

        //[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]


        //[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        public ActionResult RentalPaymentRequestDepartmentValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RentalPaymentRequestBudgetValidation> rentalPaymentRequestDepartmentValidations =_db.RentalPaymentRequestBudgetValidations.Where(x => x.RentalPaymentRequestId == id);
            DataSourceResult result = rentalPaymentRequestDepartmentValidations.ToDataSourceResult(request, rentalPaymentRequestDepartmentValidation => new
            {
                RentalPaymentRequestBudgetValidationId = rentalPaymentRequestDepartmentValidation.RentalPaymentRequestBudgetValidationId,
                PaymentRequestId = id,
                Status = rentalPaymentRequestDepartmentValidation.Status,
                Remark = rentalPaymentRequestDepartmentValidation.Remark,
                OrgBranchName = rentalPaymentRequestDepartmentValidation.OrgBranchName,
                DateCreated = rentalPaymentRequestDepartmentValidation.DateCreated,
                LastModified = rentalPaymentRequestDepartmentValidation.LastModified,
                CreatedBy = rentalPaymentRequestDepartmentValidation.CreatedBy,
                ModifiedBy = rentalPaymentRequestDepartmentValidation.ModifiedBy
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestBudgetValidations_Create([DataSourceRequest]DataSourceRequest request, RentalPaymentRequestBudgetValidation rentalPaymentRequestBudgetValidation, int id)
        {
            var record = _db.RentalPaymentRequests.Find(id);
            if (record != null && !record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("Not Fleet approved","cannot manipulate this record,it is not approved on department");

            var lines = _db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");

            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

           
            var hasApproved = _db.RentalPaymentRequestBudgetValidations
                    .Where(x => x.RentalPaymentRequestId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if(rentalPaymentRequestBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsBudgetApproved = true;
                            record.BudgetApprovedBy = User.Identity.Name;
                            record.BudgetApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsBudgetApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                          
                        }
                    }

                    if (rentalPaymentRequestBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.IsDepartmentOnlineApproved = false;
                        record.IsOnlineApproved = false;
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new RentalPaymentRequestBudgetValidation
                    {
                        RentalPaymentRequestId = id,
                        Status = rentalPaymentRequestBudgetValidation.Status,
                        Remark = rentalPaymentRequestBudgetValidation.Remark,
                        OrgBranchName = rentalPaymentRequestBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = rentalPaymentRequestBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = rentalPaymentRequestBudgetValidation.ModifiedBy
                    };

                    _db.RentalPaymentRequestBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    rentalPaymentRequestBudgetValidation.RentalPaymentRequestBudgetValidationId = entity.RentalPaymentRequestBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request department approval", "Success", User.Identity.Name, $"approved request {entity.RentalPaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { rentalPaymentRequestBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequestBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, RentalPaymentRequestBudgetValidation rentalPaymentRequestBudgetValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                if (isvalid)
                {
                    var entity = new RentalPaymentRequestBudgetValidation
                    {
                        RentalPaymentRequestId = id,
                        RentalPaymentRequestBudgetValidationId = rentalPaymentRequestBudgetValidation.RentalPaymentRequestBudgetValidationId,
                        Status = rentalPaymentRequestBudgetValidation.Status,
                        Remark = rentalPaymentRequestBudgetValidation.Remark,
                        OrgBranchName = rentalPaymentRequestBudgetValidation.OrgBranchName,
                        DateCreated = rentalPaymentRequestBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = rentalPaymentRequestBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.RentalPaymentRequestBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { rentalPaymentRequestBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
