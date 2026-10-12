using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net; // 👈 Required for WebUtility.HtmlEncode

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ClearanceObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ClearanceLetterVM GetClearanceLetter(string employeeId)
        {
            if (!int.TryParse(employeeId, out var empId))
                return new ClearanceLetterVM();

            var now = DateTime.Now;

            bool isAmharic = cultureName.Equals("am", StringComparison.OrdinalIgnoreCase) ||
                             cultureName.Equals("am-et", StringComparison.OrdinalIgnoreCase);

            bool isOromo = cultureName.Equals("om", StringComparison.OrdinalIgnoreCase);

            var clearanceRequest = db.ClearanceRequests
                .Where(x => x.EmployeeId == empId && x.IsOnlineApproved)
                .OrderByDescending(x => x.ClearanceRequestId)
                .FirstOrDefault();

            if (clearanceRequest == null)
                return new ClearanceLetterVM();

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);

            if (employee == null)
                return new ClearanceLetterVM();

            var placement = db.Placements
                .Where(x => x.EmployeeId == empId)
                .OrderByDescending(x => x.PlacementId)
                .FirstOrDefault();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            // Raw name evaluation
            string rawCompName = isAmharic
                ? organization?.AmharicName ?? organization?.BrandName
                : isOromo
                    ? organization.TradeName?? organization?.BrandName
                    : organization?.BrandName;

            string rawJobTitle = placement == null
                ? null
                : db.Lookups
                    .Where(x => x.LookupId == placement.LookupId)
                    .Select(x => x.Description)
                    .FirstOrDefault();

            string rawDepartment = placement == null
                ? null
                : db.MainCompanyStructures
                    .Where(x => x.OrgStructureId == placement.OrgStructureId)
                    .Select(x => x.Name)
                    .FirstOrDefault();

            var vm = new ClearanceLetterVM
            {
                EmployeeId = employee.EmployeeCode,
                EmployeeName = WebUtility.HtmlEncode(isAmharic || isOromo ? employee.FullNameAmharic : employee.FullNameaEnglish),
                CompanyName = WebUtility.HtmlEncode(rawCompName),
                ClearanceDate = (clearanceRequest?.DateCreated ?? now).ToString("dd.MM.yyyy"),
                Message = "Please clear the following departments.",
                JobTitle = WebUtility.HtmlEncode(rawJobTitle),
                Department = WebUtility.HtmlEncode(rawDepartment)
            };

            if (placement == null)
                return vm;

            var clearanceStatuses = db.ClearanceRequestStatus
                .Where(x =>
                    x.ClearanceRequestId == clearanceRequest.ClearanceRequestId &&
                    (x.IsDepartmentOnlineApproved ||
                     x.IsOnlineApproved ||
                     x.IsFinalApproval))
                .ToList();
            var clearanceRequestApprovers = db.ClearanceRequestApprovers.Where(x =>
                      x.ClearanceRequestId == clearanceRequest.ClearanceRequestId).ToList();

            if (!clearanceStatuses.Any() && !clearanceRequestApprovers.Any())
                return vm;

            if (clearanceStatuses.Any())
            {
                var statusDictionary = clearanceStatuses
               .GroupBy(x => x.OrgStructureId)
               .ToDictionary(x => x.Key, x => x.FirstOrDefault());

                var departmentIds = clearanceStatuses
                    .Select(x => x.OrgStructureId)
                    .Distinct()
                    .ToList();

                var departments = db.MainCompanyStructures
                    .Where(x =>
                        x.IsActive &&
                        (x.OrgStructureId == placement.OrgStructureId ||
                         x.Parent == placement.OrgStructureId ||
                         departmentIds.Contains(x.OrgStructureId)))
                    .ToList()
                    .GroupBy(x => x.OrgStructureId)
                    .Select(x => x.First())
                    .ToList();

                int serialNo = 1;

                foreach (var department in departments)
                {
                    statusDictionary.TryGetValue(department.OrgStructureId, out var statusDict);

                    vm.ClearanceLetterDetails.Add(CreateDetail(department, serialNo++, statusDict));
                }

                return vm;
            }
            else 
            {
                var statusDictionary = clearanceRequestApprovers
              .GroupBy(x => x.OrgStructureId)
              .ToDictionary(x => x.Key, x => x.FirstOrDefault());

                var departmentIds = clearanceRequestApprovers
                    .Select(x => x.OrgStructureId)
                    .Distinct()
                    .ToList();

                var departments = db.MainCompanyStructures
                    .Where(x =>
                        x.IsActive &&
                        (x.OrgStructureId == placement.OrgStructureId ||
                         x.Parent == placement.OrgStructureId ||
                         departmentIds.Contains(x.OrgStructureId)))
                    .ToList()
                    .GroupBy(x => x.OrgStructureId)
                    .Select(x => x.First())
                    .ToList();

                int serialNo = 1;

                foreach (var department in departments)
                {
                    statusDictionary.TryGetValue(department.OrgStructureId, out var statusDict);

                    vm.ClearanceLetterDetails.Add(CreateApprovalDetail(department, serialNo++, statusDict));
                }

                return vm;
            }
        }

        private ClearanceLetterDetailVM CreateDetail(MainCompanyStructure department, int serialNo, ClearanceRequestStatus status)
        {

            var detail = new ClearanceLetterDetailVM
            {
                SerialNo = serialNo,
                Department = department?.Name ?? "",
                Date = status?.DepartmentOnlineApprovedTime ?? status?.OnlineApprovedTime?? status?.DateCreated,
                Remark = status?.Remark ?? string.Empty
            };

            if (department == null)
                return detail;

            var headPlacement = db.Placements
                .Where(x =>
                    x.OrgStructureId == department.OrgStructureId &&
                    x.isCurrent &&
                    x.IsHead)
                .OrderByDescending(x => x.AssignmentDate)
                .FirstOrDefault();

            if (headPlacement == null)
                headPlacement = db.Placements
                .Where(x =>
                    x.OrgStructureId == department.OrgStructureId &&
                    x.IsHead)
                .OrderByDescending(x => x.AssignmentDate)
                .FirstOrDefault();
            if (headPlacement == null)
            {
                detail.HeadName = "No head assigned for this department";
                return detail;
            }
                

            var headEmployee = db.Person_Employee
                .FirstOrDefault(x => x.EmployeeId == headPlacement.EmployeeId);
            if(headEmployee != null)
                detail.HeadName = headEmployee.FirstNameEnglish + " "+headEmployee.MiddleNameEnglish + " "+headEmployee.LastNameEnglish ;

            var rawJobTitle = db.Lookups
                .Where(x => x.LookupId == headPlacement.LookupId)
                .Select(x => x.Description)
                .FirstOrDefault() ?? "";

            detail.JobTitle = rawJobTitle;

            var userName = UserHelper.GetUserNameByEmployeeId(headPlacement.EmployeeId);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                detail.Signature = SignatureHelper.GetUserSignature(userName);
            }

            return detail;
        }
        private ClearanceLetterDetailVM CreateApprovalDetail(MainCompanyStructure department, int serialNo, ClearanceRequestApprover requestApprover)
        {

            
            var detail = new ClearanceLetterDetailVM
            {
                SerialNo = serialNo,
                Department = department?.Name ?? "",
                Date = requestApprover?.DepartmentOnlineApprovedTime ?? requestApprover?.OnlineApprovedTime?? requestApprover?.DateCreated,
                Remark = requestApprover?.Remark ?? string.Empty
            };

            if (department == null)
                return detail;

            if (requestApprover == null)
            {
                detail.HeadName = "No employee assigned for this department";
                return detail;
            }

            var headPlacement = db.Placements
                .Where(x =>
                    x.OrgStructureId == department.OrgStructureId &&
                    x.isCurrent &&
                    x.EmployeeId == requestApprover.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .FirstOrDefault();

            if (headPlacement == null)
                headPlacement = db.Placements
                .Where(x =>
                    x.OrgStructureId == department.OrgStructureId &&
                    x.EmployeeId == requestApprover.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .FirstOrDefault();
            if (headPlacement == null)
            {
                detail.HeadName = "No employee assigned for this department";
                return detail;
            }
                

            var headEmployee = db.Person_Employee
                .FirstOrDefault(x => x.EmployeeId == headPlacement.EmployeeId);
            if(headEmployee != null)
                detail.HeadName = headEmployee.FirstNameEnglish + " "+headEmployee.MiddleNameEnglish + " "+headEmployee.LastNameEnglish ;

            var rawJobTitle = db.Lookups
                .Where(x => x.LookupId == headPlacement.LookupId)
                .Select(x => x.Description)
                .FirstOrDefault() ?? "";

            detail.JobTitle = rawJobTitle;

            var userName = UserHelper.GetUserNameByEmployeeId(headPlacement.EmployeeId);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                detail.Signature = SignatureHelper.GetUserSignature(userName);
            }

            return detail;
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}