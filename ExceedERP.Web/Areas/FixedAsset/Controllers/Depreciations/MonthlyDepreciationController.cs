using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{

   
    public class MonthlyDepreciationController : Controller
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
            var assetRegistrations = _db.AssetRegistrations;

            ViewData["Years"] = years;
            ViewData["Periods"] = periods;
            ViewData["Branches"] = branches;
            ViewData["Assets"] = assetRegistrations;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
        public ActionResult MonthlyDepreciations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MonthlyDepreciation> depreciations = _db.MonthlyDepreciations;

            DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
            {
                MonthlyDepreciationId = depreciation.MonthlyDepreciationId,
                GlFiscalYearId = depreciation.GlFiscalYearId,
                GLPeriodId = depreciation.GLPeriodId,
                BranchId = depreciation.BranchId,
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
        public ActionResult MonthlyDepreciationItems_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<MonthlyDepreciationItem> monthlyDepreciationItems = _db.MonthlyDepreciationItems.Where(x=>x.MonthlyDepreciationId==id);

            DataSourceResult result = monthlyDepreciationItems.ToDataSourceResult(request, monthlyDepreciationItem => new
            {
                MonthlyDepreciationId = monthlyDepreciationItem.MonthlyDepreciationId,
                AssetRegistrationId = monthlyDepreciationItem.AssetRegistrationId,
                DepreciationAmount = monthlyDepreciationItem.DepreciationAmount,
                BookValue = monthlyDepreciationItem.BookValue,
                
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
        //        var depreciations = _db.annual.FirstOrDefault(x => x.AssetRegistrationId == registration.AssetRegistrationId && x.GlFiscalYearId == period);
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
        public JsonResult RunDepreciation(string fiscalyearid,string periodid, string branchid)
        {
            List<string> errors = new List<string>();

            if (!String.IsNullOrEmpty(periodid) && !String.IsNullOrEmpty(branchid))
            {
                int yearidd = int.Parse(fiscalyearid);
                int periodidd = int.Parse(periodid);
                int branchidd = int.Parse(branchid);
                var branch = _db.Branch.Find(branchidd);
                var glPeriod = _db.GLPeriods.Find(periodidd);


                if (glPeriod != null && branch != null)
                {
                    DateTime dateTo = glPeriod.DateTo;
                    DateTime dateFrom = glPeriod.DateFrom;
                    var rules = _db.DepretiationRules.ToList(); //for sistribution
                    var groups = _db.FixedAssetGroups.AsNoTracking().ToList();
                    var activeAssets = _db.AssetRegistrations
                        .Where(x => x.Status == RegistrationStatus.Active || x.Status==RegistrationStatus.Revalued)
                        .ToList();

                    activeAssets = activeAssets.Where(x => x.BranchId == branchidd)
                        .Where(x => x.GRNDate <= dateTo).ToList();

                    var monthlyDepreciation = _db.MonthlyDepreciations
                        .FirstOrDefault(x => x.GLPeriodId == glPeriod.GLPeriodId && x.BranchId == branch.BranchId);

                    if (monthlyDepreciation == null)
                    {
                        monthlyDepreciation = new MonthlyDepreciation
                        {
                            GlFiscalYearId = yearidd,
                            GLPeriodId = periodidd,
                            BranchId = branchidd,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            Status="Generated"
                        };
                        _db.MonthlyDepreciations.Add(monthlyDepreciation);
                        _db.SaveChanges();
                    }

                    foreach (var asset in activeAssets)
                    {
                        #region skip if deprecation is done for this item and year

                        var exist = _db.MonthlyDepreciationItems
                            .Where(x => x.MonthlyDepreciation.GLPeriodId == glPeriod.GLPeriodId)
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

                            var depreciationPercentage = assetGroup.StraightLinePercentage;
                            double days = 0;
                            days = dateFrom > (DateTime) asset.GRNDate
                                ? (dateTo - dateFrom).TotalDays
                                : (dateTo - (DateTime) asset.GRNDate).TotalDays;
                            decimal rate = (decimal) depreciationPercentage;
                            decimal depreciated = (rate * asset.PurchasePrice * (decimal) days) / (365 * 100);


                            _db.Entry(asset).State = EntityState.Modified;
                            _db.SaveChanges();
                            //register the for item
                            var depre = new MonthlyDepreciationItem
                            {
                                MonthlyDepreciationId = monthlyDepreciation.MonthlyDepreciationId,
                                AssetRegistrationId = asset.AssetRegistrationId,
                                DepreciationAmount = depreciated,
                                BookValue = asset.BookValue,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };
                            _db.MonthlyDepreciationItems.Add(depre);
                            _db.SaveChanges();
                            //accounting

                            #endregion
                        }
                        else
                        {
                            string error = "Depreciation rule not found for asset " +
                                           asset.FixedAssetCategory.CategoryName;
                            errors.Add(error);

                        }

                    }

                    #region distribution

                    _distributionHelper.UpdateMonthlyDepreciationDistribution(monthlyDepreciation
                        .MonthlyDepreciationId);

                    #endregion
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
            var record = _db.MonthlyDepreciations.Find(id);
            #region check segment length validation

            var distributions = _db.MonthlyDepreciationDistributions.Where(x => x.MonthlyDepreciationId == id).ToList();
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

            if (record.MonthlyDepreciationItems.Count == 0)
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

            if (!record.IsSendForApproval && !record.IsOnlineApproved && record.MonthlyDepreciationItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.MonthlyDepreciations.Attach(record);
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
            var record = _db.MonthlyDepreciations.Find(myid);
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
                   .Where(x => x.Source == JournalSources.PPE_Monthly_Distribution.ToString())
                   .Where(x => x.SourceId == record.MonthlyDepreciationId.ToString())
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
                foreach (var dist in record.MonthlyDepreciationDistributions)
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
                var distribution = record.MonthlyDepreciationDistributions.ToList();
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

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Find(record.GLPeriodId);
                if (period!=null)
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

                if (record.MonthlyDepreciationDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "PPE Monthly Distribution",
                        Description = !String.IsNullOrEmpty(description) ? description : "PPE Monthly Distribution",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "PPE Monthly Distribution",
                        SourcesName = "PPE",
                        JournalNumber = "PPEMD." + record.MonthlyDepreciationId.ToString(),
                        SourceId = record.MonthlyDepreciationId.ToString(),
                        Source = JournalSources.PPE_Monthly_Distribution.ToString(),
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
                    foreach (var i in record.MonthlyDepreciationDistributions)
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