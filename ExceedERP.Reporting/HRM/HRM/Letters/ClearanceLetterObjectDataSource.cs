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

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ClearanceLetterObjectDataSource : List<ClearanceLetterData>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClearanceLetterData> GetClearanceLetter(string id, object[] transcript, bool header)
        {
            List<ClearanceLetterData> EmployeeInfoList = new List<ClearanceLetterData>();
            ClearanceLetterData EmployeeInfo = new ClearanceLetterData();
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (header)
            {
                if (Organization != null)
                {
                    EmployeeInfo.Name = Organization.TradeName;
                    EmployeeInfo.AmharicName = Organization.AmharicName;
                    EmployeeInfo.Abbreviation = Organization.Abbreviation;
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
                                EmployeeInfo.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                EmployeeInfo.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (EmployeeInfo.Address == null)
                                {
                                    EmployeeInfo.Address += a.Value + ", Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                EmployeeInfo.Email = "Email:  " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                EmployeeInfo.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                EmployeeInfo.Fax = "Fax:   " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                EmployeeInfo.Tel = "Tel:   " + a.Value;

                            }


                        }

                    }
                    var logo =
                        db.Attachments.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                    if (logo != null)
                    {
                        EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            int.TryParse(id, out int empId);
            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (personEmployee != null)
            {
                var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId);
                if (termination != null)
                {
                    var jobs = db.Lookups.ToList();

                    var departments = db.MainCompanyStructures.ToList();

                    EmployeeInfo.FullName = personEmployee.FullNameaEnglish;

                    var diff = termination.TerminationDate.Subtract(termination.ApplicationDate).Days;
                    if (diff < 30)
                    {
                        EmployeeInfo.Punishment = "This Employee Has a Punishment ";
                    }

                    var placement = db.Placements.Where(y => y.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();

                    if (placement != null)
                    {
                        EmployeeInfo.Status = Helpers.HR.HrHelper.GetEmployeeStatus(placement.EmployementType);
                        EmployeeInfo.Salary = placement.Salary;
                        var look = jobs.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (look != null)
                        {
                            EmployeeInfo.JobTitle = look.Description;
                        }
                        var depart = departments.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (depart != null)
                        {
                            EmployeeInfo.Department = depart.Name;
                        }

                    }

                    if (termination != null)
                    {

                        EmployeeInfo.TerminationDate = XAPI.EthiopicDateTime.GetEthiopicDate(termination.TerminationDate.Day, termination.TerminationDate.Month, termination.TerminationDate.Year);
                        EmployeeInfo.Reason = jobs.FirstOrDefault(x => x.LookupId == termination.LookupId).Description;
                    }
                    EmployeeInfoList.Add(EmployeeInfo);

                }
            }

            return EmployeeInfoList;
        }


    }
}
