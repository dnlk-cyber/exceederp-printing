using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Inventory;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRDepartmentHead,HrmConstants.HREmployeeSelfService)]
    public class EmployeeTransferRequestsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
     
        private void PopulateViewData()
        {


            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.Code + "-" + i.AmharicName,
                   FullNameaEnglish =i.Code+"-"+ i.Name,
                   EmployeeId = i.Id,
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
                   ToBranch1Id = i.Id,
                   ToBranchId = i.Id,
               }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
                   FromBusinessUnitId = i.Id,
               }).AsQueryable(); 
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
                   FromOrgStructureId = i.Id,

               }).AsQueryable();

            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["isManual"] = db.GeneralJobtitleSettings.FirstOrDefault()?.IsManual;

        }

        public decimal GetTotalEmployeeExperience(int empId, int empTransferId)
        {
            double totalDays = 0;
            decimal totalExperienceInYear = 0;

            var placementHistories = db.PlacementHistories
                .Where(x => x.EmployeeId == empId)
                .ToList();

            var employeeTransfer = db.EmployeeTransferRequests.Find(empTransferId);

            foreach (var place in placementHistories)
            {
                if (place.From < place.To)
                {
                    totalDays += (place.To - place.From).TotalDays;
                }
            }

            var placements = db.Placements
                .Where(x => x.EmployeeId == empId)
                .OrderBy(x => x.AssignmentDate)
                .ToList();

            for (int i = 0; i < placements.Count; i++)
            {
                DateTime from = placements[i].AssignmentDate;
                DateTime to = (placements[i].isCurrent || i == placements.Count - 1)
                    ? DateTime.Now
                    : placements[i + 1].AssignmentDate;

                if (from < to)
                {
                    totalDays += (to - from).TotalDays;
                }
            }

            totalExperienceInYear = Math.Round((decimal)(totalDays / 365.25), 2);
            //if (employeeTransfer != null)
            //{
            //    employeeTransfer.TotalExperience = totalExperienceInYear;
            //    if (!employeeTransfer.Count.HasValue || employeeTransfer.Count == 0)
            //    {
            //        employeeTransfer.Count = 1;
            //    }

            //    db.Entry(employeeTransfer).State = EntityState.Modified;
            //    db.SaveChanges(); // This works in EF 4.5
            //}

            return totalExperienceInYear;
        }

        private decimal GetTotalEmployeeExperience(int empId)
        {
            double totalExperience = 0;
            decimal totalExperienceInYear = 0;
            // Experience before using the system
            var placementHistories = db.PlacementHistories
                .Where(x => x.EmployeeId == empId)
                .ToList();

            foreach (var place in placementHistories)
            {
                if (place.From < place.To)
                {
                    totalExperience += (place.To - place.From).TotalDays;
                }
                // else skip invalid date range
            }

            // Experience after using the system
            var placements = db.Placements
                .Where(x => x.EmployeeId == empId)
                .OrderBy(x => x.AssignmentDate)
                .ToList();

            int placementCount = placements.Count;

            for (int i = 0; i < placementCount; i++)
            {
                DateTime from = placements[i].AssignmentDate;
                DateTime to;

                if (placements[i].isCurrent || i == placementCount - 1)
                {
                    to = DateTime.Now;
                }
                else
                {
                    to = placements[i + 1].AssignmentDate;
                }

                if (from < to)
                {
                    totalExperience += (to - from).TotalDays;
                }
                // else skip invalid date range
            }

            // Return experience in years, rounded to 2 decimal places
            totalExperienceInYear = Math.Round((decimal)(totalExperience / 365.25), 2);
            //return total experience
            return totalExperienceInYear;
        }
        private string GetTotalEmployeeExperienceString(int empId)
        {
            double totalExperience = 0;
            decimal totalExperienceInYear = 0;
            // Experience before using the system
            var placementHistories = db.PlacementHistories
                .Where(x => x.EmployeeId == empId)
                .ToList();

            foreach (var place in placementHistories)
            {
                if (place.From < place.To)
                {
                    totalExperience += (place.To - place.From).TotalDays;
                }
                // else skip invalid date range
            }

            // Experience after using the system
            var placements = db.Placements
                .Where(x => x.EmployeeId == empId)
                .OrderBy(x => x.AssignmentDate)
                .ToList();

            int placementCount = placements.Count;

            for (int i = 0; i < placementCount; i++)
            {
                DateTime from = placements[i].AssignmentDate;
                DateTime to;

                if (placements[i].isCurrent || i == placementCount - 1)
                {
                    to = DateTime.Now;
                }
                else
                {
                    to = placements[i + 1].AssignmentDate;
                }

                if (from < to)
                {
                    totalExperience += (to - from).TotalDays;
                }
                // else skip invalid date range
            }

            // Return experience in years, rounded to 2 decimal places
            totalExperienceInYear = Math.Round((decimal)(totalExperience / 365.25), 2);
            //return total experience
            return EmployeeHelper.GetDaystoWords(totalExperience);

        }

        public ActionResult EmployeeTransferRequests_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);

            var query = db.EmployeeTransferRequests
                          .AsNoTracking()
                          .Where(x => employeeIds.Contains(x.EmployeeId))
                          .Where(x => x.IsSendForApproval);

            // Apply paging/filtering/sorting FIRST
            var result = query.ToDataSourceResult(request);

            // Materialized data
            var data = ((IEnumerable<EmployeeTransferRequest>)result.Data).Select(employeeTransfer =>
            {
                var totalExperience =
                    employeeTransfer.Count == 1
                        ? employeeTransfer.TotalExperience
                        : GetTotalEmployeeExperience(
                            employeeTransfer.EmployeeId,
                            employeeTransfer.EmployeeTransferRequestId);

                var experienceString = GetTotalEmployeeExperienceString(employeeTransfer.EmployeeId);

                return new
                {
                    employeeTransfer.EmployeeTransferRequestId,
                    employeeTransfer.EmployeeId,
                    employeeTransfer.OrgStructureId,
                    employeeTransfer.FromOrgStructureId,
                    employeeTransfer.BranchId,
                    employeeTransfer.FromBranchId,
                    employeeTransfer.BusinessUnitId,
                    employeeTransfer.FromBusinessUnitId,
                    employeeTransfer.ToBranch1Id,
                    employeeTransfer.ToBranchId,
                    employeeTransfer.LookupId,
                    employeeTransfer.FromLookupId,
                    employeeTransfer.DesertAllowance,
                    employeeTransfer.Count,
                    TotalExperience = totalExperience,
                    Experience = totalExperience + " Years (" + experienceString + ")",
                    employeeTransfer.Remark,
                    employeeTransfer.Reason,
                    employeeTransfer.RequestDate,
                    employeeTransfer.Status,
                    employeeTransfer.ForwardedFrom,
                    employeeTransfer.ForwardedTo,
                    employeeTransfer.CurrentApprovalStep,
                    employeeTransfer.CreatedBy,
                    employeeTransfer.DateCreated,
                    employeeTransfer.DepartmentOnlineApprovedBy,
                    employeeTransfer.DepartmentOnlineApprovedTime,
                    employeeTransfer.DepartmentSendForApprovalBy,
                    employeeTransfer.DepartmentSendForApprovalTime,
                    employeeTransfer.IsDepartmentOnlineApproved,
                    employeeTransfer.IsDepartmentSendForApproval,
                    employeeTransfer.IsFinalApproval,
                    employeeTransfer.IsOnlineApproved,
                    employeeTransfer.IsOnlineTransferred,
                    employeeTransfer.IsOnlineTransferredBy,
                    employeeTransfer.IsSendForApproval,
                    employeeTransfer.LastModified,
                    employeeTransfer.ModifiedBy,
                    employeeTransfer.OnlineApprovedBy,
                    employeeTransfer.OnlineApprovedTime,
                    employeeTransfer.OnlineTransferredTime,
                    employeeTransfer.SendForApprovalBy,
                    employeeTransfer.SendForApprovalTime,
                    employeeTransfer.OrgBranchName,
                    employeeTransfer.IsVoid,
                    employeeTransfer.VoidTime,
                    employeeTransfer.VoidBy
                };
            }).ToList();

            result.Data = data;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Create([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {

            var currentPlacement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == employeeTransfer.EmployeeId)
                                            .FirstOrDefault();

            if (currentPlacement == null)
            {
                ModelState.AddModelError("exist", "There is no current Placement for this Employee");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
            }

            employeeTransfer.FromBranchId = currentPlacement.BranchId;
            employeeTransfer.BranchId = employeeTransfer.BranchId;
            employeeTransfer.FromBusinessUnitId = currentPlacement.BusinessUnitId;
            employeeTransfer.BusinessUnitId = employeeTransfer.BusinessUnitId != 0 ? employeeTransfer.BusinessUnitId : currentPlacement.BusinessUnitId;
            employeeTransfer.OrgStructureId = employeeTransfer.OrgStructureId != 0 ? employeeTransfer.OrgStructureId : currentPlacement.OrgStructureId;
            employeeTransfer.FromOrgStructureId = currentPlacement.OrgStructureId;
            employeeTransfer.FromLookupId = currentPlacement.LookupId;
            employeeTransfer.LookupId = employeeTransfer.LookupId.HasValue ? employeeTransfer.LookupId : currentPlacement.LookupId;
            if (employeeTransfer.FromBranchId==employeeTransfer.BranchId
                && employeeTransfer.BusinessUnitId == employeeTransfer.FromBusinessUnitId)
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            if (db.EmployeeTransferRequests.Where(x => x.EmployeeId == employeeTransfer.EmployeeId
                      && x.BusinessUnitId==employeeTransfer.BusinessUnitId
                      && x.FromBusinessUnitId==employeeTransfer.FromBusinessUnitId
                      && x.FromBranchId==employeeTransfer.FromBranchId
                      &&x.BranchId== employeeTransfer.BranchId).Any())
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                employeeTransfer.DateCreated = DateTime.Now;
                employeeTransfer.CreatedBy = User.Identity.Name;
                employeeTransfer.IsSendForApproval = true;
                employeeTransfer.SendForApprovalTime = DateTime.Now;
                employeeTransfer.SendForApprovalBy = User.Identity.Name;
                employeeTransfer.TotalExperience = GetTotalEmployeeExperience(employeeTransfer.EmployeeId);
                employeeTransfer.Count = 1;
                var entity = GetMappedObject(employeeTransfer);
                db.EmployeeTransferRequests.Add(entity);
                db.SaveChanges();
                employeeTransfer.EmployeeTransferRequestId = entity.EmployeeTransferRequestId;
            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Update([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {

            var currentPlacement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == employeeTransfer.EmployeeId).FirstOrDefault();
            if (currentPlacement == null)
            {
                ModelState.AddModelError("exist", "There is no current Placement for this Employee");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
            }
            employeeTransfer.FromBranchId = currentPlacement.BranchId;
            employeeTransfer.BranchId = employeeTransfer.BranchId;
            employeeTransfer.FromBusinessUnitId = currentPlacement.BusinessUnitId;
            employeeTransfer.BusinessUnitId = employeeTransfer.BusinessUnitId != 0 ? employeeTransfer.BusinessUnitId : currentPlacement.BusinessUnitId;
            employeeTransfer.OrgStructureId = employeeTransfer.OrgStructureId != 0 ? employeeTransfer.OrgStructureId : currentPlacement.OrgStructureId;
            employeeTransfer.FromOrgStructureId = currentPlacement.OrgStructureId;
            employeeTransfer.FromLookupId = currentPlacement.LookupId;
            employeeTransfer.LookupId = employeeTransfer.LookupId.HasValue ? employeeTransfer.LookupId : currentPlacement.LookupId;
            if (employeeTransfer.FromBranchId == employeeTransfer.BranchId
                || employeeTransfer.BusinessUnitId == employeeTransfer.FromBusinessUnitId)
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            if (db.EmployeeTransferRequests.Where(x => x.EmployeeId == employeeTransfer.EmployeeId
                      && x.BusinessUnitId == employeeTransfer.BusinessUnitId
                      && x.FromBusinessUnitId == employeeTransfer.FromBusinessUnitId
                      && x.FromBranchId == employeeTransfer.FromBranchId
                      && employeeTransfer.EmployeeTransferRequestId !=x.EmployeeTransferRequestId
                      && x.BranchId == employeeTransfer.BranchId).Any())
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                employeeTransfer.LastModified = DateTime.Now;
                employeeTransfer.ModifiedBy = User.Identity.Name;
                employeeTransfer.TotalExperience = GetTotalEmployeeExperience(employeeTransfer.EmployeeId);
                employeeTransfer.Count = 1;
                var entity = GetMappedObject(employeeTransfer);
                db.EmployeeTransferRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeTransfer);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Transfer   information of applicant" + employeeTransfer.EmployeeTransferRequestId);
                #endregion
                db.EmployeeTransferRequests.Attach(entity);
                db.EmployeeTransferRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        private EmployeeTransferRequest GetMappedObject(EmployeeTransferRequest employeeTransfer)
        {
            var entity = new EmployeeTransferRequest
            {
                EmployeeTransferRequestId = employeeTransfer.EmployeeTransferRequestId,
                EmployeeId = employeeTransfer.EmployeeId,
                OrgStructureId = employeeTransfer.OrgStructureId,
                FromOrgStructureId = employeeTransfer.FromOrgStructureId,
                BranchId = employeeTransfer.BranchId,
                FromBranchId = employeeTransfer.FromBranchId,
                BusinessUnitId = employeeTransfer.BusinessUnitId,
                FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                ToBranch1Id = employeeTransfer.ToBranch1Id,
                ToBranchId = employeeTransfer.ToBranchId,
                LookupId = employeeTransfer.LookupId,
                FromLookupId = employeeTransfer.FromLookupId,
                Count = employeeTransfer.Count,
                TotalExperience = employeeTransfer.TotalExperience,
                Reason = employeeTransfer.Reason,
                RequestDate = employeeTransfer.RequestDate,
                DesertAllowance = employeeTransfer.DesertAllowance,
                Remark = employeeTransfer.Remark,
                Status = employeeTransfer.Status,
                ForwardedFrom = employeeTransfer.ForwardedFrom,
                ForwardedTo = employeeTransfer.ForwardedTo,
                CurrentApprovalStep = employeeTransfer.CurrentApprovalStep,
                CreatedBy = employeeTransfer.CreatedBy,
                DateCreated = employeeTransfer.DateCreated,
                DepartmentOnlineApprovedBy = employeeTransfer.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = employeeTransfer.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalBy = employeeTransfer.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = employeeTransfer.DepartmentSendForApprovalTime,
                IsDepartmentOnlineApproved = employeeTransfer.IsDepartmentOnlineApproved,
                IsDepartmentSendForApproval = employeeTransfer.IsDepartmentSendForApproval,
                IsFinalApproval = employeeTransfer.IsFinalApproval,
                IsOnlineApproved = employeeTransfer.IsOnlineApproved,
                IsOnlineTransferred = employeeTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = employeeTransfer.IsOnlineTransferredBy,
                IsSendForApproval = employeeTransfer.IsSendForApproval,
                LastModified = employeeTransfer.LastModified,
                ModifiedBy = employeeTransfer.ModifiedBy,
                OnlineApprovedBy = employeeTransfer.OnlineApprovedBy,
                OnlineApprovedTime = employeeTransfer.OnlineApprovedTime,
                OnlineTransferredTime = employeeTransfer.OnlineTransferredTime,
                SendForApprovalBy = employeeTransfer.SendForApprovalBy,
                SendForApprovalTime = employeeTransfer.SendForApprovalTime,
                OrgBranchName = employeeTransfer.OrgBranchName,
                IsVoid = employeeTransfer.IsVoid,
                VoidTime = employeeTransfer.VoidTime,
                VoidBy = employeeTransfer.VoidBy
            };
            return entity;
        }

        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.EmployeeTransferRequests.Find(id);
            string errMsg = "";

            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
