using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ScalePromotionObjectDataSource :  IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PromotionVM GetEmployeeJob(string empId, object[] letthem, object[] transcript, bool header,string letteringEmployee)
        {
            var employeeInfo = new PromotionVM();
            if (header)
            {
                 GetOrganizationInfo(employeeInfo,header, letteringEmployee);
               // SetOrganizationInfo(employeeInfo);
                employeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }

            if (!int.TryParse(empId, out int employeeId))
                return employeeInfo;

            var employee = db.Person_Employee.FirstOrDefault(e => e.EmployeeId == employeeId && e.IsActive);
            if (employee == null)
                return employeeInfo;
            employeeInfo.Sex = HrHelper.GetGender(employee.Gender);
            employeeInfo.FullName = employee.FullNameAmharic;
            employeeInfo.EmployeeId = employee.EmployeeCode;
            if (!string.IsNullOrWhiteSpace(letteringEmployee) && int.TryParse(letteringEmployee, out int letteringEmployeeId))
            {
                var employeeLettering = db.Person_Employee
                    .FirstOrDefault(e => e.EmployeeId == letteringEmployeeId && e.IsActive);

                if (employeeLettering != null)
                {
                    employeeInfo.LetteringEmployeeName = employeeLettering.FullNameAmharic ?? string.Empty;

                    var letteringPlacement = db.Placements
                        .FirstOrDefault(p => p.isCurrent && p.EmployeeId == letteringEmployeeId);

                    if (letteringPlacement != null)
                    {
                        var lookupDescription = db.Lookups
                            .Where(d => d.LookupId == letteringPlacement.LookupId)
                            .Select(d => d.Description)
                            .FirstOrDefault();

                        employeeInfo.LetteringEmployeeOrgStructure = lookupDescription ?? string.Empty;
                    }
                }
            }


            var currentPlacement = db.Placements
                                            .Where(p =>p.EmployeeId == employeeId 
                                                    && p.AssignmentType == AssignmentType.SCALE_PROMOTION)
                                                    .OrderByDescending(x=>x.PlacementId)
                                                    .FirstOrDefault();
            if (currentPlacement != null)
            {
                FillPlacementInfo(employeeInfo, currentPlacement, isCurrent: true);
                DateTime dateTime = currentPlacement?.SalarChangeDate ?? currentPlacement.AssignmentDate;
                employeeInfo.AssignmentDate = dateTime.ToString("dd/MM/yyyy");
                //employeeInfo.AssignmentDate = dateTime.ToString("MMMM dd, yyyy");

            }

            var previousPlacement = db.Placements
                .Where(p => !p.isCurrent && p.EmployeeId == employeeId)
                .OrderByDescending(p => p.AssignmentDate)
                .FirstOrDefault();

            if (previousPlacement != null)
            {
                FillPlacementInfo(employeeInfo, previousPlacement, isCurrent: false);
            }

            employeeInfo.Diffrence = employeeInfo.CurrentSalary - employeeInfo.PreviousSalary;
            employeeInfo.DiffrenceAmount = XAPI.NumberExtensions.toWords((int)employeeInfo.Diffrence);
            employeeInfo.CurrentAmount = XAPI.NumberExtensions.toWords((int)employeeInfo.CurrentSalary);

            return employeeInfo;
        }

        private void GetOrganizationInfo(PromotionVM orgInfo, bool hasHead, string employeeId)
        {
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            int.TryParse(employeeId, out int empId);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId);
            if (placement != null)
            {
                var personEmployee = db.Person_Employee.Find(placement.EmployeeId);
                if (personEmployee != null)
                {
                    orgInfo.HeadName = personEmployee.FirstName + " " + personEmployee.MiddleName + " " + personEmployee.LastName;
                    var approvedBy =  UserHelper.GetUserNameByEmployeeId(personEmployee.EmployeeId);
                    orgInfo.ApprovedBy = approvedBy;
                    orgInfo.Signature = SignatureHelper.GetUserSignature(approvedBy);
                }

                var lookUp = db.Lookups.Find(placement.LookupId);
                if (lookUp != null)
                    orgInfo.HeadJobTitle = lookUp.Description ?? lookUp.DescriptionEng;
            }

            if (organization != null && hasHead)
            {
                orgInfo.Name = organization.TradeName;
                orgInfo.AmharicName = organization.AmharicName;
                orgInfo.Abbreviation = organization.Abbreviation;

                var addresses = db.Addresses.Where(a =>
                    a.Reference == organization.MainCompanyId.ToString()).ToList();

                foreach (var address in addresses)
                {
                    switch (address.Category)
                    {
                        case AddressType.KIFLEKETEMA:
                            orgInfo.Address = $"Subcity: {address.Value} ";
                            break;
                        case AddressType.KEBELE:
                            orgInfo.Address += $"Kebele: {address.Value} ";
                            break;
                        case AddressType.CITYPROVINCE:
                            orgInfo.Address = orgInfo.Address == null ? $"{address.Value} -Ethiopia" : orgInfo.Address;
                            break;
                        case AddressType.POBOX:
                            orgInfo.Pobox = $"P.O.Box: {address.Value}";
                            break;
                        case AddressType.OFFICE_EMAIL:
                            orgInfo.Email = $"Email: {address.Value}";
                            break;
                        case AddressType.WEBSITE:
                            orgInfo.WebSite = $"Website:{address.Value}";
                            break;
                        case AddressType.FAX:
                            orgInfo.Fax = $"Fax: {address.Value}";
                            break;
                        case AddressType.OFFICE_PHONE:
                            orgInfo.Tel = $"Tel: {address.Value}";
                            break;
                        case AddressType.REGION:
                            orgInfo.Region = $"{address.Value}";
                            break;
                        case AddressType.COUNTRY:
                            orgInfo.Country = $"{address.Value}";
                            break;
                    }
                }

                var logo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.LOGO);

                if (logo != null)
                {
                    orgInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

        }

        private void SetOrganizationInfo(PromotionVM vm)
        {
            var org = db.Companies.FirstOrDefault(c => c.Category == "0");
            if (org == null) return;

            vm.Name = org.TradeName;
            vm.AmharicName = org.AmharicName;
            vm.Abbreviation = org.Abbreviation;

            var addresses = db.Addresses
                .Where(a => a.Reference == org.MainCompanyId.ToString() && a.ReferenceType.ToLower() == "company")
                .ToList();

            foreach (var address in addresses)
            {
                switch (address.Category)
                {
                    case AddressType.KIFLEKETEMA:
                        vm.Address = $"Subcity: {address.Value} ";
                        break;
                    case AddressType.KEBELE:
                        vm.Address += $"Kebele: {address.Value} ";
                        break;
                    case AddressType.CITYPROVINCE:
                        vm.Address = vm.Address == null ? $"{address.Value} -Ethiopia" : vm.Address;
                        break;
                    case AddressType.POBOX:
                        vm.Pobox = $"P.O.Box: {address.Value}";
                        break;
                    case AddressType.OFFICE_EMAIL:
                        vm.Email = $"Email: {address.Value}";
                        break;
                    case AddressType.WEBSITE:
                        vm.WebSite = $"Website:{address.Value}";
                        break;
                    case AddressType.FAX:
                        vm.Fax = $"Fax: {address.Value}";
                        break;
                    case AddressType.OFFICE_PHONE:
                        vm.Tel = $"Tel: {address.Value}";
                        break;
                    case AddressType.REGION:
                        vm.Region = $"{address.Value}";
                        break;
                    case AddressType.COUNTRY:
                        vm.Country = $"{address.Value}";
                        break;
                }
            }

            var logo = db.Attachments.FirstOrDefault(a =>
                a.Reference == org.MainCompanyId.ToString() &&
                a.ReferenceType == AttachmentType.LOGO);

            if (logo != null)
            {
                vm.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
            }
        }
        private void FillPlacementInfo(PromotionVM vm, Placement placement, bool isCurrent)
        {
            var branch = db.Branch.FirstOrDefault(b => b.BranchId == placement.BranchId);
            var subBranch = branch != null && branch.ParentBranchId.HasValue
                ? db.Branch.FirstOrDefault(b => b.BranchId == branch.ParentBranchId.Value)
                : null;
            var step = db.Steps.FirstOrDefault(s => s.StepId == placement.StepId)?.StepName;
            var rank = db.Ranks.FirstOrDefault(r => r.RankId == placement.RankId)?.RankName;
            var jobTitle = db.Lookups.FirstOrDefault(l => l.LookupId == placement.LookupId)?.Description;
            var dept = db.MainCompanyStructures.FirstOrDefault(d => d.OrgStructureId == placement.OrgStructureId)?.Name;

            if (isCurrent)
            {
                vm.CurrentSubBranch = branch?.Name;
                vm.CurrentBranch = subBranch?.Name?? branch?.Name;
                vm.CurrentStep = step;
                vm.CurrentRank = rank;
                vm.CurrentJobTitle = jobTitle;
                vm.CurrentDepartement = dept;
                vm.CurrentSalary = placement.Salary;
            }
            else
            {
                vm.PreviousSubBranch = branch?.Name;
                vm.PreviousBranch = subBranch?.Name ?? branch?.Name;
                vm.PreviousStep = step;
                vm.PreviousRank = rank;
                vm.PreviousJobTitle = jobTitle;
                vm.PreviousDepartement = dept;
                vm.PreviousSalary = placement.Salary;
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
        

    }

}