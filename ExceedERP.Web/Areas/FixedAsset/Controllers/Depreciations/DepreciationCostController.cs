using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{

   
    public class DepreciationCostController : Controller
    {
        ExceedDbContext _db = new ExceedDbContext();
       
      

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();

        }

        private void PopulateViewData()
        {
           
            var branches = _db.Branch;
            var periods = _db.GLPeriods;
            var assetRegistrations = _db.AssetRegistrations;

            ViewData["Periods"] = periods;
            ViewData["Branches"] = branches;
            ViewData["Assets"] = assetRegistrations;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult Depreciations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DepreciationForCost> depreciations = _db.DepreciationForCosts.AsQueryable();
            DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
            {
                DepreciationForCostId = depreciation.DepreciationForCostId,
                AssetRegistrationId = depreciation.AssetRegistrationId,
                BranchId = depreciation.BranchId,
                GLPeriodId = depreciation.GLPeriodId,
                BookValue = depreciation.BookValue,
                DepreciationAmount = depreciation.DepreciationAmount

            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        //public ActionResult ReverseDepreciation(int year, int period, string store)
        //{

        //    var registrations = _db.AssetRegistrations.Where(x => x.StoreCode == store).ToList();

        //    foreach (var registration in registrations)
        //    {
        //        string stPeriod = period.ToString();
        //        var reverseValue =
        //            _db.DeprciationReverses.FirstOrDefault(
        //                x => x.RegistrationID == registration.AssetRegistrationId && x.Period == stPeriod);
        //        var depreciations = _db.AnnualDepreciations.FirstOrDefault(x => x.AssetRegistrationId == registration.AssetRegistrationId && x.GlFiscalYearId == period);
        //        decimal prevValue = depreciations.BookValue - reverseValue.BookValue;
        //        depreciations.BookValue = reverseValue.BookValue;
        //       // depreciations.Accomodate -= prevValue;

        //    }
        //    var depreciationState =
        //       _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
        //    depreciationState.Status = DepreciationStatus.Reversed;
        //    _db.Entry(depreciationState).State = EntityState.Modified;
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationApprover)]
        //public ActionResult CloseDepreciation(int year, int period, string store)
        //{
        //    //var depreciationState =_db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
        //    //depreciationState.Status = DepreciationStatus.closed;
        //    //_db.Entry(depreciationState).State = EntityState.Modified;
        //    //_db.SaveChanges();
        //    //return RedirectToAction("Index");
        //}
        public static int DiffMonths(DateTime startDate, DateTime endDate)
        {
            return (startDate.Year * 12 + startDate.Month + startDate.Day / System.DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    - (endDate.Year * 12 + endDate.Month + endDate.Day / System.DateTime.DaysInMonth(endDate.Year, endDate.Month));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        public JsonResult RunDepreciation(string fiscalyearid,string periodid, string branchid)
        {

            if (!String.IsNullOrEmpty(periodid) && !String.IsNullOrEmpty(branchid))
            {
                decimal minimumAssetValue = FixedAssetConfigurationHelper.GetAssetMinimumLevel();
                int periodidd = int.Parse(periodid);
                int branchidd = int.Parse(branchid);
                var branch = _db.Branch.Find(branchidd);
                var glPeriod = _db.GLPeriods.Find(periodidd);
                var rules = _db.DepretiationRules.ToList();
               
                var activeAssets = _db.AssetRegistrations
                        .Where(x => x.Status == RegistrationStatus.Active || x.Status == RegistrationStatus.Revalued)
                        .Where(x => x.PurchasePrice>=minimumAssetValue)//added to exclude small items 
                      .ToList();
                if (glPeriod != null &&branch!=null)
                {
                    foreach (var asset in activeAssets)
                    {
                        #region skip if deprecation is done for this item and year

                        var exist = _db.DepreciationForCosts.Where(x => x.GLPeriodId == glPeriod.GLPeriodId)
                                .Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                                .ToList();
                        if (exist.Any())
                            continue;

                        #endregion
                        var rule = rules.FirstOrDefault(x => x.CategoryCode == asset.FixedAssetCategory.CategoryCode);
                        if (rule != null)
                        {
                            var depreciationPercentage = rule.FixedAssetGroup.StraightLinePercentage;
                           
                            #region pool based depreciation
                            var depreciated = (asset.OriginalValue * (decimal)depreciationPercentage) / 100;
                            _db.Entry(asset).State = EntityState.Modified;
                            _db.SaveChanges();
                            //register the deprecation
                            var depre = new DepreciationForCost
                            {
                                AssetRegistrationId = asset.AssetRegistrationId,
                                DepreciationAmount = depreciated,
                                BookValue = asset.BookValue,
                                GLPeriodId = glPeriod.GLPeriodId,
                                BranchId = branch.BranchId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };
                            _db.DepreciationForCosts.Add(depre);
                            _db.SaveChanges();
                            //accounting

                            #endregion
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Depreciation rule not found for asset " + asset.FixedAssetCategory.CategoryName,
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }

                    }
                }


            }
            else
            {
                var formattedData = new
                {
                    Message = "Please select period and branch",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "Depreciation complete.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}