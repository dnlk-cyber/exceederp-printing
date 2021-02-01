using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class RecruitmentObjectDataSource : PromotionVM
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PromotionVM GetEmployeeJob(string Id, object[] letthem, object[] transcript, bool header)
        {

            PromotionVM EmployeeInfo = new PromotionVM();
            db = new ExceedDbContext();

            var ranks = db.Ranks.ToList();
            db = new ExceedDbContext();
            var lookups = db.Lookups.ToList();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");// CompanyCategory.MAIN_COMPANY);
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == Id && x.IsActive);

            if (employee != null)
            {
                EmployeeInfo = new PromotionVM();
                if (header)
                {
                    if (organization != null)
                    {
                        EmployeeInfo.Name = organization.TradeName;
                        EmployeeInfo.AmharicName = organization.AmharicName;
                        EmployeeInfo.Abbreviation = organization.Abbreviation;


                        var address =
                 db.Addresses.Where(
                     a =>
                         a.Reference == organization.MainCompanyId.ToString() &&
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
                                        EmployeeInfo.Address += a.Value + " -Ethiopia";
                                    }
                                }
                                else if (a.Category == AddressType.POBOX)
                                {

                                    EmployeeInfo.POBox = "P.O.Box: " + a.Value;

                                }
                                else if (a.Category == AddressType.OFFICE_EMAIL)
                                {

                                    EmployeeInfo.Email = "Email: " + a.Value;

                                }
                                else if (a.Category == AddressType.WEBSITE)
                                {

                                    EmployeeInfo.WebSite = "Website: " + a.Value;

                                }
                                else if (a.Category == AddressType.FAX)
                                {

                                    EmployeeInfo.Fax = "Fax:  " + a.Value;

                                }
                                else if (a.Category == AddressType.OFFICE_PHONE)
                                {

                                    EmployeeInfo.Tel = "Tel:  " + a.Value;

                                }
                            }

                        }


                        var logo =
                            db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                    a.ReferenceType == AttachmentType.LOGO);
                        if (logo != null)
                        {
                            EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }

                        EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    }
                }

                EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(employee.EmployeeId); ;
                EmployeeInfo.EmployeeId = employee.EmployeeCode;
                var placements = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                if (placements != null)
                {


                    var firstOrDefault = ranks.FirstOrDefault(x => x.RankId == placements.RankId);
                    if (firstOrDefault != null)
                        EmployeeInfo.CurrentRank = firstOrDefault.RankName;
                    var orDefault = lookups.FirstOrDefault(x => x.LookupId == placements.LookupId);
                    if (orDefault != null)
                        EmployeeInfo.CurrentJobTitle = orDefault.Description;

                    EmployeeInfo.CurrentSalary = placements.Salary;
                    var deptt = db.MainCompanyStructures
                        .FirstOrDefault(x => x.OrgStructureId == placements.OrgStructureId);
                    if (deptt != null) EmployeeInfo.CurrentDepartement = deptt.Name;

                    EmployeeInfo.AssignmentDate = placements.AssignmentDateAmharic;


                }

                EmployeeInfo.CurrentAmount = XAPI.NumberExtensions.toWords((int)EmployeeInfo.CurrentSalary);

            }



            return EmployeeInfo;
        }
    }
}