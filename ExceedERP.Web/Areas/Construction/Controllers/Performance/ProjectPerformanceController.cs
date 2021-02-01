using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    [AuthorizeRoles(ConstructionConstants.ProjectTracking)]
    public class ProjectPerformanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ProjectPerformance
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId}).ToList();

            ViewData["ConstructionPeriods"] = db.ConstructionPeriod.Select(p => new { p.Name, p.ConstructionPeriodId}).ToList();

            ViewData["ItemCategorys"] = db.ConstructionItemCategories.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(itm => new { itm.Name, itm.ItemCode}).ToList();

            return View();
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract;
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            var mainContrats = allContracts.Where(main => !ids.Contains(main.ProjectContractId));

            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var projIds = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));
            var allowedProjects = db.ProjectInformation.Where(p => !p.IsDeleted && projIds.Contains(p.ProjectInformationId));

            var joinedInfo = (from p in allowedProjects
                              join c in mainContrats
                              on p.ProjectInformationId equals c.ProjectInformationId

                              select new
                              {
                                  ProjectInformationId = p.ProjectInformationId,
                                  ProjectName = p.Name,
                                  ProjectType = p.ProjectType,
                                  ProjectCode = p.Code,
                                  ContractSigningDate = c.ContractSigningDate,
                                  CommencementDate = c.CommencementDate,
                                  ContractAmount = c.ContractAmount,
                                  RevisedContractAmount = c.RevisedContractAmount,
                                  OriginalcompletionTime = c.ContractTime,
                                  RevisedCompletionDate = c.RevisedCompletionDate,
                                  ContractPeriod = c.ContractPeriod,
                                  ApprovedTimeExtension = c.ApprovedTimeExtension,
                                  TotalProjectPeriod = c.TotalProjectPeriod,
                                  TimeElapsedToDate = c.TimeElapsedToDate,
                                  Rebate = c.Rebate,
                                  Variation = c.Variation,
                                  Supplemetary = c.Supplemetary,
                                  TotalProjectAmount = c.TotalProjectAmount,
                                  NetGrand = c.NetGrand,
                                  ExecutedToDate = c.ExecutedToDate
                              }).AsQueryable();

            IQueryable<PerformanceProjectGeneralInformationVM> list = joinedInfo.Select(generalInformation => new PerformanceProjectGeneralInformationVM
            {
                ProjectInformationId = generalInformation.ProjectInformationId,
                ProjectName = generalInformation.ProjectName,
                ProjectCode = generalInformation.ProjectCode,
                ProjectType =generalInformation.ProjectType,

                ContractSigningDate = generalInformation.ContractSigningDate.ToString(),
                CommencementDate = generalInformation.CommencementDate.ToString(),
                ContractPeriod = generalInformation.ContractPeriod == null ? 0 : generalInformation.ContractPeriod,
                ApprovedTimeExtension = generalInformation.ApprovedTimeExtension == null ? 0 : generalInformation.ApprovedTimeExtension,
                TotalProjectPeriod = generalInformation.TotalProjectPeriod == null ? 0 : generalInformation.TotalProjectPeriod,
                OriginalcompletionTime = generalInformation.OriginalcompletionTime.ToString(),
                RevisedCompletionDate = generalInformation.RevisedCompletionDate.ToString(),
                TimeElapsedToDate = generalInformation.TimeElapsedToDate == null ? 0 : generalInformation.TimeElapsedToDate,

                ContractAmount = generalInformation.ContractAmount,
                Rebate = generalInformation.Rebate == null ? 0 : generalInformation.Rebate,
                Variation = generalInformation.Variation == null ? 0 : generalInformation.Variation,
                Supplemetary = generalInformation.Supplemetary == null ? 0 : generalInformation.Supplemetary,
                TotalProjectAmount = generalInformation.TotalProjectAmount == null ? 0 : generalInformation.TotalProjectAmount,
                NetGrand = generalInformation.NetGrand == null ? 0 : generalInformation.NetGrand,
                ExecutedToDate = generalInformation.ExecutedToDate == null ? 0 : generalInformation.ExecutedToDate
            });
            

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConstructionPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<ConstructionPeriod> constructionPeriods = db.ConstructionPeriod;
            DataSourceResult result = constructionPeriods.ToDataSourceResult(request, gLPeriod => new
            {

                ConstructionPeriodId = gLPeriod.ConstructionPeriodId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }

        public decimal GetCalculatedPlannedAmountTillPreviousMonth(int periodMappingId)
        {
            var amntTillPrevMonth = new decimal();
            var currentPeriodMapping = db.PeriodMapping.Find(periodMappingId);
            if(currentPeriodMapping != null)
            {
                var allEvaluationPeriods = 
                    db.PeriodMapping.Where(p => p.ProjectInformationId == currentPeriodMapping.ProjectInformationId &&
                    p.ConstructionPeriod.DateTo < currentPeriodMapping.ConstructionPeriod.DateTo
                ).OrderBy(pm => pm.ConstructionPeriod.DateTo).ToList();
                var prevPeriodEvaluationPeriod =
                    allEvaluationPeriods.LastOrDefault();
                if(prevPeriodEvaluationPeriod != null)
                {
                    var progressRecordOfPrevMonth = db.MonthlyProjectProgress.FirstOrDefault(pr => pr.PeriodMappingId == prevPeriodEvaluationPeriod.PeriodMappingId);
                    if(progressRecordOfPrevMonth != null)
                    {
                        amntTillPrevMonth = progressRecordOfPrevMonth.PlannedToDate;
                    }
                }
            }
            return amntTillPrevMonth;
        }
        public decimal GetCalculatedExecutedAmountTillPreviousMonth(int periodMappingId)
        {
            var amntTillPrevMonth = new decimal();
            var currentPeriodMapping = db.PeriodMapping.Find(periodMappingId);
            if (currentPeriodMapping != null)
            {
                var allEvaluationPeriods =
                    db.PeriodMapping.Where(p => p.ProjectInformationId == currentPeriodMapping.ProjectInformationId &&
                    p.ConstructionPeriod.DateTo < currentPeriodMapping.ConstructionPeriod.DateTo
                ).OrderBy(pm => pm.ConstructionPeriod.DateTo).ToList();
                var prevPeriodEvaluationPeriod =
                    allEvaluationPeriods.LastOrDefault();
                if (prevPeriodEvaluationPeriod != null)
                {
                    var progressRecordOfPrevMonth = db.MonthlyProjectProgress.FirstOrDefault(pr => pr.PeriodMappingId == prevPeriodEvaluationPeriod.PeriodMappingId);
                    if (progressRecordOfPrevMonth != null)
                    {
                        amntTillPrevMonth = progressRecordOfPrevMonth.ExecutedAmountToDate;
                    }
                }
            }
            return amntTillPrevMonth;
        }
        public decimal GetCalculatedPriceAdjustmentTillPreviousMonth(int periodMappingId)
        {
            var amntTillPrevMonth = new decimal();
            var currentPeriodMapping = db.PeriodMapping.Find(periodMappingId);
            if (currentPeriodMapping != null)
            {
                var allEvaluationPeriods =
                    db.PeriodMapping.Where(p => p.ProjectInformationId == currentPeriodMapping.ProjectInformationId &&
                    p.ConstructionPeriod.DateTo < currentPeriodMapping.ConstructionPeriod.DateTo
                ).OrderBy(pm => pm.ConstructionPeriod.DateTo).ToList();
                var prevPeriodEvaluationPeriod =
                    allEvaluationPeriods.LastOrDefault();
                if (prevPeriodEvaluationPeriod != null)
                {
                    var progressRecordOfPrevMonth = db.MonthlyProjectProgress.FirstOrDefault(pr => pr.PeriodMappingId == prevPeriodEvaluationPeriod.PeriodMappingId);
                    if (progressRecordOfPrevMonth != null)
                    {
                        amntTillPrevMonth = progressRecordOfPrevMonth.PriceAdjustmentToDate;
                    }
                }
            }
            return amntTillPrevMonth;
        }


        public decimal GetCalculatedActualDirectCostTillPreviousMonth(int periodMappingId)
        {
            var amnt = new decimal();
            var currentPeriodMapping = db.PeriodMapping.Find(periodMappingId);
            if (currentPeriodMapping != null)
            {
                var allEvaluationPeriods =
                    db.PeriodMapping.Where(p => p.ProjectInformationId == currentPeriodMapping.ProjectInformationId &&
                    p.ConstructionPeriod.DateTo < currentPeriodMapping.ConstructionPeriod.DateTo
                ).OrderBy(pm => pm.ConstructionPeriod.DateTo).ToList();
                var prevPeriodEvaluationPeriod =
                    allEvaluationPeriods.LastOrDefault();
                if (prevPeriodEvaluationPeriod != null)
                {
                    var costRecordOfPreviousMonth = db.MonthlyProjectCost.FirstOrDefault(pr => pr.PeriodMappingId == prevPeriodEvaluationPeriod.PeriodMappingId);
                    if (costRecordOfPreviousMonth != null)
                    {
                        amnt = costRecordOfPreviousMonth.ActualDirectCostToDate;
                    }
                }
            }
            return amnt;
        }
        public decimal GetCalculatedActualProjectOverHeadCostTillPreviousMonth(int periodMappingId)
        {
            var amnt = new decimal();
            var currentPeriodMapping = db.PeriodMapping.Find(periodMappingId);
            if (currentPeriodMapping != null)
            {
                var allEvaluationPeriods =
                    db.PeriodMapping.Where(p => p.ProjectInformationId == currentPeriodMapping.ProjectInformationId &&
                    p.ConstructionPeriod.DateTo < currentPeriodMapping.ConstructionPeriod.DateTo
                ).OrderBy(pm => pm.ConstructionPeriod.DateTo).ToList();
                var prevPeriodEvaluationPeriod =
                    allEvaluationPeriods.LastOrDefault();
                if (prevPeriodEvaluationPeriod != null)
                {
                    var costRecordOfPreviousMonth = db.MonthlyProjectCost.FirstOrDefault(pr => pr.PeriodMappingId == prevPeriodEvaluationPeriod.PeriodMappingId);
                    if (costRecordOfPreviousMonth != null)
                    {
                        amnt = costRecordOfPreviousMonth.ActualProjectOHCostToDate;
                    }
                }
            }
            return amnt;
        }
    }
}