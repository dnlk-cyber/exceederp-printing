using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class DailyLaborPrintOutObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyLaborPrintOutViewModel> GetDailyLaborPrintOutData(int id)
        {
            var resultList = new List<DailyLaborPrintOutViewModel>();

            //var master = _db.DailyLaborers.FirstOrDefault(x => x.DailyLaborerId == id);
            //if (master == null)
            //{
            //    return resultList;
            //}

            //var branch = _db.Branch.FirstOrDefault(b => b.BranchId == master.BranchId);
            //var fiscalYear = _db.GLFiscalYears.FirstOrDefault(y => y.GlFiscalYearId == master.GlFiscalYearId);
            //var period = _db.GLPeriods.FirstOrDefault(p => p.GLPeriodId == master.GlPeriodId);

            //var viewModel = new DailyLaborPrintOutViewModel
            //{
            //    DailyLaborerId = master.DailyLaborerId,
            //    BranchName = branch != null ? branch.Name : string.Empty,
            //    FiscalYearName = fiscalYear != null ? fiscalYear.Name : string.Empty,
            //    PeriodName = period != null ? period.Name : string.Empty,
            //    IsOnlineApproved = master.IsOnlineApproved,
            //    IsRejected = master.IsRejected,
            //    Remark = master.Remark,
            //    CreatedBy = master.CreatedBy,
            //    DateCreated = master.DateCreated,
            //    RejectionReason = master.RejectionReason,

            //    // Resolve each approver step to Full Name, Job Title, and Signature
            //    PreparedByHR = GetApproverDetails(master.PreparedByHR),
            //    CheckedBySite = GetApproverDetails(master.CheckedBySite),
            //    ApprovedByMaterial = GetApproverDetails(master.ApprovedByMaterial),
            //    CheckedByOffice = GetApproverDetails(master.CheckedByOffice),
            //    ApprovedByHRA = GetApproverDetails(master.ApprovedByHRA),
            //    AuthorizedBy = GetApproverDetails(master.AuthorizedBy),
            //    RejectedBy = GetApproverDetails(master.RejectedBy)
            //};

            //// Fetch associated employee records
            //var details = _db.DailyLaborerEmployees
            //    .Where(x => x.DailyLaborerId == id)
            //    .ToList();

            //foreach (var item in details)
            //{
            //    viewModel.LaborerEmployees.Add(new DailyLaborerEmployeeRowVM
            //    {
            //        DailyLaborerEmployeeId = item.DailyLaborerEmployeeId,
            //        EmployeeFullName = item.Employee,
            //        BankAccountNumber = item.BankAccountNumber,
            //        JobTitle = item.JobTitle,
            //        NormalDays = item.NormalDays,
            //        DayWage = item.DayWage,
            //        DayOT = item?.DayOT ?? 0,
            //        TotalDays = (decimal)item.TotalDays,
            //        NormalNight = item.NormalNight,
            //        NightWage = item.NightWage,
            //        NightOT = item?.NightOT ?? 0,
            //        NightOTWage = item?.NightOTWage ?? 0,
            //        RestTimeOT = item.RestTimeOT ?? 0,
            //        RestTimeOTWage = item.RestTimeOTWage ?? 0,
            //        HolidayOT = item.HolidayOT ?? 0,
            //        HolidayOTWage = item.HolidayOTWage ?? 0,
            //        TotalNights = (decimal)item.TotalNights,
            //        TotalWage = (decimal)item.TotalWage,
            //        IncomeTax = (decimal)item.IncomeTax,
            //        NetPay = (decimal)item.NetPay
            //    });
            //}

            //resultList.Add(viewModel);
            return resultList;
        }

        /// <summary>
        /// Resolves an approver's username to their Person_Employee record, current Job Title, and Signature
        /// </summary>
        private ApproverInfo GetApproverDetails(string userName)
        {
            var info = new ApproverInfo { UserName = userName };

            if (string.IsNullOrWhiteSpace(userName))
            {
                return info;
            }

            // 1. Resolve employee id from username
            int empId = UserHelper.GetUserEmployeeId(userName);

            // 2. Fetch employee person details
            var personEmployee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (personEmployee != null)
            {
                info.FullName = $"{personEmployee.FirstName} {personEmployee.MiddleName} {personEmployee.LastName}".Trim();
                info.Signature = UserHelper.GetUserSignature(userName);
            }
            else
            {
                // Fallback to username if no employee record is linked
                info.FullName = userName;
            }

            // 3. Fetch current placement and job title lookup
            var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent)
                            ?? _db.Placements.FirstOrDefault(x => x.EmployeeId == empId);

            if (placement != null)
            {
                var lookup = _db.Lookups.Find(placement.LookupId);
                if (lookup != null)
                {
                    info.JobTitle = !string.IsNullOrEmpty(lookup.Description)
                        ? lookup.Description
                        : lookup.DescriptionEng;
                }
            }

            return info;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}