using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractPaymentScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult ContractPaymentSchedules_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ContractPaymentSchedule> contractpaymentschedules = db.ContractPaymentSchedules.Where(x=>x.ContractManagementID==id);
            DataSourceResult result = contractpaymentschedules.ToDataSourceResult(request, contractPaymentSchedule => new {
                ContractPaymentScheduleId = contractPaymentSchedule.ContractPaymentScheduleId,
                ContractManagementID = contractPaymentSchedule.ContractManagementID,
                PaymentType = contractPaymentSchedule.PaymentType,
                Percentage = contractPaymentSchedule.Percentage,
                Remark = contractPaymentSchedule.Remark,
                OrgBranchName = contractPaymentSchedule.OrgBranchName,
                DateCreated = contractPaymentSchedule.DateCreated,
                LastModified = contractPaymentSchedule.LastModified,
                CreatedBy = contractPaymentSchedule.CreatedBy,
                ModifiedBy = contractPaymentSchedule.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentSchedules_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractPaymentSchedule contractPaymentSchedule)
        {
            #region validation

            decimal prev = 0;

            var previous = db.ContractPaymentSchedules.Where(x => x.ContractManagementID == id).ToList();
            
            if(previous.Any())
               prev= previous.Sum(x => x.Percentage);

            if (prev + contractPaymentSchedule.Percentage > 100)
            {
                ModelState.AddModelError("above100","The total percentage will be above 100,please check again. ");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentSchedule
                {
                    ContractManagementID =id,
                    PaymentType = contractPaymentSchedule.PaymentType,
                    Percentage = contractPaymentSchedule.Percentage,
                    Remark = contractPaymentSchedule.Remark,
                    OrgBranchName = contractPaymentSchedule.OrgBranchName,
                    DateCreated = contractPaymentSchedule.DateCreated,
                    LastModified = contractPaymentSchedule.LastModified,
                    CreatedBy = contractPaymentSchedule.CreatedBy,
                    ModifiedBy = contractPaymentSchedule.ModifiedBy
                };

                db.ContractPaymentSchedules.Add(entity);
                db.SaveChanges();
                contractPaymentSchedule.ContractPaymentScheduleId = entity.ContractPaymentScheduleId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment schedule", "Success", User.Identity.Name, $"created contract schedule {entity.Percentage} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentSchedules_Update(int id,[DataSourceRequest]DataSourceRequest request, ContractPaymentSchedule contractPaymentSchedule)
        {
            #region validation

            decimal prev = 0;

            var previous = db.ContractPaymentSchedules.Where(x => x.ContractPaymentScheduleId != contractPaymentSchedule.ContractPaymentScheduleId)
                                                      .Where(x => x.ContractManagementID == contractPaymentSchedule.ContractManagementID).ToList();

            if (previous.Any())
                prev = previous.Sum(x => x.Percentage);

            if (prev + contractPaymentSchedule.Percentage > 100)
            {
                ModelState.AddModelError("above100", "The total percentage will be above 100,please check again. ");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentSchedule
                {
                    ContractManagementID = contractPaymentSchedule.ContractManagementID,
                    ContractPaymentScheduleId = contractPaymentSchedule.ContractPaymentScheduleId,
                    PaymentType = contractPaymentSchedule.PaymentType,
                    Percentage = contractPaymentSchedule.Percentage,
                    Remark = contractPaymentSchedule.Remark,
                    OrgBranchName = contractPaymentSchedule.OrgBranchName,
                    DateCreated = contractPaymentSchedule.DateCreated,
                    LastModified = contractPaymentSchedule.LastModified,
                    CreatedBy = contractPaymentSchedule.CreatedBy,
                    ModifiedBy = contractPaymentSchedule.ModifiedBy
                };

                db.ContractPaymentSchedules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment schedule", "Success", User.Identity.Name, $"updated contract schedule {entity.Percentage} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentSchedules_Destroy([DataSourceRequest]DataSourceRequest request, ContractPaymentSchedule contractPaymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentSchedule
                {
                    ContractPaymentScheduleId = contractPaymentSchedule.ContractPaymentScheduleId,
                    PaymentType = contractPaymentSchedule.PaymentType,
                    Percentage = contractPaymentSchedule.Percentage,
                    Remark = contractPaymentSchedule.Remark,
                    OrgBranchName = contractPaymentSchedule.OrgBranchName,
                    DateCreated = contractPaymentSchedule.DateCreated,
                    LastModified = contractPaymentSchedule.LastModified,
                    CreatedBy = contractPaymentSchedule.CreatedBy,
                    ModifiedBy = contractPaymentSchedule.ModifiedBy
                };

                db.ContractPaymentSchedules.Attach(entity);
                db.ContractPaymentSchedules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
