using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class HealthPaymentSelfCareObjectDataSource:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeHealthPayment> GetEmployeeHealthPayment(int empId, bool header, string year)
        {
            List<EmployeeHealthPayment> employeeHealthPayments = new List<EmployeeHealthPayment>();

            EmployeeHealthPayment EmployeeInfo = new EmployeeHealthPayment();
            EmployeeInfo.HealthPaymentDetaiList = new List<HealthPaymentDetaiList>();

            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
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

                                EmployeeInfo.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                EmployeeInfo.WebSite = "Website:" + a.Value;

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
                        EmployeeInfo.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year);

                    }

                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        EmployeeInfo.LogoFileName =
                            OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }

            }

            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (personEmployee != null)
            {
                EmployeeInfo.EmployeeName = personEmployee.FullNameaEnglish;
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                    if (branch != null)
                    {
                        EmployeeInfo.Branch = branch.Name;
                    }
                    var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                    if (department != null)
                    {
                        EmployeeInfo.Department = department.Name;
                    }
                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookup != null)
                    {
                        EmployeeInfo.JobTitle = lookup.Description;
                    }
                }
                var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderBy(x=>x.RecoredDate).ToList();
                if(year != null)
                {
                    var fiscalYearId = Convert.ToInt32(year);
                   healthPayments = healthPayments.Where(x => x.GlFiscalYearId == fiscalYearId).OrderBy(x => x.RecoredDate).ToList();
                }
                foreach (var hp in healthPayments)
                {
                    var paymentDetail = new HealthPaymentDetaiList();

                    var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == hp.GlFiscalYearId);
                    if (fiscalYear != null)
                    {
                        paymentDetail.FiscalYear = fiscalYear.Name;
                    }
                    var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == hp.GLPeriodId);
                    if (period != null)
                    {
                        paymentDetail.Period = period.Name;
                    }
                    paymentDetail.TreatmentDate = hp.DateAmh;
                    paymentDetail.ReferalDate = hp.DateAmh;
                    paymentDetail.RecoredDate = hp.RecoredDateAmh;
                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == hp.EmployeeId && x.isCurrent);
                    if (place != null &&hp.BenefitType== HealthBenefitType.NA)
                    {
                        var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                        #region comments 
                        //if (rank != null)
                        //{
                        //    if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                        //    {
                        //        var allowedAmount = rank.CurrentValue;
                        //        paymentDetail.AllowedAmt = allowedAmount;
                        //        paymentDetail.UsedAmt = hp.UsedAmount;
                        //        paymentDetail.AllowedSum = allowedAmount;
                        //        paymentDetail.RemainingSum = (Convert.ToDecimal(allowedAmount) - healthPayments.Where(x =>x.GlFiscalYearId == hp.GlFiscalYearId).Sum(x => x.UsedAmount)).ToString();
                        //        paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId).Sum(x => x.UsedAmount);
                        //        var totalUsed = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.RecoredDate < hp.RecoredDate);
                        //        if (totalUsed.Any())
                        //        {
                        //            var totalSum = totalUsed.Sum(x => x.UsedAmount) + hp.UsedAmount;
                        //            paymentDetail.RemainingAmt = (Convert.ToDecimal(allowedAmount) - totalSum).ToString();

                        //        }
                        //        else
                        //        {
                        //            paymentDetail.RemainingAmt = (Convert.ToInt32(allowedAmount) - hp.UsedAmount).ToString();
                        //        }

                        //    }
                        //    else if (rank != null && rank.CurrentValue.ToLower() == "unlimited")
                        //    {
                        //        paymentDetail.UsedAmt = hp.UsedAmount;
                        //        paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId).Sum(x => x.UsedAmount);
                        //        paymentDetail.AllowedAmt = "unlimited";
                        //        paymentDetail.AllowedSum = "unlimited";
                        //        paymentDetail.RemainingSum = "unlimited";
                        //        paymentDetail.RemainingAmt = "unlimited";//(Convert.ToDecimal(allowedAmounttt) - health.UsedAmount).ToString();
                        //    }
                        //   // paymentDetail.AllowedAmt = rank.CurrentValue;
                        //}
                        #endregion
                        var level = db.HealthResponsiblityLevelValueSettings.Where(x=>x.Level == place.ResponsibilityLevel).FirstOrDefault();
                        if (rank != null && IsNumeric(rank.CurrentValue))
                        {
                            var allowedAmount = rank.CurrentValue;
                            paymentDetail.AllowedAmt = rank.CurrentValue;
                            paymentDetail.UsedAmt = hp.UsedAmount;
                            paymentDetail.AllowedSum = allowedAmount;
                            paymentDetail.RemainingSum = hp.RemainingAmount.ToString();
                            paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.BenefitType == HealthBenefitType.NA).Sum(x => x.UsedAmount);
                            //var totalUsed = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.RecoredDate < hp.RecoredDate && x.EmployeeId == hp.EmployeeId && x.BenefitType == HealthBenefitType.NA);
                            //if (totalUsed.Any())
                            //{
                            //    var totalSum = totalUsed.Sum(x => x.UsedAmount) + hp.UsedAmount;
                            //    paymentDetail.RemainingAmt = (Convert.ToDecimal(allowedAmount) - totalSum).ToString();

                            //}
                            //else
                            //{
                            //    paymentDetail.RemainingAmt = (Convert.ToInt32(allowedAmount) - hp.UsedAmount).ToString();
                            //}

                        }
                        else if (rank != null && IsAlphabetic(rank.CurrentValue))
                        {
                            paymentDetail.UsedAmt = hp.UsedAmount;
                            paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId&& x.BenefitType == HealthBenefitType.NA).Sum(x => x.UsedAmount);
                            paymentDetail.AllowedAmt = "unlimited";
                            paymentDetail.AllowedSum = "unlimited";
                            paymentDetail.RemainingSum = "unlimited";
                            paymentDetail.RemainingAmt = "unlimited";
                        }
                        else if (level != null && IsAlphabetic(level.CurrentValue))
                        {
                            paymentDetail.UsedAmt = hp.UsedAmount;
                            paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.BenefitType == HealthBenefitType.NA).Sum(x => x.UsedAmount);
                            paymentDetail.AllowedAmt = "unlimited";
                            paymentDetail.AllowedSum = "unlimited";
                            paymentDetail.RemainingSum = "unlimited";
                            paymentDetail.RemainingAmt = "unlimited";
                        }
                        else if (level != null && IsNumeric(level.CurrentValue))
                        {
                            var allowedAmount = level.CurrentValue;
                            paymentDetail.AllowedAmt = level.CurrentValue;
                            paymentDetail.UsedAmt = hp.UsedAmount;
                            paymentDetail.AllowedSum = allowedAmount;
                            paymentDetail.RemainingSum = hp.RemainingAmount.ToString();
                            //paymentDetail.RemainingSum = (Convert.ToDecimal(allowedAmount) - healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.BenefitType == HealthBenefitType.NA).Sum(x => x.UsedAmount)).ToString();
                            //paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.BenefitType == HealthBenefitType.NA).Sum(x => x.UsedAmount);
                            //var totalUsed = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.RecoredDate < hp.RecoredDate && x.BenefitType == HealthBenefitType.NA);
                            //if (totalUsed.Any())
                            //{
                            //    var totalSum = totalUsed.Sum(x => x.UsedAmount) + hp.UsedAmount;
                            //    paymentDetail.RemainingAmt = (Convert.ToDecimal(allowedAmount) - totalSum).ToString();

                            //}
                            //else
                            //{
                            //    paymentDetail.RemainingAmt = (Convert.ToInt32(allowedAmount) - hp.UsedAmount).ToString();
                            //}

                        }
                        // paymentDetail.UsedAmt = hp.UsedAmount;

                    }
                    else
                    {
                        var benefits = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == hp.BenefitType);
                        if(benefits != null)
                        {
                            paymentDetail.AllowedAmt = benefits.BenfitValue.ToString();
                            paymentDetail.UsedAmt = hp.UsedAmount;
                            paymentDetail.RemainingAmt = hp.RemainingAmount.ToString(); 
                            paymentDetail.AllowedSum = benefits.BenfitValue.ToString();
                            paymentDetail.RemainingSum = (benefits.BenfitValue - healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId && x.EmployeeId == hp.EmployeeId && x.BenefitType == benefits.BenefitType).Sum(x => x.UsedAmount)).ToString();
                            paymentDetail.UsedSum = healthPayments.Where(x => x.GlFiscalYearId == hp.GlFiscalYearId &&x.EmployeeId==hp.EmployeeId&& x.BenefitType == benefits.BenefitType).Sum(x => x.UsedAmount);
                        }
                       
                    }
                    EmployeeInfo.HealthPaymentDetaiList.Add(paymentDetail);
                }
            }
            employeeHealthPayments.Add(EmployeeInfo);
            return employeeHealthPayments;
        }
        public static bool IsNumeric(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }

        public static bool IsAlphabetic(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
