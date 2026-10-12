using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class PromotionLetterTwoObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PromotionLetterTwoVM> GetEmployeeJob(string empId, object[] letthem, object[] transcript, bool header)
        {
            List<PromotionLetterTwoVM> promotionList = new List<PromotionLetterTwoVM>();
            PromotionLetterTwoVM EmployeeInfo = new PromotionLetterTwoVM();
            
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");


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

                        var logo =
                            db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() && a.ReferenceType == AttachmentType.LOGO);
                        if (logo != null)
                        {
                            EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }
                    }
                }
                EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }

            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);

                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);

                if (employee != null)
                {
                    EmployeeInfo = new PromotionLetterTwoVM();
                    var EmployeeDetail1 = new PromotionLetterTwoDetail();

                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                    EmployeeInfo.FullName = employee.FullNameAmharic;
                    EmployeeInfo.EmployeeId = employee.EmployeeCode;

                    var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId && x.AssignmentType == AssignmentType.PROMOTION);
                    if (placement != null)
                    {
                        var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.CurrentRank = rank.RankName;
                        }
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.CurrentJobTitle = lookup.Description;
                        }

                        EmployeeInfo.CurrentSalary = placement.Salary;

                        var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.CurrentDepartement = department.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.CurrentBranch = branch.Name;
                        }
                        EmployeeInfo.AssignmentDate = placement.AssignmentDateAmharic;
                    }
                    var previousPlacement = db.Placements.Where(x => x.isCurrent == false && x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                    if (previousPlacement != null)
                    {
                        var rank = db.Ranks.FirstOrDefault(x => x.RankId == previousPlacement.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.PreviousRank = rank.RankName;
                        }
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == previousPlacement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.PreviousJobTitle = lookup.Description;
                        }

                        EmployeeInfo.PreviousSalary = previousPlacement.Salary;

                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.PreviousDepartement = deptt.Name;
                        }
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.PreviousBranch = branch.Name;
                        }
                    }

                    EmployeeInfo.Diffrence = EmployeeInfo.CurrentSalary - EmployeeInfo.PreviousSalary;
                    EmployeeInfo.DiffrenceAmount = XAPI.NumberExtensions.toWords((int)EmployeeInfo.Diffrence);
                    EmployeeInfo.CurrentAmount = XAPI.NumberExtensions.toWords((int)EmployeeInfo.CurrentSalary);
                    EmployeeInfo.PromotionLetters = new List<PromotionLetterTwoDetail>();
                    EmployeeInfo.SalaryInfo = new List<SalaryInfoVM>();
                    if(placement !=null || previousPlacement != null)
                    {
                        var SalaryInfo = new SalaryInfoVM();
                        SalaryInfo.Descrption = "ደሞዝ";
                        SalaryInfo.OldValue = previousPlacement != null?previousPlacement.Salary:0;
                        SalaryInfo.NewValue = placement != null?placement.Salary:0;
                        EmployeeInfo.SalaryInfo.Add(SalaryInfo);
                    }
                    var employeeAllowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == employeeId).ToList();
                    foreach (var allowance in employeeAllowances)
                    {
                        var promotionDetail = new PromotionLetterTwoDetail();
                        var allownacevaName = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
                        if (allownacevaName != null)
                        {
                            promotionDetail.Descrption = allownacevaName.AllowanceType.ToString();
                            promotionDetail.OldValue  = "_______";
                            promotionDetail.NewValue  = allownacevaName.Amount;
                            EmployeeInfo.PromotionLetters.Add(promotionDetail);
                        }
                        
                    }

                    EmployeeInfo.GrossTotal = EmployeeInfo.SalaryInfo.Sum(x => x.NewValue) + EmployeeInfo.PromotionLetters.Sum(x => x.NewValue);
                    EmployeeInfo.TotalAmount = XAPI.NumberExtensions.toWords((int)EmployeeInfo.GrossTotal);
                    promotionList.Add(EmployeeInfo);
                }
            }
            return promotionList;
            //List<PromotionLetterVM> promotionLetterList = new List<PromotionLetterVM>();

            //foreach (var pro in promotionList)
            //{
            //    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.PromotionAm);

            //    if (template != null)
            //    {
            //        var promotion = new PromotionLetterVM();

            //        #region Headers

            //        promotion.Abbreviation = pro.Abbreviation;
            //        promotion.Address = pro.Address;
            //        promotion.AmharicName = pro.AmharicName;
            //        promotion.City = pro.City;
            //        promotion.Email = pro.Email;
            //        promotion.Fax = pro.Fax;
            //        promotion.LogoFileName = pro.LogoFileName;
            //        promotion.Name = pro.Name;
            //        promotion.Tel = pro.Tel;
            //        promotion.POBox = pro.POBox;
            //        promotion.WebSite = pro.WebSite;
            //        promotion.Abbreviation = pro.Abbreviation;
            //        promotion.Date = pro.Date;

            //        #endregion

            //        promotion.Title = "ጉዳዩ፡- የደረጃ እድገትን ይመለከታል";

            //        string doc = "";

            //        doc = template.Document.Replace("-PreviousDepartement-", pro.PreviousDepartement);
            //        doc = doc.Replace("-PreviousJobTitle-", pro.PreviousJobTitle);
            //        doc = doc.Replace("-CurrentDepartement-", pro.CurrentDepartement);
            //        doc = doc.Replace("-CurrentJobTitle-", pro.CurrentJobTitle);
            //        doc = doc.Replace("-AssignmentDate-", pro.AssignmentDate);
            //        doc = doc.Replace("-Diffrence-", pro.Diffrence.ToString());
            //        doc = doc.Replace("-DiffrenceAmount-", pro.DiffrenceAmount);
            //        doc = doc.Replace("-CurrentRank-", pro.CurrentRank);
            //        doc = doc.Replace("-CurrentSalary-", pro.CurrentSalary.ToString());
            //        doc = doc.Replace("-CurrentAmount-", pro.CurrentAmount.ToString());


            //        promotion.PromotionLetter = doc;

            //        promotion.FullName = pro.FullName;

            //        promotionLetterList.Add(promotion);
            //    }
            //}

            //return promotionLetterList;
        }
      
        public void Dispose()
        {
            db.Dispose();
        }
        
    }

    public class PromotionLetterTwoVM
    {
        public string PromotionLetter { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }

       
        /// <summary>
        /// //
        /// </summary>
        public string EmployeeId { get; set; }
        public string Sex { get; set; }
        public string CurrentDepartement { get; set; }
        public string CurrentJobTitle { get; set; }
        public string CurrentBranch { get; set; }
        public string CurrentRank { get; set; }
        public decimal CurrentSalary { get; set; }
        public string CurrentAmount { get; set; }
        public string PreviousDepartement { get; set; }
        public string PreviousJobTitle { get; set; }
        public string PreviousBranch { get; set; }
        public string PreviousRank { get; set; }
        public decimal PreviousSalary { get; set; }
        public string PreviousAmount { get; set; }
        public decimal GrossTotal { get; set; }
        public string TotalAmount { get; set; }
        public string AssignmentDate { get; set; }
        public string AssignmentDateGC { get; set; }
        public decimal Diffrence { get; set; }
        public string DiffrenceAmount { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Tel { get; set; }
        public string WebSite { get; set; }
        public string POBox { get; set; }
        public string Date { get; set; }
        public List<JobTitleInfoVm> JobTitleInfos { get; set; }
        public List<SalaryInfoVM> SalaryInfo { get; set; }
        public List<PromotionLetterTwoDetail> PromotionLetters { get; set; }
    }

    public class PromotionLetterTwoDetail
    {
        public string Descrption { get; set; }
        public string OldValue { get; set; }
        public decimal NewValue { get; set; }

    }
    public class SalaryInfoVM
    {
        public string Descrption { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }

    }
    public class JobTitleInfoVm
    {
        public string Descrption { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }

    }
}