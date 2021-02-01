using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.FixedAsset;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciation
{

   
    public class AnnualDepreciationController : Controller
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
            var years = _db.GLFiscalYears;
            var assetRegistrations = _db.AssetRegistrations;

            ViewData["Years"] = years;
            ViewData["Branches"] = branches;
            ViewData["Assets"] = assetRegistrations;

        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult Depreciations_Read([DataSourceRequest]DataSourceRequest request)
        {
          
            IQueryable<AnnualDepreciation> depreciations = _db.AnnualDepreciations.AsQueryable();
            DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
            {
                AnnualDepreciationID = depreciation.AnnualDepreciationID,
                AssetRegistrationId = depreciation.AssetRegistrationId,
                GlFiscalYearId = depreciation.GlFiscalYearId,
                BookValue = depreciation.BookValue,
                DepreciationAmount = depreciation.DepreciationAmount,
                AccumulatedDepreciationAmount = depreciation.AccumulatedDepreciationAmount

            });

            return Json(result);


        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        public ActionResult ReverseDepreciation(int year, int period, string store)
        {

            var registrations = _db.AssetRegistrations.Where(x => x.StoreCode == store).ToList();

            foreach (var registration in registrations)
            {
                string stPeriod = period.ToString();
                var reverseValue =
                    _db.DeprciationReverses.FirstOrDefault(
                        x => x.RegistrationID == registration.AssetRegistrationId && x.Period == stPeriod);
                var depreciations = _db.AnnualDepreciations.FirstOrDefault(x => x.AssetRegistrationId == registration.AssetRegistrationId && x.GlFiscalYearId == period);
                decimal prevValue = depreciations.BookValue - reverseValue.BookValue;
                depreciations.BookValue = reverseValue.BookValue;
               // depreciations.Accomodate -= prevValue;

            }
            var depreciationState =
               _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
            depreciationState.Status = DepreciationStatus.Reversed;
            _db.Entry(depreciationState).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult CloseDepreciation(int year, int period, string store)
        {
            var depreciationState =_db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
            depreciationState.Status = DepreciationStatus.closed;
            _db.Entry(depreciationState).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public static int DiffMonths(DateTime startDate, DateTime endDate)
        {
            return (startDate.Year * 12 + startDate.Month + startDate.Day / System.DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    - (endDate.Year * 12 + endDate.Month + endDate.Day / System.DateTime.DaysInMonth(endDate.Year, endDate.Month));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        public JsonResult RunDepreciation(string fiscalyearid, string branchid)
        {

            if (!String.IsNullOrEmpty(fiscalyearid) && !String.IsNullOrEmpty(branchid))
            {
                int yearidd=int.Parse(fiscalyearid);
                int branchidd = int.Parse(branchid);
                var branch = _db.Branch.Find(branchidd);
                var fiscalyear = _db.GLFiscalYears.Find(yearidd);

                var rules = _db.DepretiationRules.ToList();

                var activeAssets = _db.AssetRegistrations.Where(x => x.Status == RegistrationStatus.Active).ToList();
                if (fiscalyear != null)
                {
                    foreach (var asset in activeAssets)
                    {
                        #region skip if deprecation is done for this item and year

                        var exist =_db.AnnualDepreciations.Where(x => x.GlFiscalYearId == fiscalyear.GlFiscalYearId)
                                .Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                                .ToList();
                        if (exist.Any())
                            continue;

                        #endregion
                        var rule=rules.FirstOrDefault(x => x.CategoryCode==asset.FixedAssetCategory.CategoryCode);
                        if (rule != null)
                        {
                            var depreciationPercentage = rule.FixedAssetGroup.PoolingPercentage;
                            var salvagevalue = rule.FixedAssetGroup.SalvageValue;
                            #region pool based depreciation
                            var depreciated = (asset.BookValue * (decimal)depreciationPercentage) / 100;
                            if ((asset.BookValue - depreciated) <= salvagevalue)
                            {
                                depreciated = asset.BookValue - salvagevalue;
                                asset.BookValue = salvagevalue;
                            }
                            else if ((asset.BookValue - depreciated) > salvagevalue)
                            {
                                asset.BookValue = asset.BookValue - depreciated;
                            }
                            _db.Entry(asset).State = EntityState.Modified;
                            _db.SaveChanges();
                            //register the deprecation
                            var depre = new AnnualDepreciation
                            {
                                AssetRegistrationId = asset.AssetRegistrationId,
                                DepreciationAmount = depreciated,
                                AccumulatedDepreciationAmount=0,
                                BookValue = asset.BookValue,
                                GlFiscalYearId = fiscalyear.GlFiscalYearId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };
                            _db.AnnualDepreciations.Add(depre);
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
                    Message = "Please select fiscal year and branch",
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


      
    }
}