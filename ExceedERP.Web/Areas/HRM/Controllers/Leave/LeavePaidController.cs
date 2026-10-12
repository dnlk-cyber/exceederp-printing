using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
    public class LeavePaidController : BaseController
    {
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        HRMInputs inputs = new HRMInputs();
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
              FullNameAmharic = i.AmharicName,
              FullNameaEnglish = i.Name,
              FullName = i.Name,
              FullNameCode = i.Code + " " + i.Name,
              FullNameWithOutCode = i.Name,
              EmployeeCode = i.Code,
              EmployeeId = i.Id,
              PersonId = i.Id,

          }).AsQueryable();


            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
                            i => new
                            {
                                Name = i.Name,
                                GlFiscalYearId = i.Id,
                            }).AsQueryable();

        }
        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
      
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        public ActionResult LeavePaid_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            var leavepaid = from lp in db.LeavePaid
                            join p in db.Placements on lp.EmployeeId equals p.EmployeeId
                            join e in db.Person_Employee on lp.EmployeeId equals e.EmployeeId
                            where p.isCurrent
                                  && e.IsActive
                            select new
                            {
                                lp.LeavePaidId,
                                lp.EmployeeId,
                                lp.GlFiscalYearId,
                                lp.Amount,
                                lp.AmountInBirr,
                                lp.TaxPayable,
                                lp.GrossAmount,
                                lp.DailyRate,
                                lp.BasicSalary,
                                lp.TotalPayableDays,
                                lp.AccumulatedLeaveDays,
                                lp.NetPay,
                                lp.IsCalculated,
                                lp.IsSentToFinance,
                                lp.PaymentRequestId,
                                lp.Remark,
                                lp.DateCreated,
                                lp.DateSent,
                                lp.SentBy,
                                lp.LastModified,
                                lp.CreatedBy,
                                lp.ModifiedBy,
                                lp.EmployeeCode,
                                lp.OrgBranchName
                            };

            DataSourceResult result = leavepaid.Distinct().ToDataSourceResult(request);

            return Json(result);
        }






        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Create([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();

            // 1. Validate Input Days
            decimal currentYearDays = (decimal)leavePaid.Amount;
            decimal historyDays = (decimal)leavePaid.AccumulatedLeaveDays;

            if (currentYearDays < 0 || historyDays < 0 || (currentYearDays + historyDays) <= 0)
            {
                ModelState.AddModelError("exist", "Total leave days paid in cash (Current + Accumulated) must be greater than zero.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            // 2. Validate Placement & Basic Salary
            var placement = db.Placements
                .Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId)
                .FirstOrDefault()
                ?? db.Placements
                    .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                    .OrderByDescending(x => x.AssignmentDate)
                    .FirstOrDefault();

            if (placement == null || placement.Salary <= 0)
            {
                ModelState.AddModelError("PayableDays", "Employee must have a valid placement with a basic salary.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            // 3. Validate Available Balances
            var activeEntitlements = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId)
                .ToList();

            decimal totalActiveAvailable = activeEntitlements.Sum(x => (decimal)x.Balance);
            if (currentYearDays > totalActiveAvailable)
            {
                ModelState.AddModelError("exist", $"Requested current days ({currentYearDays}) exceed available active balance of {totalActiveAvailable}.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            var historyEntitlements = db.LeaveEntitlementHistories
                .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                .ToList();

            decimal totalHistoryAvailable = historyEntitlements.Sum(x => (decimal)x.Balance);
            if (historyDays > totalHistoryAvailable)
            {
                ModelState.AddModelError("exist", $"Requested accumulated days ({historyDays}) exceed available history balance of {totalHistoryAvailable}.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            historyDays = totalHistoryAvailable;
            // 4. Financial & Tax Calculation
            decimal totalPayableDays = currentYearDays + totalHistoryAvailable;
            var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault(x => x.CurrentValue > 0);
            int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);

            decimal basicSalary = placement.Salary;
            decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
            decimal grossAmount = Math.Round(dailyRate * totalPayableDays, 2);

            decimal monthlyShare = grossAmount / 12m;
            decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
            decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)(basicSalary + monthlyShare));
            decimal totalTaxPayable = Math.Round((newTax - baseTax) * 12m, 2);
            decimal netPay = grossAmount - totalTaxPayable;

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var employeeInfo = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePaid.EmployeeId);

                        var entity = new LeavePaid
                        {
                            EmployeeId = leavePaid.EmployeeId,
                            GlFiscalYearId = leavePaid.GlFiscalYearId,
                            Amount = (double)currentYearDays,
                            AccumulatedLeaveDays = historyDays,
                            TotalPayableDays = totalPayableDays,
                            BasicSalary = basicSalary,
                            DailyRate = dailyRate,
                            GrossAmount = grossAmount,
                            AmountInBirr = grossAmount,
                            TaxPayable = totalTaxPayable,
                            NetPay = netPay,
                            IsCalculated = true,
                            Remark = leavePaid.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = User.Identity.Name,
                            EmployeeCode = employeeInfo?.EmployeeCode,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        };

                        db.LeavePaid.Add(entity);
                        db.SaveChanges(); // Generates LeavePaidId

                        // 5. FIFO Deduction on Active Entitlements
                        decimal remainingActiveToDeduct = currentYearDays;
                        var sortedActive = (from ent in activeEntitlements
                                            join gl in db.GLFiscalYears on ent.GlFiscalYearId equals gl.GlFiscalYearId
                                            orderby gl.DateTo ascending
                                            select ent).ToList();

                        foreach (var ent in sortedActive)
                        {
                            if (remainingActiveToDeduct <= 0) break;

                            decimal deducted = Math.Min((decimal)ent.Balance, remainingActiveToDeduct);
                            ent.Balance -= deducted;
                            remainingActiveToDeduct -= deducted;

                            db.Entry(ent).State = EntityState.Modified;

                            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
                            {
                                LeavePaidId = entity.LeavePaidId,
                                LeaveEntitleId = ent.LeaveEntitleId,
                                LeaveEntitlementHistoryId = null,
                                SourceType = "Active",
                                DeductedDays = deducted,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            });
                        }

                        // 6. FIFO Deduction on Leave Entitlement History
                        decimal remainingHistoryToDeduct = historyDays;
                        var sortedHistory = historyEntitlements
                            .OrderBy(h => h.GlFiscalYearId)
                            .ThenBy(h => h.LeaveEntitlementHistoryId)
                            .ToList();

                        foreach (var hist in sortedHistory)
                        {
                            if (remainingHistoryToDeduct <= 0) break;

                            decimal deducted = Math.Min((decimal)hist.Balance, remainingHistoryToDeduct);
                            hist.Balance -= deducted;
                            remainingHistoryToDeduct -= deducted;

                            db.Entry(hist).State = EntityState.Modified;

                            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
                            {
                                LeavePaidId = entity.LeavePaidId,
                                LeaveEntitleId = null,
                                LeaveEntitlementHistoryId = hist.LeaveEntitlementHistoryId,
                                SourceType = "History",
                                DeductedDays = deducted,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            });
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        leavePaid.LeavePaidId = entity.LeavePaidId;
                        leavePaid.TotalPayableDays = entity.TotalPayableDays;
                        leavePaid.BasicSalary = basicSalary;
                        leavePaid.DailyRate = dailyRate;
                        leavePaid.GrossAmount = grossAmount;
                        leavePaid.AmountInBirr = grossAmount;
                        leavePaid.TaxPayable = totalTaxPayable;
                        leavePaid.NetPay = netPay;
                        leavePaid.IsCalculated = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("error", "Failed to save leave payout: " + ex.Message);
                        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
                    }
                }
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Update([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();

            var existingEntity = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == leavePaid.LeavePaidId);
            if (existingEntity == null)
            {
                ModelState.AddModelError("exist", "The specified leave payment record was not found.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            if (existingEntity.IsSentToFinance)
            {
                ModelState.AddModelError("exist", "Cannot update a leave payment record that has already been sent to Finance.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            decimal currentYearDays = (decimal)leavePaid.Amount;
            decimal historyDays = (decimal)leavePaid.AccumulatedLeaveDays;

            if (currentYearDays < 0 || historyDays < 0 || (currentYearDays + historyDays) <= 0)
            {
                ModelState.AddModelError("exist", "Total leave days paid in cash (Current + Accumulated) must be greater than zero.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            var placement = db.Placements
                .Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId)
                .FirstOrDefault()
                ?? db.Placements
                    .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                    .OrderByDescending(x => x.AssignmentDate)
                    .FirstOrDefault();

            if (placement == null || placement.Salary <= 0)
            {
                ModelState.AddModelError("PayableDays", "This employee must have a valid placement record with a basic salary greater than zero.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            // Restore prior deductions in-memory to validate available balances accurately
            var existingDeductions = db.LeavePaidDeductionDetails
                .Where(x => x.LeavePaidId == existingEntity.LeavePaidId)
                .ToList();

            var activeEntitlements = db.LeavesEntitle
                .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                .ToList();

            var historyEntitlements = db.LeaveEntitlementHistories
                .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                .ToList();

            decimal activeRestoredSum = existingDeductions.Where(d => d.LeaveEntitleId.HasValue).Sum(d => d.DeductedDays);
            decimal historyRestoredSum = existingDeductions.Where(d => d.LeaveEntitlementHistoryId.HasValue).Sum(d => d.DeductedDays);

            decimal currentAvailable = activeEntitlements.Where(x => !x.IsExpired).Sum(x => (decimal)x.Balance) + activeRestoredSum;
            decimal historyAvailable = historyEntitlements.Sum(x => (decimal)x.Balance) + historyRestoredSum;

            if (currentYearDays > currentAvailable)
            {
                ModelState.AddModelError("exist", $"Requested current days ({currentYearDays}) exceed available active balance of {currentAvailable}.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            if (historyDays > historyAvailable)
            {
                ModelState.AddModelError("exist", $"Requested accumulated days ({historyDays}) exceed available history balance of {historyAvailable}.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            historyDays = historyAvailable;

            // Financial & Tax Recalculation
            decimal totalPayableDays = currentYearDays + historyDays;
            var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault(x => x.CurrentValue > 0);
            int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);

            decimal basicSalary = placement.Salary;
            decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
            decimal grossAmount = Math.Round(dailyRate * totalPayableDays, 2);

            decimal monthlyShare = grossAmount / 12m;
            decimal newMonthlyGross = basicSalary + monthlyShare;

            decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
            decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)newMonthlyGross);
            decimal totalTaxPayable = Math.Round((newTax - baseTax) * 12m, 2);
            decimal netPay = grossAmount - totalTaxPayable;

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. REVERSE PREVIOUS DEDUCTIONS
                        foreach (var detail in existingDeductions)
                        {
                            if (detail.LeaveEntitleId.HasValue)
                            {
                                var ent = activeEntitlements.FirstOrDefault(x => x.LeaveEntitleId == detail.LeaveEntitleId.Value);
                                if (ent != null)
                                {
                                    ent.Balance += detail.DeductedDays;
                                    db.Entry(ent).State = EntityState.Modified;
                                }
                            }
                            else if (detail.LeaveEntitlementHistoryId.HasValue)
                            {
                                var hist = historyEntitlements.FirstOrDefault(x => x.LeaveEntitlementHistoryId == detail.LeaveEntitlementHistoryId.Value);
                                if (hist != null)
                                {
                                    hist.Balance += detail.DeductedDays;
                                    db.Entry(hist).State = EntityState.Modified;
                                }
                            }

                            db.LeavePaidDeductionDetails.Remove(detail);
                        }

                        // 2. UPDATE MAIN ENTITY
                        var employeeInfo = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePaid.EmployeeId);

                        existingEntity.Amount = (double)currentYearDays;
                        existingEntity.AccumulatedLeaveDays = historyDays;
                        existingEntity.TotalPayableDays = totalPayableDays;
                        existingEntity.BasicSalary = basicSalary;
                        existingEntity.DailyRate = dailyRate;
                        existingEntity.GrossAmount = grossAmount;
                        existingEntity.AmountInBirr = grossAmount;
                        existingEntity.TaxPayable = totalTaxPayable;
                        existingEntity.NetPay = netPay;
                        existingEntity.IsCalculated = true;
                        existingEntity.Remark = leavePaid.Remark;
                        existingEntity.LastModified = DateTime.Now;
                        existingEntity.ModifiedBy = User.Identity.Name;

                        if (employeeInfo != null)
                        {
                            existingEntity.EmployeeCode = employeeInfo.EmployeeCode;
                        }

                        db.Entry(existingEntity).State = EntityState.Modified;

                        // 3. APPLY NEW ACTIVE FIFO DEDUCTIONS
                        decimal remainingActiveToDeduct = currentYearDays;
                        var sortedActive = (from ent in activeEntitlements.Where(x => !x.IsExpired)
                                            join gl in db.GLFiscalYears on ent.GlFiscalYearId equals gl.GlFiscalYearId
                                            orderby gl.DateTo ascending
                                            select ent).ToList();

                        foreach (var ent in sortedActive)
                        {
                            if (remainingActiveToDeduct <= 0) break;

                            decimal deducted = Math.Min((decimal)ent.Balance, remainingActiveToDeduct);
                            ent.Balance -= deducted;
                            remainingActiveToDeduct -= deducted;

                            db.Entry(ent).State = EntityState.Modified;

                            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
                            {
                                LeavePaidId = existingEntity.LeavePaidId,
                                LeaveEntitleId = ent.LeaveEntitleId,
                                LeaveEntitlementHistoryId = null,
                                SourceType = "Active",
                                DeductedDays = deducted,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            });
                        }

                        // 4. APPLY NEW HISTORY FIFO DEDUCTIONS
                        decimal remainingHistoryToDeduct = historyDays;
                        var sortedHistory = historyEntitlements
                            .OrderBy(h => h.GlFiscalYearId)
                            .ThenBy(h => h.LeaveEntitlementHistoryId)
                            .ToList();

                        foreach (var hist in sortedHistory)
                        {
                            if (remainingHistoryToDeduct <= 0) break;

                            decimal deducted = Math.Min((decimal)hist.Balance, remainingHistoryToDeduct);
                            hist.Balance -= deducted;
                            remainingHistoryToDeduct -= deducted;

                            db.Entry(hist).State = EntityState.Modified;

                            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
                            {
                                LeavePaidId = existingEntity.LeavePaidId,
                                LeaveEntitleId = null,
                                LeaveEntitlementHistoryId = hist.LeaveEntitlementHistoryId,
                                SourceType = "History",
                                DeductedDays = deducted,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            });
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        leavePaid.TotalPayableDays = totalPayableDays;
                        leavePaid.BasicSalary = basicSalary;
                        leavePaid.DailyRate = dailyRate;
                        leavePaid.GrossAmount = grossAmount;
                        leavePaid.AmountInBirr = grossAmount;
                        leavePaid.TaxPayable = totalTaxPayable;
                        leavePaid.NetPay = netPay;
                        leavePaid.IsCalculated = true;

                        #region Operation Log
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == existingEntity.EmployeeId);
                        if (person != null)
                        {
                            UserHelper.OperationLog(
                                Request.UserHostAddress,
                                "Leave Paid",
                                "Success",
                                User.Identity.Name,
                                $"Leave Paid of {person.EmployeeCode} updated by {User.Identity.Name}. Total Days: {totalPayableDays}, Gross: {grossAmount:N2}, Net: {netPay:N2}",
                                Modules.HRM);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("error", "Failed to update leave payout: " + ex.Message);
                        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
                    }
                }
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Destroy([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();

            var existingEntity = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == leavePaid.LeavePaidId);
            if (existingEntity == null)
            {
                ModelState.AddModelError("exist", "Record not found.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            if (existingEntity.IsSentToFinance)
            {
                ModelState.AddModelError("exist", "Cannot delete a leave payment record that has been sent to Finance.");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var deductionDetails = db.LeavePaidDeductionDetails
                            .Where(x => x.LeavePaidId == existingEntity.LeavePaidId)
                            .ToList();

                        // Exact balance restoration from single deduction detail table
                        foreach (var detail in deductionDetails)
                        {
                            if (detail.LeaveEntitleId.HasValue)
                            {
                                var entitlement = db.LeavesEntitle.FirstOrDefault(x => x.LeaveEntitleId == detail.LeaveEntitleId.Value);
                                if (entitlement != null)
                                {
                                    entitlement.Balance += detail.DeductedDays;
                                    db.Entry(entitlement).State = EntityState.Modified;
                                }
                            }
                            else if (detail.LeaveEntitlementHistoryId.HasValue)
                            {
                                var history = db.LeaveEntitlementHistories.FirstOrDefault(x => x.LeaveEntitlementHistoryId == detail.LeaveEntitlementHistoryId.Value);
                                if (history != null)
                                {
                                    history.Balance += detail.DeductedDays;
                                    db.Entry(history).State = EntityState.Modified;
                                }
                            }

                            db.LeavePaidDeductionDetails.Remove(detail);
                        }

                        db.LeavePaid.Remove(existingEntity);
                        db.SaveChanges();
                        transaction.Commit();

                        #region Delete Log
                        UserHelper.DeleteLog(
                            Request.UserHostAddress,
                            "Delete",
                            "Success",
                            User.Identity.Name,
                            $"Deleted leave paid cash record for {existingEntity.EmployeeCode}. Restored active and history leave balances.",
                            Modules.HRM);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("error", "Failed to delete leave payout record: " + ex.Message);
                        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
                    }
                }
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }       
        
        //public ActionResult LeavePaid_Create([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        //{
        //    ModelState.Clear();

        //    if (leavePaid.Amount <= 0)
        //    {
        //        ModelState.AddModelError("exist", "Leave amount of days paid in cash must be greater than zero(0)");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    var placement = db.Placements
        //        .Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId)
        //        .FirstOrDefault()
        //        ?? db.Placements
        //            .Where(x => x.EmployeeId == leavePaid.EmployeeId)
        //            .OrderByDescending(x => x.AssignmentDate)
        //            .FirstOrDefault();

        //    if (placement == null || placement.Salary <= 0)
        //    {
        //        ModelState.AddModelError("PayableDays", "Employee must have a valid placement with basic salary.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    var leaveEntitleBalance = db.LeavesEntitle
        //        .Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId)
        //        .ToList();


        //    decimal requestedDays = (decimal)leavePaid.Amount;
        //    if (leaveEntitleBalance.Sum(x => x.Balance) < requestedDays)
        //    {
        //        ModelState.AddModelError("exist", "Requested days exceed available balance of " + leaveEntitleBalance.Sum(x => x.Balance));
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }
        //    var leaveEntitleHistoryBalances = db.LeaveEntitlementHistories
        //    .Where(x => x.EmployeeId == leavePaid.EmployeeId).ToList();

        //    decimal leaveEntitleHistoryBalance = leaveEntitleHistoryBalances.Any() ? leaveEntitleHistoryBalances.Sum(x => x.Balance):0;

        //    if (leaveEntitleHistoryBalance < leavePaid.AccumulatedLeaveDays)
        //    {
        //        ModelState.AddModelError("exist", "Requested days exceed available balance of Leave EntitmentHistory Balance" + leaveEntitleHistoryBalance);
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }
        //    if (leaveEntitleHistoryBalance > 0)
        //        requestedDays += leaveEntitleHistoryBalance;
        //    // Calculations
        //    var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault(x => x.CurrentValue > 0);
        //    int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);

        //    decimal basicSalary = placement.Salary;
        //    decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
        //    decimal grossAmount = Math.Round(dailyRate * requestedDays, 2);

        //    decimal monthlyShare = grossAmount / 12m;
        //    decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
        //    decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)(basicSalary + monthlyShare));
        //    decimal totalTaxPayable = Math.Round((newTax - baseTax) * 12m, 2);
        //    decimal netPay = grossAmount - totalTaxPayable;

        //    if (ModelState.IsValid)
        //    {
        //        var employeeInfo = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePaid.EmployeeId);

        //        var entity = new LeavePaid
        //        {
        //            EmployeeId = leavePaid.EmployeeId,
        //            GlFiscalYearId = leavePaid.GlFiscalYearId,
        //            Amount = leavePaid.Amount,
        //            AccumulatedLeaveDays = leavePaid.AccumulatedLeaveDays,
        //            TotalPayableDays = (leavePaid?.AccumulatedLeaveDays??0)+(decimal)leavePaid.Amount,
        //            BasicSalary = basicSalary,
        //            DailyRate = dailyRate,
        //            GrossAmount = grossAmount,
        //            AmountInBirr = grossAmount,
        //            TaxPayable = totalTaxPayable,
        //            NetPay = netPay,
        //            IsCalculated = true,
        //            Remark = leavePaid.Remark,
        //            DateCreated = DateTime.Now,
        //            LastModified = DateTime.Now,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = User.Identity.Name,
        //            EmployeeCode = employeeInfo?.EmployeeCode,
        //            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
        //        };

        //        db.LeavePaid.Add(entity);
        //        db.SaveChanges(); // Generates LeavePaidId

        //        // FIFO Deduction + Audit Mappings Insertion
        //        decimal daysToDeduct = requestedDays;
        //        var activeEntitlements = (from ent in leaveEntitleBalance
        //                                  join gl in db.GLFiscalYears on ent.GlFiscalYearId equals gl.GlFiscalYearId
        //                                  orderby gl.DateTo ascending
        //                                  select ent).ToList();

        //        foreach (var ent in activeEntitlements)
        //        {
        //            if (daysToDeduct <= 0) break;

        //            decimal deductedFromThisYear = Math.Min(ent.Balance, daysToDeduct);

        //            ent.Balance -= deductedFromThisYear;
        //            daysToDeduct -= deductedFromThisYear;

        //            if (ent.Balance == 0)
        //            {
        //                ent.IsExpired = true;
        //            }

        //            db.Entry(ent).State = EntityState.Modified;

        //            // Save Deduction Detail Line
        //            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
        //            {
        //                LeavePaidId = entity.LeavePaidId,
        //                LeaveEntitleId = ent.LeaveEntitleId,
        //                DeductedDays = deductedFromThisYear,
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name
        //            });
        //        }

        //        db.SaveChanges();

        //        leavePaid.LeavePaidId = entity.LeavePaidId;
        //        leavePaid.BasicSalary = basicSalary;
        //        leavePaid.DailyRate = dailyRate;
        //        leavePaid.GrossAmount = grossAmount;
        //        leavePaid.AmountInBirr = grossAmount;
        //        leavePaid.TaxPayable = totalTaxPayable;
        //        leavePaid.NetPay = netPay;
        //        leavePaid.IsCalculated = true;
        //    }

        //    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //}


        //[AuthorizeRoles(HrmConstants.HRLeaveUser)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeavePaid_Update([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        //{
        //    ModelState.Clear();

        //    // 1. Fetch Existing Record
        //    var existingEntity = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == leavePaid.LeavePaidId);
        //    if (existingEntity == null)
        //    {
        //        ModelState.AddModelError("exist", "The specified leave payment record was not found.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    // 2. Validate Input Days
        //    if (leavePaid.Amount <= 0)
        //    {
        //        ModelState.AddModelError("exist", "Leave amount of days paid in cash must be greater than zero(0)");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    // 3. Fetch Employee Placement & Salary
        //    var placement = db.Placements
        //        .Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId)
        //        .FirstOrDefault();

        //    if (placement == null)
        //    {
        //        placement = db.Placements
        //            .Where(x => x.EmployeeId == leavePaid.EmployeeId)
        //            .OrderByDescending(x => x.AssignmentDate)
        //            .FirstOrDefault();
        //    }

        //    if (placement == null || placement.Salary <= 0)
        //    {
        //        ModelState.AddModelError("PayableDays", "This employee must have a valid placement record with a basic salary greater than zero.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    // 4. Temporarily Reverse Existing Deductions to Check True Available Balance
        //    var existingDetails = db.LeavePaidDeductionDetails
        //        .Where(x => x.LeavePaidId == existingEntity.LeavePaidId)
        //        .ToList();

        //    var activeEntitlements = db.LeavesEntitle
        //        .Where(x => x.EmployeeId == leavePaid.EmployeeId)
        //        .ToList();

        //    // Restore balances in memory to compute effective available balance
        //    decimal currentAvailableBalance = activeEntitlements.Where(x => !x.IsExpired).Sum(x => x.Balance);
        //    decimal previousDeductedTotal = existingDetails.Sum(x => x.DeductedDays);
        //    decimal effectiveAvailableBalance = currentAvailableBalance + previousDeductedTotal;

        //    decimal requestedDays = (decimal)leavePaid.Amount;

        //    if (requestedDays > effectiveAvailableBalance)
        //    {

        //        ModelState.AddModelError("exist", $"The requested days ({requestedDays}) exceed the total available entitlement balance of {effectiveAvailableBalance}.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

        //    }

        //    // 5. Recalculate Payout & Tax Spread
        //    var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault(x => x.CurrentValue > 0);
        //    int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);

        //    decimal basicSalary = placement.Salary;
        //    decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
        //    decimal grossAmount = Math.Round(dailyRate * requestedDays, 2);

        //    // 12-Month Spread Tax Calculations
        //    decimal monthlyShare = grossAmount / 12m;
        //    decimal newMonthlyGross = basicSalary + monthlyShare;

        //    decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
        //    decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)newMonthlyGross);
        //    decimal monthlyTaxDiff = newTax - baseTax;

        //    decimal totalTaxPayable = Math.Round(monthlyTaxDiff * 12m, 2);
        //    decimal netPay = grossAmount - totalTaxPayable;

        //    // 6. Apply Changes to DB
        //    if (ModelState.IsValid)
        //    {
        //        // A. REVERSE OLD DEDUCTIONS IN DB
        //        foreach (var detail in existingDetails)
        //        {
        //            var entitlement = activeEntitlements.FirstOrDefault(x => x.LeaveEntitleId == detail.LeaveEntitleId);
        //            if (entitlement != null)
        //            {
        //                entitlement.Balance += detail.DeductedDays;
        //                if (entitlement.Balance > 0)
        //                {
        //                    entitlement.IsExpired = false;
        //                }
        //                db.Entry(entitlement).State = EntityState.Modified;
        //            }
        //            db.LeavePaidDeductionDetails.Remove(detail); // Remove old mapping detail
        //        }

        //        // B. UPDATE MAIN LEAVE PAID ENTITY
        //        var employeeInfo = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePaid.EmployeeId);

        //        existingEntity.Amount = leavePaid.Amount;
        //        existingEntity.BasicSalary = basicSalary;
        //        existingEntity.DailyRate = dailyRate;
        //        existingEntity.GrossAmount = grossAmount;
        //        existingEntity.AmountInBirr = grossAmount; // Backwards compatibility
        //        existingEntity.TaxPayable = totalTaxPayable;
        //        existingEntity.NetPay = netPay;
        //        existingEntity.IsCalculated = true;
        //        existingEntity.Remark = leavePaid.Remark;
        //        existingEntity.LastModified = DateTime.Now;
        //        existingEntity.ModifiedBy = User.Identity.Name;

        //        if (employeeInfo != null)
        //        {
        //            existingEntity.EmployeeCode = employeeInfo.EmployeeCode;
        //        }

        //        db.Entry(existingEntity).State = EntityState.Modified;

        //        // C. APPLY NEW FIFO DEDUCTIONS & CREATE NEW MAPPING DETAILS
        //        decimal daysToDeduct = requestedDays;

        //        // Sort active/restored entitlements by fiscal year ascending (FIFO)
        //        var sortedEntitlements = (from ent in activeEntitlements.Where(x => !x.IsExpired)
        //                                  join gl in db.GLFiscalYears on ent.GlFiscalYearId equals gl.GlFiscalYearId
        //                                  orderby gl.DateTo ascending
        //                                  select ent).ToList();

        //        foreach (var ent in sortedEntitlements)
        //        {
        //            if (daysToDeduct <= 0) break;

        //            decimal deductedFromThisYear = Math.Min(ent.Balance, daysToDeduct);

        //            ent.Balance -= deductedFromThisYear;
        //            daysToDeduct -= deductedFromThisYear;

        //            if (ent.Balance == 0)
        //            {
        //                ent.IsExpired = true;
        //            }

        //            db.Entry(ent).State = EntityState.Modified;

        //            // Create new audit record
        //            db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
        //            {
        //                LeavePaidId = existingEntity.LeavePaidId,
        //                LeaveEntitleId = ent.LeaveEntitleId,
        //                DeductedDays = deductedFromThisYear,
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name
        //            });
        //        }

        //        db.SaveChanges(); // Commit entity, entitlement updates, and new mapping details

        //        // Pass updated values back to UI model
        //        leavePaid.BasicSalary = basicSalary;
        //        leavePaid.DailyRate = dailyRate;
        //        leavePaid.GrossAmount = grossAmount;
        //        leavePaid.AmountInBirr = grossAmount;
        //        leavePaid.TaxPayable = totalTaxPayable;
        //        leavePaid.NetPay = netPay;
        //        leavePaid.IsCalculated = true;

        //        #region Operation Log
        //        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == existingEntity.EmployeeId);
        //        if (person != null)
        //        {
        //            UserHelper.OperationLog(
        //                Request.UserHostAddress,
        //                "Leave Paid",
        //                "Success",
        //                User.Identity.Name,
        //                $"Leave Paid of {person.EmployeeCode} updated by {User.Identity.Name}. Gross: {grossAmount:N2}, Tax: {totalTaxPayable:N2}, Net: {netPay:N2}",
        //                Modules.HRM);
        //        }
        //        #endregion
        //    }

        //    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //}


        //[AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeavePaid_Destroy([DataSourceRequest] DataSourceRequest request, LeavePaid leavePaid)
        //{
        //    ModelState.Clear();

        //    var existingEntity = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == leavePaid.LeavePaidId);
        //    if (existingEntity == null)
        //    {
        //        ModelState.AddModelError("exist", "Record not found.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    if (existingEntity.IsSentToFinance)
        //    {
        //        ModelState.AddModelError("exist", "Cannot delete a leave payment record that has been sent to Finance.");
        //        return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // 1. Fetch exact deduction details mapped to this payment
        //        var deductionDetails = db.LeavePaidDeductionDetails
        //            .Where(x => x.LeavePaidId == existingEntity.LeavePaidId)
        //            .ToList();

        //        // 2. Exact Balance Restoration
        //        foreach (var detail in deductionDetails)
        //        {
        //            var entitlement = db.LeavesEntitle.FirstOrDefault(x => x.LeaveEntitleId == detail.LeaveEntitleId);
        //            if (entitlement != null)
        //            {
        //                entitlement.Balance += detail.DeductedDays;

        //                // If it was marked expired due to 0 balance, reactivate it
        //                if (entitlement.Balance > 0)
        //                {
        //                    entitlement.IsExpired = false;
        //                }

        //                db.Entry(entitlement).State = EntityState.Modified;
        //            }

        //            // Remove mapping detail
        //            db.LeavePaidDeductionDetails.Remove(detail);
        //        }

        //        // 3. Remove Main Payment Entity & Save
        //        db.LeavePaid.Remove(existingEntity);
        //        db.SaveChanges();

        //        #region Delete Log
        //        UserHelper.DeleteLog(
        //            Request.UserHostAddress,
        //            "Delete",
        //            "Success",
        //            User.Identity.Name,
        //            $"Deleted leave paid cash record for {existingEntity.EmployeeCode}. Restored exact deductions.",
        //            Modules.HRM);
        //        #endregion
        //    }

        //    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        //}


        [HttpPost]
        public JsonResult CalculateLeaveBirrAmount(int id)
        {
            var leavePaid = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == id);
            if (leavePaid == null)
            {
                return Json(new { Message = "Record not found", Status = "NOT-OK" });
            }

            if (leavePaid.IsSentToFinance)
            {
                return Json(new { Message = "Already sent for Payment.", Status = "NOT-OK" });
            }

            // 1. Fetch Employee Placement & Basic Salary
            var placement = db.Placements
                .Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId)
                .FirstOrDefault()
                ?? db.Placements
                    .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                    .OrderByDescending(x => x.AssignmentDate)
                    .FirstOrDefault();

            if (placement == null || placement.Salary <= 0)
            {
                return Json(new { Message = "Employee must have a valid placement record with a basic salary greater than zero.", Status = "NOT-OK" });
            }

            // Begin Database Transaction
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 2. Fetch Active Entitlements & Existing Mapping Records
                    var activeEntitlements = db.LeavesEntitle
                        .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                        .ToList();

                    var existingDetails = db.LeavePaidDeductionDetails
                        .Where(x => x.LeavePaidId == leavePaid.LeavePaidId)
                        .ToList();

                    // Compute effective available balance (Current Active Balance + Any Previously Deducted Days for this record)
                    decimal currentAvailableBalance = activeEntitlements.Where(x => !x.IsExpired).Sum(x => x.Balance);
                    decimal previouslyDeductedTotal = existingDetails.Sum(x => x.DeductedDays);
                    decimal totalEffectiveAvailable = currentAvailableBalance + previouslyDeductedTotal;

                    decimal requestedDays = (decimal)leavePaid.Amount;

                    if (requestedDays > totalEffectiveAvailable)
                    {
                        transaction.Rollback();
                        return Json(new { Message = $"The requested amount of {requestedDays} days exceeds your available balance of {totalEffectiveAvailable}.", Status = "NOT-OK" });
                    }

                    // 3. Perform 12-Month Spread Tax Calculation
                    var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault(x => x.CurrentValue > 0);
                    int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);

                    decimal basicSalary = placement.Salary;
                    decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
                    decimal grossAmount = Math.Round(dailyRate * requestedDays, 2);

                    // Spread Gross over 12 months
                    decimal monthlyShare = grossAmount / 12m;
                    decimal newMonthlyGross = basicSalary + monthlyShare;

                    decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
                    decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)newMonthlyGross);
                    decimal monthlyTaxDiff = newTax - baseTax;

                    decimal totalTaxPayable = Math.Round(monthlyTaxDiff * 12m, 2);
                    decimal netPay = grossAmount - totalTaxPayable;

                    // 4. Update Main LeavePaid Record
                    leavePaid.BasicSalary = basicSalary;
                    leavePaid.DailyRate = dailyRate;
                    leavePaid.GrossAmount = grossAmount;
                    leavePaid.AmountInBirr = grossAmount; // Backwards compatibility
                    leavePaid.TaxPayable = totalTaxPayable;
                    leavePaid.NetPay = netPay;
                    leavePaid.IsCalculated = true;
                    leavePaid.LastModified = DateTime.Now;
                    leavePaid.ModifiedBy = User.Identity.Name;

                    db.Entry(leavePaid).State = EntityState.Modified;

                    // 5. Handle Entitlement Deductions & LeavePaidDeductionDetails Mappings
                    if (existingDetails.Any())
                    {
                        // A. Reverse Old Deductions in Memory/DB
                        foreach (var detail in existingDetails)
                        {
                            var entitlement = activeEntitlements.FirstOrDefault(x => x.LeaveEntitleId == detail.LeaveEntitleId);
                            if (entitlement != null)
                            {
                                entitlement.Balance += detail.DeductedDays;
                                if (entitlement.Balance > 0)
                                {
                                    entitlement.IsExpired = false;
                                }
                                db.Entry(entitlement).State = EntityState.Modified;
                            }
                            db.LeavePaidDeductionDetails.Remove(detail);
                        }
                    }

                    // B. Apply Fresh FIFO Deductions and Generate New LeavePaidDeductionDetails
                    decimal daysToDeduct = requestedDays;
                    var sortedEntitlements = (from ent in activeEntitlements.Where(x => !x.IsExpired)
                                              join gl in db.GLFiscalYears on ent.GlFiscalYearId equals gl.GlFiscalYearId
                                              orderby gl.DateTo ascending // Oldest fiscal year first
                                              select ent).ToList();

                    foreach (var ent in sortedEntitlements)
                    {
                        if (daysToDeduct <= 0) break;

                        decimal deductedFromThisYear = Math.Min(ent.Balance, daysToDeduct);

                        ent.Balance -= deductedFromThisYear;
                        daysToDeduct -= deductedFromThisYear;

                        if (ent.Balance == 0)
                        {
                            ent.IsExpired = true;
                        }

                        db.Entry(ent).State = EntityState.Modified;

                        // Create new deduction mapping record
                        db.LeavePaidDeductionDetails.Add(new LeavePaidDeductionDetail
                        {
                            LeavePaidId = leavePaid.LeavePaidId,
                            LeaveEntitleId = ent.LeaveEntitleId,
                            DeductedDays = deductedFromThisYear,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        });
                    }

                    // 6. Commit Database Changes
                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new
                    {
                        Message = $"Successfully calculated leave payout! Gross: {grossAmount:N2} ETB, Tax: {totalTaxPayable:N2} ETB, Net: {netPay:N2} ETB",
                        Days = leavePaid.Amount,
                        BasicSalary = basicSalary,
                        DailyRate = dailyRate,
                        GrossAmount = grossAmount,
                        TaxPayable = totalTaxPayable,
                        NetPay = netPay,
                        AmountInBirr = grossAmount,
                        Status = "OK"
                    });
                }
                catch (Exception ex)
                {
                    // Roll back all changes if any error occurs during calculation or saving
                    transaction.Rollback();
                    return Json(new { Message = "Error saving calculation to database: " + ex.Message, Status = "NOT-OK" });
                }
            }
        }
        #region existing code 
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Createexisted([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            var leaveEntitleBalance = db.LeavesEntitle.Where(x => !x.IsExpired
            && x.EmployeeId == leavePaid.EmployeeId).ToList();

            if (leavePaid.Amount <= 0)
            {
                ModelState.AddModelError("exist", "Leave amount of days paid in cash must be greater than zero(0)");
                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

            }

            if (leavePaid.AmountInBirr <= 0)
            {
                var payableDetail = db.LeavePayableDetails
                    .FirstOrDefault(x =>
                        x.Payable.GlFiscalYearId == leavePaid.GlFiscalYearId &&
                        x.EmployeeId == leavePaid.EmployeeId);

                if (payableDetail == null)
                {
                    ModelState.AddModelError(
                        "exist",
                        "The employee does not have a generated leave payable. Please generate it first.");

                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
                }

                if (payableDetail.PayableDays <= 0)
                {
                    ModelState.AddModelError(
                        "PayableDays",
                        "Payable days must be greater than zero.");

                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
                }

                // Calculate paid amount in ETB
                leavePaid.AmountInBirr = Math.Round(
                    ((decimal)leavePaid.Amount * payableDetail.NetPayment) / payableDetail.PayableDays,
                    2,
                    MidpointRounding.AwayFromZero);

                leavePaid.IsCalculated = true;
            }

            var placement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId).FirstOrDefault();
            if (placement == null)
            {
                placement = db.Placements.Where(x => x.EmployeeId == leavePaid.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
            }
            if (placement == null && placement.Salary <= 0)
            {
                ModelState.AddModelError(
                      "PayableDays",
                      "THis EMployee Must have placement with corrcet Basic salary.");

                return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
            }
            if (leaveEntitleBalance.Any())
            {
                if ((decimal)leavePaid.Amount > leaveEntitleBalance.Sum(x => x.Balance))
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than your balance which is =" + leaveEntitleBalance.Sum(x => x.Balance));
                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

                }
            }
            else
            {
                decimal maximumValue = 0;
                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
                if (maximumLeave != null)
                {
                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
                }
                if ((decimal)leavePaid.Amount > maximumValue)
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than maximum  Leave Value is =" + maximumValue);
                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

                }
            }
            var leavePayableSetting = db.LeavePayableSettings.Where(x => x.CurrentValue > 0).FirstOrDefault();

            int workingDaysPerMonth = (int)(leavePayableSetting?.CurrentValue ?? 22);
            decimal basicSalary = placement.Salary;
            // 1. Calculations
            decimal dailyRate = Math.Round(basicSalary / workingDaysPerMonth, 2);
            decimal grossAmount = Math.Round(dailyRate * (decimal)leavePaid.Amount, 2);

            // 2. Tax Spread (12 Months) Calculation
            decimal monthlyShare = grossAmount / 12m;
            decimal newMonthlyGross = basicSalary + monthlyShare;

            decimal baseTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)basicSalary);
            decimal newTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)newMonthlyGross);
            decimal monthlyTaxDiff = newTax - baseTax;

            decimal totalTaxPayable = Math.Round(monthlyTaxDiff * 12m, 2);
            decimal netPay = grossAmount - totalTaxPayable;

            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    AmountInBirr = leavePaid.AmountInBirr,
                    Remark = leavePaid.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = leavePaid.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = leavePaid.ModifiedBy,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leavePaid.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.LeavePaid.Add(entity);
                db.SaveChanges();
                leavePaid.LeavePaidId = entity.LeavePaidId;

                decimal dateAsked = Convert.ToDecimal(leavePaid.Amount);
                decimal dateLeft = 0;
                do
                {

                    List<LeaveModel> leaveList = new List<LeaveModel>();
                    var yearLists = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId).ToList();



                    foreach (var year in yearLists)
                    {
                        var leavel = new LeaveModel();
                        var gl = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == year.GlFiscalYearId);
                        if (gl != null)
                        {
                            leavel.Year = gl.DateTo;
                        }

                        leaveList.Add(leavel);
                    }
                    var smallestDate = leaveList.Min(x => x.Year);
                    if (smallestDate != null)
                    {
                        var id = db.GLFiscalYears.FirstOrDefault(x => x.DateTo.Year == smallestDate.Year);
                        if (id != null)
                        {
                            var entiteledEmployee = db.LeavesEntitle.FirstOrDefault
                                (x => x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == id.GlFiscalYearId && !x.IsExpired);
                            if (entiteledEmployee != null)
                            {
                                if (entiteledEmployee.Balance < dateAsked)
                                {
                                    dateLeft = dateAsked - entiteledEmployee.Balance;
                                    if (dateLeft >= 0)
                                    {
                                        entiteledEmployee.Balance = 0;
                                        entiteledEmployee.IsExpired = true;

                                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                                        db.SaveChanges();

                                        dateAsked = dateLeft;
                                    }
                                }

                                else
                                {
                                    dateLeft = entiteledEmployee.Balance - dateAsked;
                                    if (dateLeft >= 0)
                                    {
                                        entiteledEmployee.Balance = dateLeft;

                                        if (dateLeft == 0)
                                        {
                                            entiteledEmployee.IsExpired = true;
                                        }

                                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                                        db.SaveChanges();

                                        dateLeft = 0;
                                    }
                                }
                            }
                        }
                    }


                } while (dateLeft > 0);

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Paid", "Success", User.Identity.Name, "Leave Paid of " + person.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_UpdateExisted([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            var leaveEntitleBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == leavePaid.GlFiscalYearId).ToList();
            if (leaveEntitleBalance.Count() > 0)
            {
                if ((decimal)leavePaid.Amount > leaveEntitleBalance.Sum(x=>x.Balance))
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than your balance which is =" + leaveEntitleBalance.Sum(x => x.Balance));
                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

                }
            }
            else
            {
                decimal maximumValue = 0;
                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
                if (maximumLeave != null)
                {
                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
                }
                if ((decimal)leavePaid.Amount > maximumValue)
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than maximum  Leave Value is =" + maximumValue);
                    return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));

                }
            }
            
            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    LeavePaidId = leavePaid.LeavePaidId,
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    AmountInBirr = leavePaid.AmountInBirr,
                    Remark = leavePaid.Remark,
                    DateCreated = leavePaid.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = leavePaid.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leavePaid.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = leavePaid.OrgBranchName
                };

                db.LeavePaid.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Paid", "Success", User.Identity.Name, "Leave Paid of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);

                }
                #endregion
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_DestroyExisted([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    LeavePaidId = leavePaid.LeavePaidId,
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    AmountInBirr = leavePaid.AmountInBirr,
                    Remark = leavePaid.Remark,
                    DateCreated = leavePaid.DateCreated,
                    LastModified = leavePaid.LastModified,
                    CreatedBy = leavePaid.CreatedBy,
                    ModifiedBy = leavePaid.ModifiedBy,
                    EmployeeCode = leavePaid.EmployeeCode,
                    OrgBranchName = leavePaid.OrgBranchName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave paid in cash  of employee" + leavePaid.EmployeeCode + " and fiscal year " + leavePaid.GlFiscalYear.Name);
                #endregion
                db.LeavePaid.Attach(entity);
                db.LeavePaid.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public JsonResult CalculateLeaveBirrAmountexisted(int id)
        {
            var leavePaid = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == id);
            if (leavePaid == null)
            {
                return Json(new { Message = "Record not found", Status = "NOT-OK" });
            }

            if (leavePaid.IsSentToFinance)
            {
                return Json(new { Message = "Already sent for Payment.", Status = "NOT-OK" });
            }

            var payableDetail = db.LeavePayableDetails.FirstOrDefault(f => f.Payable.GlFiscalYearId == leavePaid.GlFiscalYearId && f.EmployeeId == leavePaid.EmployeeId);
            if (payableDetail == null)
            {
                return Json(new { Message = "Employee has not generated leave payable, please generate first", Status = "NOT-OK" });
            }

            var leaveEntitleBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == leavePaid.GlFiscalYearId).ToList();

            if (leaveEntitleBalance.Any())
            {
                if ((decimal)leavePaid.Amount > leaveEntitleBalance.Sum(x => x.Balance) && (decimal)leavePaid.Amount > payableDetail.Balance)
                {
                    return Json(new { Message = $"The amount you entered is more than your balance of {leaveEntitleBalance.Sum(x => x.Balance)}", Status = "NOT-OK" });
                }
            }

            // Perform Calculation
            leavePaid.AmountInBirr = ((decimal)leavePaid.Amount * payableDetail.NetPayment) / payableDetail.PayableDays;
            leavePaid.IsCalculated = true;

            try
            {
                db.SaveChanges(); // CRITICAL: This saves your calculation to the database
                return Json(new { Message = $"Successfully calculated amount:{leavePaid.AmountInBirr:N2} Birr", Days = leavePaid.Amount, AmountInBirr = leavePaid.AmountInBirr, Status = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = "Error saving to database: " + ex.Message, Status = "NOT-OK" });
            }
        }

        #endregion



        [HttpPost]
        public JsonResult SendToFinance(int id)
        {
            var leavePaid = db.LeavePaid.FirstOrDefault(x=>x.LeavePaidId == id);
            if (leavePaid == null)
            {
                return Json(new { Message = "Record tot found", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            if (leavePaid.IsSentToFinance)
            {
                return Json(new { Message = "Already sent for Payment .", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);

            }

            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == leavePaid.EmployeeId);
            if(placement == null)
            {
                placement = db.Placements.Where(x =>x.EmployeeId == leavePaid.EmployeeId).OrderByDescending(x=>x.AssignmentDate).FirstOrDefault();
            }
            if(placement == null)
            {
                return Json(new { Message = "employee has never had any placement .", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);

            }
            var requestEntity = new PaymentRequest
            {
                SupplierId = "0",
                IsNewSupplier = false,
                Supplier = " ",
                Purchaser = 0,
                ReasonOfPayment = "Leave Paid in Cash ",
                Source = PaymentRequestSource.LeavePaidInCash.ToString(),
                SourceId = leavePaid.LeavePaidId.ToString(),
                RequestDate = DateTime.Now.Date,
                RequestDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                InvoiceNo = "0",
                PurchaseRequestNo = "0",
                PONo = "0",
                GRN = "0",
                BranchId = placement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                OrgStuructureId = placement.OrgStructureId,
                EmployeeId = leavePaid.EmployeeId,
                Paid = false,
                IsSendForApproval = true,
                SendForApprovalBy = User.Identity.Name,
                SendForApprovalTime = DateTime.Now,
                IsDepartmentOnlineApproved = true,
                DepartmentOnlineApprovedBy = User.Identity.Name,
                DepartmentOnlineApprovedTime = DateTime.Now,

                FromBranchId = placement.BranchId,
                FromBusinessUnitId = placement.BusinessUnitId,
                CreatedAtBranchId = placement.BranchId,
                CreatedAtBusinessUnitId = placement.BusinessUnitId,
                CreatedAtOrgStructureId = placement.OrgStructureId,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now.Date,
                ModifiedBy = User.Identity.Name,
                LastModified = DateTime.Now.Date




            };
            db.PaymentRequests.Add(requestEntity);
            db.SaveChanges();
           
            var RequestEntityDetail = new PaymentRequestLine
            {
                PaymentRequestId = requestEntity.PaymentRequestId,
                Amount = leavePaid.NetPay,
                TaxAmount = leavePaid.TaxPayable,
                TotalAmount = leavePaid.NetPay,
                ItemName = "Leave Paid in Cash",
                GLTaxRateID = null,
                Description = "Leave Paid in Cash",
                Remark = "Leave Paid in Cash",
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now.Date,
                ModifiedBy = User.Identity.Name,
                LastModified = DateTime.Now.Date,

            };
            db.PaymentRequestLines.Add(RequestEntityDetail);
            db.SaveChanges();
            
           
            leavePaid.IsSentToFinance = true;
            leavePaid.SentBy = User.Identity.Name;
            leavePaid.DateSent = DateTime.Now.Date;
            leavePaid.PaymentRequestId = requestEntity.PaymentRequestId;
            db.Entry(leavePaid).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Message = "Successfully Sent for payment Request.", Status = "OK" }, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave paid in cash");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave paid in cash");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpGet]
        public ActionResult PrintFinalLeavePaid(int id)
        {
            var leavePaid = db.LeavePaid.FirstOrDefault(x => x.LeavePaidId == id);
            if (leavePaid == null)
            {
                return HttpNotFound("Leave payment record not found.");
            }

            // 1. Fetch Company & ISO Setup
            var company = db.Companies.AsNoTracking().FirstOrDefault();
            var docFormat = db.IsoDocuments.AsNoTracking()
                .FirstOrDefault(d => d.Module == Modules.HRM && d.DocumentType == "LeavePaidInCash")
                ?? db.IsoDocuments.AsNoTracking().FirstOrDefault(d => d.Module == Modules.HRM);

            var companyId = company != null ? company.MainCompanyId.ToString() : null;
            var logo = companyId != null
                ? db.Attachments.AsNoTracking().FirstOrDefault(a => a.Reference == companyId && a.ReferenceType == AttachmentType.LOGO)
                : null;

            // 2. Fetch Employee & Placement
            var employee = db.Person_Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == leavePaid.EmployeeId);
            var placement = db.Placements.AsNoTracking()
                .Where(x => x.EmployeeId == leavePaid.EmployeeId && x.isCurrent)
                .OrderByDescending(x => x.PlacementId)
                .FirstOrDefault()
                ?? db.Placements.AsNoTracking()
                    .Where(x => x.EmployeeId == leavePaid.EmployeeId)
                    .OrderByDescending(x => x.PlacementId)
                    .FirstOrDefault();

            string branchName = "-";
            string departmentName = "-";
            string jobTitleName = "-";

            if (placement != null)
            {
                var branch = db.Branch.AsNoTracking().FirstOrDefault(x => x.BranchId == placement.BranchId);
                if (branch != null) branchName = branch.Name ?? "-";

                var dept = db.MainCompanyStructures.AsNoTracking().FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (dept != null) departmentName = dept.Name ?? "-";

                var lookup = db.Lookups.AsNoTracking().FirstOrDefault(x => x.LookupId == placement.LookupId);
                if (lookup != null) jobTitleName = lookup.Description ?? "-";
            }

            var fiscalYear = db.GLFiscalYears.AsNoTracking().FirstOrDefault(x => x.GlFiscalYearId == leavePaid.GlFiscalYearId);

            // 3. Fetch Deductions Breakdown (Active + History)
            var deductionDetails = db.LeavePaidDeductionDetails.AsNoTracking()
                .Where(x => x.LeavePaidId == id)
                .ToList();

            var breakdownList = new List<LeavePaidDeductionPrintItem>();
            foreach (var d in deductionDetails)
            {
                string periodName = "-";
                if (d.LeaveEntitleId.HasValue)
                {
                    var ent = db.LeavesEntitle.AsNoTracking().FirstOrDefault(e => e.LeaveEntitleId == d.LeaveEntitleId.Value);
                    if (ent != null)
                    {
                        var fy = db.GLFiscalYears.AsNoTracking().FirstOrDefault(f => f.GlFiscalYearId == ent.GlFiscalYearId);
                        periodName = fy != null ? fy.Name : "Current Active";
                    }
                }
                else if (d.LeaveEntitlementHistoryId.HasValue)
                {
                    var hist = db.LeaveEntitlementHistories.AsNoTracking().FirstOrDefault(h => h.LeaveEntitlementHistoryId == d.LeaveEntitlementHistoryId.Value);
                    periodName = hist != null ? hist.GlFiscalYear.Name : "History Balance";
                }

                breakdownList.Add(new LeavePaidDeductionPrintItem
                {
                    SourceType = d.SourceType ?? (d.LeaveEntitlementHistoryId.HasValue ? "History" : "Active"),
                    YearOrPeriod = periodName,
                    DeductedDays = d.DeductedDays
                });
            }

            // 4. Build Model
            var model = new LeavePaidPrintVM
            {
                CompanyNameEn = !string.IsNullOrEmpty(company?.TradeName) ? company.TradeName : (!string.IsNullOrEmpty(company?.BrandName) ? company.BrandName : company?.AmharicName),
                CompanyNameAm = company?.AmharicName,
                LogoFile = logo?.BinaryFile,
                DocumentNumber = docFormat != null && !string.IsNullOrEmpty(docFormat.FormNumber) ? docFormat.FormNumber : "MCG/HR/LP/01",
                IssueNumber = docFormat != null && !string.IsNullOrEmpty(docFormat.FormTitle) ? docFormat.FormTitle : "1",
                EffectiveDate = docFormat != null && docFormat.DateCreated.HasValue ? docFormat.DateCreated.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),

                LeavePaidId = leavePaid.LeavePaidId,
                PaymentRequestId = leavePaid.PaymentRequestId.HasValue ? leavePaid.PaymentRequestId.Value.ToString() : "Pending",
                FiscalYear = fiscalYear?.Name ?? "-",
                PrintDate = DateTime.Now.ToString("dd/MM/yyyy"),
                Remark = leavePaid.Remark,

                EmployeeCode = employee?.EmployeeCode ?? leavePaid.EmployeeCode ?? "-",
                FullName = employee != null ? $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}".Trim() : "-",
                Branch = branchName,
                Department = departmentName,
                JobTitle = jobTitleName,
                EmploymentDate =  employee.HiredDate.ToString("dd/MM/yyyy") ,

                CurrentYearDays = leavePaid.Amount,
                AccumulatedDays = leavePaid?.AccumulatedLeaveDays??0,
                TotalPayableDays = leavePaid.TotalPayableDays > 0 ? leavePaid.TotalPayableDays : ((decimal)leavePaid.Amount + leavePaid?.AccumulatedLeaveDays??0),

                BasicSalary = leavePaid.BasicSalary,
                DailyRate = leavePaid.DailyRate,
                GrossAmount = leavePaid.GrossAmount > 0 ? leavePaid.GrossAmount : leavePaid.AmountInBirr,
                TaxPayable = leavePaid.TaxPayable,
                NetPay = leavePaid.NetPay,

                DeductionBreakdown = breakdownList,
                PreparedBy = leavePaid.CreatedBy,
                PreparedDate = leavePaid.DateCreated.HasValue ? leavePaid.DateCreated.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                ApprovedBy = leavePaid.SentBy,
                ApprovedDate = leavePaid.DateSent.HasValue ? leavePaid.DateSent.Value.ToString("dd/MM/yyyy") : "-"
            };

            return View("PrintFinalLeavePaid", model);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    public class LeavePaidPrintVM
    {
        // Company Header
        public string CompanyNameEn { get; set; }
        public string CompanyNameAm { get; set; }
        public byte[] LogoFile { get; set; }
        public string DocumentNumber { get; set; }
        public string IssueNumber { get; set; }
        public string EffectiveDate { get; set; }

        // Master Reference Info
        public int LeavePaidId { get; set; }
        public string PaymentRequestId { get; set; }
        public string FiscalYear { get; set; }
        public string PrintDate { get; set; }
        public string Remark { get; set; }

        // Employee Info
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string EmploymentDate { get; set; }

        // Days Breakdown
        public double CurrentYearDays { get; set; }
        public decimal AccumulatedDays { get; set; }
        public decimal TotalPayableDays { get; set; }

        // Financial Calculation Breakdown
        public decimal BasicSalary { get; set; }
        public decimal DailyRate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TaxPayable { get; set; }
        public decimal NetPay { get; set; }

        // Deduction Source Details
        public List<LeavePaidDeductionPrintItem> DeductionBreakdown { get; set; } = new List<LeavePaidDeductionPrintItem>();

        // Approvals / Sign-off Metadata
        public string PreparedBy { get; set; }
        public byte[] PreparedBySignature { get; set; }
        public string PreparedDate { get; set; }
        public string ApprovedBy { get; set; }
        public byte[] ApprovedBySignature { get; set; }
        public string ApprovedDate { get; set; }
    }

    public class LeavePaidDeductionPrintItem
    {
        public string SourceType { get; set; }
        public string YearOrPeriod { get; set; }
        public decimal DeductedDays { get; set; }
    }
}
