using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class WorkInProgressDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult WorkInProgressLine_Read([DataSourceRequest]DataSourceRequest request, int wipId)
        {
            IQueryable<WorkInProgressDetail> workInProgressLines = db.WorkInProgressDetails.Where(wipLine => wipLine.WorkInProgressId == wipId);
            DataSourceResult result = workInProgressLines.ToDataSourceResult(request, progressDetail => new
            {
                WorkInProgressDetailId = progressDetail.WorkInProgressDetailId,
                WorkInProgressId = progressDetail.WorkInProgressId,
                Service = progressDetail.Service,
                WorkItemPerProjectId = progressDetail.WorkItemPerProjectId,
                Description = progressDetail.Description,
                AccountCode = progressDetail.AccountCode,
                Quantity = progressDetail.Quantity,
                UnitPrice = progressDetail.UnitPrice,
                SubTotal = progressDetail.SubTotal,
                GLTaxRateID = progressDetail.GLTaxRateID,
                TaxAmount = progressDetail.TaxAmount,
                TotalAmount = progressDetail.TotalAmount,


                OrgBranchName = progressDetail.OrgBranchName,
                DateCreated = progressDetail.DateCreated,
                LastModified = progressDetail.LastModified,
                CreatedBy = progressDetail.CreatedBy,
                ModifiedBy = progressDetail.ModifiedBy
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var progressDetail = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (progressDetail != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressLine_Create([DataSourceRequest]DataSourceRequest request, WorkInProgressDetail progressDetail, int wipIdCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new WorkInProgressDetail
                {
                    WorkInProgressDetailId = progressDetail.WorkInProgressDetailId,
                    WorkInProgressId = wipIdCreate,
                    WorkItemPerProjectId = progressDetail.WorkItemPerProjectId,
                    Service = progressDetail.Service,
                    Description = progressDetail.Description,
                    AccountCode = progressDetail.AccountCode,
                    Quantity = ProjectEstimationHelper.GetWorkItemTotalQuantityOfWork(progressDetail.WorkItemPerProjectId),
                    UnitPrice = ProjectEstimationHelper.GetUnitPriceOfProjectWorkItem(progressDetail.WorkItemPerProjectId),
                    SubTotal = progressDetail.SubTotal,
                    GLTaxRateID = progressDetail.GLTaxRateID,
                    TaxAmount = progressDetail.TaxAmount,
                    TotalAmount = progressDetail.TotalAmount,

                    OrgBranchName = progressDetail.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = progressDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = progressDetail.ModifiedBy
                };
                entity.SubTotal = entity.Quantity * entity.UnitPrice;
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                db.WorkInProgressDetails.Add(entity);
                db.SaveChanges();
                progressDetail.WorkInProgressDetailId = entity.WorkInProgressDetailId;

                progressDetail.SubTotal = entity.SubTotal;
                progressDetail.TaxAmount = entity.TaxAmount;
                progressDetail.TotalAmount = entity.TotalAmount;

                //}
                new ConstructionDistributionHelper().UpdateWorkInProgressDistribution(GetCurrentWorkInProgress(entity.WorkInProgressId));
            }

            return Json(new[] { progressDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressLine_Update([DataSourceRequest]DataSourceRequest request, WorkInProgressDetail progressDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgressDetail
                {
                    WorkInProgressDetailId = progressDetail.WorkInProgressDetailId,
                    WorkInProgressId = progressDetail.WorkInProgressId,
                    WorkItemPerProjectId = progressDetail.WorkItemPerProjectId,
                    Service = progressDetail.Service,
                    Description = progressDetail.Description,
                    AccountCode = progressDetail.AccountCode,
                    Quantity = ProjectEstimationHelper.GetWorkItemTotalQuantityOfWork(progressDetail.WorkItemPerProjectId),
                    UnitPrice = ProjectEstimationHelper.GetUnitPriceOfProjectWorkItem(progressDetail.WorkItemPerProjectId),
                    SubTotal = progressDetail.SubTotal,
                    GLTaxRateID = progressDetail.GLTaxRateID,
                    TaxAmount = progressDetail.TaxAmount,
                    TotalAmount = progressDetail.TotalAmount,

                    OrgBranchName = progressDetail.OrgBranchName,
                    DateCreated = progressDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = progressDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.SubTotal = entity.Quantity * entity.UnitPrice;
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                db.WorkInProgressDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                progressDetail.SubTotal = entity.SubTotal;
                progressDetail.TaxAmount = entity.TaxAmount;
                progressDetail.TotalAmount = entity.TotalAmount;

                new ConstructionDistributionHelper().UpdateWorkInProgressDistribution(GetCurrentWorkInProgress(entity.WorkInProgressId));

            }

            return Json(new[] { progressDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressLine_Destroy([DataSourceRequest]DataSourceRequest request, WorkInProgressDetail progressDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgressDetail
                {
                    WorkInProgressDetailId = progressDetail.WorkInProgressDetailId,
                    WorkInProgressId = progressDetail.WorkInProgressId,
                    WorkItemPerProjectId = progressDetail.WorkItemPerProjectId,
                    Service = progressDetail.Service,
                    Description = progressDetail.Description,
                    AccountCode = progressDetail.AccountCode,
                    Quantity = progressDetail.Quantity,
                    UnitPrice = progressDetail.UnitPrice,
                    SubTotal = progressDetail.SubTotal,
                    GLTaxRateID = progressDetail.GLTaxRateID,
                    TaxAmount = progressDetail.TaxAmount,
                    TotalAmount = progressDetail.TotalAmount,

                    OrgBranchName = progressDetail.OrgBranchName,
                    DateCreated = progressDetail.DateCreated,
                    LastModified = progressDetail.LastModified,
                    CreatedBy = progressDetail.CreatedBy,
                    ModifiedBy = progressDetail.ModifiedBy
                };

                db.WorkInProgressDetails.Attach(entity);
                db.WorkInProgressDetails.Remove(entity);
                db.SaveChanges();

                new ConstructionDistributionHelper().UpdateWorkInProgressDistribution(GetCurrentWorkInProgress(entity.WorkInProgressId));
            }

            return Json(new[] { progressDetail }.ToDataSourceResult(request, ModelState));
        }

        private WorkInProgress GetCurrentWorkInProgress(int id)
        {
            var wip = new WorkInProgress();
            var temp = db.WorkInProgress.Find(id);
            if (temp != null)
            {
                wip = temp;
            }
            return wip;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}