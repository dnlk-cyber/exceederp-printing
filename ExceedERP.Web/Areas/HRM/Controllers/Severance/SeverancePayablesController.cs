using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Severance;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
namespace ExceedERP.Web.Areas.HRM.Controllers.Severance
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class SeverancePayablesController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSetting
        HRMInputs inputs = new HRMInputs();

        public ActionResult Index()
        {
            GetPeriod();
            PopulateViewData();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            ViewData["businessUnits"] = db.BusinessUnits;
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = db.Person_Employee.Select(e=>new
            {
                PersonId = e.EmployeeId,
                FullNameWithOutCode = e.FirstName + " " + e.MiddleName + " " + e.LastName,
            }).ToList();
        }

        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeverancePayables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable< SeverancePayable> severancePayables = db.SeverancePayables;
            DataSourceResult result = severancePayables.ToDataSourceResult(request, severancePayable => new
            {
                SeverancePayableId = severancePayable.SeverancePayableId,
                GlFiscalYearId = severancePayable.GlFiscalYearId,
                Description = severancePayable.Description,
                DateCreated = severancePayable.DateCreated,
                CreatedBy = severancePayable.CreatedBy,
                LastModified = severancePayable.LastModified,
                ModifiedBy = severancePayable.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayables_Create([DataSourceRequest]DataSourceRequest request, SeverancePayable severance)
        {
            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(severance);
                db.SeverancePayables.Add(entity);
                db.SaveChanges();

                #region operation log
                  UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable ", "Success", User.Identity.Name, "Severance Payable  is Created By  " + User.Identity.Name,Modules.HRM);
                #endregion
            }
            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayables_Update([DataSourceRequest]DataSourceRequest request, SeverancePayable severance)
        {

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(severance);
                db.SeverancePayables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable", "Success", User.Identity.Name, "Severance Payable is Updated By  " + User.Identity.Name,Modules.HRM);
                #endregion
            }


            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayables_Destroy([DataSourceRequest]DataSourceRequest request, SeverancePayable severance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(severance);
                #region delete log
                var employee = db.GLFiscalYears.Find(severance.GlFiscalYearId);
                var delete = employee.GlFiscalYearId;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee"  + delete);
                #endregion
                db.SeverancePayables.Attach(entity);
                db.SeverancePayables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }
        private SeverancePayable GetMappedObject(SeverancePayable severancePayable)
        {
            var entity = new SeverancePayable
            {
                SeverancePayableId = severancePayable.SeverancePayableId,
                GlFiscalYearId = severancePayable.GlFiscalYearId,
                Description = severancePayable.Description,
                DateCreated = severancePayable.DateCreated,
                CreatedBy = severancePayable.CreatedBy,
                LastModified = severancePayable.LastModified,
                ModifiedBy = severancePayable.ModifiedBy
            };
            return entity;
        }
        public ActionResult GenerateSeverancePayableCopy(int id)
        {
            
            var payable = db.SeverancePayables.Find(id);
            if (payable != null)
            {
                var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
                    foreach (var employee in employees)
                    {
                        var payableDetail = db.SeverancePayableDetails.Where(x => x.EmployeeId == employee.EmployeeId&&x.SeverancePayableId==id).FirstOrDefault();
                        if(payableDetail == null)
                        {
                            
                            DateTime toDate = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).DateTo;
                            var placement = db.Placements.Where(x=>x.EmployeeId==employee.EmployeeId).Where(x => x.isCurrent).FirstOrDefault();
                            if (placement != null &&toDate !=null && ((((decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays) / 365)>5))
                            {
                                decimal balance = 0;
                                decimal totaYearOfExperience = (decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays;
                                totaYearOfExperience = totaYearOfExperience / 365;
                                balance =  (decimal)(1.0 / 3) * (decimal)(totaYearOfExperience - 1) * placement.Salary+ placement.Salary;
                                var entity = new SeverancePayableDetail
                                {
                                    SeverancePayableId = id,
                                    EmployeeCode = employee.EmployeeCode,
                                    EmployeeId = employee.EmployeeId,
                                    Balance = balance,
                                    BasicSalary = placement.Salary,
                                    BranchId = placement.BranchId,
                                    BusinessUnitId = placement.BusinessUnitId,
                                    OrgStructureId = placement.OrgStructureId,
                                    GrossComponsation = balance,
                                    NetPay = placement.Salary!=0?(balance - (balance / placement.Salary) * (decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary)):0,
                                    NoneTaxableTotal = balance,
                                    NumberOFYears = Math.Round(totaYearOfExperience,2)  + "  years",
                                    SpecialSalaryDeduction = 0,
                                    TaxWithheldComTax = placement.Salary != 0 ? (balance/placement.Salary)*(decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary):0,
                                    TaxOnBasicSalary = (decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary),
                                    TotalDeductions = placement.Salary != 0 ? (balance / placement.Salary) * (decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary) : 0,
                                    DateCreated = DateTime.Now.Date,
                                    CreatedBy = User.Identity.Name,
                                    LastModified = DateTime.Now.Date,
                                    ModifiedBy = User.Identity.Name
                                };
                                db.SeverancePayableDetails.Add(entity);
                                db.SaveChanges();
                            }
                        }

                    }
                    var formattedData2 = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                         
                }

                var formattedData21 = new
                {
                    Message = " there is no record for this fiscal year.",
                    Status = "not-OK"
                };
                return Json(formattedData21, JsonRequestBehavior.AllowGet);



            }



            var formattedData3 = new
            {
                Message = "File Not Found  ",
                Status = "not-OK"
            };
            return Json(formattedData3, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PaySeverancePayble(int id)
        {
            var payableDetail = db.SeverancePayableDetails.Find(id);
            if (payableDetail == null)
            {
                return Json(new { Message = "Record tot found", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            if (payableDetail.IsSendForPayment)
            {
                return Json(new { Message = "Already sent for Payment .", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);

            }
          
            var requestEntity = new PaymentRequest
            {
                SupplierId = "0",
                IsNewSupplier = false,
                Supplier = " ",
                Purchaser = 0,
                ReasonOfPayment = "Severance Payment ",
                RequestDate = DateTime.Now.Date,
                RequestDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                InvoiceNo = "0",
                PurchaseRequestNo = "0",
                PONo = "0",
                GRN = "0",
                BranchId = payableDetail.BranchId,
                BusinessUnitId = payableDetail.BusinessUnitId??0,
                OrgStuructureId = payableDetail.OrgStructureId,
                EmployeeId = payableDetail.EmployeeId,
                Paid = false,
                FromBranchId = payableDetail.BranchId,
                FromBusinessUnitId = payableDetail.BusinessUnitId??0,
                CreatedAtBranchId = payableDetail.BranchId,
                CreatedAtBusinessUnitId = payableDetail.BusinessUnitId??0,
                CreatedAtOrgStructureId = payableDetail.OrgStructureId,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now.Date,
                ModifiedBy = User.Identity.Name,
                LastModified = DateTime.Now.Date,
                CurrentApprovalStep = 0,
                IsDepartmentOnlineApproved = true,
                DepartmentOnlineApprovedBy = User.Identity.Name,
                DepartmentOnlineApprovedTime = DateTime.Now,



            };
            db.PaymentRequests.Add(requestEntity);
            db.SaveChanges();
            var glTaxRate = db.HealthExpenseAndTaxSettings.FirstOrDefault();
            if (glTaxRate != null)
            {
                var RequestEntityDetail = new PaymentRequestLine
                {
                    PaymentRequestId = requestEntity.PaymentRequestId,
                    Amount = payableDetail.NetPay,
                    TaxAmount = 0,
                    TotalAmount = payableDetail.NetPay,
                    ItemName = glTaxRate.ApSetupItemId.ToString(),
                    GLTaxRateID = glTaxRate.GLTaxRateID,
                    Description = "Severance Payment",
                    Remark = "Severance Payment",
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now.Date,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date,

                };
                db.PaymentRequestLines.Add(RequestEntityDetail);
                db.SaveChanges();
            }
            else
            {
                var RequestEntityDetail = new PaymentRequestLine
                {
                    PaymentRequestId = requestEntity.PaymentRequestId,
                    Amount = payableDetail.NetPay,
                    TaxAmount = 0,
                    TotalAmount = payableDetail.NetPay,
                    ItemName = "0",
                    GLTaxRateID = 0,
                    Description = "Severance Payment",
                    Remark = "Description",
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now.Date,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date,

                };
                db.PaymentRequestLines.Add(RequestEntityDetail);
                db.SaveChanges();
            }
            payableDetail.IsSendForPayment = true;
            payableDetail.SendBy = User.Identity.Name;
            payableDetail.DateSent = DateTime.Now.Date;
            payableDetail.PaymentRequestId = requestEntity.PaymentRequestId;
            db.Entry(payableDetail).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Message = "Successfully Sent for payment Request.", Status = "OK" }, JsonRequestBehavior.AllowGet);


        }
        #region Generate payable
        public JsonResult GenerateSeverancePayable(int id, string severanceDate, string empId = null)
        {
            var payable = db.SeverancePayables.Find(id);
            if (payable == null)
            {
                return Json(new { Message = "Record not found", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
            if (fiscalYear == null)
            {
                return Json(new { Message = "No record for this fiscal year.", Status = "NOK-OK" }, JsonRequestBehavior.AllowGet);
            }
            var setting = db.SeverancePayableDetails.FirstOrDefault();
            if (fiscalYear == null)
            {
                return Json(new { Message = "set the severance setting first .", Status = "NOK-OK" }, JsonRequestBehavior.AllowGet);
            }

            //include recent terminated employee
            var terminatedEmployeeIds = db.Terminations.Where(x => x.TerminationDate >= fiscalYear.DateFrom&&x.IsOnlineApproved).Select(x => x.EmployeeId);
            var employees = db.Person_Employee.Where(x => x.IsActive||terminatedEmployeeIds.Contains(x.EmployeeId)).ToList();
            if(!string.IsNullOrEmpty(empId)&& employees.Any())
            {
                int.TryParse(empId, out int employeeId);
                employees = employees.Where(x => x.EmployeeId == employeeId).ToList();
            }
            
            DateTime toDate = fiscalYear.DateTo;
            DateTime severanceDateGC = DateTime.Now;
            DateTime now = DateTime.Now;

            if (!string.IsNullOrEmpty(severanceDate))
            {
                var splitDateTo = severanceDate.Split('/');
                severanceDateGC = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

            }
            if (severanceDateGC < now)
            {
                now = severanceDateGC;
            }
            foreach (var employee in employees)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.isCurrent);
                if (placement == null)
                {
                    placement = db.Placements.Where(x => x.EmployeeId == employee.EmployeeId&&terminatedEmployeeIds.Contains(x.EmployeeId)).OrderByDescending(x => x.PlacementId).FirstOrDefault();
                    if (placement ==null)
                    {
                        continue;
                    }
                   
                   
                }
               
                DateTime date = fiscalYear.DateTo > DateTime.Now ? DateTime.Now : fiscalYear.DateTo;
                
                decimal totaYearOfExperience = 0;
                string Experience = "";
                decimal years = 0;
                decimal months = 0;
                decimal totalServiceInmonth = 0;
                decimal grossCompensatioPayment = 0;
                decimal compensatioPaymentT = 0;
                decimal grossCompensation = 0;
                decimal taxOnMonthlySalaryCompensation = 0;
                decimal taxdiffernce= 0;
                decimal accTaxdiffernce= 0;
                decimal days = 0;
                decimal tax = 0;
                decimal netPay = 0;
                decimal balance = 0;
                if (HasRequiredExperience(now, employee))
                {
                     totaYearOfExperience = CalculateTotalYearsOfExperience(now, employee.HiredDate);
                     balance = CalculateBalance(totaYearOfExperience, placement);
                     years = TotalYears(now, employee.HiredDate);
                     months = TotalMonths(now, employee.HiredDate);
                     days = TotalDays(now, employee.HiredDate);
                    //grossCompensatioPayment = CalculateBalance(totaYearOfExperience, placement);
                    var (year, month, day) = TotalYearsOfExperience(now, employee.HiredDate);

                    //totalServiceInmonth = TotalServiceInMonths(now, employee.HiredDate);
                    totalServiceInmonth = (year * 12) + month + ((decimal)day / 30); ;
                    grossCompensatioPayment = CalculateGrossCompensationPayment(totalServiceInmonth, placement);
                    compensatioPaymentT = grossCompensatioPayment / totalServiceInmonth;
                    grossCompensation = compensatioPaymentT + placement.Salary;
                    Experience = TotalYearsOfExperienceWord(now, employee.HiredDate);
                    tax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary);
                    taxOnMonthlySalaryCompensation = CalculateTaxOnMonthlySalaryAndComponsation(grossCompensation);
                    taxdiffernce = taxOnMonthlySalaryCompensation - tax;
                    accTaxdiffernce = totalServiceInmonth * taxdiffernce;
                    //netPay = CalculateNetPay(balance, placement.Salary, tax);
                    netPay = grossCompensatioPayment-accTaxdiffernce;
                    var payableDetail = db.SeverancePayableDetails
                       .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.SeverancePayableId == id);
                    if (payableDetail == null)
                    {
                        var entity = new SeverancePayableDetail
                        {
                            SeverancePayableId = id,
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeId = employee.EmployeeId,
                            Balance = balance,
                            BranchId = placement.BranchId,
                            BusinessUnitId = placement.BusinessUnitId,
                            OrgStructureId = placement.OrgStructureId,
                            Years = year,
                            Months = month,
                            Days = day,
                            ServicePeriod = employee.HiredDate.ToString("dd.MM.yyyy") +"-"+ now.ToString("dd.MM.yyyy"),
                            BasicSalary = placement.Salary,
                            TotalServiceMonths = totalServiceInmonth,
                            GrossCompensationPayment = grossCompensatioPayment,
                            CompensationPaymentT = compensatioPaymentT,
                            GrossComponsation = grossCompensation,
                            TaxOnMonthlySalaryAndCompensation = taxOnMonthlySalaryCompensation ,
                            TaxOnBasicSalary = tax,
                            TaxDifference = taxdiffernce,
                            AccTaxDifference = accTaxdiffernce,
                            NetPay = netPay,
                            NoneTaxableTotal = balance,
                            NumberOFYears = Experience,
                            SpecialSalaryDeduction = 0,
                            OrgBranchName = " ",
                            TaxWithheldComTax = CalculateTaxWithheld(balance, placement.Salary, tax),
                            TotalDeductions = CalculateDeductions(balance, placement.Salary, tax),
                            Remark = "this Severance is Generated On " +now.ToString("dd.MM.yyyy"),
                            DateCreated = DateTime.Now.Date,
                            CreatedBy = User.Identity.Name,
                            LastModified = null,
                            ModifiedBy = null
                        };

                        db.SeverancePayableDetails.Add(entity);
                        db.SaveChanges();
                    
                    }
                    else
                    {
                        payableDetail.BasicSalary = placement.Salary;
                        payableDetail.BranchId = placement.BranchId;
                        payableDetail.BusinessUnitId = placement.BusinessUnitId;
                        payableDetail.OrgStructureId = placement.OrgStructureId;
                        payableDetail.Years = year;
                        payableDetail.Months = month;
                        payableDetail.Days = day;
                        payableDetail.ServicePeriod = employee.HiredDate.ToString("dd.MM.yyyy")+" (" +employee.HiredDateAmharic+ ") - " + now.ToString("dd.MM.yyyy")+"("+XAPI.EthiopicDateTime.GetEthiopicDate(now.Day,now.Month,now.Year) +")";
                        payableDetail.BasicSalary = placement.Salary;
                        payableDetail.TotalServiceMonths = totalServiceInmonth;
                        payableDetail.GrossCompensationPayment = grossCompensatioPayment;
                        payableDetail.CompensationPaymentT = compensatioPaymentT;
                        payableDetail.GrossComponsation = grossCompensation;
                        payableDetail.TaxOnMonthlySalaryAndCompensation = taxOnMonthlySalaryCompensation;
                        payableDetail.TaxOnBasicSalary = tax;
                        payableDetail.TaxDifference = taxdiffernce;
                        payableDetail.AccTaxDifference = accTaxdiffernce;
                        payableDetail.NetPay = netPay;
                        payableDetail.NoneTaxableTotal = balance;
                        payableDetail.NumberOFYears = Experience;
                        payableDetail.SpecialSalaryDeduction = 0;
                        payableDetail.OrgBranchName = " ";
                        payableDetail.TaxWithheldComTax = CalculateTaxWithheld(balance, placement.Salary, tax);
                        payableDetail.TotalDeductions = CalculateDeductions(balance, placement.Salary, tax);
                        payableDetail.Remark = "This Severance is regenerated on " + now.ToString("dd.MM.yyyy");
                        payableDetail.TotalDeductions = CalculateDeductions(balance, placement.Salary, tax);
                        payableDetail.LastModified = DateTime.Now;
                        payableDetail.ModifiedBy = User.Identity.Name;
                        db.Entry(payableDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }

            return Json(new { Message = "Successfully Generated.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        //check if the empleyee has  minimum requied experience
        private bool HasRequiredExperience(DateTime date, Person_Employee employee)
        {
            return ((decimal)((date - employee.HiredDate).TotalDays / 365.25)) > 5;
        }

        private decimal CalculateTotalYearsOfExperience(DateTime dateTo, DateTime hiredDate)
        {
            return (decimal)((dateTo - hiredDate).TotalDays / 365.25);
        }
        //private string TotalYearsOfExperienceWord(DateTime dateTo, DateTime hiredDate)
        //{
        //    TimeSpan totalExperience = dateTo - hiredDate;

        //    int years = (int)(totalExperience.TotalDays / 365.25);
        //    int remainingDaysAfterYears = (int)(totalExperience.TotalDays % 365.25);
        //    int months = remainingDaysAfterYears/30; // Approximate, assumes 30 days per month
        //    int days = remainingDaysAfterYears % 30;

        //    return $"{years} years, {months} months, {days} days";
        //}
        private string TotalYearsOfExperienceWord(DateTime dateTo, DateTime hiredDate)
        {
            // Ensure hiredDate is not after dateTo
            if (hiredDate > dateTo)
            {
                throw new ArgumentException("Hired date cannot be after the reference date.");
            }

            // Calculate years, months, and days using DateTime manipulation
            int years = dateTo.Year - hiredDate.Year;
            int months = dateTo.Month - hiredDate.Month;
            int days = dateTo.Day - hiredDate.Day;

            // Adjust for negative months or days
            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(hiredDate.Year, hiredDate.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years},{months},{days}";
        }

        private (int year, int month, int day) TotalYearsOfExperience(DateTime dateTo, DateTime hiredDate)
        {
            

            // Calculate years, months, and days using DateTime manipulation
            int years = dateTo.Year - hiredDate.Year;
            int months = dateTo.Month - hiredDate.Month;
            int days = dateTo.Day - hiredDate.Day;

            // Adjust for negative months or days
            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(hiredDate.Year, hiredDate.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            // Return values as a tuple
            return (years, months, days);
        }
        private decimal TotalYears(DateTime dateTo, DateTime hiredDate)
        {
            TimeSpan totalExperience = dateTo - hiredDate;

            int years = (int)(totalExperience.TotalDays / 365.25);
            int remainingDaysAfterYears = (int)(totalExperience.TotalDays % 365);
            int months = remainingDaysAfterYears / 30; // Approximate, assumes 30 days per month
            int days = remainingDaysAfterYears % 30;

            return years;
        }

        private decimal TotalMonths(DateTime dateTo, DateTime hiredDate)
        {
            TimeSpan totalExperience = dateTo - hiredDate;

            int years = (int)(totalExperience.TotalDays / 365.25);
            int remainingDaysAfterYears = (int)(totalExperience.TotalDays % 365);
            int months = remainingDaysAfterYears / 30; // Approximate, assumes 30 days per month
            int days = remainingDaysAfterYears % 30;

            return months;
        }
        private int TotalDays(DateTime dateTo, DateTime hiredDate)
        {
            if (dateTo <= hiredDate)
            {
                // Return 0 if the end date is the same as or earlier than the hire date
                return 0;
            }

            // Initialize total days
            int totalDays = 0;

            // Loop through each month from the hire date to the end date
            DateTime current = hiredDate;
            while (current < dateTo)
            {
                int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

                // If the current month extends beyond the end date, only count up to the end date
                if (current.Year == dateTo.Year && current.Month == dateTo.Month)
                {
                    totalDays += (dateTo - current).Days;
                    break;
                }
                else
                {
                    totalDays += daysInMonth - current.Day + 1; // Count remaining days in the current month
                    current = current.AddMonths(1); // Move to the next month
                    current = new DateTime(current.Year, current.Month, 1); // Reset to the 1st day of the new month
                }
            }

            return totalDays;
        }

        //private decimal TotalDays(DateTime dateTo, DateTime hiredDate)
        //{
        //    TimeSpan totalExperience = dateTo - hiredDate;

        //    int years = (int)(totalExperience.TotalDays / 365.25);
        //    int remainingDaysAfterYears = (int)(totalExperience.TotalDays % 365);
        //    int months = remainingDaysAfterYears / 30; // Approximate, assumes 30 days per month
        //    int days = remainingDaysAfterYears % 30;

        //    return days;
        //}
        private decimal TotalServiceInMonths(DateTime dateTo, DateTime hiredDate)
        {
            if (dateTo <= hiredDate)
            {
                // Return 0 if the end date is the same as or earlier than the hire date
                return 0;
            }

            int years = dateTo.Year - hiredDate.Year;
            int months = dateTo.Month - hiredDate.Month;

            // Adjust for partial years
            if (months < 0)
            {
                years--;
                months += 12;
            }

            // Total months including years
            decimal totalMonths = years * 12 + months;

            // Adjust for partial months
            if (dateTo.Day < hiredDate.Day)
            {
                // Calculate the number of days in the month of hire date
                int daysInHiredMonth = DateTime.DaysInMonth(hiredDate.Year, hiredDate.Month);

                // Subtract the partial month
                totalMonths -= (hiredDate.Day - dateTo.Day) / (decimal)daysInHiredMonth;
            }
            else if (dateTo.Day > hiredDate.Day)
            {
                // Calculate the number of days in the month of the end date
                int daysInCurrentMonth = DateTime.DaysInMonth(dateTo.Year, dateTo.Month);

                // Add the partial month
                totalMonths += (dateTo.Day - hiredDate.Day) / (decimal)daysInCurrentMonth;
            }

            return totalMonths;
        }

        private decimal CalculateBalance(decimal totalYearsOfExperience,Placement placement  )
        {
            var setting = db.SeverancePayableSettings.FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);
            if(setting !=null &&setting.SalaryValue != 0)
            {
                return (1.0M / setting.SalaryValue) * (totalYearsOfExperience - 1) * placement.Salary + placement.Salary;

            }
            else
            {
                return (1.0M / 3) * (totalYearsOfExperience - 1) * placement.Salary + placement.Salary;

            }
        }
        private decimal CalculateGrossCompensationPayment(decimal totalMonths, Placement placement)
        {
            if (totalMonths < 13)
            {
                return totalMonths * placement.Salary / 12;
            }
            else // K6 > 12
            {
                var setting = db.SeverancePayableSettings.FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);
                if (setting != null && setting.SalaryValue != 0)
                {
                    return (placement.Salary * 12 / 12) + (placement.Salary * (totalMonths - 12) / 12 * setting.SalaryValue);

                    //return (1.0M / setting.SalaryValue) * (totalYearsOfExperience - 1) * placement.Salary + placement.Salary;

                }
                else
                {
                    return (placement.Salary * 12 / 12) + (placement.Salary * (totalMonths - 12) / 12 * 1.0m / 3);

                   // return (1.0M / 3) * (totalYearsOfExperience - 1) * placement.Salary + placement.Salary;

                }
            }
           
        }

        private decimal CalculateNetPay(decimal balance, decimal salary, decimal tax)
        {
            return salary != 0 ? balance - (balance / salary) * tax : 0;
        }

        private decimal CalculateTaxWithheld(decimal balance, decimal salary, decimal tax)
        {
            return salary != 0 ? (balance / salary) * tax : 0;
        }

        decimal CalculateTaxOnMonthlySalaryAndComponsation(decimal N5)
        {
            if (N5 <= 600)
            {
                return N5 * 0;
            }
            else if (N5 <= 1650)
            {
                return N5 * 0.1m - 60;
            }
            else if (N5 <= 3200)
            {
                return N5 * 0.15m - 142.5m;
            }
            else if (N5 <= 5250)
            {
                return N5 * 0.2m - 302.5m;
            }
            else if (N5 <= 7800)
            {
                return N5 * 0.25m - 565;
            }
            else if (N5 <= 10900)
            {
                return N5 * 0.3m - 955;
            }
            else // N5 > 10900
            {
                return N5 * 0.35m - 1500;
            }
        }

        private decimal CalculateDeductions(decimal balance, decimal salary, decimal tax)
        {
            return CalculateTaxWithheld(balance, salary, tax);
        }

        #endregion
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
