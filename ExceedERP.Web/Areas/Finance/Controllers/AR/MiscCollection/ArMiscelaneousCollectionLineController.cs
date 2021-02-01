using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.MiscCollection
{
    public class ArMiscelaneousCollectionLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArMiscelaneousCollectionLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArMiscelaneousCollectionLine> armiscelaneouscollectionlines = _db.ArMiscelaneousCollectionLines.Where(x=>x.ARMiscelaneousCollectionID==id);
            DataSourceResult result = armiscelaneouscollectionlines.ToDataSourceResult(request, arMiscelaneousCollectionLine => new {
                ArMiscelaneousCollectionLineId = arMiscelaneousCollectionLine.ArMiscelaneousCollectionLineId,
                ARMiscelaneousCollectionID= arMiscelaneousCollectionLine.ARMiscelaneousCollectionID,
                Activity = arMiscelaneousCollectionLine.Activity,
                Amount = arMiscelaneousCollectionLine.Amount,
                RevenueAccount = arMiscelaneousCollectionLine.RevenueAccount,
                RevenueAccountDesc = arMiscelaneousCollectionLine.RevenueAccountDesc,
                TaxType = arMiscelaneousCollectionLine.TaxType,
                TaxAmount = arMiscelaneousCollectionLine.TaxAmount,
                WithholdingType = arMiscelaneousCollectionLine.WithholdingType,
                WithholdingAmount = arMiscelaneousCollectionLine.WithholdingAmount,
                WithholdTax = arMiscelaneousCollectionLine.WithholdTax,
                WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTaxAmount,
                TotalAmount = arMiscelaneousCollectionLine.TotalAmount,
                NetReceipt = arMiscelaneousCollectionLine.NetReceipt,
                DateCreated = arMiscelaneousCollectionLine.DateCreated,
                LastModified = arMiscelaneousCollectionLine.LastModified,
                CreatedBy = arMiscelaneousCollectionLine.CreatedBy,
                ModifiedBy = arMiscelaneousCollectionLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArMiscelaneousCollectionLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ArMiscelaneousCollectionLine arMiscelaneousCollectionLine)
        {

            #region tax calc
            if (!String.IsNullOrEmpty(arMiscelaneousCollectionLine.TaxType))
            {
                var tax = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == arMiscelaneousCollectionLine.TaxType);
                if (tax != null)
                {
                    arMiscelaneousCollectionLine.TaxAmount = arMiscelaneousCollectionLine.Amount * tax.CalculatedRate;
                    arMiscelaneousCollectionLine.TaxAmount=Math.Round(arMiscelaneousCollectionLine.TaxAmount, 2);
                    arMiscelaneousCollectionLine.TotalAmount =
                        arMiscelaneousCollectionLine.Amount + arMiscelaneousCollectionLine.TaxAmount;
                    arMiscelaneousCollectionLine.TotalAmount = Math.Round(arMiscelaneousCollectionLine.TotalAmount, 2);
                }
            }
            #endregion
            #region withholding calc
            if (!String.IsNullOrEmpty(arMiscelaneousCollectionLine.WithholdingType))
            {
                var withholding = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == arMiscelaneousCollectionLine.WithholdingType);
                if (withholding != null)
                {
                    arMiscelaneousCollectionLine.WithholdingAmount = arMiscelaneousCollectionLine.Amount * withholding.CalculatedRate;
                    arMiscelaneousCollectionLine.WithholdingAmount = Math.Round(arMiscelaneousCollectionLine.WithholdingAmount, 2);
                }
            }
            #endregion
            #region withheld tax

            arMiscelaneousCollectionLine.WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTax ? arMiscelaneousCollectionLine.TaxAmount : 0;

            #endregion
            #region net
            arMiscelaneousCollectionLine.NetReceipt = arMiscelaneousCollectionLine.Amount
                                                     - arMiscelaneousCollectionLine.WithholdingAmount
                                                     + arMiscelaneousCollectionLine.TaxAmount
                                                     - arMiscelaneousCollectionLine.WithholdTaxAmount;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArMiscelaneousCollectionLine
                {
                    ARMiscelaneousCollectionID=id,
                    Activity = arMiscelaneousCollectionLine.Activity,
                    Amount = arMiscelaneousCollectionLine.Amount,
                    RevenueAccount = arMiscelaneousCollectionLine.RevenueAccount,
                    RevenueAccountDesc = arMiscelaneousCollectionLine.RevenueAccountDesc,
                    TaxType = arMiscelaneousCollectionLine.TaxType,
                    TaxAmount = arMiscelaneousCollectionLine.TaxAmount,
                    WithholdingType = arMiscelaneousCollectionLine.WithholdingType,
                    WithholdingAmount = arMiscelaneousCollectionLine.WithholdingAmount,
                    WithholdTax = arMiscelaneousCollectionLine.WithholdTax,
                    WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTaxAmount,
                    TotalAmount = arMiscelaneousCollectionLine.TotalAmount,
                    NetReceipt = arMiscelaneousCollectionLine.NetReceipt,
                    DateCreated = arMiscelaneousCollectionLine.DateCreated,
                    LastModified = arMiscelaneousCollectionLine.LastModified,
                    CreatedBy = arMiscelaneousCollectionLine.CreatedBy,
                    ModifiedBy = arMiscelaneousCollectionLine.ModifiedBy
                };

                _db.ArMiscelaneousCollectionLines.Add(entity);
                _db.SaveChanges();
                arMiscelaneousCollectionLine.ArMiscelaneousCollectionLineId = entity.ArMiscelaneousCollectionLineId;
                #region distribution

                _distributionHelper.UpdateArMiscCollectionDistribution(id);
                UpdateCollectionBalances(id);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection item", "Success", User.Identity.Name, $"created misc collection item {entity.Activity} under record {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arMiscelaneousCollectionLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArMiscelaneousCollectionLines_Update([DataSourceRequest]DataSourceRequest request, ArMiscelaneousCollectionLine arMiscelaneousCollectionLine)
        {
            #region tax calc
            if (!String.IsNullOrEmpty(arMiscelaneousCollectionLine.TaxType))
            {
                var tax = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == arMiscelaneousCollectionLine.TaxType);
                if (tax != null)
                {
                    arMiscelaneousCollectionLine.TaxAmount = arMiscelaneousCollectionLine.Amount * tax.CalculatedRate;
                    arMiscelaneousCollectionLine.TaxAmount = Math.Round(arMiscelaneousCollectionLine.TaxAmount, 2);
                    arMiscelaneousCollectionLine.TotalAmount =arMiscelaneousCollectionLine.Amount + arMiscelaneousCollectionLine.TaxAmount;
                    arMiscelaneousCollectionLine.TotalAmount = Math.Round(arMiscelaneousCollectionLine.TotalAmount, 2);
                }
            }
            #endregion
            #region withholding calc
            if (!String.IsNullOrEmpty(arMiscelaneousCollectionLine.TaxType))
            {
                var withholding = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == arMiscelaneousCollectionLine.WithholdingType);
                if (withholding != null)
                {
                    arMiscelaneousCollectionLine.WithholdingAmount = arMiscelaneousCollectionLine.Amount * withholding.CalculatedRate;
                    arMiscelaneousCollectionLine.WithholdingAmount = Math.Round(arMiscelaneousCollectionLine.WithholdingAmount, 2);
                }
            }
            #endregion
            #region withheld tax

            arMiscelaneousCollectionLine.WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTax ? arMiscelaneousCollectionLine.TaxAmount : 0;

            #endregion
            #region net
            arMiscelaneousCollectionLine.NetReceipt = arMiscelaneousCollectionLine.Amount
                                                      - arMiscelaneousCollectionLine.WithholdingAmount
                                                      + arMiscelaneousCollectionLine.TaxAmount
                                                      - arMiscelaneousCollectionLine.WithholdTaxAmount;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArMiscelaneousCollectionLine
                {
                    ArMiscelaneousCollectionLineId = arMiscelaneousCollectionLine.ArMiscelaneousCollectionLineId,
                    ARMiscelaneousCollectionID = arMiscelaneousCollectionLine.ARMiscelaneousCollectionID,
                    Activity = arMiscelaneousCollectionLine.Activity,
                    Amount = arMiscelaneousCollectionLine.Amount,
                    RevenueAccount = arMiscelaneousCollectionLine.RevenueAccount,
                    RevenueAccountDesc = arMiscelaneousCollectionLine.RevenueAccountDesc,
                    TaxType = arMiscelaneousCollectionLine.TaxType,
                    TaxAmount = arMiscelaneousCollectionLine.TaxAmount,
                    WithholdingType = arMiscelaneousCollectionLine.WithholdingType,
                    WithholdingAmount = arMiscelaneousCollectionLine.WithholdingAmount,
                    WithholdTax = arMiscelaneousCollectionLine.WithholdTax,
                    WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTaxAmount,
                    TotalAmount = arMiscelaneousCollectionLine.TotalAmount,
                    NetReceipt = arMiscelaneousCollectionLine.NetReceipt,
                    DateCreated = arMiscelaneousCollectionLine.DateCreated,
                    LastModified = arMiscelaneousCollectionLine.LastModified,
                    CreatedBy = arMiscelaneousCollectionLine.CreatedBy,
                    ModifiedBy = arMiscelaneousCollectionLine.ModifiedBy
                };

                _db.ArMiscelaneousCollectionLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region distribution

                _distributionHelper.UpdateArMiscCollectionDistribution(arMiscelaneousCollectionLine.ARMiscelaneousCollectionID);
                UpdateCollectionBalances(arMiscelaneousCollectionLine.ARMiscelaneousCollectionID);
                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection item", "Success", User.Identity.Name, $"updated misc collection item {entity.Activity} under record {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arMiscelaneousCollectionLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArMiscelaneousCollectionLines_Destroy([DataSourceRequest]DataSourceRequest request, ArMiscelaneousCollectionLine arMiscelaneousCollectionLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArMiscelaneousCollectionLine
                {
                    ArMiscelaneousCollectionLineId = arMiscelaneousCollectionLine.ArMiscelaneousCollectionLineId,
                    ARMiscelaneousCollectionID = arMiscelaneousCollectionLine.ARMiscelaneousCollectionID,
                    Activity = arMiscelaneousCollectionLine.Activity,
                    Amount = arMiscelaneousCollectionLine.Amount,
                    RevenueAccount = arMiscelaneousCollectionLine.RevenueAccount,
                    RevenueAccountDesc = arMiscelaneousCollectionLine.RevenueAccountDesc,
                    TaxType = arMiscelaneousCollectionLine.TaxType,
                    TaxAmount = arMiscelaneousCollectionLine.TaxAmount,
                    WithholdingType = arMiscelaneousCollectionLine.WithholdingType,
                    WithholdingAmount = arMiscelaneousCollectionLine.WithholdingAmount,
                    WithholdTax = arMiscelaneousCollectionLine.WithholdTax,
                    WithholdTaxAmount = arMiscelaneousCollectionLine.WithholdTaxAmount,
                    TotalAmount = arMiscelaneousCollectionLine.TotalAmount,
                    NetReceipt = arMiscelaneousCollectionLine.NetReceipt,
                    DateCreated = arMiscelaneousCollectionLine.DateCreated,
                    LastModified = arMiscelaneousCollectionLine.LastModified,
                    CreatedBy = arMiscelaneousCollectionLine.CreatedBy,
                    ModifiedBy = arMiscelaneousCollectionLine.ModifiedBy
                };

                _db.ArMiscelaneousCollectionLines.Attach(entity);
                _db.ArMiscelaneousCollectionLines.Remove(entity);
                _db.SaveChanges();
                #region distribution

                _distributionHelper.UpdateArMiscCollectionDistribution(arMiscelaneousCollectionLine.ARMiscelaneousCollectionID);
                UpdateCollectionBalances(arMiscelaneousCollectionLine.ARMiscelaneousCollectionID);

                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted misc. collection item " + arMiscelaneousCollectionLine.ARMiscelaneousCollectionID + " " + arMiscelaneousCollectionLine.Activity, Modules.Finance);
                #endregion
            }

            return Json(new[] { arMiscelaneousCollectionLine }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateCollectionBalances(int? id)
        {
            var collection = _db.ARMiscelaneousCollections.Find(id);
            if (collection != null)
            {
                #region update parent balances
                var lines = _db.ArMiscelaneousCollectionLines.Where(x => x.ARMiscelaneousCollectionID == id).ToList();
                collection.LineAmount = Math.Round(lines.Sum(x => x.Amount), 2);
                collection.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                collection.TotalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);

                _db.Entry(collection).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion

            }



        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
