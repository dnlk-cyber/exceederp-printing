using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.ShipmentUser)]
    public class SalesShipmentRevenueDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        #region cost distribution
        //public ActionResult CostDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    //IQueryable<SalesShipmentDistribution> costDistributions = db.SalesShipmentDistribution.Where(x => x.DistributionType == SalesShipmentDistributionType.CostDistribution && x.SalesShipmentId == id);
        //    //DataSourceResult result = costDistributions.ToDataSourceResult(request, s => new
        //    //{
        //    //    SalesShipmentDistributionId = s.SalesShipmentDistributionId,
        //    //    SalesShipmentId = s.SalesShipmentId,
        //    //    AccountCode = s.AccountCode,
        //    //    AccountDesc = s.AccountDesc,
        //    //    Credit = s.Credit,
        //    //    Debit = s.Debit,
        //    //});
        //    //return Json(costDistributions.Select(s => new SalesShipmentDistribution
        //    //{
        //    //    SalesShipmentDistributionId = s.SalesShipmentDistributionId,
        //    //    SalesShipmentId = s.SalesShipmentId,
        //    //    AccountCode = s.AccountCode,
        //    //    AccountDesc = s.AccountDesc,
        //    //    Credit = s.Credit,
        //    //    Debit = s.Debit,
        //    //}).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    return Json(null);
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CostDistribution_Create([DataSourceRequest]DataSourceRequest request, SalesShipmentDistribution distribution, int id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new SalesShipmentDistribution
        //        {
        //            SalesShipmentDistributionId = distribution.SalesShipmentDistributionId,
        //            DistributionType = SalesShipmentDistributionType.CostDistribution,
        //            SalesShipmentId = id,
        //            AccountCode = distribution.AccountCode,
        //            AccountDesc = distribution.AccountDesc,
        //            Credit = distribution.Credit,
        //            Debit = distribution.Debit,

        //            DateCreated = DateTime.Now,
        //            CreatedBy = User.Identity.Name
        //        };

        //        db.SalesShipmentDistribution.Add(entity);
        //        db.SaveChanges();

        //        distribution.SalesShipmentDistributionId = entity.SalesShipmentDistributionId;
        //    }
        //    return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CostDistribution_Update([DataSourceRequest]DataSourceRequest request, SalesShipmentDistribution distribution)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new SalesShipmentDistribution
        //        {
        //            SalesShipmentDistributionId = distribution.SalesShipmentDistributionId,
        //            DistributionType = distribution.DistributionType,
        //            SalesShipmentId = distribution.SalesShipmentId,
        //            AccountCode = distribution.AccountCode,
        //            AccountDesc = distribution.AccountDesc,
        //            Credit = distribution.Credit,
        //            Debit = distribution.Debit,

        //            DateCreated = distribution.DateCreated,
        //            CreatedBy = distribution.CreatedBy,
        //            LastModified = DateTime.Now,
        //            ModifiedBy = User.Identity.Name
        //        };

        //        db.SalesShipmentDistribution.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CostDistribution_Destroy([DataSourceRequest]DataSourceRequest request, SalesShipmentDistribution distribution)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new SalesShipmentDistribution
        //        {
        //            SalesShipmentDistributionId = distribution.SalesShipmentDistributionId,
        //            DistributionType = distribution.DistributionType,
        //            SalesShipmentId = distribution.SalesShipmentId,
        //            AccountCode = distribution.AccountCode,
        //            AccountDesc = distribution.AccountDesc,
        //            Credit = distribution.Credit,
        //            Debit = distribution.Debit,

        //            DateCreated = distribution.DateCreated,
        //            CreatedBy = distribution.CreatedBy,
        //            LastModified = distribution.LastModified,
        //            ModifiedBy = distribution.ModifiedBy
        //        };

        //        db.SalesShipmentDistribution.Attach(entity);
        //        db.SalesShipmentDistribution.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        //}

        #endregion
        #region revenue distribution
        public ActionResult RevenueDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesShipmentDistribution> revenueDistributions = db.SalesShipmentDistribution.Where(x => x.DistributionType == SalesShipmentDistributionType.RevenuDistribution && x.SalesShipmentId == id);

            DataSourceResult result = revenueDistributions.ToDataSourceResult(request, s => new RevenueDistributionViewModel
            {
                RevenueDistributionId = s.SalesShipmentDistributionId,
                DistributionType = s.DistributionType,
                SalesShipmentId = s.SalesShipmentId,
                AccountCode = s.AccountCode,
                AccountDesc = s.AccountDesc,
                AdditionalDescription = s.AdditionalDescription,
                Credit = s.Credit,
                Debit = s.Debit,
            });
            return Json(result);
            //return Json(revenueDistributions.Select(s => new SalesShipmentDistribution
            //{
            //    SalesShipmentDistributionId = s.SalesShipmentDistributionId,
            //    SalesShipmentId = s.SalesShipmentId,
            //    AccountCode = s.AccountCode,
            //    AccountDesc = s.AccountDesc,
            //    Credit = s.Credit,
            //    Debit = s.Debit,
            //}
            //).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueDistribution_Create([DataSourceRequest]DataSourceRequest request, RevenueDistributionViewModel distribution, int id)
        {
            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var shipmentHeader = db.SalesShipments.Find(id);
            string branchSeg = HrHelper.GetBranchLocationAccount(shipmentHeader != null ? shipmentHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);



            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentDistribution
                {
                    SalesShipmentDistributionId = distribution.RevenueDistributionId,
                    DistributionType = SalesShipmentDistributionType.RevenuDistribution,
                    SalesShipmentId = id,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.SalesShipmentDistribution.Add(entity);
                db.SaveChanges();

                distribution.RevenueDistributionId = entity.SalesShipmentDistributionId;
            }
            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueDistribution_Update([DataSourceRequest]DataSourceRequest request, RevenueDistributionViewModel distribution)
        {

            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();

            int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
            var shipmentHeader = db.SalesShipments.Find(distribution.SalesShipmentId);
            string branchSeg = HrHelper.GetBranchLocationAccount(shipmentHeader != null ? shipmentHeader.BranchId : 0);
            string newacc = _dynamicAccount.ReplaceSegment(distribution.AccountCode, branchAccountIndex, branchSeg);


            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentDistribution
                {
                    SalesShipmentDistributionId = distribution.RevenueDistributionId,
                    DistributionType = distribution.DistributionType,
                    SalesShipmentId = distribution.SalesShipmentId,
                    AccountCode = newacc,
                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesShipmentDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueDistribution_Destroy([DataSourceRequest]DataSourceRequest request, RevenueDistributionViewModel distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentDistribution
                {
                    SalesShipmentDistributionId = distribution.RevenueDistributionId,
                    DistributionType = distribution.DistributionType,
                    SalesShipmentId = distribution.SalesShipmentId,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    AdditionalDescription = distribution.AdditionalDescription,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    DateCreated = distribution.DateCreated,
                    CreatedBy = distribution.CreatedBy,
                    LastModified = distribution.LastModified,
                    ModifiedBy = distribution.ModifiedBy
                };

                db.SalesShipmentDistribution.Attach(entity);
                db.SalesShipmentDistribution.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        #endregion
    }
}