using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Presentation;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{


    public class IfrsDepreciationController : Controller
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FixedAssetDistributionHelper _distributionHelper = new FixedAssetDistributionHelper();


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
            var periods = _db.GLPeriods;
            var assetRegistrations = _db.AssetRegistrations.Select(
                i => new
                {
                    Name = i.Tag + " " + i.Name,
                    Code = i.AssetRegistrationId
                }).AsQueryable();
            var groups = _db.FixedAssetGroups;

            ViewData["Years"] = years;
            ViewData["Periods"] = periods;
            ViewData["Branches"] = branches;
            ViewData["Groups"] = groups;
            ViewData["Assets"] = assetRegistrations;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult IfrsDepreciations_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<IfrsDepreciation> depreciations = _db.IfrsDepreciations;

            DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
            {
                IfrsDepreciationId = depreciation.IfrsDepreciationId,
                GlFiscalYearId = depreciation.GlFiscalYearId,
                Status = depreciation.Status,
                Remark = depreciation.Remark,
                IsVoid = depreciation.IsVoid,
                VoidBy = depreciation.VoidBy,
                VoidTime = depreciation.VoidTime,
                IsOnlineApproved = depreciation.IsOnlineApproved,
                OnlineApprovedBy = depreciation.OnlineApprovedBy,
                OnlineApprovedTime = depreciation.OnlineApprovedTime,
                IsSendForApproval = depreciation.IsSendForApproval,
                SendForApprovalBy = depreciation.SendForApprovalBy,
                SendForApprovalTime = depreciation.SendForApprovalTime,
                IsOnlineTransferred = depreciation.IsOnlineTransferred,
                IsOnlineTransferredBy = depreciation.IsOnlineTransferredBy,
                OnlineTransferredTime = depreciation.OnlineTransferredTime,

                DateCreated = depreciation.DateCreated,
                CreatedBy = depreciation.CreatedBy,
                LastModified = depreciation.LastModified,
                ModifiedBy = depreciation.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult IfrsDepreciationItems_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            IQueryable<IfrsDepreciationItem> IfrsDepreciationItems = _db.IfrsDepreciationItems.Where(x => x.IfrsDepreciationId == id);

            DataSourceResult result = IfrsDepreciationItems.ToDataSourceResult(request, IfrsDepreciationItem => new
            {
                IfrsDepreciationId = IfrsDepreciationItem.IfrsDepreciationId,
                AssetRegistrationId = IfrsDepreciationItem.AssetRegistrationId,
                DepreciationAmount = IfrsDepreciationItem.DepreciationAmount,
                BookValue = IfrsDepreciationItem.BookValue,
                BranchId = IfrsDepreciationItem.BranchId,
                BusinessUnitId = IfrsDepreciationItem.BusinessUnitId,

            });

            return Json(result);
        }

        //[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        //public ActionResult ReverseDepreciation(int year, int period, string store)
        //{

        //    var registrations = _db.AssetRegistrations.Where(x => x.StoreCode == store).ToList();

        //    foreach (var registration in registrations)
        //    {
        //        string stPeriod = period.ToString();
        //        var reverseValue =
        //            _db.DeprciationReverses.FirstOrDefault(
        //                x => x.RegistrationID == registration.AssetRegistrationId && x.Period == stPeriod);
        //        var depreciations = _db.Ifrs.FirstOrDefault(x => x.AssetRegistrationId == registration.AssetRegistrationId && x.GlFiscalYearId == period);
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        public JsonResult RunDepreciation(string fiscalyearid, string periodId)
        {
            List<string> errors = new List<string>();

            if (!string.IsNullOrEmpty(fiscalyearid))
            {
                decimal minimumAssetValue = FixedAssetConfigurationHelper.GetAssetMinimumLevel();
                int yearidd = int.Parse(fiscalyearid);
                int glPeriodId = 0;

                bool isByPeriod = GlobalHelper.IsDepreciationMonthly();
                if (string.IsNullOrEmpty(periodId) && isByPeriod)
                {
                    var formattedData = new
                    {
                        Message = "Please select period.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                else if (!string.IsNullOrEmpty(periodId) && isByPeriod)
                {
                    glPeriodId = int.Parse(periodId);

                }


                var glyear = _db.GLFiscalYears.Find(yearidd);



                if (glyear != null)
                {
                    DateTime dateTo = glyear.DateTo.Date;
                    DateTime dateFrom = glyear.DateFrom.Date;


                    if (glPeriodId != 0)
                    {
                        var period = _db.GLPeriods.Find(glPeriodId);
                        if (period != null)
                        {
                            dateTo = period.DateTo.Date;
                            dateFrom = period.DateFrom.Date;
                        }
                    }

                    int daysInYearPeriod = (dateTo - dateFrom).Days + 1;

                    // var rules = _db.DepretiationRules.AsNoTracking().ToList(); //for sistribution
                    var groups = _db.FixedAssetGroups.AsNoTracking().ToList();

                    var activeAssets = _db.AssetRegistrations
                        .Where(x => x.Status == RegistrationStatus.Active || x.Status == RegistrationStatus.Revalued)
                        .Where(x => x.PurchasePrice >= minimumAssetValue)
                        .Include(x => x.FixedAssetCategory)
                        .Select(x => new { x.AssetRegistrationId, x.GRNDate, x.FixedAssetCategory, x.Quanity, x.PurchasePrice, x.OriginalValue, x.SalvageValue, x.SalvageValueType, x.IFRSBookValue, x.UsefulLifeYears, x.BranchId, x.BusinessUnitId })
                        .AsQueryable();


                    activeAssets = activeAssets
                            .Where(x => x.GRNDate <= dateTo).AsQueryable();

                    var ifrs = _db.IfrsDepreciations
                        .Where(x => x.GlFiscalYearId == glyear.GlFiscalYearId)
                        .Where(x => x.GlPeriodId == glPeriodId).FirstOrDefault();

                    if (ifrs == null)
                    {
                        ifrs = new IfrsDepreciation
                        {
                            GlFiscalYearId = yearidd,
                            GlPeriodId = glPeriodId,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            Status = "Generated"
                        };
                        _db.IfrsDepreciations.Add(ifrs);
                        _db.SaveChanges();
                    }
                    List<IfrsDepreciationItem> depreList = new List<IfrsDepreciationItem>();
                    var activeList = activeAssets.ToList();
                    foreach (var asset in activeList)
                    {
                        #region skip if deprecation is done for this item and year

                        var exist = _db.IfrsDepreciationItems
                            .Where(x => x.IfrsDepreciationId == ifrs.IfrsDepreciationId)
                            .Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                            .ToList();
                        if (exist.Any())
                            continue;

                        #endregion

                        var assetGroup = groups.FirstOrDefault(x =>
                            x.FixedAssetGroupID == asset.FixedAssetCategory.FixedAssetGroupID);

                        if (assetGroup != null && (asset.GRNDate != null))
                        {
                            #region straight line based depreciation



                            var depreciationPercentage = assetGroup.IfrsPercentage;
                            // Use max to prevent negative days
                            DateTime effectiveFrom = dateFrom > asset.GRNDate ? dateFrom : (DateTime)asset.GRNDate;
                            int days = (dateTo - effectiveFrom).Days + 1;

                            if (days <= 0 || asset.OriginalValue <= 0 || asset.Quanity <= 0 || depreciationPercentage <= 0)
                            {
                                continue;
                            }

                            decimal depreciated = 0;
                            decimal bookValue = asset.Quanity * asset.OriginalValue;
                            decimal salvageValue = asset.SalvageValueType == PPEAmountType.Amount ? asset.SalvageValue : (asset.OriginalValue * asset.SalvageValue) / 100;


                            if (asset.UsefulLifeYears > 0)
                            {
                                depreciated = ((bookValue - salvageValue) / asset.UsefulLifeYears * days) / (daysInYearPeriod);
                            }
                            else
                            {
                                depreciated = ((bookValue - salvageValue) * (decimal)depreciationPercentage * days) / (daysInYearPeriod * 100);
                            }



                            //_db.Entry(asset).State = EntityState.Modified;
                            //_db.SaveChanges();
                            //register the for item
                            var depre = new IfrsDepreciationItem
                            {
                                IfrsDepreciationId = ifrs.IfrsDepreciationId,
                                AssetRegistrationId = asset.AssetRegistrationId,
                                DepreciationAmount = depreciated,
                                BookValue = asset.IFRSBookValue - depreciated,
                                AccumulatedDepreciationAmount = asset.OriginalValue - depreciated,
                                BranchId = asset.BranchId,
                                BusinessUnitId = asset.BusinessUnitId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };

                            depreList.Add(depre);
                            //accounting

                            #endregion
                        }

                        else
                        {
                            errors.Add($"Depreciation rule not found for category {asset.FixedAssetCategory.CategoryName}");

                        }

                    }

                    //save depreciation list
                    _db.IfrsDepreciationItems.AddRange(depreList);
                    _db.SaveChanges();

                    #region distribution

                    _distributionHelper.UpdateIfrsDepreciationDistribution(ifrs
                        .IfrsDepreciationId);

                    #endregion
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Fiscal year is not found",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var formattedData = new
                {
                    Message = "Please select Fiscal year",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (errors.Any())
            {
                var formattedData = new
                {
                    Message = errors,
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.IfrsDepreciations.Find(id);
            #region check segment length validation

            var distributions = _db.IfrsDepreciationDistributions.Where(x => x.IfrsDepreciationId == id).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
            {
                string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }


            #endregion


            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IfrsDepreciationItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "There are no depreciation items.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsSendForApproval && !record.IsOnlineApproved && record.IfrsDepreciationItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.IfrsDepreciations.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.IfrsDepreciations.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                   .Where(x => x.Source == JournalSources.PPE_IFRS_Depreciation.ToString())
                   .Where(x => x.SourceId == record.IfrsDepreciationId.ToString())
                   .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.IfrsDepreciationDistributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.IfrsDepreciationDistributions.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period

                FiscalYearHelper yearHelper = new FiscalYearHelper();
                int lastPeriodId = yearHelper.GetFiscalYearLastPeriod(record.GlFiscalYearId);
                var period = _db.GLPeriods.Find(lastPeriodId);
                if (period != null)
                {
                    var myPeriod = period;
                    glperiod = myPeriod.Name;
                    glyear = myPeriod.GlFiscalYear.Name.ToString();
                    if (myPeriod.Status == FiscalYearStatus.Close)
                    {
                        var formattedData = new
                        {
                            Message = "The period " + glperiod +
                                      " is closed,the record cannot be transfered to closed period.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Period not found.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (record.IfrsDepreciationDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "PPE Ifrs Distribution",
                        Description = !String.IsNullOrEmpty(description) ? description : "PPE Ifrs Distribution",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "PPE Ifrs Distribution",
                        SourcesName = "PPE",
                        JournalNumber = "PPEMD." + record.IfrsDepreciationId.ToString(),
                        SourceId = record.IfrsDepreciationId.ToString(),
                        Source = JournalSources.PPE_IFRS_Depreciation.ToString(),
                        EffectiveDate = period.DateTo,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.IfrsDepreciationDistributions)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }


                    #region update record

                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion

                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion

                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public static int DiffMonths(DateTime startDate, DateTime endDate)
        {
            return (startDate.Year * 12 + startDate.Month + startDate.Day / System.DateTime.DaysInMonth(startDate.Year, startDate.Month))
                   - (endDate.Year * 12 + endDate.Month + endDate.Day / System.DateTime.DaysInMonth(endDate.Year, endDate.Month));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}