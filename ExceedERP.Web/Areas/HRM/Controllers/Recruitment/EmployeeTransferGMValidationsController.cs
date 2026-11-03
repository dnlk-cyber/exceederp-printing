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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.EmployeeTransferAdmin, HrmConstants.EmployeeTransferApprover)]
    public class EmployeeTransferGMValidationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult EmployeeTransferGMValidations_Read([DataSourceRequest]DataSourceRequest request, int employeeTRId)
        {
            IQueryable<TransferRequestGMValidation> requestValidations = db.TransferRequestGMValidations.Where(x=>x.EmployeeTransferRequestId== employeeTRId);
            DataSourceResult result = requestValidations.ToDataSourceResult(request, requestValidation => new
            {

                TransferRequestGMValidationId = requestValidation.TransferRequestGMValidationId,
                EmployeeTransferRequestId = requestValidation.EmployeeTransferRequestId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy = requestValidation.CreatedBy,
                DateCreated = requestValidation.DateCreated,
                LastModified = requestValidation.LastModified,
                ModifiedBy = requestValidation.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferGMValidations_Create( [DataSourceRequest] DataSourceRequest request,TransferRequestGMValidation requestValidation,int employeeTRId)
        {
            ModelState.Clear();

            var record = db.EmployeeTransferRequests.Find(employeeTRId);
            if (record == null)
            {
                ModelState.AddModelError("approved", "Record not found");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }

            if (!record.IsDGMApproved)
            {
                ModelState.AddModelError("approved", "DGM approval is required before this record can proceed to GM approval.");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }
            if (record.IsGMApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already approved");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }

            if (!ModelState.IsValid)
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));

            var now = DateTime.Now;
            var user = User.Identity.Name;
            var GM = db.CompanyApprovalSettings.FirstOrDefault(x => x.IsEnabled && x.Role == Core.Domain.HRM.Company.ApprovalRole.GM);
            bool isGM = GM != null;
            bool isAutoPlacementAllowed = GM != null && GM.AllowAutoPlacement;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    requestValidation.EmployeeTransferRequestId = employeeTRId;
                    requestValidation.DateCreated = now;
                    requestValidation.CreatedBy = user;

                    // ----- STATUS HANDLING -----
                    switch (requestValidation.Status)
                    {
                        case ApprovalStatuses.Reject:
                            record.Status = "GM Rejected";
                            break;

                        case ApprovalStatuses.Pending:
                            record.Status = "GM pending";
                            break;

                        case ApprovalStatuses.Approve:
                            record.Status = "GM Approved";
                            record.IsGMApproved = true;
                            record.GMApprovedBy = user;
                            record.GMApprovedTime = now;
                            if (isAutoPlacementAllowed)
                            {
                                AutoPlacement(record, now, user);
                                HandlePlacementToEmployeeCard(record, EmployeeCardType.EmployeeTransfer);
                            }
                            break;
                    }

                    var entity = GetMappedObject(requestValidation);
                    db.TransferRequestGMValidations.Add(entity);
                    db.SaveChanges();
                    transaction.Commit();

                    requestValidation.TransferRequestGMValidationId =
                        entity.TransferRequestGMValidationId;

                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "Employee Transfer Request approval",
                        "Success",
                        user,
                        $"approved transfer request {entity.EmployeeTransferRequestId}",
                        Modules.HRM);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferGMValidations_Update([DataSourceRequest]DataSourceRequest request, TransferRequestGMValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestValidation.LastModified = DateTime.Now;
                requestValidation.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestValidation);
                db.TransferRequestGMValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferGMValidations_Destroy([DataSourceRequest]DataSourceRequest request, TransferRequestGMValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestValidation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Transfer   information of applicant" + requestValidation.TransferRequestGMValidationId);
                #endregion
                db.TransferRequestGMValidations.Attach(entity);
                db.TransferRequestGMValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        // ================================== Add To Employee Card ==============================================
        private void HandlePlacementToEmployeeCard(EmployeeTransferRequest employeeTransfer, EmployeeCardType employeeCardType)
        {
            DateTime now = DateTime.Now;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeTransfer.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                var entity = new EmployeeCard
                {
                    EmployeeId = employeeTransfer.EmployeeId,
                    FromBranchId = employeeTransfer.FromBranchId,
                    FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                    FromOrgStructureId = employeeTransfer.FromOrgStructureId,
                    FromLookupId = employeeTransfer.FromLookupId,
                    ToEmployeeId = employeeTransfer.EmployeeId,
                    OrgStructureId = employeeTransfer.OrgStructureId,
                    LookupId = employeeTransfer.LookupId??0,
                    BranchId = employeeTransfer.BranchId,
                    BusinessUnitId = employeeTransfer.BusinessUnitId,
                    EachPrimaryId = employeeTransfer.EmployeeTransferRequestId,
                    CardType = employeeCardType,
                    IsActive = false,
                    ActionTaken = "EmployeeTransfer",
                    Salary = placement.Salary,
                    Remark = employeeCardType.ToString(),
                    FromDate = employeeTransfer.RequestDate,
                    FromDateAm = HrHelper.GetECDate(employeeTransfer.RequestDate),
                    ToDate = null,
                    ToDateTimeAm = null,
                    CreatedBy = User.Identity.Name,
                    DateCreated = now,
                    ModifiedBy = null,
                    LastModified = null,
                    OrgBranchName = placement.OrgBranchName
                };
                db.EmployeeCards.Add(entity);
               
            }

        }
        private void AutoPlacement(EmployeeTransferRequest record, DateTime now, string user)
        {
          
            var currentPlacements = db.Placements
                .Where(x => x.isCurrent && x.EmployeeId == record.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .ToList();

            var currentPlacement = currentPlacements.FirstOrDefault();
            if (currentPlacement == null) return;

            bool isChanged =
                currentPlacement.BranchId != record.BranchId ||
                currentPlacement.BusinessUnitId != record.BusinessUnitId ||
                currentPlacement.OrgStructureId != record.OrgStructureId;

            if (!isChanged) return;

            // IMPORTANT: deactivate ALL current placements first
            foreach (var prev in currentPlacements)
            {
                prev.isCurrent = false;
                prev.IsTemporary = false;
                prev.LastModified = now;
                prev.ModifiedBy = user;
            }

            var newPlacement = new Placement
            {
                EmployeeId = record.EmployeeId,
                HeadId = currentPlacement.HeadId,
                ImmediateSupervisor = currentPlacement.ImmediateSupervisor,
                IsHead = currentPlacement.IsHead,
                AssignmentDate = now,
                AssignmentDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year),
                MappingId = currentPlacement.MappingId,
                RankId = currentPlacement.RankId,
                StepId = currentPlacement.StepId,
                LookupId = record.LookupId ?? currentPlacement.LookupId,
                OrgStructureId = record.OrgStructureId,
                ParentOrgStructureId = currentPlacement.ParentOrgStructureId,
                Salary = currentPlacement.Salary,
                ReportsTo = currentPlacement.ReportsTo,
                JobTitleName = currentPlacement.JobTitleName,
                Department = currentPlacement.Department,
                ReportsToMappingId = currentPlacement.ReportsToMappingId,
                AssignmentType = AssignmentType.TRANSFER,
                ResponsibilityLevel = currentPlacement.ResponsibilityLevel,
                BranchId = record.BranchId,
                BusinessUnitId = record.BusinessUnitId ?? 0,
                Remark = currentPlacement.Remark,
                DateCreated = now,
                CreatedBy = user,
                isCurrent = true,
                IsApprove = true,
                IsTemporary = false,
                IsPend = currentPlacement.IsPend,
                Index = currentPlacement.Index,
                EmployementType = currentPlacement.EmployementType,
                EmployeeCode = currentPlacement.EmployeeCode,
                OrgBranchName = currentPlacement.OrgBranchName,
                PreparedByEmpId = currentPlacement.PreparedByEmpId
            };

            db.Placements.Add(newPlacement);

            if (newPlacement.IsHead)
            {
                var subordinateIds =
                    EmployeeHelper.GetSubordinates_FilteredByBranchAlso(newPlacement.EmployeeId);

                var subPlacements = db.Placements
                    .Where(x => subordinateIds.Contains(x.EmployeeId) && x.isCurrent)
                    .ToList();

                foreach (var p in subPlacements)
                    p.HeadId = newPlacement.EmployeeId;
            }
            AddEmployeeAllowance(record, newPlacement);
        }

       
        private TransferRequestGMValidation GetMappedObject(TransferRequestGMValidation requestValidation)
        {
            var entity = new TransferRequestGMValidation
            {
                TransferRequestGMValidationId = requestValidation.TransferRequestGMValidationId,
                EmployeeTransferRequestId = requestValidation.EmployeeTransferRequestId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy= requestValidation.CreatedBy,
                DateCreated= requestValidation.DateCreated,
                LastModified= requestValidation.LastModified,
                ModifiedBy= requestValidation.ModifiedBy,
                

            };
            return entity;
        }
        private void AddEmployeeAllowance(EmployeeTransferRequest employeeTransfer, Placement placement)
        {
            var now = DateTime.Now;
            var user = User.Identity.Name;

            // 1. Get DESERT allowance definition IDs ONCE
            var desertDefIds = db.AllowanceDefinations
                .Where(x => x.AllowanceType == AllowanceType.DESERT)
                .Select(x => x.AllowanceDefinationId)
                .ToList();

            if (!desertDefIds.Any())
                return;

            // 2. Load all relevant mappings ONCE
            var mappings = db.AllowanceMappings
                .Where(x => desertDefIds.Contains(x.AllowanceDefinationId))
                .ToList();

            // Helper function for priority matching
            AllowanceMapping FindBestMapping(int? lookupId, int? branchId, int? rankId)
            {
                return mappings.FirstOrDefault(x => x.LookupId == lookupId && x.BranchId == branchId && x.RankId == rankId)
                    ?? mappings.FirstOrDefault(x => x.BranchId == branchId)
                    ?? mappings.FirstOrDefault(x => x.LookupId == lookupId)
                    ?? mappings.FirstOrDefault(x => x.RankId == rankId);
            }

            // 3. Get current & previous mappings
            //previous mappings
            var prevMapping = FindBestMapping(employeeTransfer.FromLookupId, employeeTransfer.FromBranchId, placement.RankId);

            // 4. Pend OLD allowances (single query)
            if (prevMapping != null)
            {
                var oldAllowances = db.AllowancePerEmployees
                    .Where(a => a.EmployeeId == placement.EmployeeId &&
                                a.AllowanceDefinationId == prevMapping.AllowanceDefinationId)
                    .ToList();

                foreach (var old in oldAllowances)
                {
                    old.Pend = true;
                    old.LastModified = now;
                    old.ModifiedBy = user;
                    db.Entry(old).State = EntityState.Modified;

                }
            }
            // Get current  mappings
            var currentMapping = FindBestMapping(placement.LookupId, placement.BranchId, placement.RankId);
            if (currentMapping == null)
                return;

            // 5. Check existing NEW allowance
            var existing = db.AllowancePerEmployees
                .FirstOrDefault(a => a.EmployeeId == placement.EmployeeId &&
                                     a.AllowanceDefinationId == currentMapping.AllowanceDefinationId);

            if (existing != null)
            {
                // If already active → pend it=>unpend it
                if (existing.Pend)
                {
                    existing.Pend = false;
                    existing.LastModified = now;
                    existing.ModifiedBy = user;
                    db.Entry(existing).State = EntityState.Modified;

                }

                return;
            }

            // 6. Create NEW allowance
            var newAllowance = new AllowancePerEmployee
            {
                EmployeeId = placement.EmployeeId,
                AllowanceDefinationId = currentMapping.AllowanceDefinationId,
                Remark = currentMapping.Remark,
                DateCreated = now,
                LastModified = now,
                CreatedBy = user,
                ModifiedBy = user,
                From = employeeTransfer.RequestDate,
                To = null,
                Pend = false,
                EmployeeCode = placement.EmployeeCode,
                OrgBranchName = placement.OrgBranchName
            };

            db.AllowancePerEmployees.Add(newAllowance);
        }

        //private void AddEmployeeAllowance(EmployeeTransferRequest employeeTransfer, Placement placement)
        //{
        //    // 1. Try LookupId + Rank+Brach
        //    #region current mapping
        //    // on this only desret allownance is to bepend f this branch or lookid or rank has desrent allowance befor it should be pend if it has not create for this new allowance defintion for the empployee
        //    var desereAllowancedefintionId = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.DESERT).Select(x => x.AllowanceDefinationId);

        //    var allowanceMapping = db.AllowanceMappings.FirstOrDefault(x => x.LookupId == placement.LookupId
        //                            &&desereAllowancedefintionId.Contains(x.AllowanceDefinationId)
        //                            && x.BranchId == placement.BranchId 
        //                            && x.RankId == placement.RankId);

        //    // 2. Try LookupId only
        //    if (allowanceMapping == null)
        //    {
        //        allowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.LookupId == placement.LookupId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }

        //    // 3. Try Rank only
        //    if (allowanceMapping == null)
        //    {
        //        allowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.RankId == placement.RankId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }
        //    // 4. Try branch only
        //    if (allowanceMapping == null)
        //    {
        //        allowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.BranchId == placement.BranchId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }

        //    if (allowanceMapping == null)
        //        return; // No allowance rule found

        //    #endregion
        //    #region Prev
        //    var prevAllowanceMapping = db.AllowanceMappings.FirstOrDefault(x => x.LookupId == employeeTransfer.FromLookupId
        //                         && x.BranchId == employeeTransfer.FromBranchId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId)
        //                         && x.RankId == placement.RankId);

        //    // 2. Try LookupId only
        //    if (prevAllowanceMapping == null)
        //    {
        //        prevAllowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.BranchId == employeeTransfer.FromBranchId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }

        //    // 3. Try Rank only
        //    if (prevAllowanceMapping == null)
        //    {
        //        prevAllowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.RankId == placement.RankId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }
        //    // 4. Try branch only
        //    if (prevAllowanceMapping == null)
        //    {
        //        prevAllowanceMapping = db.AllowanceMappings
        //            .FirstOrDefault(x => x.LookupId == employeeTransfer.FromLookupId && desereAllowancedefintionId.Contains(x.AllowanceDefinationId));
        //    }
        //    #endregion
        //    //pend all OLD ALLOWANCES 
        //    if(prevAllowanceMapping !=null)
        //    {
        //        var oldAllowances = db.AllowancePerEmployees
        //        .Where(a => a.EmployeeId == placement.EmployeeId
        //            && a.AllowanceDefinationId == prevAllowanceMapping.AllowanceDefinationId)
        //        .ToList();

        //        foreach (var old in oldAllowances)
        //        {
        //            old.Pend = true;
        //            old.LastModified = DateTime.Now;
        //            old.ModifiedBy = User.Identity.Name;
        //        }

        //    }


        //    //  CHECK IF NEW ALLOWANCE ALREADY EXISTS

        //    var existing = db.AllowancePerEmployees
        //        .FirstOrDefault(a => a.EmployeeId == placement.EmployeeId &&
        //                             a.AllowanceDefinationId == allowanceMapping.AllowanceDefinationId);

        //    if (existing != null)
        //    {
        //        if(existing.Pend)
        //            return;

        //        existing.Pend = true;
        //        db.Entry(existing).State = EntityState.Modified;
        //        return;
        //    }



        //    //  CREATE NEW ALLOWANCE
        //    var newAllowance = new AllowancePerEmployee
        //    {
        //        EmployeeId = placement.EmployeeId,
        //        AllowanceDefinationId = allowanceMapping.AllowanceDefinationId,
        //        Remark = allowanceMapping.Remark,
        //        DateCreated = DateTime.Now,
        //        LastModified = DateTime.Now,
        //        CreatedBy = User.Identity.Name,
        //        ModifiedBy = User.Identity.Name,
        //        From = null,
        //        To = null,
        //        Pend = false,
        //        EmployeeCode = placement.EmployeeCode,
        //        OrgBranchName = placement.OrgBranchName
        //    };

        //    db.AllowancePerEmployees.Add(newAllowance);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
