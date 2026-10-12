using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource
{
    [DataObject]
   public class EmployeeTransferObjectDataSource : IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChangeOfPayrollVM> GetEmployeeTranfer(string id)
        {
            if (!int.TryParse(id, out int transferId))
                return new List<ChangeOfPayrollVM>();

            var employeeTransfer = db.EmployeeTransferRequests.Find(transferId);

            if (employeeTransfer == null)
                return new List<ChangeOfPayrollVM>();

            var vm = new ChangeOfPayrollVM();

            vm.FullName = db.Person_Employee
                .Where(x => x.EmployeeId == employeeTransfer.EmployeeId)
                .Select(x => x.EmployeeCode+"-"+x.FirstNameEnglish
                            +" "+x.MiddleNameEnglish
                            +" "+x.LastNameEnglish)

                .FirstOrDefault();

            vm.FromBranch = db.Branch
                .Where(x => x.BranchId == employeeTransfer.FromBranchId)
                .Select(x => x.Name)
                .FirstOrDefault();

            vm.Branch = db.Branch
                .Where(x => x.BranchId == employeeTransfer.BranchId)
                .Select(x => x.Name)
                .FirstOrDefault();

            vm.FromBussnessUnit = db.BusinessUnits
                .Where(x => x.BusinessUnitId == employeeTransfer.FromBusinessUnitId)
                .Select(x => x.Name)
                .FirstOrDefault();

            vm.BussnessUnit = db.BusinessUnits
                .Where(x => x.BusinessUnitId == employeeTransfer.BusinessUnitId)
                .Select(x => x.Name)
                .FirstOrDefault();

            vm.Department = db.MainCompanyStructures
                .Where(x => x.OrgStructureId == employeeTransfer.OrgStructureId)
                .Select(x => x.Name)
                .FirstOrDefault();

            var placement = db.Placements
                .FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeTransfer.EmployeeId);

            if (placement != null)
            {
                vm.JobTitle = db.Lookups
                    .Where(x => x.LookupId == placement.LookupId)
                    .Select(x => x.Description)
                    .FirstOrDefault();

                vm.Step = db.Steps
                    .Where(x => x.StepId == placement.StepId)
                    .Select(x => x.StepName)
                    .FirstOrDefault();

                vm.Rank = db.Ranks
                    .Where(x => x.RankId == placement.RankId)
                    .Select(x => x.RankName)
                    .FirstOrDefault();

                vm.Salary = placement.Salary.ToString();
                vm.AssignmentDate = placement.AssignmentDateAmharic;
                vm.AssignmentType = placement.AssignmentType.ToString();
            }
            else
            {
                vm.JobTitle = db.Lookups
                    .Where(x => x.LookupId == employeeTransfer.LookupId)
                    .Select(x => x.Description)
                    .FirstOrDefault();
            }

            // Approved By
            var approverUserId =
                employeeTransfer.GMApprovedBy ??
                employeeTransfer.DGMApprovedBy ??
                employeeTransfer.OnlineApprovedBy ??
                employeeTransfer.DepartmentOnlineApprovedBy;

            if (!string.IsNullOrWhiteSpace(approverUserId))
            {
                int approverEmployeeId = UserHelper.GetUserEmployeeId(approverUserId);

                var approver = db.Person_Employee
                    .FirstOrDefault(x => x.EmployeeId == approverEmployeeId);

                if (approver != null)
                {
                    vm.ApprovedBy = string.Join(" ",
                        new[]
                        {
                    approver.FirstNameEnglish,
                    approver.MiddleNameEnglish,
                    approver.LastNameEnglish
                        }.Where(x => !string.IsNullOrWhiteSpace(x)));
                }

                vm.Signature = SignatureHelper.GetUserSignature(approverUserId);
            }
            // 8.General Manager Approval Details
            // --- DGM Approval Details ---
            if (!string.IsNullOrWhiteSpace(employeeTransfer.DGMApprovedBy))
            {
                int dGMEmployeeId = UserHelper.GetUserEmployeeId(employeeTransfer.DGMApprovedBy);
                var dGMapprover = db.Person_Employee
                  .FirstOrDefault(x => x.EmployeeId == dGMEmployeeId);
                if (dGMapprover != null)
                {
                    vm.DGMApprovedBy = string.Join(" ",
                        new[]
                        {
                    dGMapprover.FirstNameEnglish,
                    dGMapprover.MiddleNameEnglish,
                    dGMapprover.LastNameEnglish

                        }.Where(x => !string.IsNullOrWhiteSpace(x)));

                }
                vm.DGMApprovedDate = employeeTransfer.DGMApprovedTime?.ToString("dd/MM/yyyy");

                // If your SignatureHelper returns byte[] / Image / string path:
                vm.DGMSignature = SignatureHelper.GetUserSignature(employeeTransfer.DGMApprovedBy);
            }

            // --- GM Approval Details ---
            int GMEmployeeId = UserHelper.GetUserEmployeeId(employeeTransfer.DGMApprovedBy);
            var GMapprover = db.Person_Employee
              .FirstOrDefault(x => x.EmployeeId == GMEmployeeId);
            if (GMapprover != null)
            {
                vm.GMApprovedBy = string.Join(" ",
                    new[]
                    {
                    GMapprover.FirstNameEnglish,
                    GMapprover.MiddleNameEnglish,
                    GMapprover.LastNameEnglish

                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                // Prepared By
            }

            if (!string.IsNullOrWhiteSpace(employeeTransfer.CreatedBy))
            {
                int creatorEmployeeId = UserHelper.GetUserEmployeeId(employeeTransfer.CreatedBy);

                var creator = db.Person_Employee
                    .FirstOrDefault(x => x.EmployeeId == creatorEmployeeId);

                if (creator != null)
                {
                    vm.PreparedBy = string.Join(" ",
                        new[]
                        {
                    creator.FirstNameEnglish,
                    creator.MiddleNameEnglish,
                    creator.LastNameEnglish
                        }.Where(x => !string.IsNullOrWhiteSpace(x)));
                }
            }

            return new List<ChangeOfPayrollVM> { vm };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTransferLetterVM> GetEmployeeTransferRequest(string id)
        {
            if (!int.TryParse(id, out int transferId))
                return new List<EmployeeTransferLetterVM>();

            var transfer = db.EmployeeTransferRequests
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeTransferRequestId == transferId);

            if (transfer == null)
                return new List<EmployeeTransferLetterVM>();

            var model = new EmployeeTransferLetterVM();

            // 1. Dynamic Company Header
            var company = db.Companies.AsNoTracking().FirstOrDefault();
            var docFormat = db.IsoDocuments.AsNoTracking()
                .FirstOrDefault(d => d.Module == Modules.HRM && d.DocumentType == "EmployeeTransfer")
                ?? db.IsoDocuments.AsNoTracking().FirstOrDefault(d => d.Module == Modules.HRM);

            var companyId = company != null ? company.MainCompanyId.ToString() : null;
            var logo = companyId != null
                ? db.Attachments.AsNoTracking().FirstOrDefault(a => a.Reference == companyId && a.ReferenceType == AttachmentType.LOGO)
                : null;

            model.CompanyHeader = new CompanyHeaderVM
            {
                CompanyNameEn = !string.IsNullOrEmpty(company?.TradeName) ? company.TradeName : (!string.IsNullOrEmpty(company?.BrandName) ? company.BrandName : company?.AmharicName),
                CompanyNameAm = company?.AmharicName,
                LogoFile = logo?.BinaryFile,
                DocumentNumber = docFormat != null && !string.IsNullOrEmpty(docFormat.FormNumber) ? docFormat.FormNumber : "_____",
                IssueNumber = docFormat != null && !string.IsNullOrEmpty(docFormat.FormTitle) ? docFormat.FormTitle : "1",
                EffectiveDate =  DateTime.Now.ToString("dd/MM/yyyy")
            };

            // 2. Transferred Employee Profile
            var employee = db.Person_Employee
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeId == transfer.EmployeeId);

            model.EmployeeDetails = new TransferEmployeeDetailsVM
            {
                EmployeeId = transfer.EmployeeId,
                EmployeeCode = employee?.EmployeeCode,
                FullName = employee != null
                    ? $"{employee.EmployeeCode}-{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}".Trim()
                    : string.Empty,
                FromBranch = transfer.FromBranchId !=0
                    ? db.Branch.AsNoTracking().Where(x => x.BranchId == transfer.FromBranchId).Select(x => x.Name).FirstOrDefault()
                    : string.Empty,
                FromBusinessUnit = transfer.FromBusinessUnitId.HasValue
                    ? db.BusinessUnits.AsNoTracking().Where(x => x.BusinessUnitId == transfer.FromBusinessUnitId.Value).Select(x => x.Name).FirstOrDefault()
                    : string.Empty
            };

            // 3. Current Placement & Destination Details
            var currentPlacement = db.Placements
                .AsNoTracking()
                .Where(x => x.isCurrent && x.EmployeeId == transfer.EmployeeId)
                .OrderByDescending(x => x.PlacementId)
                .FirstOrDefault();

            model.EmployeeDetails.CurrentJobTitle = currentPlacement != null
                ? db.Lookups.AsNoTracking().Where(x => x.LookupId == currentPlacement.LookupId).Select(x => x.Description).FirstOrDefault()
                : string.Empty;
            model.EmployeeDetails.CurrentSalary = currentPlacement != null ? currentPlacement.Salary.ToString("N2") : "0.00";

            int? targetJobLookupId = transfer.LookupId ?? currentPlacement?.LookupId;

            model.TransferPlacement = new TransferPlacementDetailsVM
            {
                TargetBranch = transfer.BranchId !=0
                    ? db.Branch.AsNoTracking().Where(x => x.BranchId == transfer.BranchId).Select(x => x.Name).FirstOrDefault()
                    : string.Empty,
                TargetBusinessUnit = transfer.BusinessUnitId.HasValue
                    ? db.BusinessUnits.AsNoTracking().Where(x => x.BusinessUnitId == transfer.BusinessUnitId.Value).Select(x => x.Name).FirstOrDefault()
                    : string.Empty,
                TargetDepartment = transfer.OrgStructureId !=0
                    ? db.MainCompanyStructures.AsNoTracking().Where(x => x.OrgStructureId == transfer.OrgStructureId).Select(x => x.Name).FirstOrDefault()
                    : string.Empty,
                NewJobTitle = targetJobLookupId.HasValue
                    ? db.Lookups.AsNoTracking().Where(x => x.LookupId == targetJobLookupId.Value).Select(x => x.Description).FirstOrDefault()
                    : string.Empty,
                IsCurrentPosition = transfer.LookupId == null || (currentPlacement != null && transfer.LookupId == currentPlacement.LookupId),
                IsNewPosition = transfer.LookupId != null && (currentPlacement == null || transfer.LookupId != currentPlacement.LookupId),
                BasicSalary = currentPlacement != null ? currentPlacement.Salary.ToString("N2") : "0.00",
                Step = currentPlacement != null
                    ? db.Steps.AsNoTracking().Where(x => x.StepId == currentPlacement.StepId).Select(x => x.StepName).FirstOrDefault()
                    : string.Empty,
                Rank = currentPlacement != null
                    ? db.Ranks.AsNoTracking().Where(x => x.RankId == currentPlacement.RankId).Select(x => x.RankName).FirstOrDefault()
                    : string.Empty,
                AssignmentDate = currentPlacement?.AssignmentDateAmharic,
                AssignmentType = currentPlacement != null ? currentPlacement.AssignmentType.ToString() : string.Empty
            };

            // 4. Employee Allowances & Benefits
            var allowances = (from empAll in db.AllowancePerEmployees.AsNoTracking()
                              join def in db.AllowanceDefinations.AsNoTracking() on empAll.AllowanceDefinationId equals def.AllowanceDefinationId
                              where empAll.EmployeeId == transfer.EmployeeId
                              select new TransferBenefitVM
                              {
                                  BenefitType = def.AllowanceType.ToString(),
                                  Name = def.AllowanceName,
                                  MeasureType = def.AmtPercent.ToString(),
                                  Amount = def.Amount
                              }).ToList();

            model.Benefits = allowances;
            var perDiem = allowances.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) &&
                (x.Name.ToLower().Contains("perdiem") || x.Name.ToLower().Contains("per diem")));

            model.TransferPlacement.PerdiemAllowance = perDiem != null ? perDiem.Amount.ToString("N2") : "0.00";
            model.TransferPlacement.OtherBenefitsSummary = string.Join(", ", allowances.Where(x => x != perDiem).Select(x => $"{x.Name}: {x.Amount:N2}"));

            // 5. Letter Metadata (Safe Date Formatting - No .Value without check)
            model.ReferenceNumber = $"MCGHR /TR/ {transfer.EmployeeTransferRequestId:D4} / {DateTime.Now.Year}";
            model.FormDate = transfer.DateCreated.HasValue
                ? transfer.DateCreated.Value.ToString("dd/MM/yyyy")
                :  transfer.RequestDate.ToString("dd/MM/yyyy");
            model.ReasonForTransfer = transfer.Reason;
            model.Remark = transfer.Remark;

            // 6. Multi-Level Approvals & Signatures
            model.Approvals = new TransferApprovalMetadataVM();

            // Division Head / Creator
            if (!string.IsNullOrWhiteSpace(transfer.CreatedBy))
            {
                int creatorEmpId = UserHelper.GetUserEmployeeId(transfer.CreatedBy);
                var creator = db.Person_Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == creatorEmpId);
                model.Approvals.PreparedBy = creator != null
                    ? $"{creator.FirstNameEnglish} {creator.MiddleNameEnglish} {creator.LastNameEnglish}".Trim()
                    : transfer.CreatedBy;
                model.Approvals.PreparedBySignature = UserHelper.GetUserSignature(transfer.CreatedBy);
                model.Approvals.PreparedDate = transfer.DateCreated.HasValue ? transfer.DateCreated.Value.ToString("dd/MM/yyyy") : null;
            }

            // Department Manager
            if (!string.IsNullOrWhiteSpace(transfer.DepartmentOnlineApprovedBy))
            {
                int deptEmpId = UserHelper.GetUserEmployeeId(transfer.DepartmentOnlineApprovedBy);
                var deptMgr = db.Person_Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == deptEmpId);
                model.Approvals.DepartmentManager = deptMgr != null
                    ? $"{deptMgr.FirstNameEnglish} {deptMgr.MiddleNameEnglish} {deptMgr.LastNameEnglish}".Trim()
                    : transfer.DepartmentOnlineApprovedBy;
                model.Approvals.DepartmentManagerSignature = UserHelper.GetUserSignature(transfer.DepartmentOnlineApprovedBy);
                model.Approvals.DepartmentManagerDate = transfer.DepartmentOnlineApprovedTime.HasValue
                    ? transfer.DepartmentOnlineApprovedTime.Value.ToString("dd/MM/yyyy")
                    : null;
            }

            // Deputy General Manager (DGM)
            if (!string.IsNullOrWhiteSpace(transfer.DGMApprovedBy))
            {
                int dgmEmpId = UserHelper.GetUserEmployeeId(transfer.DGMApprovedBy);
                var dgm = db.Person_Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == dgmEmpId);
                model.Approvals.DGMApprovedBy = dgm != null
                    ? $"{dgm.FirstNameEnglish} {dgm.MiddleNameEnglish} {dgm.LastNameEnglish}".Trim()
                    : transfer.DGMApprovedBy;
                model.Approvals.DGMSignature = UserHelper.GetUserSignature(transfer.DGMApprovedBy);
                model.Approvals.DGMApprovedDate = transfer.DGMApprovedTime.HasValue
                    ? transfer.DGMApprovedTime.Value.ToString("dd/MM/yyyy")
                    : null;
            }

            // General Manager (GM)
            if (!string.IsNullOrWhiteSpace(transfer.GMApprovedBy))
            {
                int gmEmpId = UserHelper.GetUserEmployeeId(transfer.GMApprovedBy);
                var gm = db.Person_Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == gmEmpId);
                model.Approvals.GMApprovedBy = gm != null
                    ? $"{gm.FirstNameEnglish} {gm.MiddleNameEnglish} {gm.LastNameEnglish}".Trim()
                    : transfer.GMApprovedBy;
                model.Approvals.GMSignature = UserHelper.GetUserSignature(transfer.GMApprovedBy);
                model.Approvals.GMApprovedDate = transfer.GMApprovedTime.HasValue
                    ? transfer.GMApprovedTime.Value.ToString("dd/MM/yyyy")
                    : null;
            }

            return new List<EmployeeTransferLetterVM> { model };
        }
        //public List<ChangeOfPayrollVM> GetEmployeeTranfer(string id)
        //{
        //    List<ChangeOfPayrollVM> changeOfPayrolls = new List<ChangeOfPayrollVM>();
        //    ChangeOfPayrollVM changeOfPayroll = new ChangeOfPayrollVM();
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int did = int.Parse(id);
        //        var employeeTransfer = db.EmployeeTransferRequests.Find(did);
        //        if (employeeTransfer != null)
        //        {
        //            employeeTransfer.Reason = employeeTransfer.Reason;
        //            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeTransfer.EmployeeId);
        //            if (person != null)
        //            {
        //                changeOfPayroll.FullName = person.FullNameAmharic;
        //            }
        //            var fromBranch = db.Branch.FirstOrDefault(x => x.BranchId == employeeTransfer.FromBranchId);
        //            if (fromBranch != null)
        //            {
        //                changeOfPayroll.FromBranch = fromBranch.Name;
        //            }
        //            var branch = db.Branch.FirstOrDefault(x => x.BranchId == employeeTransfer.BranchId);
        //            if (branch != null)
        //            {
        //                changeOfPayroll.Branch = branch.Name;
        //            }
        //            var fromBusinessUnit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == employeeTransfer.FromBusinessUnitId);
        //            if (fromBusinessUnit != null)
        //            {
        //                changeOfPayroll.FromBussnessUnit = fromBusinessUnit.Name;
        //            }
        //            var businessUnit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == employeeTransfer.BusinessUnitId);
        //            if (businessUnit != null)
        //            {
        //                changeOfPayroll.BussnessUnit = businessUnit.Name;
        //            }
        //            var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == employeeTransfer.OrgStructureId);
        //            if (department != null)
        //            {
        //                changeOfPayroll.Department = department.Name;
        //            }
        //            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeTransfer.EmployeeId);
        //            if(placement != null)
        //            {
        //                var look = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
        //                if (look != null)
        //                {
        //                    changeOfPayroll.JobTitle = look.Description;
        //                }
        //                var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
        //                if (step != null)
        //                {
        //                    changeOfPayroll.Step = step.StepName;
        //                }
        //                var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
        //                if (rank != null)
        //                {
        //                    changeOfPayroll.Rank = rank.RankName;
        //                }
        //                changeOfPayroll.Salary = placement.Salary.ToString();
        //                changeOfPayroll.AssignmentDate = placement.AssignmentDateAmharic;
        //                changeOfPayroll.AssignmentType = placement.AssignmentType.ToString();

        //                var approverUserId = employeeTransfer.GMApprovedBy ??
        //                                                           employeeTransfer.DGMApprovedBy ??
        //                                                           employeeTransfer.OnlineApprovedBy ??
        //                                                           employeeTransfer.DepartmentOnlineApprovedBy;

        //                if (!string.IsNullOrWhiteSpace(approverUserId))
        //                {
        //                    int approverId = UserHelper.GetUserEmployeeId(approverUserId);

        //                    var approver = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == approverId);

        //                    changeOfPayroll.ApprovedBy =
        //                                $"{approver?.FirstNameEnglish} {approver?.MiddleNameEnglish} {approver?.LastNameEnglish}";
        //                    changeOfPayroll.Signature = SignatureHelper.GetUserSignature(approverUserId);

        //                }

        //                var creatorEmpId = UserHelper.GetUserEmployeeId(employeeTransfer.CreatedBy);
        //                var prepared = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == creatorEmpId);

        //                changeOfPayroll.PreparedBy = $"{prepared?.FirstNameEnglish} {prepared?.MiddleNameEnglish} {prepared?.LastNameEnglish}";


        //            }
        //            if (placement == null)
        //            {
        //                var look = db.Lookups.FirstOrDefault(x => x.LookupId == employeeTransfer.LookupId);
        //                if (look != null)
        //                {
        //                    changeOfPayroll.JobTitle = look.Description;
        //                }


        //                var approverUserId = employeeTransfer.GMApprovedBy ??
        //                                     employeeTransfer.DGMApprovedBy ??
        //                                     employeeTransfer.OnlineApprovedBy ??
        //                                     employeeTransfer.DepartmentOnlineApprovedBy;

        //                if (!string.IsNullOrWhiteSpace(approverUserId))
        //                {
        //                    int approverId = UserHelper.GetUserEmployeeId(approverUserId);

        //                    var approver = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == approverId);

        //                    changeOfPayroll.ApprovedBy = $"{approver?.FirstNameEnglish} {approver?.MiddleNameEnglish} {approver?.LastNameEnglish}";
        //                    changeOfPayroll.Signature = SignatureHelper.GetUserSignature(approverUserId);
        //                }

        //                var creatorEmpId = UserHelper.GetUserEmployeeId(employeeTransfer.CreatedBy);
        //                var prepared = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == creatorEmpId);

        //                changeOfPayroll.PreparedBy = $"{prepared?.FirstNameEnglish} {prepared?.MiddleNameEnglish} {prepared?.LastNameEnglish}";

        //            }
        //            changeOfPayrolls.Add(changeOfPayroll);
        //        }
        //    }
        //    return changeOfPayrolls;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
    [DataObject]
   public class EmployeeTransferBeefitObjectDataSource : IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChangeOfPayrollBenefitVM> GetBenefits(string id)
        {
            List<ChangeOfPayrollBenefitVM> changeOfPayrollBenefits = new List<ChangeOfPayrollBenefitVM>();
            if (!string.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var employeeTransfer = db.EmployeeTransferRequests.Find(did);
                if (employeeTransfer != null)
                {

                    var allowanceperEmployees = db.AllowancePerEmployees.Where(x => x.EmployeeId == employeeTransfer.EmployeeId);
                    foreach (var emp in allowanceperEmployees)
                    {
                        var allowance = new ChangeOfPayrollBenefitVM();
                        var allowanceDefinition = db.AllowanceDefinations.Find(emp.AllowanceDefinationId);
                        if (allowanceDefinition != null)
                        {
                            allowance.BenfitType = allowanceDefinition.AllowanceType.ToString();
                            allowance.Name = allowanceDefinition.AllowanceName;
                            allowance.MeasureType = allowanceDefinition.AmtPercent.ToString();
                            allowance.Amount = allowanceDefinition.Amount;

                            changeOfPayrollBenefits.Add(allowance);
                        }
                    }
                }
            }
            return changeOfPayrollBenefits;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
    // Master Container ViewModel
    public class EmployeeTransferLetterVM
    {
        public CompanyHeaderVM CompanyHeader { get; set; } = new CompanyHeaderVM();
        public TransferEmployeeDetailsVM EmployeeDetails { get; set; } = new TransferEmployeeDetailsVM();
        public TransferPlacementDetailsVM TransferPlacement { get; set; } = new TransferPlacementDetailsVM();
        public TransferApprovalMetadataVM Approvals { get; set; } = new TransferApprovalMetadataVM();
        public List<TransferBenefitVM> Benefits { get; set; } = new List<TransferBenefitVM>();

        public string ReferenceNumber { get; set; }
        public string FormDate { get; set; }
        public string ReasonForTransfer { get; set; }
        public string LengthOfStay { get; set; }
        public string Remark { get; set; }
        public string HRDepartmentComments { get; set; }
    }

    // 1. Dynamic Company Header Information
    public class CompanyHeaderVM
    {
        public string CompanyNameEn { get; set; }
        public string CompanyNameAm { get; set; }
        public byte[] LogoFile { get; set; }
        public string DocumentNumber { get; set; }
        public string IssueNumber { get; set; }
        public string EffectiveDate { get; set; }
    }

    // 2. Transferred Employee Profile
    public class TransferEmployeeDetailsVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string CurrentJobTitle { get; set; }
        public string CurrentSalary { get; set; }
        public string FromBranch { get; set; }
        public string FromBusinessUnit { get; set; }
    }

    // 3. Destination & New Placement Details
    public class TransferPlacementDetailsVM
    {
        public string TargetBranch { get; set; }
        public string TargetBusinessUnit { get; set; }
        public string TargetDepartment { get; set; }
        public string NewJobTitle { get; set; }
        public bool IsCurrentPosition { get; set; }
        public bool IsNewPosition { get; set; }
        public string BasicSalary { get; set; }
        public string PerdiemAllowance { get; set; }
        public string OtherBenefitsSummary { get; set; }
        public string Step { get; set; }
        public string Rank { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentType { get; set; }
    }

    // 4. Approval Signatures & Timestamps
    public class TransferApprovalMetadataVM
    {
        public string PreparedBy { get; set; }
        public byte[] PreparedBySignature { get; set; }
        public string PreparedDate { get; set; }

        public string DepartmentManager { get; set; }
        public byte[] DepartmentManagerSignature { get; set; }
        public string DepartmentManagerDate { get; set; }

        public string DGMApprovedBy { get; set; }
        public byte[] DGMSignature { get; set; }
        public string DGMApprovedDate { get; set; }

        public string GMApprovedBy { get; set; }
        public byte[] GMSignature { get; set; }
        public string GMApprovedDate { get; set; }
    }

    // 5. Individual Allowances/Benefits
    public class TransferBenefitVM
    {
        public string BenefitType { get; set; }
        public string Name { get; set; }
        public string MeasureType { get; set; }
        public decimal Amount { get; set; }
    }
}
