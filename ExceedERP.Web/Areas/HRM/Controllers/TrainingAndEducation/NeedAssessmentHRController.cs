using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using PagedList;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class NeedAssessmentHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
           
            //ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            GetCourse();
            GetProvider();
            GetProvider1();
            PopulateViewData();
            GetTargetGroup();
            TrainingType();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
            i => new
            {
                FullNameAmharic = i.Code + "-" + i.AmharicName,
                FullNameaEnglish = i.Code + "-" + i.Name,
                EmployeeId = i.Id,
                PersonId = i.Id,
                FullName = i.Name,
                FullNameCode = i.Code + "-" + i.Name,
                FullNameWithOutCode =  i.Name,
                EmployeeCode = i.Code,
            }).AsQueryable();

            ViewData["EmployeeName"] = CacheData.GetEmployeeCaches().Select(
            i => new
            {
                FullNameAmharic = i.Code + "-"+i.AmharicName,
                FullNameaEnglish = i.Name,
                FullName = i.Code + i.Code + "-"+i.Name,
                FullNameCode = i.Code + "-" + i.Name,
                EmployeeCode = i.Code,
                EmployeeId = i.Id,
            }).AsQueryable();

            //var employeName = db.Person_Employee.ToList();
            //ViewData["EmployeeName"] = employeName;
            //var structres = db.MainCompanyStructures.AsQueryable();
            //ViewData["organization"] = structres;
            //GetDepartments();
            //var branch = db.Branch.ToList();
            //ViewData["Branches"] = branch;
            //var periodList = db.GLFiscalYears;
            //ViewData["PeriodList"] = periodList;

            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
                              i => new
                              {
                                  Name = i.Name,
                                  GlFiscalYearId = i.Id,
                              }).AsQueryable();

            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.Id,
              }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable(); 

            ViewData["allDepartments"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["organization"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["Departments"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
              i => new
              {
                  Description = i.Name,
                  LookupId = i.Id,
              }).AsQueryable();
            //ViewData["allDepartments"] = db.MainCompanyStructures.ToList();

            ViewData["TargetGroup"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);
            //var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            //ViewData["businessUnits"] = businessUnits;
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

        }

        public JsonResult GetTargetGroup()
        {
            var targetGroupList = db.Lookups.AsQueryable().ToList();
            if (targetGroupList.Count > 0)
            {
                targetGroupList = targetGroupList.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive).ToList();
            }
            ViewData["TargetGroupList"] = targetGroupList.Count > 0 ? targetGroupList.ToList() : new List<Lookup>();
            return Json(targetGroupList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public JsonResult GetCourse()
        {
            var courseList = db.Lookups.AsQueryable();
            if (courseList.Count() > 0)
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();
            }
            ViewData["CourseList"] = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courseList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TrainingType()
        {
            var traTypeList = db.Lookups.AsQueryable();
            if (traTypeList.Count() > 0)
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType);

            }
            ViewData["TraTypeList"] = traTypeList != null ? traTypeList.ToList() : new List<Lookup>();
            return Json(traTypeList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider()
        {
            var providerlist = db.Companies.AsQueryable();
            if (providerlist.Count() > 0)
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).AsQueryable();
            }
           // ViewData["Companies"] = providerlist.Count > 0 ? providerlist.ToList() : new List<MainCompany>();
            ViewData["ProviderList"] = providerlist != null ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new { MainCompanyId = p.MainCompanyId, TradeName = p.TradeName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider1()
        {
            var providerlist = db.Companies.AsQueryable().ToList();
            if (providerlist.Any())
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).ToList();
            }
            ViewData["Companies"] = providerlist.Count > 0 ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new
            {
                MainCompanyId = p.MainCompanyId,
                TradeName = p.TradeName
            }), JsonRequestBehavior.AllowGet);
        }
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        [HttpPost]
        public ActionResult SummarizeNeedSummary(int[] assessmentIds, bool selectAll = false)
        {
            // Get user branches (if needed)
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // Start query for assessments sent for approval and not summarized
            IQueryable<NeedAssessment> query = db.NeedAssessments
                .Where(x => x.IsSendForApproval && !x.IsSummarized);

            // Filter by provided assessment IDs
            if (!selectAll && assessmentIds?.Any() == true)
            {
                // Use only the selected IDs from the current page
                query = query.Where(x => assessmentIds.Contains(x.AssesmentId));
            }

            // Optional: Filter by user's branches
            // Uncomment the line below if branch filtering is required
            // query = query.Where(x => myBranches.Contains(x.OrgBranchName));

            // Execute query
            List<NeedAssessment> needAssessments = query.ToList();

            if (needAssessments.Any())
            {
                var needAssessmentSummaries = needAssessments
                    .GroupBy(x => new { x.GlFiscalYearId, x.LookupId })
                    .Select(g => new NeedAssessmentSummary
                    {
                        GlFiscalYearId = g.Key.GlFiscalYearId,
                        LookupId = g.Key.LookupId,
                        NoOfStaff = g.Sum(s => s.NoOfStaff),
                        RequiredTraining = g.Sum(x => x.RequiredTraining??0),
                        CostPerPerson = 0,
                        Check = true,
                        DateCreated = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name
                    }).ToList();

                foreach (var summary in needAssessmentSummaries)
                {
                    var entity = new NeedAssessmentSummary
                    {
                        GlFiscalYearId = summary.GlFiscalYearId,
                        LookupId = summary.LookupId,
                        NoOfStaff = summary.NoOfStaff,
                        RequiredTraining = 0,
                        CostPerPerson = 0,
                        IsTransfered = false,
                        IsSendForApproval = false,
                        IsSecondSendForApproval = false,
                        Check = false,
                        DateCreated = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name
                    };

                    db.NeedAssessmentSummary.Add(entity);
                    db.SaveChanges();

                    var relatedAssessments = needAssessments
                        .Where(n => n.GlFiscalYearId == summary.GlFiscalYearId 
                        && n.LookupId == summary.LookupId)
                        .Select(n => n.AssesmentId);

                    foreach (var assessmentId in relatedAssessments)
                    {
                        db.NeedAssessmentSummaryDetails.Add(new NeedAssessmentSummaryDetail
                        {
                            AssesmentId = assessmentId,
                            AssesmentSummaryId = entity.NeedAssessmentSummaryId
                        });
                    }
                    db.SaveChanges();
                }

                // Mark assessments as summarized
                foreach (var assessment in needAssessments)
                {
                    assessment.IsSummarized = true;
                    db.Entry(assessment).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new
            {
                Message = "SuccessFully Summerized.",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }
        private bool GetRejectedStatus(int id)
        {
            var summaryId = db.NeedAssessmentSummaryDetails
                              .Where(x => x.AssesmentId == id)
                              .Select(x => x.AssesmentSummaryId)
                              .FirstOrDefault();

            if (summaryId == 0) // No matching detail record found
            {
                return false;
            }

            return db.NeedAssessmentSummary
                     .Where(x => x.NeedAssessmentSummaryId == summaryId)
                     .Select(x => x.IsRejected)
                     .FirstOrDefault();
        }
        public ActionResult NeedAssessmentSummaryDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            HashSet<int> assesmentIds = new HashSet<int>(db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentSummaryId == id).Select(x => x.AssesmentId));
            IQueryable<NeedAssessment> needassessments = db.NeedAssessments.Where(x => assesmentIds.Contains(x.AssesmentId)); ;

            DataSourceResult result = needassessments.ToDataSourceResult(request, needAssessment => new
            {
                AssesmentId = needAssessment.AssesmentId,
                OrgStructureId = needAssessment.OrgStructureId,
                BusinessUnitId = needAssessment.BusinessUnitId,
                NoOfStaff = needAssessment.NoOfStaff,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                RequiredTraining = needAssessment.RequiredTraining,
                IsTransfered = needAssessment.IsTransfered,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                From = needAssessment.From,
                FromAmharic = needAssessment.FromAmharic,
                To = needAssessment.To,
                ToAmharic = needAssessment.ToAmharic,
                TotalDays = needAssessment.TotalDays,
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
                IsSendForApproval = needAssessment.IsSendForApproval,
                IsSummarized = needAssessment.IsSummarized,
                IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
                Check = needAssessment.Check,
                MainCompanyId = needAssessment.MainCompanyId,
                DevelopmentRequired = needAssessment.DevelopmentRequired,
                DevelopmentBy = needAssessment.DevelopmentBy,
                Remark = needAssessment.Remark,
                DateCreated = needAssessment.DateCreated,
                LastModified = needAssessment.LastModified,
                CreatedBy = needAssessment.CreatedBy,
                ModifiedBy = needAssessment.ModifiedBy,
                OrgBranchName = needAssessment.OrgBranchName,
            });
            return Json(result);
        }

        //public ActionResult SummarizeNeedSummary(int[] assessmentIds)
        //{
        //    var needAssessments = db.NeedAssessments.Where(x => assessmentIds.Contains(x.AssesmentId)).ToList();
        //    if (needAssessments.Count() > 0)
        //    {
        //        var needAssessmentSummaries = needAssessments
        //            .GroupBy(x => new
        //            {
        //                x.GlFiscalYearId,
        //                x.LookupId,
        //            }).Select(g => new NeedAssessmentSummary
        //            {
        //                GlFiscalYearId = g.Key.GlFiscalYearId,
        //                LookupId = g.Key.LookupId,
        //                NoOfStaff = g.Sum(s => s.NoOfStaff),
        //                RequiredTraining = g.Sum(x=>x.RequiredTraining),
        //                CostPerPerson = 0,
        //                Check = true,
        //                DateCreated = DateTime.Now.Date,
        //                CreatedBy = User.Identity.Name,
        //                LastModified = null,
        //                ModifiedBy = null

        //            }).ToList();
        //        foreach (var needAssessmentSummary in needAssessmentSummaries)
        //        {
        //            var entity = new NeedAssessmentSummary
        //            {
        //                GlFiscalYearId = needAssessmentSummary.GlFiscalYearId,
        //                LookupId = needAssessmentSummary.LookupId,
        //                NoOfStaff = needAssessmentSummary.NoOfStaff,
        //                RequiredTraining = 0,
        //                CostPerPerson = 0,
        //                IsTransfered = false,
        //                IsSendForApproval = false,
        //                IsSecondSendForApproval = false,
        //                Check = false,
        //                DateCreated = DateTime.Now.Date,
        //                CreatedBy = User.Identity.Name,
        //                LastModified = null,
        //                ModifiedBy = null

        //            };
        //            db.NeedAssessmentSummary.Add(entity);
        //            db.SaveChanges();
        //            var needAssessmentIds = needAssessments.Where(n => n.GlFiscalYearId == needAssessmentSummary.GlFiscalYearId && n.LookupId == needAssessmentSummary.LookupId).Select(n => n.AssesmentId);
        //            foreach(var assessmentId in needAssessmentIds)
        //            {
        //                var entity1 = new NeedAssessmentSummaryDetail
        //                {
        //                    AssesmentId = assessmentId,
        //                    AssesmentSummaryId = entity.NeedAssessmentSummaryId
        //                };
        //                db.NeedAssessmentSummaryDetails.Add(entity1);
        //                db.SaveChanges();
        //            }
        //        }
        //        foreach(var assessmentId in assessmentIds)
        //        {
        //            var needAssessmentUpdate = db.NeedAssessments.Find(assessmentId);
        //            if(needAssessmentUpdate != null)
        //            {
        //                needAssessmentUpdate.IsSummarized = true;
        //                db.NeedAssessments.Attach(needAssessmentUpdate);
        //                db.Entry(needAssessmentUpdate).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //        }

        //    }
        //    return Json("Summarized");
        //}
        //public ActionResult NeedAssessments_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //    IQueryable<NeedAssessment> needassessments = db.NeedAssessments.Where(x=>x.IsSendForApproval && !x.IsSummarized).AsQueryable();;

        //    DataSourceResult result = needassessments.ToDataSourceResult(request, needAssessment => new
        //    {
        //        AssesmentId = needAssessment.AssesmentId,
        //        OrgStructureId = needAssessment.OrgStructureId,
        //        BusinessUnitId = needAssessment.BusinessUnitId,
        //        NoOfStaff = needAssessment.NoOfStaff,
        //        GlFiscalYearId = needAssessment.GlFiscalYearId,
        //        RequiredTraining = needAssessment.RequiredTraining,
        //        IsTransfered = needAssessment.IsTransfered,
        //        LookupId = needAssessment.LookupId,
        //        Description = needAssessment.Description,
        //        Type = needAssessment.Type,
        //        From = needAssessment.From,
        //        FromAmharic = needAssessment.FromAmharic,
        //        To = needAssessment.To,
        //        ToAmharic = needAssessment.ToAmharic,
        //        TotalDays = needAssessment.TotalDays,
        //        IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
        //        IsSendForApproval = needAssessment.IsSendForApproval,
        //        IsSummarized = needAssessment.IsSummarized,
        //        IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
        //        Check = needAssessment.Check,
        //        MainCompanyId = needAssessment.MainCompanyId,
        //        DevelopmentRequired = needAssessment.DevelopmentRequired,
        //        DevelopmentBy = needAssessment.DevelopmentBy,
        //        Remark = needAssessment.Remark,
        //        DateCreated = needAssessment.DateCreated,
        //        LastModified = needAssessment.LastModified,
        //        CreatedBy = needAssessment.CreatedBy,
        //        ModifiedBy = needAssessment.ModifiedBy,
        //        OrgBranchName = needAssessment.OrgBranchName,
        //        IsRejected = GetRejectedStatus(needAssessment.AssesmentId),
        //    });
        //    return Json(result);
        //}
        public ActionResult NeedAssessments_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            var query =
                from na in db.NeedAssessments
                join nad in db.NeedAssessmentSummaryDetails
                    on na.AssesmentId equals nad.AssesmentId into nadGroup
                from nad in nadGroup.DefaultIfEmpty()
                join nas in db.NeedAssessmentSummary
                    on nad.AssesmentSummaryId equals nas.NeedAssessmentSummaryId into nasGroup
                from nas in nasGroup.DefaultIfEmpty()
                where na.IsSendForApproval && na.IsHRLevel && !na.IsSummarized
                select new
                {
                    AssesmentId = na.AssesmentId,
                    BranchId = na.BranchId,
                    OrgStructureId = na.OrgStructureId,
                    BusinessUnitId = na.BusinessUnitId,
                    NoOfStaff = na.NoOfStaff,
                    GlFiscalYearId = na.GlFiscalYearId,
                    RequiredTraining = na.RequiredTraining,
                    IsTransfered = na.IsTransfered,
                    LookupId = na.LookupId,
                    Description = na.Description,
                    Role = na.Role,
                    IsHRLevel = na.IsHRLevel,
                    Type = na.Type,
                    From = na.From,
                    FromAmharic = na.FromAmharic,
                    To = na.To,
                    ToAmharic = na.ToAmharic,
                    TotalDays = na.TotalDays,
                    IsSecondSendForApproval = na.IsSecondSendForApproval,
                    IsSendForApproval = na.IsSendForApproval,
                    IsSummarized = na.IsSummarized,
                    IsThirdSendForApproval = na.IsThirdSendForApproval,
                    Check = na.Check,
                    MainCompanyId = na.MainCompanyId,
                    DevelopmentRequired = na.DevelopmentRequired,
                    DevelopmentBy = na.DevelopmentBy,
                    Remark = na.Remark,
                    DateCreated = na.DateCreated,
                    LastModified = na.LastModified,
                    CreatedBy = na.CreatedBy,
                    ModifiedBy = na.ModifiedBy,
                    OrgBranchName = na.OrgBranchName,
                    IsRejected = nas != null && nas.IsRejected
                };

            DataSourceResult result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Create([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            if (ModelState.Any(entry => entry.Key.StartsWith(nameof(NeedAssessment.SelectedBranchIds), StringComparison.Ordinal)
                && entry.Value.Errors.Count > 0))
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));

            ModelState.Clear();
            var userName = User.Identity.Name;
            var assignedBranches = UserInformationHelper.GetUserBranchsAsInt(userName);
            var selectedBranches = (needAssessment.SelectedBranchIds ?? new int[0]).Distinct().ToArray();
            if (selectedBranches.Length == 0 && needAssessment.BranchId.HasValue)
                selectedBranches = new[] { needAssessment.BranchId.Value };

            if (selectedBranches.Any(branchId => !assignedBranches.Contains(branchId)) ||
                (selectedBranches.Length > 0 && db.Branch.Count(branch => branch.IsActive &&
                    selectedBranches.Contains(branch.BranchId)) != selectedBranches.Length))
            {
                ModelState.AddModelError(nameof(NeedAssessment.SelectedBranchIds), "Select active branches assigned to you.");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
            }

            needAssessment.OrgStructureId = null;
            needAssessment.BusinessUnitId = 0;

            bool datePreference = HrHelper.GetDatePreference();

            if (datePreference)
            {
                // Ethiopic date input
                if (!string.IsNullOrWhiteSpace(needAssessment.FromAmharic))
                {
                    needAssessment.From = HrHelper.ParseEthiopicDate(needAssessment.FromAmharic);
                }

                if (!string.IsNullOrWhiteSpace(needAssessment.ToAmharic))
                {
                    needAssessment.To = HrHelper.ParseEthiopicDate(needAssessment.ToAmharic);
                }
            }
            else
            {
                // Gregorian date input, convert to Amharic
                if (needAssessment.From.HasValue)
                {
                    var from = needAssessment.From.Value;
                    needAssessment.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (needAssessment.To.HasValue)
                {
                    var to = needAssessment.To.Value;
                    needAssessment.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }

            var roles = (needAssessment.Role ?? "").Split(',')
                .Select(role => role.Trim()).Where(role => role.Length > 0).ToList();
            var hasRoles = roles.Count > 0;
            var branchScope = selectedBranches.Length > 0 ? selectedBranches : assignedBranches.ToArray();
            var placements = db.Placements.Where(placement => hasRoles && placement.isCurrent && branchScope.Contains(placement.BranchId))
                .Select(placement => new { placement.EmployeeId, placement.OrgStructureId, placement.LookupId,
                    placement.BranchId, placement.ResponsibilityLevel, placement.AssignmentDate, placement.PlacementId })
                .ToList()
                .Where(placement => roles.Contains(placement.ResponsibilityLevel.ToString()))
                .ToList();

            needAssessment.IsSendForApproval = true;
            needAssessment.IsHRLevel = true;
            needAssessment.DateCreated = DateTime.Now;
            needAssessment.CreatedBy = userName;
            needAssessment.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(userName);

            var assessmentBranches = selectedBranches.Length == 0
                ? new int?[] { null }
                : selectedBranches.Select(branchId => (int?)branchId).ToArray();
            var assessments = assessmentBranches.Select(branchId =>
            {
                var trainees = placements.Where(placement => !branchId.HasValue || placement.BranchId == branchId.Value)
                    .GroupBy(placement => placement.EmployeeId)
                    .Select(group => group.OrderByDescending(placement => placement.AssignmentDate)
                        .ThenByDescending(placement => placement.PlacementId).First())
                    .ToList();
                var assessment = GetMappedObject(needAssessment);
                assessment.BranchId = branchId;
                assessment.NoOfStaff = hasRoles ? trainees.Count : needAssessment.NoOfStaff;
                return new { Assessment = assessment, Trainees = trainees };
            }).ToList();

            if (assessments.Any(item => item.Assessment.NoOfStaff <= 0))
            {
                ModelState.AddModelError("", hasRoles
                    ? "The selected roles must have employees in each selected branch."
                    : "The staff count must be greater than zero.");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                db.NeedAssessments.AddRange(assessments.Select(item => item.Assessment));
                db.SaveChanges();

                db.TrainingNeedRequests.AddRange(assessments.SelectMany(item => item.Trainees.Select(placement =>
                    new TrainingNeedRequest
                    {
                        AssesmentId = item.Assessment.AssesmentId,
                        EmployeeId = placement.EmployeeId,
                        OrgStructureId = placement.OrgStructureId,
                        GlFiscalYearId = item.Assessment.GlFiscalYearId,
                        LookupId = placement.LookupId
                    })));
                db.SaveChanges();
                transaction.Commit();
            }

            var employeeId = UserInformationHelper.GetEmployeeId(userName);
            var employee = db.Person_Employee.FirstOrDefault(person => person.EmployeeId == employeeId);
            if (employee != null)
            {
                UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", userName,
                    employee.EmployeeCode + " added " + assessments.Count + " need assessment(s)", Modules.HRM);
            }

            // Kendo expects one result for the submitted row. The grid re-reads after create.
            return Json(new[] { assessments[0].Assessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Update([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                // Ethiopic date input
                if (!string.IsNullOrWhiteSpace(needAssessment.FromAmharic))
                {
                    needAssessment.From = HrHelper.ParseEthiopicDate(needAssessment.FromAmharic);
                }

                if (!string.IsNullOrWhiteSpace(needAssessment.ToAmharic))
                {
                    needAssessment.To = HrHelper.ParseEthiopicDate(needAssessment.ToAmharic);
                }
            }
            else
            {
                // Gregorian date input, convert to Amharic
                if (needAssessment.From.HasValue)
                {
                    var from = needAssessment.From.Value;
                    needAssessment.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (needAssessment.To.HasValue)
                {
                    var to = needAssessment.To.Value;
                    needAssessment.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }
            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            if (!string.IsNullOrWhiteSpace(needAssessment.Role) && placements.Any())
            {
                var roles = needAssessment.Role.Split(',')
                                   .Select(x => x.Trim())
                                   .ToList();


                placements = placements.Where(e => roles.Contains(e.ResponsibilityLevel.ToString()) && e.isCurrent).ToList();
                needAssessment.NoOfStaff = placements.Count();
            }

            if (needAssessment.NoOfStaff <= 0)
            {
                ModelState.AddModelError("", "No of staff should be greater than zero");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));

            }
            needAssessment.LastModified = DateTime.UtcNow;
            needAssessment.ModifiedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(needAssessment);
                db.NeedAssessments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrWhiteSpace(needAssessment.Role)&& placements.Any())
                {
                    var roles = needAssessment.Role.Split(',')
                                       .Select(x => x.Trim())
                                       .ToList();

                    placements = placements.Where(e => roles.Contains(e.ResponsibilityLevel.ToString()) && e.isCurrent).ToList();
                    if (needAssessment.BranchId.HasValue)
                    {
                        placements = placements.Where(e => e.BranchId == needAssessment.BranchId).ToList();
                    }
                    foreach (var place in placements)
                    {
                        var trainingNeedRequests = db.TrainingNeedRequests.FirstOrDefault(x => x.EmployeeId == place.EmployeeId && x.AssesmentId == needAssessment.AssesmentId);
                        if(trainingNeedRequests == null)
                        {
                            var Trainingentity = new TrainingNeedRequest
                            {
                                AssesmentId = needAssessment.AssesmentId,
                                EmployeeId = place.EmployeeId,
                                OrgStructureId = place.OrgStructureId,
                                GlFiscalYearId = needAssessment.GlFiscalYearId,
                                LookupId = place.LookupId,


                            };
                            db.TrainingNeedRequests.Add(Trainingentity);
                        }
                        

                    }
                    db.SaveChanges();

                }

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", User.Identity.Name, name.EmployeeCode + " update Need Assessment Info updated date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                }

                #endregion
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Destroy([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(needAssessment);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + needAssessment.LookupId);
                #endregion
                db.NeedAssessments.Attach(entity);
                db.NeedAssessments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }

        private NeedAssessment GetMappedObject(NeedAssessment needAssessment)
        {
            var entity = new NeedAssessment
            {
                AssesmentId = needAssessment.AssesmentId,
                BranchId = needAssessment.BranchId,
                OrgStructureId = needAssessment.OrgStructureId,
                BusinessUnitId = needAssessment.BusinessUnitId,
                NoOfStaff = needAssessment.NoOfStaff,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                RequiredTraining = needAssessment.RequiredTraining,
                Role = needAssessment.Role,
                IsHRLevel = needAssessment.IsHRLevel,
                IsTransfered = needAssessment.IsTransfered,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                From = needAssessment.From,
                FromAmharic = needAssessment.FromAmharic,
                To = needAssessment.To,
                ToAmharic = needAssessment.ToAmharic,
                TotalDays = needAssessment.TotalDays,
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
                IsSendForApproval = needAssessment.IsSendForApproval,
                IsSummarized = needAssessment.IsSummarized,
                IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
                Check = needAssessment.Check,
                MainCompanyId = needAssessment.MainCompanyId,
                DevelopmentRequired = needAssessment.DevelopmentRequired,
                DevelopmentBy = needAssessment.DevelopmentBy,
                Remark = needAssessment.Remark,
                DateCreated = needAssessment.DateCreated,
                LastModified = needAssessment.LastModified,
                CreatedBy = needAssessment.CreatedBy,
                ModifiedBy = needAssessment.ModifiedBy,
                OrgBranchName = needAssessment.OrgBranchName,
            };
            return entity;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Transfer_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<NeedAssessment> needAssessments)
        {
            ModelState.Clear();
            var results = new List<NeedAssessment>();


            TrainingPlan trainingPlan = new TrainingPlan();
            List<TrainingPlan> planList = new List<TrainingPlan>();

            ExceedDbContext dbs = new ExceedDbContext();

            List<NeedAssessment> needAssessment = db.NeedAssessments.ToList();

            if (needAssessments.Any() && ModelState.IsValid)
            {
                foreach (var check in needAssessments)
                {
                    if (check.Check)
                    {
                        trainingPlan = new TrainingPlan();

                        //trainingPlan.GlFiscalYearId = product.MainCompanyId;
                        //trainingPlan.GlFiscalYearId = 5;//check.GlFiscalYearId,  
                        trainingPlan.WorkUnit = check.OrgStructureId??0;
                        trainingPlan.LookupId = check.LookupId;
                        trainingPlan.MainCompanyId = check.MainCompanyId;
                        trainingPlan.GlFiscalYearId = check.GlFiscalYearId;
                        //trainingPlan.WorkUnit = check.WorkUnit,  
                        //trainingPlan.CostPerPerson = check.CostPerPerson,
                        //trainingPlan.TotalCost = check.TotalCost,                       
                        trainingPlan.Course = check.LookupId;
                        trainingPlan.NoofTrainee = check.NoOfStaff;
                        trainingPlan.OrgBranchName = check.OrgBranchName;
                        //trainingPlan.LookupId = check.LookupId;
                        trainingPlan.CreatedBy = User.Identity.Name;
                        trainingPlan.DateCreated = DateTime.Now;
                        trainingPlan.ModifiedBy = null;
                        UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        trainingPlan.LastModified = null;
                        planList.Add(trainingPlan);

                        db.TrainingPlans.Add(trainingPlan);
                        db.SaveChanges();

                        #region remove transfered assessment
                        TrainingAssessmentViewModel train = new TrainingAssessmentViewModel();
                        // train.TrainingAssessmentViewModelId=check
                        var assessment = db.NeedAssessments.Find(check.AssesmentId);
                        if (assessment != null)
                        {
                            assessment.IsTransfered = true;

                            db.Entry(assessment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                    }


                }
             
            }

            return Json(planList.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetJobTitle(int OrganizationId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrganizationId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    titleList.Add(lookup);

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
