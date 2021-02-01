using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.FixedAsset;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Revaluation = ExceedERP.Core.Domain.FixedAsset.FixedAsset.FixedAssetRevaluation;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{
     
    [AuthorizeRoles(FixedAssetConstantes.FixedAssetAdmin, FixedAssetConstantes.FixedAssetDepreciationApprover, FixedAssetConstantes.FixedAssetDepreciationUser)]
    public class AnnualDepreciation12Controller : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        ExceedDbContext _db = new ExceedDbContext();
        List<DepreciationVm> _depreciations = new List<DepreciationVm>();
        private DepreciationVm _depreciation;


        #region Depretiation

        private void Depreciation(string itemcode , int year)
            {
            
            
            }
        #endregion
        #region Appretiation
        private void Appretiation()
            {
            
            }
        #endregion
        #region Revaluation

        private void Revaluation()
            {
            
            }
        #endregion

        public static GLPeriod GetPreviosPeriod(GlFiscalYear fiscalYear, GLPeriod period)
        {
            var periods = fiscalYear.GLPeriod;
            GLPeriod previousPeriod = new GLPeriod();
           
            foreach (GLPeriod glPeriod in periods)
            {
             int monthDiff =   diffMonths(period.DateFrom, glPeriod.DateTo);

                if (monthDiff == 1)
                {
                    previousPeriod = glPeriod;
                    break;
                }

            }
            return previousPeriod;

        }

      
       


        [AuthorizeRoles(FixedAssetConstantes.FixedAssetAdmin, FixedAssetConstantes.FixedAssetDepreciationApprover, FixedAssetConstantes.FixedAssetDepreciationUser)]
        public ActionResult Depreciations_Read([DataSourceRequest]DataSourceRequest request)
        {
            foreach (var fixedAsset in _db.Depreciations.ToList() )
            {
                    Registration fixRegistration = _db.Registrations.Find(fixedAsset.RegistrationID);
                    InventoryItem iventoryItem = _db.InventoryItems.Find(fixRegistration.Name);
                    ItemCategory itemCategory = _db.ItemCategorys.Find(iventoryItem.ItemCategoryCode);
                    ItemCategory parentCategory = new ItemCategory();
                    DepreciationRule record = new DepreciationRule();
                    Depreciation previousDep = new Depreciation();
              

                    //if (itemCategory.HaveParent)
                    //{
                    //    parentCategory = _db.ItemCategorys.Find(itemCategory.ParentName);
                    //    record =
                    //        _db.DepreciationRules.FirstOrDefault(
                    //            x =>
                    //                x.ItemCategoryCode == parentCategory.ItemCategoryCode &&
                    //                x.SubCategoryCode == itemCategory.ItemCategoryCode);
                    //}
                    //else
                    //{
                    //    record =
                    //        _db.DepreciationRules.FirstOrDefault(
                    //            x => x.ItemCategoryCode == itemCategory.ItemCategoryCode);
                    //}
                    _depreciation = new DepreciationVm();
                    _depreciation.ItemCode = fixRegistration.Tag;
                    _depreciation.RegistrationID = fixedAsset.RegistrationID;

                   

                    
                            _depreciation.Accumulation = fixedAsset.Amount;
                            
                      
                     
                    if (parentCategory.ItemCategoryCode != null)
                        {
                            _depreciation.SubCategory = itemCategory.Name;
                            _depreciation.CategoryName = parentCategory.Name; 
                        }
                    else
                        {
                            _depreciation.CategoryName = itemCategory.Name; 
                        _depreciation.SubCategory = String.Empty;
                            
                        }
                  
                    _depreciation.SalvageValue = record.FixedAssetGroup.SalvageValue;
                    _depreciation.BookValue = fixedAsset.BookValue;
                    _depreciation.OriginalValue = fixRegistration.PurchasePrice;
                        
                   _depreciations.Add(_depreciation); 

                }
            IQueryable<DepreciationVm> depreciations = _depreciations.AsQueryable();
            DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
            {
                ItemCode = depreciation.ItemCode,
                Accumulation = depreciation.Accumulation,
                CategoryName = depreciation.CategoryName,
                SubCategory = depreciation.SubCategory,
                BookValue = depreciation.BookValue,
                OriginalValue = depreciation.OriginalValue,
                SalvageValue = depreciation.SalvageValue
               
            });

            return Json(result);

           
        }

        public ActionResult ReverseDepreciation(int year, int period, string store)
        {

            var registrations = _db.Registrations.Where(x => x.StoreCode == store).ToList();

            foreach (var registration in registrations)
            {
                string stPeriod = period.ToString();
                var reverseValue =
                    _db.DeprciationReverses.FirstOrDefault(
                        x => x.RegistrationID == registration.RegistrationID && x.Period == stPeriod);
                var depreciations = _db.Depreciations.FirstOrDefault(x => x.RegistrationID == registration.RegistrationID && x.GlPeriod == period);
                decimal prevValue = depreciations.BookValue - reverseValue.BookValue;
                depreciations.BookValue = reverseValue.BookValue;
                depreciations.Accomodate -= prevValue;

            }
            var depreciationState =
               _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
            depreciationState.Status = DepreciationStatus.Reversed;
            _db.Entry(depreciationState).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CloseDepreciation(int year, int period, string store)
        {
            var depreciationState =
                 _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
            depreciationState.Status = DepreciationStatus.closed;
            _db.Entry(depreciationState).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public static int diffMonths(DateTime startDate, DateTime endDate)
        {
            return (startDate.Year * 12 + startDate.Month + startDate.Day / System.DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    - (endDate.Year * 12 + endDate.Month + endDate.Day / System.DateTime.DaysInMonth(endDate.Year, endDate.Month));
        }
        [AuthorizeRoles(FixedAssetConstantes.FixedAssetAdmin, FixedAssetConstantes.FixedAssetDepreciationApprover, FixedAssetConstantes.FixedAssetDepreciationUser)]
        public ActionResult Depretiation_Run(int year, int period, string store)
        {

            GlFiscalYear fiscalYear = _db.GLFiscalYears.Find(year);
            GLPeriod periods = _db.GLPeriods.Find(period);
            DateTime from = periods.DateFrom;
            DateTime to = periods.DateTo;
            List<DepreciationPeriod> depreciationPeriod =
                    _db.DepreciationPeriods.Where(x => x.StoreCode == store).ToList();

            string reversePeriod = period.ToString();


            var depreciationState =
                _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
            if (depreciationState == null)
            {
                DepreciationPeriod depPeriod = new DepreciationPeriod();
                depPeriod.StoreCode = store;
                depPeriod.FiscalYear = year;
                depPeriod.Period = period;
                depPeriod.Status = DepreciationStatus.onEdit;
                _db.DepreciationPeriods.Add(depPeriod);
                _db.SaveChanges();
            }

            foreach (var fixedAsset in _db.Registrations.Where(x => x.StoreCode == store).ToList())
            {

                var reverseHistory =
                    _db.DeprciationReverses.FirstOrDefault(
                        x => x.RegistrationID == fixedAsset.RegistrationID && x.Period == reversePeriod);
                if (reverseHistory == null)
                {

                    InventoryItem iventoryItem = _db.InventoryItems.Find(fixedAsset.Name);
                    ItemCategory itemCategory = _db.ItemCategorys.Find(iventoryItem.ItemCategoryCode);
                    ItemCategory parentCategory = new ItemCategory();
                    decimal bookValue = fixedAsset.BookValue;

                    DepreciationRule record;
                    Appreciation appreciation =
                        _db.Appreciations.FirstOrDefault(i => i.RegistrationId == fixedAsset.RegistrationID && i.AppreciationDate >= from && i.AppreciationDate <= to);
                    if (appreciation != null)
                    {
                        fixedAsset.BookValue += appreciation.Amount;
                        bookValue += appreciation.Amount;
                    }

                    else
                    {

                        Revaluation revaluation =
                       _db.FixedAssetRevaluations
                         .FirstOrDefault(r => r.RegistrationID == fixedAsset.RegistrationID && r.RevaluationDate >= from && r.RevaluationDate <= to);
                        if (revaluation != null)
                        {
                            bookValue += revaluation.AddedValue;
                            fixedAsset.BookValue += revaluation.AddedValue;
                        }
                        else
                        {
                            Depreciation previousDep =
                                _db.Depreciations.FirstOrDefault(x => x.DepreciationTime >= from && x.DepreciationTime <= to && x.RegistrationID == fixedAsset.RegistrationID);

                            if (previousDep != null)
                            {
                                bookValue += previousDep.Amount;
                                fixedAsset.BookValue = previousDep.BookValue;
                            }
                        }
                    }

                    if (itemCategory.HaveParent)
                    {
                        parentCategory = _db.ItemCategorys.Find(itemCategory.ParentName);
                    }


                    #region Check For Operation

                    //if (parentCategory.ItemCategoryCode != null)
                    //{
                    //    record = _db.DepreciationRules.FirstOrDefault(x => x.SubCategoryCode == itemCategory.ItemCategoryCode);
                    //}
                    //else
                    //{
                    //    record =
                    //        _db.DepreciationRules.FirstOrDefault(
                    //            x => x.ItemCategoryCode == itemCategory.ItemCategoryCode && x.FixedAssetGroup.DepreciationMethod == DepreciationMethod.StraightLine);
                    //}
                    #endregion
                    float dep = 0;
                    //var type = record.FixedAssetGroup.DepreciationMethod;
                    //if (type == DepreciationMethod.StraightLine)
                    //{
                    //    int datefromto = diffMonths(periods.DateFrom, periods.DateTo);
                    //    dep = record.FixedAssetGroup.YearlyDepreciationPercentage * (float)fixedAsset.PurchasePrice / (100 * datefromto) ;

                    //}

                    var rule = _db.DepretiationRules.FirstOrDefault(x => x.CategoryCode == fixedAsset.FixedAseetCategoryCode);

                    int datefromto = diffMonths(periods.DateFrom, periods.DateTo);
                    if (rule != null)
                        dep = rule.FixedAssetGroup.StraightLinePercentage * (float)fixedAsset.PurchasePrice / (100 * datefromto);

                    Depreciation depreciation =
                    _db.Depreciations.FirstOrDefault(
                     x => x.Registration.Name == iventoryItem.ItemCode && x.DepreciationTime >= from && x.DepreciationTime <= to);

                    Depreciation depreciations = new Depreciation();
                    depreciations.BookValue = fixedAsset.BookValue - (decimal)dep;
                    depreciations.Amount = (decimal)dep;
                    depreciations.RegistrationID = fixedAsset.RegistrationID;
                    depreciations.OriginalPrice = fixedAsset.PurchasePrice;
                    depreciations.DepreciationTime = periods.DateTo;
                    if (depreciation != null)
                    {
                        depreciations.Accomodate = Math.Abs(depreciation.Accomodate + (decimal)dep);

                    }
                    else
                    {
                        depreciations.Accomodate = Math.Abs((decimal)dep);

                    }
                    _db.Depreciations.Add(depreciations);
                    _db.SaveChanges();

                    bookValue = depreciations.BookValue;
                    fixedAsset.BookValue = bookValue;
                    DeprciationReverse reverse = new DeprciationReverse();
                    reverse.StoreCode = store;
                    reverse.BookValue = bookValue;
                    reverse.Period = period.ToString();
                    reverse.FiscalYear = year.ToString();


                    _db.Registrations.Attach(fixedAsset);
                    _db.Entry(fixedAsset).State = EntityState.Modified;
                    _db.SaveChanges();

                }
                else
                {
                    string strPrd = period.ToString();
                    var depreciationEa =
                        _db.Depreciations.FirstOrDefault(
                            x => x.RegistrationID == fixedAsset.RegistrationID && x.GlPeriod == period);
                    var reverseValue = _db.DeprciationReverses.FirstOrDefault(x => x.RegistrationID == fixedAsset.RegistrationID && x.Period == strPrd);


                    var depreciations = _db.Depreciations.FirstOrDefault(x => x.RegistrationID == fixedAsset.RegistrationID && x.GlPeriod == period);
                    decimal prevValue = depreciations.BookValue - reverseValue.BookValue;
                    depreciations.BookValue = reverseValue.BookValue;
                    depreciations.Accomodate -= prevValue;
                    _db.Entry(depreciations).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }


            return RedirectToAction("Index", "Depreciation");

        }
        public JsonResult GetYears()
            {
            var years = _db.GLFiscalYears;
            return Json(years.Select(x => new {Name = x.Name , Glid = x.GlFiscalYearId}), JsonRequestBehavior.AllowGet);
            }

        public JsonResult GetPeriods(int id)
            {
            var periods = _db.GLPeriods.Where(x => x.GlFiscalYearId == id);
            return Json(periods.Select(x => new {Name = x.Name, per = x.GLPeriodId}), JsonRequestBehavior.AllowGet);
            }
        public JsonResult GetStores()
            {
            var stores = _db.InventoryStores.ToList();
            return Json(stores.Select(x => new {Name = x.Name, Code = x.StoreCode}), JsonRequestBehavior.AllowGet);
            }
    }
}