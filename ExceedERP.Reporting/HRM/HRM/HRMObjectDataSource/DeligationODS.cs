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
    public class DeligationODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DeligationVM> GetDeligation(string id, bool header)
        {
            List<DeligationVM> singleDeligation = new List<DeligationVM>();
            DeligationVM delg = new DeligationVM();
            var persons = db.Person_Employee.ToList();
            var deligation = db.Delegations.FirstOrDefault(x => x.DelegationId.ToString() == id);
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (header)
            {
                if (Organization != null)
                {
                    delg.Name = Organization.TradeName;
                    delg.AmharicName = Organization.AmharicName;
                    delg.Abbreviation = Organization.Abbreviation;
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
                                delg.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                delg.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (delg.Address == null)
                                {
                                    delg.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                delg.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                delg.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                delg.WebSite = "Website:" + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                delg.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                delg.Tel = "Tel:  " + a.Value;

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
                        delg.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
            }

            if (deligation != null)
            {
                var person1 = persons.FirstOrDefault(x => x.EmployeeId == deligation.FromEmployeeId);
                if (person1 != null)
                {
                    delg.FromEmployee = person1.FullNameAmharic;

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == person1.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            delg.BranchFrom = branch.AmharicName;
                        }
                        var dep = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (dep != null)
                        {
                            delg.DepartmentFrom = dep.Name;
                        }
                        var job = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                        if (job != null)
                        {
                            delg.JobTitleFrom = job.Description;
                        }
                    }
                }
                var person2 = persons.FirstOrDefault(x => x.EmployeeId == deligation.EmployeeId);
                if (person2 != null)
                {
                    delg.ToEmployee = person2.FullNameAmharic;

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == person2.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            delg.BranchTo = branch.AmharicName;
                        }
                        var dep = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (dep != null)
                        {
                            delg.DepartmentTo = dep.Name;
                        }
                        var job = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                        if (job != null)
                        {
                            delg.JobTitleTo = job.Description;
                        }
                    }
                }

                delg.DateFrom = deligation.FromDateTimeAm;
                delg.DateTo = deligation.ToDateTimeAm;
                var delegReason = db.DelegationReasons.FirstOrDefault(x => x.DelegationId == deligation.DelegationId);
                if (delegReason != null)
                {
                    delg.Reason = delegReason.Reason;
                }

              
                delg.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
                singleDeligation.Add(delg);
            }

            return singleDeligation;
        }
    }
}
