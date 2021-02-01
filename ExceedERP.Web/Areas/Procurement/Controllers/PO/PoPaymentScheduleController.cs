using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    public class PoPaymentScheduleController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult PoPaymentSchedules_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<PoPaymentSchedule> poPaymentSchedules = _db.PoPaymentSchedules.Where(x=>x.PurchaseOrderID==id);
            DataSourceResult result = poPaymentSchedules.ToDataSourceResult(request, poPaymentSchedule => new {
                PoPaymentScheduleId = poPaymentSchedule.PoPaymentScheduleId,
                PurchaseOrderID = poPaymentSchedule.PurchaseOrderID,
                PaymentType = poPaymentSchedule.PaymentType,
                Percentage = poPaymentSchedule.Percentage,
                Remark = poPaymentSchedule.Remark,
                OrgBranchName = poPaymentSchedule.OrgBranchName,
                DateCreated = poPaymentSchedule.DateCreated,
                LastModified = poPaymentSchedule.LastModified,
                CreatedBy = poPaymentSchedule.CreatedBy,
                ModifiedBy = poPaymentSchedule.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPaymentSchedules_Create(int id,[DataSourceRequest]DataSourceRequest request, PoPaymentSchedule poPaymentSchedule)
        {
            #region validation

            decimal prev = 0;

            var previous = _db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == id).ToList();
            
            if(previous.Any())
               prev= previous.Sum(x => x.Percentage);

            if (prev + poPaymentSchedule.Percentage > 100)
            {
                ModelState.AddModelError("above100","The total percentage will be above 100,please check again. ");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PoPaymentSchedule
                {
                    PurchaseOrderID =id,
                    PaymentType = poPaymentSchedule.PaymentType,
                    Percentage = poPaymentSchedule.Percentage,
                    Remark = poPaymentSchedule.Remark,
                    OrgBranchName = poPaymentSchedule.OrgBranchName,
                    DateCreated = poPaymentSchedule.DateCreated,
                    LastModified = poPaymentSchedule.LastModified,
                    CreatedBy = poPaymentSchedule.CreatedBy,
                    ModifiedBy = poPaymentSchedule.ModifiedBy
                };

                _db.PoPaymentSchedules.Add(entity);
                _db.SaveChanges();
                poPaymentSchedule.PoPaymentScheduleId = entity.PoPaymentScheduleId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order payment schedule", "Success", User.Identity.Name, $"created schedule {entity.PoPaymentScheduleId} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPaymentSchedules_Update(int id,[DataSourceRequest]DataSourceRequest request, PoPaymentSchedule poPaymentSchedule)
        {
            #region validation

            decimal prev = 0;

            var previous = _db.PoPaymentSchedules.Where(x => x.PoPaymentScheduleId != poPaymentSchedule.PoPaymentScheduleId)
                                                      .Where(x => x.PurchaseOrderID == poPaymentSchedule.PurchaseOrderID).ToList();

            if (previous.Any())
                prev = previous.Sum(x => x.Percentage);

            if (prev + poPaymentSchedule.Percentage > 100)
            {
                ModelState.AddModelError("above100", "The total percentage will be above 100,please check again. ");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PoPaymentSchedule
                {
                    PurchaseOrderID = poPaymentSchedule.PurchaseOrderID,
                    PoPaymentScheduleId = poPaymentSchedule.PoPaymentScheduleId,
                    PaymentType = poPaymentSchedule.PaymentType,
                    Percentage = poPaymentSchedule.Percentage,
                    Remark = poPaymentSchedule.Remark,
                    OrgBranchName = poPaymentSchedule.OrgBranchName,
                    DateCreated = poPaymentSchedule.DateCreated,
                    LastModified = poPaymentSchedule.LastModified,
                    CreatedBy = poPaymentSchedule.CreatedBy,
                    ModifiedBy = poPaymentSchedule.ModifiedBy
                };

                _db.PoPaymentSchedules.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order payment schedule", "Success", User.Identity.Name, $"updated schedule {entity.PoPaymentScheduleId} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPaymentSchedules_Destroy([DataSourceRequest]DataSourceRequest request, PoPaymentSchedule poPaymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoPaymentSchedule
                {
                    PoPaymentScheduleId = poPaymentSchedule.PoPaymentScheduleId,
                    PaymentType = poPaymentSchedule.PaymentType,
                    Percentage = poPaymentSchedule.Percentage,
                    Remark = poPaymentSchedule.Remark,
                    OrgBranchName = poPaymentSchedule.OrgBranchName,
                    DateCreated = poPaymentSchedule.DateCreated,
                    LastModified = poPaymentSchedule.LastModified,
                    CreatedBy = poPaymentSchedule.CreatedBy,
                    ModifiedBy = poPaymentSchedule.ModifiedBy
                };

                _db.PoPaymentSchedules.Attach(entity);
                _db.PoPaymentSchedules.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { poPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
