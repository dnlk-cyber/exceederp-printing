using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeCardsController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public ActionResult DashBoard()
        {
            return View();
        }
        public JsonResult GetEmployeeCardCount()
        {
        
        var New_Placement = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.New_Placement).ToList();
        var Placement_Change = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Placement_Change).ToList();
        var Leave = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Leave).ToList();
        var Attendance = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Attendance).ToList();
        var Delegation = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Delegation).ToList();
        var Accident = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Accident).ToList();
        var Temination = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Temination).ToList();
        var Others = db.EmployeeCards.Where(x => x.IsActive && x.CardType == EmployeeCardType.Others).ToList();
                                                                                                                                      
            var formattedData = new
            {
                Data = new int[] { New_Placement.Count(), Placement_Change.Count(), Leave.Count(), Attendance.Count(), Delegation.Count(),Accident.Count(), Temination.Count(), Others.Count() },
                Labels = new string[] { "New_Placement", "Placement_Change", "Leave", "Attendance", "Delegation", "Accident", "Temination", "Others" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountEmployeeCardActionStatu()
        {
            var activeCard = db.EmployeeCards
                .Where(x => x.IsActive ).ToList();
            var inactive = db.EmployeeCards
                .Where(x => !x.IsActive ).ToList();
         


            var formattedData = new
            {
                Data = new int[]
                {

                    activeCard.Count(), inactive.Count(),activeCard.Count()+inactive.Count()

                },
                Labels = new string[] { "Action Taken Cards", "No action Taken cards",  "Total Cards" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        private void PopulateViewData()
        {

            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable(); ;
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = CacheData.GetStepCaches().Select(
               i => new
               {
                   StepName = i.Name,
                   StepId = i.Id,

               }).AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.Code+" "+i.AmharicName,
                   FullNameaEnglish = i.Code + " "+i.Name,
                   EmployeeId = i.Id,
                   ToEmployeeId = i.Id,
                   HeadId = i.Id
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
                   FromLookupId = i.Id,
               }).AsQueryable();
            ViewData["branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
                   FromBranchId = i.Id,
               }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
                   FromBusinessUnitId = i.Id,
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
                   FromOrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["isManual"] = db.GeneralJobtitleSettings.FirstOrDefault()?.IsManual;
            var alloanceList = db.AllowanceDefinations.AsQueryable();
            ViewData["AllowanceList"] = alloanceList.OrderBy(o => o.AllowanceName);

        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeeCards_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<EmployeeCard> employeeCards = db.EmployeeCards.Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = employeeCards.ToDataSourceResult(request, employeeCard => new
            {
                EmployeeCardId = employeeCard.EmployeeCardId,
                EmployeeId = employeeCard.EmployeeId,
                ToEmployeeId = employeeCard.ToEmployeeId,
                OrgStructureId = employeeCard.OrgStructureId,
                FromBranchId = employeeCard.FromBranchId,
                FromBusinessUnitId = employeeCard.FromBusinessUnitId,
                FromOrgStructureId = employeeCard.FromOrgStructureId,
                FromLookupId = employeeCard.FromLookupId,
                ActionTakenDate = employeeCard.ActionTakenDate,
                ActionTaken = employeeCard.ActionTaken,
                ActionTakenBy = employeeCard.ActionTakenBy,
                LookupId = employeeCard.LookupId,
                BranchId = employeeCard.BranchId,
                BusinessUnitId = employeeCard.BusinessUnitId,
                EachPrimaryId = employeeCard.EachPrimaryId,
                CardType = employeeCard.CardType,
                IsActive = employeeCard.IsActive,
                Salary = employeeCard.Salary,
                Remark = employeeCard.Remark,
                FromDate = employeeCard.FromDate,
                FromDateAm = employeeCard.FromDateAm,
                ToDate = employeeCard.ToDate,
                ToDateTimeAm = employeeCard.ToDateTimeAm,
                CreatedBy = employeeCard.CreatedBy,
                DateCreated = employeeCard.DateCreated,
                ModifiedBy = employeeCard.ModifiedBy,
                LastModified = employeeCard.LastModified,
                OrgBranchName = employeeCard.OrgBranchName
            });

            return Json(result);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Create([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {

            ModelState.Clear();

            bool useEthiopianDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (useEthiopianDate)
            {
                employeeCard.FromDate = string.IsNullOrWhiteSpace(employeeCard?.FromDateAm)
                    ? (DateTime?)null
                    : HrHelper.ParseEthiopicDate(employeeCard.FromDateAm);

                employeeCard.ToDate = string.IsNullOrWhiteSpace(employeeCard?.ToDateTimeAm)
                    ? (DateTime?)null
                    : HrHelper.ParseEthiopicDate(employeeCard.ToDateTimeAm);
            }
            else
            {
                if (employeeCard.FromDate.HasValue)
                {
                    var from = employeeCard.FromDate.Value;
                    employeeCard.FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (employeeCard.ToDate.HasValue)
                {
                    var to = employeeCard.ToDate.Value;
                    employeeCard.ToDateTimeAm = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }



            if (employeeCard.FromDate is DateTime fromDate &&
                employeeCard.ToDate is DateTime toDate &&
                fromDate > toDate)
            {
                ModelState.AddModelError("", "From date should be less than or equal to the to date.");
                return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                employeeCard.CreatedBy = User.Identity.Name;
                employeeCard.DateCreated = DateTime.Now;
                employeeCard.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var entity = GetMappedObject(employeeCard);
                db.EmployeeCards.Add(entity);
                db.SaveChanges();
                employeeCard.EmployeeCardId = entity.EmployeeCardId;
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeCard", "Success", User.Identity.Name, "Employee employeeCard for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Update([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {
            ModelState.Clear();

            bool useEthiopianDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (useEthiopianDate)
            {
                employeeCard.FromDate = string.IsNullOrWhiteSpace(employeeCard?.FromDateAm)
                    ? (DateTime?)null
                    : HrHelper.ParseEthiopicDate(employeeCard.FromDateAm);

                employeeCard.ToDate = string.IsNullOrWhiteSpace(employeeCard?.ToDateTimeAm)
                    ? (DateTime?)null
                    : HrHelper.ParseEthiopicDate(employeeCard.ToDateTimeAm);
            }
            else
            {
                if (employeeCard.FromDate.HasValue)
                {
                    var from = employeeCard.FromDate.Value;
                    employeeCard.FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (employeeCard.ToDate.HasValue)
                {
                    var to = employeeCard.ToDate.Value;
                    employeeCard.ToDateTimeAm = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }


            if (employeeCard.FromDate is DateTime fromDate &&
               employeeCard.ToDate is DateTime toDate &&
               fromDate > toDate)
            {
                ModelState.AddModelError("", "From date should be less than or equal to the to date.");
                return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(employeeCard);
                db.EmployeeCards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {

                    UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeCard", "Success", User.Identity.Name, "Emoployee employeeCard of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin,HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCards_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeCard employeeCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeCard);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employeeCard information of employee Id" + employeeCard.Employee.EmployeeCode);
                #endregion
                db.EmployeeCards.Attach(entity);
                db.EmployeeCards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeCard }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeCard GetMappedObject(EmployeeCard employeeCard)
        {
            var entity = new EmployeeCard
            {
                EmployeeCardId = employeeCard.EmployeeCardId,
                EmployeeId = employeeCard.EmployeeId,
                ToEmployeeId = employeeCard.ToEmployeeId,
                FromBranchId = employeeCard.FromBranchId,
                FromBusinessUnitId = employeeCard.FromBusinessUnitId,
                FromOrgStructureId = employeeCard.FromOrgStructureId,
                FromLookupId = employeeCard.FromLookupId,
                ActionTakenDate = employeeCard.ActionTakenDate,
                ActionTaken = employeeCard.ActionTaken,
                ActionTakenBy = employeeCard.ActionTaken,
                OrgStructureId = employeeCard.OrgStructureId,
                LookupId = employeeCard.LookupId,
                BranchId = employeeCard.BranchId,
                BusinessUnitId = employeeCard.BusinessUnitId,
                EachPrimaryId = employeeCard.EachPrimaryId,
                CardType = employeeCard.CardType,
                Remark = employeeCard.Remark,
                IsActive = employeeCard.IsActive,
                Salary = employeeCard.Salary,
                FromDate = employeeCard.FromDate,
                FromDateAm = employeeCard.FromDateAm,
                ToDate = employeeCard.ToDate,
                ToDateTimeAm = employeeCard.ToDateTimeAm,
                CreatedBy = employeeCard.CreatedBy,
                DateCreated = employeeCard.DateCreated,
                ModifiedBy = employeeCard.ModifiedBy,
                LastModified = employeeCard.LastModified,
                OrgBranchName = employeeCard.OrgBranchName
            };
            return entity;
        }

        public ActionResult EnableEployeeCardAction(int id)
        {
            // Find the incoming letter by ID
            var employeeCard = db.EmployeeCards.Find(id);
            if (employeeCard == null)
            {
                // Return JSON response for record not found
                return Json(new
                {
                    Message = "Record not found.",
                    Status = "Error"
                }, JsonRequestBehavior.AllowGet);
            }
            employeeCard.IsActive = true;
            employeeCard.LastModified = DateTime.Now;
            employeeCard.ModifiedBy = User.Identity.Name;
            db.Entry(employeeCard).State = EntityState.Modified;
            db.SaveChanges();
            // Return JSON response for success
            return Json(new
            {
                Message = "You have successfully enabled this card.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee placment");
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