using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ClearanceLetterObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClearanceLetterData> GetClearanceLetter(string id, object[] transcript, bool header)
        {
            var result = new List<ClearanceLetterData>();

            if (!int.TryParse(id, out int empId))
                return result;

            var employeeInfo = new ClearanceLetterData();

            #region Company Header
            if (header)
            {
                var organization = db.Companies
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Category == "0");

                if (organization != null)
                {
                    employeeInfo.Name = organization.TradeName;
                    employeeInfo.AmharicName = organization.AmharicName;
                    employeeInfo.Abbreviation = organization.Abbreviation;

                    if (organization.MainCompanyId != 0)
                    {
                        string companyId = organization.MainCompanyId.ToString();

                        var addresses = db.Addresses
                            .AsNoTracking()
                            .Where(a => a.Reference == companyId &&
                                        a.ReferenceType != null &&
                                        a.ReferenceType.Equals("Company", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        foreach (var address in addresses)
                        {
                            switch (address.Category)
                            {
                                case AddressType.KIFLEKETEMA:
                                    employeeInfo.Address += $"Subcity: {address.Value} ";
                                    break;
                                case AddressType.KEBELE:
                                    employeeInfo.Address += $"Kebele: {address.Value} ";
                                    break;
                                case AddressType.CITYPROVINCE:
                                    employeeInfo.Address += $"{address.Value}, Ethiopia ";
                                    break;
                                case AddressType.POBOX:
                                    employeeInfo.Pobox = $"P.O.Box: {address.Value}";
                                    break;
                                case AddressType.OFFICE_PHONE:
                                    employeeInfo.Tel = $"Tel: {address.Value}";
                                    break;
                                case AddressType.FAX:
                                    employeeInfo.Fax = $"Fax: {address.Value}";
                                    break;
                                case AddressType.OFFICE_EMAIL:
                                    employeeInfo.Email = $"Email: {address.Value}";
                                    break;
                                case AddressType.WEBSITE:
                                    employeeInfo.WebSite = $"Website: {address.Value}";
                                    break;
                            }
                        }

                        var logo = db.Attachments
                            .AsNoTracking()
                            .FirstOrDefault(a => a.Reference == companyId && a.ReferenceType == AttachmentType.LOGO);

                        if (logo?.BinaryFile != null)
                        {
                            employeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }
                    }
                }
            }
            #endregion

            var employee = db.Person_Employee
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeId == empId);

            if (employee == null)
                return result;

            var termination = db.Terminations
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

            employeeInfo.FullName = employee.FullNameaEnglish ?? $"{employee.FirstNameEnglish} {employee.LastNameEnglish}";

            var placement = db.Placements
                .AsNoTracking()
                .Where(x => x.EmployeeId == employee.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .FirstOrDefault();

            if (termination != null)
            {
                if (termination.TerminationDate.HasValue)
                {
                    var diff = (termination.TerminationDate.Value - termination.ApplicationDate).Days;
                    if (diff < 30)
                    {
                        employeeInfo.Punishment = "This Employee Has a Punishment";
                    }

                    employeeInfo.TerminationDate = HrHelper.GetECDate(termination.TerminationDate.Value);
                }

                if (termination.LookupId > 0)
                {
                    int termLookupId = termination.LookupId;
                    var reason = db.Lookups
                        .AsNoTracking()
                        .FirstOrDefault(x => x.LookupId == termLookupId);

                    employeeInfo.Reason = reason?.Description;
                }
            }

            if (placement != null)
            {
                int orgStructureId = placement.OrgStructureId;
                int lookupId = placement.LookupId;

                employeeInfo.Status = HrHelper.GetEmployeeStatus(placement.EmployementType);
                employeeInfo.Salary = placement.Salary;

                var job = db.Lookups
                    .AsNoTracking()
                    .FirstOrDefault(x => x.LookupId == lookupId);

                if (job != null)
                {
                    employeeInfo.JobTitle = job.Description;
                }

                var department = db.MainCompanyStructures
                    .AsNoTracking()
                    .FirstOrDefault(x => x.OrgStructureId == orgStructureId);

                if (department != null)
                {
                    employeeInfo.Department = department.Name;
                }

                // 2. Resolve Respective Signer / Manager Details
                int headId = placement.HeadId ?? 0;
                if (headId > 0)
                {
                    var head = db.Person_Employee
                        .AsNoTracking()
                        .FirstOrDefault(x => x.EmployeeId == headId);

                    if (head != null)
                    {
                        // Map signers (Assumes your ClearanceLetterData model has properties for Signer Name & Title. 
                        // If your view model uses different property names, adjust them to match ClearanceLetterData).
                        string headName = $"{head.FirstNameEnglish} {head.LastNameEnglish}".Trim();

                        int headLookupId = db.Placements
                            .AsNoTracking()
                            .Where(x => x.EmployeeId == head.EmployeeId && x.isCurrent)
                            .Select(x => x.LookupId)
                            .FirstOrDefault();

                        var headJob = headLookupId > 0
                            ? db.Lookups.AsNoTracking().FirstOrDefault(x => x.LookupId == headLookupId)
                            : null;

                        string headTitle = headJob?.Description ?? "Department Manager";

                        // Attach to your view model properties (ensure properties exist in ClearanceLetterData model)
                        // employeeInfo.SignerName = headName;
                        // employeeInfo.SignerTitle = headTitle;

                        var approverUserName = UserHelper.GetUserNameByEmployeeId(head.EmployeeId);
                        employeeInfo.SignatureImage = UserHelper.GetUserSignature(approverUserName);
                    }
                }
            }

            result.Add(employeeInfo);
            return result;
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}