using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
   public class ChangeOfPayrollObjectDataSource:IDisposable
   {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChangeOfPayrollVM> GetChangeOfPayroll(string id, bool header)
        {
            List<ChangeOfPayrollVM> changeOfPayrolls = new List<ChangeOfPayrollVM>();
            ChangeOfPayrollVM changeOfPayroll = new ChangeOfPayrollVM();
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();

            if (header)
            {
                if (Organization != null)
                {
                    changeOfPayroll.Name = Organization.TradeName;
                    changeOfPayroll.AmharicName = Organization.AmharicName;
                    changeOfPayroll.Abbreviation = Organization.Abbreviation;
                    var address =
                        db.Addresses.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Any())
                    {
                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                changeOfPayroll.Address = "Subcity: " + a.Value + " ";
                            }

                            if (a.Category == AddressType.KEBELE)
                            {
                                changeOfPayroll.Address += "Kebele: " + a.Value + " ";
                            }

                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (changeOfPayroll.Address == null)
                                {
                                    changeOfPayroll.Address += a.Value + ", Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                changeOfPayroll.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                changeOfPayroll.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                changeOfPayroll.WebSite = "Website:" + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                changeOfPayroll.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                changeOfPayroll.Tel = "Tel:  " + a.Value;

                            }

                        }

                    }

                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        changeOfPayroll.LogoFileName =
                            OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }

            }

            if (!string.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var placement = db.Placements.Find(did);
                if (placement != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
                    if (person != null)
                    {
                        changeOfPayroll.FullName = $"{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}";
                        changeOfPayroll.FullNameAmharic = $"{person.FirstName} {person.MiddleName} {person.LastName}";
                        changeOfPayroll.EmpCode = person.EmployeeCode;
                        changeOfPayroll.HiredDate = person.HiredDateAmharic;
                    }
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                    if (branch != null)
                    {
                        changeOfPayroll.Branch = branch.Name;
                    }
                    var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                    if (department != null)
                    {
                        changeOfPayroll.Department = department.Name;
                    }
                    var look = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (look != null)
                    {
                        changeOfPayroll.JobTitle = look.Description;
                    }
                    var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
                    if (step != null)
                    {
                        changeOfPayroll.Step = step.StepName;
                    }
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                    if (rank != null)
                    {
                        changeOfPayroll.Rank = rank.RankName;
                    }
                    changeOfPayroll.Salary = placement.Salary.ToString();
                    changeOfPayroll.AssignmentDate = placement.AssignmentDateAmharic;
                    changeOfPayroll.AssignmentType = placement.AssignmentType.ToString();

                    var approver = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == placement.ApprovedByEmpId);
                    if(approver != null)
                    {
                        changeOfPayroll.ApprovedBy = approver.FullNameaEnglish;
                    }

                    var prepared = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == placement.PreparedByEmpId);
                    if(prepared != null)
                    {
                        changeOfPayroll.PreparedBy = prepared.FullNameaEnglish;
                    }

                    changeOfPayrolls.Add(changeOfPayroll);
                }
            }
            return changeOfPayrolls;
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }
}
