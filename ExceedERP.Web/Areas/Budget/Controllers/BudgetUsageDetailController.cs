using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetUsageDetailController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult BudgetUsageDetail_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<BudgetUsageDetail> BudgetUsageDetails = db.BudgetUsageDetails.Where(x=>x.BudgetUsageID==id);
            DataSourceResult result = BudgetUsageDetails.ToDataSourceResult(request, usageDetail => new
            {
                BudgetUsageDetailId = usageDetail.BudgetUsageDetailId,
                BudgetUsageID = usageDetail.BudgetUsageID,
                PlanCategoryId = usageDetail.PlanCategoryId,
                BranchId = usageDetail.BranchId,
                BusinessUnitId = usageDetail.BusinessUnitId,
                OrgStructureId = usageDetail.OrgStructureId,
                BudgetCode = usageDetail.BudgetCode,
                BudgetItem = usageDetail.BudgetItem,
                AccountCode = usageDetail.AccountCode,
                AccountDesc = usageDetail.AccountDesc,

                UsageAmount = usageDetail.UsageAmount,
                CostCenter = usageDetail.CostCenter,

                DateCreated = usageDetail.DateCreated,
                LastModified = usageDetail.LastModified,
                CreatedBy = usageDetail.CreatedBy,
                ModifiedBy = usageDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageDetail_Create([DataSourceRequest]DataSourceRequest request, BudgetUsageDetail usageDetail,int id)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetUsageDetail
                {
                    BudgetUsageID = id,
                    PlanCategoryId = usageDetail.PlanCategoryId,
                    BranchId = usageDetail.BranchId,
                    BusinessUnitId = usageDetail.BusinessUnitId,
                    OrgStructureId = usageDetail.OrgStructureId,
                    BudgetCode = usageDetail.BudgetCode,
                    BudgetItem = usageDetail.BudgetItem,
                    AccountCode = usageDetail.AccountCode,
                    AccountDesc = usageDetail.AccountDesc,
                    UsageAmount = usageDetail.UsageAmount,
                    CostCenter = usageDetail.CostCenter,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetUsageDetails.Add(entity);
                db.SaveChanges();
                usageDetail.BudgetUsageDetailId = entity.BudgetUsageDetailId;
            }


            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageDetail_Update([DataSourceRequest]DataSourceRequest request, BudgetUsageDetail usageDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsageDetail
                {
                    BudgetUsageDetailId = usageDetail.BudgetUsageDetailId,
                    BudgetUsageID = usageDetail.BudgetUsageID,
                    PlanCategoryId = usageDetail.PlanCategoryId,
                    BranchId = usageDetail.BranchId,
                    BusinessUnitId = usageDetail.BusinessUnitId,
                    OrgStructureId = usageDetail.OrgStructureId,
                    BudgetCode = usageDetail.BudgetCode,
                    BudgetItem = usageDetail.BudgetItem,
                    AccountCode = usageDetail.AccountCode,
                    AccountDesc = usageDetail.AccountDesc,

                    UsageAmount = usageDetail.UsageAmount,
                    CostCenter = usageDetail.CostCenter,

                    DateCreated =usageDetail.DateCreated,
                    CreatedBy = usageDetail.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetUsageDetails.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageDetail_Destroy([DataSourceRequest]DataSourceRequest request, BudgetUsageDetail usageDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsageDetail
                {
                    BudgetUsageDetailId = usageDetail.BudgetUsageDetailId,
                    BudgetUsageID = usageDetail.BudgetUsageID,
                    PlanCategoryId = usageDetail.PlanCategoryId,
                    BranchId = usageDetail.BranchId,
                    BusinessUnitId = usageDetail.BusinessUnitId,
                    OrgStructureId = usageDetail.OrgStructureId,
                    BudgetCode = usageDetail.BudgetCode,
                    BudgetItem = usageDetail.BudgetItem,
                    AccountCode = usageDetail.AccountCode,
                    AccountDesc = usageDetail.AccountDesc,
                   
                };

                db.BudgetUsageDetails.Attach(entity);
                db.BudgetUsageDetails.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Usage number " + usageDetail.BudgetUsageDetailId);

            }

            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

     
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}






