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
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Reporting.HRM.HRM.Letters.DataSource;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRAdmin,HrmConstants.EmployeeTransferAdmin, HrmConstants.EmployeeTransferApprover)]
    public class EmployeeTransferRequestHRsController : BaseController
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
                   FullNameaEnglish = i.Code + "-" + i.Name,
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
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
                   FromOrgStructureId = i.Id,

               }).AsQueryable();

            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["isDgmOrDGM"] = db.CompanyApprovalSettings.FirstOrDefault(x=>x.IsEnabled
                                                    &&(x.Role==Core.Domain.HRM.Company.ApprovalRole.GM
                                                    ||x.Role==Core.Domain.HRM.Company.ApprovalRole.GM))?.IsMandatory;

        }

        // 1. Action to render the Transfer Request Letter / Form View

        [HttpGet]
        public ActionResult PrintTransferForm(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Content("<div class='alert alert-danger' style='margin: 15px;'><i class='fa fa-exclamation-triangle'></i> Invalid Request: Transfer ID is missing.</div>");
            }

            try
            {
                using (var ds = new EmployeeTransferObjectDataSource())
                {
                    // Note: Match the exact method name in your DataSource
                    var model = ds.GetEmployeeTransferRequest(id).FirstOrDefault();

                    if (model == null)
                    {
                        return Content("<div class='alert alert-warning' style='margin: 15px;'><i class='fa fa-info-circle'></i> No transfer record found for ID: " + id + "</div>");
                    }

                    // Return partial view (specify path if needed)
                    return PartialView("_TransferRequestFormPrint", model);
                }
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger' style='margin: 15px;'><i class='fa fa-exclamation-triangle'></i> Error generating report: " + ex.Message + "</div>");
            }
        }

        private string GetTotalEmployeeExperienceString(int empId)
        {
            double totalDays = 0;
            double totalExperience = 0;
            decimal totalExperienceInYear = 0;
            DateTime now = DateTime.Now;
            var personEmployee = db.Person_Employee.Find(empId);
            if (personEmployee != null)
            {
                totalDays += (now - personEmployee.HiredDate).TotalDays;

            }
            totalExperienceInYear = Math.Round((decimal)(totalDays / 365.25), 2);
            //return total experience
            return EmployeeHelper.GetDaystoWords(totalDays);
            
            
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

        private decimal GetTotalEmployeeExperience(int empId, int empTransferId)
        {
            double totalDays = 0;
            DateTime now = DateTime.Now;
            var personEmployee = db.Person_Employee.Find(empId);
            //// Fetch employee transfer data once
            
            if(personEmployee != null)
            {
                totalDays += (now - personEmployee.HiredDate).TotalDays;

            }
            // Convert to years
            return Math.Round((decimal)(totalDays / 365.25), 2);
        }

        public ActionResult EmployeeTransferRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(username);
            IQueryable<EmployeeTransferRequest> employeeTransfers = db.EmployeeTransferRequests
                                                                                .Where(x => x.IsDepartmentOnlineApproved 
                                                                                && myBranches.Contains(x.BranchId));
            DataSourceResult result = employeeTransfers.ToDataSourceResult(request, employeeTransfer => new
            {

                EmployeeTransferRequestId = employeeTransfer.EmployeeTransferRequestId,
                EmployeeId = employeeTransfer.EmployeeId,
                FromOrgStructureId = employeeTransfer.FromOrgStructureId,
                OrgStructureId = employeeTransfer.OrgStructureId,
                BranchId = employeeTransfer.BranchId,
                FromBranchId = employeeTransfer.FromBranchId,
                ToBranch1Id = employeeTransfer.ToBranch1Id,
                ToBranchId = employeeTransfer.ToBranchId,
                FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                BusinessUnitId = employeeTransfer.BusinessUnitId,
                LookupId = employeeTransfer.LookupId,
                FromLookupId = employeeTransfer.FromLookupId,
                Reason = employeeTransfer.Reason,
                RequestDate = employeeTransfer.RequestDate,
                TotalExperience = employeeTransfer.Count == 1 ? employeeTransfer.TotalExperience : GetTotalEmployeeExperience(employeeTransfer.EmployeeId, employeeTransfer.EmployeeTransferRequestId),
                Experience = employeeTransfer.TotalExperience + "Years (" + GetTotalEmployeeExperienceString(employeeTransfer.EmployeeId) + ")",
                DesertAllowance = employeeTransfer.DesertAllowance,
                DesertWorkExperienceYears = employeeTransfer.DesertWorkExperienceYears,
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
                VoidBy = employeeTransfer.VoidBy,
                IsDGMApproved = employeeTransfer.IsDGMApproved,
                DGMApprovedBy = employeeTransfer.DGMApprovedBy,
                DGMApprovedTime = employeeTransfer.DGMApprovedTime,
               
                IsGMApproved = employeeTransfer.IsGMApproved,
                GMApprovedBy = employeeTransfer.GMApprovedBy,
                GMApprovedTime = employeeTransfer.GMApprovedTime
            });

            return Json(result);
        }
        
        public ActionResult EmployeeTransferRequests_ReadCC([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(username);
            IQueryable<EmployeeTransferRequest> employeeTransfers = db.EmployeeTransferRequests
                                                                                .Where(x => x.IsDepartmentOnlineApproved
                                                                                && myBranches.Contains(x.BranchId));
            DataSourceResult result = employeeTransfers.ToDataSourceResult(request, employeeTransfer => new
            {

                EmployeeTransferRequestId = employeeTransfer.EmployeeTransferRequestId,
                EmployeeId = employeeTransfer.EmployeeId,
                FromOrgStructureId = employeeTransfer.FromOrgStructureId,
                OrgStructureId = employeeTransfer.OrgStructureId,
                BranchId = employeeTransfer.BranchId,
                ToBranch1Id = employeeTransfer.ToBranch1Id,
                ToBranchId = employeeTransfer.ToBranchId,
                FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                BusinessUnitId = employeeTransfer.BusinessUnitId,
                LookupId = employeeTransfer.LookupId,
                FromLookupId = employeeTransfer.FromLookupId,
                Reason = employeeTransfer.Reason,
                RequestDate = employeeTransfer.RequestDate,
                TotalExperience = employeeTransfer.Count == 1 ? employeeTransfer.TotalExperience : GetTotalEmployeeExperience(employeeTransfer.EmployeeId, employeeTransfer.EmployeeTransferRequestId),
                Experience =  employeeTransfer.TotalExperience+ "Years ("+GetTotalEmployeeExperienceString(employeeTransfer.EmployeeId)+")",
                DesertAllowance = employeeTransfer.DesertAllowance,
                DesertWorkExperienceYears = employeeTransfer.DesertWorkExperienceYears,
                Remark = employeeTransfer.Remark,
                Status = employeeTransfer.Status,
                ForwardedFrom = employeeTransfer.ForwardedFrom,
                ForwardedTo = employeeTransfer.ForwardedTo,
                FromBranchId = employeeTransfer.FromBranchId,
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
                IsVoid = employeeTransfer.IsVoid,
                VoidTime = employeeTransfer.VoidTime,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Create([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
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
            if (employeeTransfer.FromBranchId == employeeTransfer.BranchId
                && employeeTransfer.BusinessUnitId == employeeTransfer.FromBusinessUnitId)
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            if (db.EmployeeTransferRequests.Where(x => x.EmployeeId == employeeTransfer.EmployeeId
                      && x.BusinessUnitId == employeeTransfer.BusinessUnitId
                      && x.FromBusinessUnitId == employeeTransfer.FromBusinessUnitId
                      && x.FromBranchId == employeeTransfer.FromBranchId
                      && x.BranchId == employeeTransfer.BranchId).Any())
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            employeeTransfer.IsSendForApproval = true;
            employeeTransfer.SendForApprovalBy = User.Identity.Name;
            employeeTransfer.SendForApprovalTime = DateTime.Now;
            employeeTransfer.Status = "Department Approved";
            employeeTransfer.IsDepartmentOnlineApproved = true;
            employeeTransfer.DepartmentOnlineApprovedBy = User.Identity.Name;
            employeeTransfer.DepartmentOnlineApprovedTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                employeeTransfer.DateCreated = DateTime.Now;
                employeeTransfer.CreatedBy = User.Identity.Name;
                employeeTransfer.TotalExperience = GetTotalEmployeeExperience(employeeTransfer.EmployeeId);
                var entity = GetMappedObject(employeeTransfer);
                db.EmployeeTransferRequests.Add(entity);
                db.SaveChanges();
                employeeTransfer.EmployeeTransferRequestId = entity.EmployeeTransferRequestId;
                var departmentValidationentity = new TransferRequestDepartmentValidation
                {
                    EmployeeTransferRequestId = entity.EmployeeTransferRequestId,
                    Status = ApprovalStatuses.Approve,
                    Remark = "Department Approved",
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };
                db.TransferRequestDepartmentValidations.Add(departmentValidationentity);
                db.SaveChanges();

            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Update([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
            var employee = db.Placements.Where(x => x.isCurrent && x.EmployeeId == employeeTransfer.EmployeeId).FirstOrDefault();
            if (employee != null)
            {
                employeeTransfer.FromBranchId = employee.BranchId;
                employeeTransfer.FromBusinessUnitId = employee.BusinessUnitId;
            }
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
                      && employeeTransfer.EmployeeTransferRequestId != x.EmployeeTransferRequestId
                      && x.BranchId == employeeTransfer.BranchId).Any())
            {
                ModelState.AddModelError("exist", " transfering to the same is not possible");
                return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                employeeTransfer.LastModified = DateTime.Now;
                employeeTransfer.ModifiedBy = User.Identity.Name;
                employeeTransfer.TotalExperience = GetTotalEmployeeExperience(employeeTransfer.EmployeeId);
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
                EmployeeTransferRequestId=employeeTransfer.EmployeeTransferRequestId,
                EmployeeId =employeeTransfer.EmployeeId,
                OrgStructureId =employeeTransfer.OrgStructureId,
                FromOrgStructureId = employeeTransfer.FromOrgStructureId,
                BranchId = employeeTransfer.BranchId,
                FromBranchId = employeeTransfer.FromBranchId,
                LookupId = employeeTransfer.LookupId,
                FromLookupId = employeeTransfer.FromLookupId,
                Count = employeeTransfer.Count,
                BusinessUnitId = employeeTransfer.BusinessUnitId,
                FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                ToBranch1Id = employeeTransfer.ToBranch1Id,
                ToBranchId = employeeTransfer.ToBranchId,
                TotalExperience = employeeTransfer.TotalExperience,
                DesertAllowance = employeeTransfer.DesertAllowance,
                DesertWorkExperienceYears = employeeTransfer.DesertWorkExperienceYears,
                Reason = employeeTransfer.Reason,
                RequestDate=employeeTransfer.RequestDate,
                Remark=employeeTransfer.Remark,
                Status=employeeTransfer.Status,

                ForwardedFrom=employeeTransfer.ForwardedFrom,
                ForwardedTo=employeeTransfer.ForwardedTo,
                CurrentApprovalStep=employeeTransfer.CurrentApprovalStep,
                CreatedBy=employeeTransfer.CreatedBy,
                DateCreated=employeeTransfer.DateCreated,
                DepartmentOnlineApprovedBy=employeeTransfer.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime=employeeTransfer.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalBy=employeeTransfer.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime=employeeTransfer.DepartmentSendForApprovalTime,
                IsDepartmentOnlineApproved=employeeTransfer.IsDepartmentOnlineApproved,
                IsDepartmentSendForApproval=employeeTransfer.IsDepartmentSendForApproval,
                IsFinalApproval=employeeTransfer.IsFinalApproval,
                IsOnlineApproved=employeeTransfer.IsOnlineApproved,
                IsOnlineTransferred=employeeTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy=employeeTransfer.IsOnlineTransferredBy,
                IsSendForApproval=employeeTransfer.IsSendForApproval,
                LastModified=employeeTransfer.LastModified,
                ModifiedBy=employeeTransfer.ModifiedBy,
                OnlineApprovedBy = employeeTransfer.OnlineApprovedBy,
                OnlineApprovedTime=employeeTransfer.OnlineApprovedTime,
                OnlineTransferredTime=employeeTransfer.OnlineTransferredTime,
                SendForApprovalBy=employeeTransfer.SendForApprovalBy,
                SendForApprovalTime=employeeTransfer.SendForApprovalTime,
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

            if (record == null)
            {
                return Json(new
                {
                    Message = "Record not found.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            if (record.IsDepartmentOnlineApproved)
            {
                return Json(new
                {
                    Message = "You cannot manipulate this record, it is already approved.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            if (record.IsDepartmentSendForApproval)
            {
                return Json(new
                {
                    Message = "This record is already sent for department approval.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            //var hasDetails = db.TransferRequestDetails
            //                   .Any(x => x.EmployeeTransferRequestId == id);

            //if (!hasDetails)
            //{
            //    return Json(new
            //    {
            //        Message = "Please add job title.",
            //        Status = "NOT_OK"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            // Update record
            record.Status = OperationStatuses.SendForDepartmentApproval.ToString(); // replace magic number
            record.IsDepartmentSendForApproval = true;
            record.DepartmentSendForApprovalBy = User.Identity.Name;
            record.DepartmentSendForApprovalTime = DateTime.Now;

            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Message = "Successfully sent.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        public class EmployeeTransferRequestVM
        {
            public int EmployeeTransferRequestId { get; set; }
            public int EmployeeId { get; set; }
            public int? FromBusinessUnitId { get; set; }
            public int? BusinessUnitId { get; set; }
            public int? FromBranchId { get; set; }
            public int? BranchId { get; set; }
            public int? OrgStructureId { get; set; }
            public DateTime? RequestDate { get; set; }
            public string Reason { get; set; }
            public string Remark { get; set; }
            public int? Count { get; set; }
            public decimal? TotalExperience { get; set; }
            public string Experience { get; set; }

            // Add other needed fields (excluding navigation properties)
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    
}

