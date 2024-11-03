using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
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

namespace ExceedERP.Web.Areas.HRM.Controllers.Severance
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRAttendanceUser)]
    public class SeveranceWithProbabilitiesController : BaseController
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
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }

        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeveranceWithProbabilities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SeveranceWithProbability> severances = db.SeveranceWithProbabilities;
            DataSourceResult result = severances.ToDataSourceResult(request, severance => new
            {
                EmployeeCode = severance.EmployeeCode,
                EmployeeId = severance.EmployeeId,
                YearWorked = severance.YearWorked,
                BranchId = severance.BranchId,
                BusinessUnitId = severance.BusinessUnitId,
                OrgStructureId = severance.OrgStructureId,
                DateOfBirth = severance.DateOfBirth,
                DateOfBirthAm = severance.DateOfBirthAm,
                HiredDate = severance.HiredDate,
                HiredDateAmharic = severance.HiredDateAmharic,
                NormalRetirmentAge = severance.NormalRetirmentAge,
                NormalRetirmentAgeAm = severance.NormalRetirmentAgeAm,
                Gender = severance.Gender,
                Salary = severance.Salary,
                GlFiscalYearId = severance.GlFiscalYearId,
                RemainingNoOfYears = severance.RemainingNoOfYears,
                Age = severance.Age,
                ProbabilityOfTurnover = severance.ProbabilityOfTurnover,
                SalaryUpOnRetirement = severance.SalaryUpOnRetirement,
                PVOfSalaryUponRetirement = severance. PVOfSalaryUponRetirement,
                MaxEntitelementAmount = severance.MaxEntitelementAmount,
                MaxAllowedPeriod = severance.MaxAllowedPeriod,
                EV1 = severance.EV1,
                EV2 = severance.EV2,
                TotalEV = severance.TotalEV,
                MortalityRate = severance.MortalityRate,
                Description = severance.Description,
                DateCreated = severance.DateCreated,
                CreatedBy = severance.CreatedBy,
                LastModified = severance.LastModified,
                ModifiedBy = severance.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceWithProbabilities_Create([DataSourceRequest]DataSourceRequest request, SeveranceWithProbability severance)
        {
            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(severance);
                db.SeveranceWithProbabilities.Add(entity);
                db.SaveChanges();

                #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable ", "Success", User.Identity.Name, "Severance Payable  is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceWithProbabilities_Update([DataSourceRequest]DataSourceRequest request, SeveranceWithProbability severance)
        {

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(severance);
                db.SeveranceWithProbabilities.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
              
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable", "Success", User.Identity.Name, "Severance Payable is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceWithProbabilities_Destroy([DataSourceRequest]DataSourceRequest request, SeveranceWithProbability severance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(severance);
                #region delete log
                var employee = db.GLFiscalYears.Find(severance.GlFiscalYearId);
                var delete = employee.GlFiscalYearId;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee"  + delete);
                #endregion
                db.SeveranceWithProbabilities.Attach(entity);
                db.SeveranceWithProbabilities.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }
        private SeveranceWithProbability GetMappedObject(SeveranceWithProbability severance)
        {
            var entity = new SeveranceWithProbability
            {
                EmployeeCode = severance.EmployeeCode,
                EmployeeId = severance.EmployeeId,
                YearWorked = severance.YearWorked,
                BranchId = severance.BranchId,
                BusinessUnitId = severance.BusinessUnitId,
                OrgStructureId = severance.OrgStructureId,
                DateOfBirth = severance.DateOfBirth,
                DateOfBirthAm = severance.DateOfBirthAm,
                HiredDate = severance.HiredDate,
                HiredDateAmharic = severance.HiredDateAmharic,
                NormalRetirmentAge = severance.NormalRetirmentAge,
                NormalRetirmentAgeAm = severance.NormalRetirmentAgeAm,
                Gender = severance.Gender,
                Salary = severance.Salary,
                GlFiscalYearId = severance.GlFiscalYearId,
                RemainingNoOfYears = severance.RemainingNoOfYears,
                Age = severance.Age,
                ProbabilityOfTurnover = severance.ProbabilityOfTurnover,
                SalaryUpOnRetirement = severance.SalaryUpOnRetirement,
                PVOfSalaryUponRetirement = severance.PVOfSalaryUponRetirement,
                MaxEntitelementAmount = severance.MaxEntitelementAmount,
                MaxAllowedPeriod = severance.MaxAllowedPeriod,
                EV1 = severance.EV1,
                EV2 = severance.EV2,
                TotalEV = severance.TotalEV,
                MortalityRate = severance.MortalityRate,
                Description = severance.Description,
                DateCreated = severance.DateCreated,
                CreatedBy = severance.CreatedBy,
                LastModified = severance.LastModified,
                ModifiedBy = severance.ModifiedBy
            };
            return entity;
        }
        public ActionResult GenerateSeveranceProbability(int yearId)
        {
            var fiscalYear = db.GLFiscalYears.Find(yearId);
            if (fiscalYear == null)
            {
                return Json(new { Message = "There is no record for this fiscal year.", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            var retirementConfig = db.Configurations
                .FirstOrDefault(x => x.Reference == "Pension" && x.Attribute == PensionAttribute.RETIREMENT_AGE.ToString());

            if (retirementConfig == null)
            {
                //return new JavaScriptResult { Script = "alert('You have to set the retirement age first');" };
                return Json(new { Message = "alert('You have to set the retirement age first.", Status = "not-OK" }, JsonRequestBehavior.AllowGet);

            }

            int.TryParse(retirementConfig.CurrentValue, out int retirementAge);
            var now = DateTime.Now;
            var empIds = db.Placements.Where(x=>x.isCurrent).Select(x => x.EmployeeId);
            employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            foreach (var employee in employees)
            {

                if ((((decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays) / 365) >= 5)
                {



                    if (db.SeveranceWithProbabilities.Any(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == yearId))
                    {
                        continue;
                    }

                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.isCurrent);
                    if (placement == null)
                    {
                        continue;
                    }

                    decimal totalYearsOfExperience = (decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays / 365;
                    decimal age = (decimal)((now - employee.DateOfBirth).TotalDays / 365);
                    DateTime normalRetirementAge = employee.DateOfBirth.AddYears(retirementAge);
                    decimal remainingYears = (decimal)(normalRetirementAge - now).TotalDays / 365;
                    double yearOfExperience = Math.Round((double)totalYearsOfExperience, 2);

                    decimal salaryUponRetirement = placement.Salary * (decimal)Math.Pow(1 + 0.125, (double)remainingYears);
                    decimal pvOfSalaryUponRetirement = salaryUponRetirement / (decimal)Math.Pow(1 + 0.14725, (double)remainingYears);
                    decimal maximumEntitlement = pvOfSalaryUponRetirement + (((decimal)yearOfExperience - 1) / 3) * pvOfSalaryUponRetirement;
                    decimal probabilityOfTurnover = CalculatePercentage(age);
                    decimal eV1 = maximumEntitlement * probabilityOfTurnover;
                    decimal mortalityRate = (decimal)calculateValue((now - employee.DateOfBirth).TotalDays / 365) * 100;
                    decimal eV2 = maximumEntitlement *mortalityRate;

                    var severance = db.SeveranceWithProbabilities.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == yearId);
                    if(severance == null)
                    {
                        var entity = new SeveranceWithProbability
                        {
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeId = employee.EmployeeId,
                            YearWorked = (decimal)yearOfExperience,
                            BranchId = placement.BranchId,
                            BusinessUnitId = placement.BusinessUnitId,
                            OrgStructureId = placement.OrgStructureId,
                            DateOfBirth = employee.DateOfBirth,
                            DateOfBirthAm = employee.DateOfBirthAm,
                            HiredDate = employee.HiredDate,
                            HiredDateAmharic = employee.HiredDateAmharic,
                            NormalRetirmentAge = normalRetirementAge,
                            NormalRetirmentAgeAm = XAPI.EthiopicDateTime.GetEthiopicDate(normalRetirementAge.Day, normalRetirementAge.Month, normalRetirementAge.Year),
                            Gender = employee.Gender,
                            Salary = placement.Salary,
                            GlFiscalYearId = yearId,
                            RemainingNoOfYears = remainingYears,
                            Age = age,
                            ProbabilityOfTurnover = probabilityOfTurnover,
                            SalaryUpOnRetirement = salaryUponRetirement,
                            PVOfSalaryUponRetirement = pvOfSalaryUponRetirement,
                            MaxEntitelementAmount = maximumEntitlement,
                            MaxAllowedPeriod = yearOfExperience <= 34 ? (decimal)yearOfExperience : 34,
                            EV1 = eV1,
                            EV2 = eV2,
                            TotalEV = eV1 + eV2,
                            MortalityRate = mortalityRate,
                            Description = "Severance",
                            DateCreated = now.Date,
                            CreatedBy = User.Identity.Name,
                            LastModified = now.Date,
                            ModifiedBy = User.Identity.Name
                        };

                        db.SeveranceWithProbabilities.Add(entity);
                        db.SaveChanges();
                    }
                    else 
                    {
                        severance.RemainingNoOfYears = remainingYears;
                        severance.Age = age;
                        severance.ProbabilityOfTurnover = probabilityOfTurnover;
                        severance.SalaryUpOnRetirement = salaryUponRetirement;
                        severance.PVOfSalaryUponRetirement = pvOfSalaryUponRetirement;
                        severance.MaxEntitelementAmount = maximumEntitlement;
                        severance.MaxAllowedPeriod = yearOfExperience <= 34 ? (decimal)yearOfExperience : 34;
                        severance.EV1 = eV1;
                        severance.EV2 = eV2;
                        severance.TotalEV = eV1 + eV2;
                        severance.MortalityRate = mortalityRate;
                        severance.Description = "Severance";
                        severance.LastModified = now.Date;
                        severance.ModifiedBy = User.Identity.Name;
                        db.Entry(severance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            return Json(new { Message = "Successfully Generated.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateSeveranceProbabilityCopy(int yearId)
        {
            
            
            var fiscalYear = db.GLFiscalYears.Find(yearId);
                   
            if (fiscalYear != null)
            {
                       
                            
                var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
                foreach (var employee in employees)
                {
                    var severance = db.SeveranceWithProbabilities.Where(x => x.EmployeeId == employee.EmployeeId&&x.GlFiscalYearId==yearId).FirstOrDefault();
                    if(severance == null)
                    {

                            
                        var placement = db.Placements.Where(x=>x.EmployeeId==employee.EmployeeId).Where(x => x.isCurrent).FirstOrDefault();
                        //if (placement != null  && ((((decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays) / 365)>5))
                        if (placement != null)
                        {
                            decimal totaYearOfExperience = (decimal)(fiscalYear.DateTo - employee.HiredDate).TotalDays;
                            totaYearOfExperience = totaYearOfExperience / 365;
                            int age = (int)((DateTime.Now - employee.DateOfBirth).TotalDays) / 365;
                            var retirement = db.Configurations.Where(x => x.Reference == "Pension")
                                                             .Where(o => o.Attribute == PensionAttribute.RETIREMENT_AGE.ToString()).FirstOrDefault();
                            DateTime normalRetirmentAge = employee.DateOfBirth.AddYears(60);
                            if (retirement != null)
                            {
                                int.TryParse(retirement.CurrentValue, out int retirementage);
                                normalRetirmentAge = employee.DateOfBirth.AddYears(retirementage);


                            }
                            else
                            {
                                var messagesInitial = "You have  set retirement aga first";

                                return new JavaScriptResult() { Script = "alert('" + messagesInitial + "');" };
                            }
                            var retirementageAm = XAPI.EthiopicDateTime.GetEthiopicDate(normalRetirmentAge.Day, normalRetirmentAge.Month, normalRetirmentAge.Year);

                            decimal remainingYears = (decimal)((normalRetirmentAge - DateTime.Now).TotalDays) / 365;
                            double yearOfExpperience = (double)Math.Round(totaYearOfExperience, 2);
                            decimal salaryUponRetirement = placement.Salary * (decimal)(Math.Pow(1 + 0.125, yearOfExpperience));
                            decimal pVOfSalaryUponRetirement = salaryUponRetirement / (decimal)(Math.Pow(1 + 0.125, (double)remainingYears));

                            decimal maximumEntitlemnt = pVOfSalaryUponRetirement + (totaYearOfExperience - 1) / 3 * pVOfSalaryUponRetirement;
                            var eV1 = maximumEntitlemnt * CalculatePercentage(age);
                            var eV2 = maximumEntitlemnt * CalculatePercentage(age);
                            var mortalityRate = (decimal)calculateValue((DateTime.Now - employee.DateOfBirth).TotalDays / 365) * 100;

                            var entity = new SeveranceWithProbability
                            {
                                EmployeeCode = employee.EmployeeCode,
                                EmployeeId = employee.EmployeeId,
                                YearWorked = Math.Round(totaYearOfExperience, 2),
                                BranchId = placement.BranchId,
                                BusinessUnitId = placement.BusinessUnitId,
                                OrgStructureId = placement.OrgStructureId,
                                DateOfBirth = employee.DateOfBirth,
                                DateOfBirthAm = employee.DateOfBirthAm,
                                HiredDate = employee.HiredDate,
                                HiredDateAmharic = employee.HiredDateAmharic,
                                NormalRetirmentAge = normalRetirmentAge,
                                NormalRetirmentAgeAm = retirementageAm,
                                Gender = employee.Gender,
                                Salary = placement.Salary,
                                GlFiscalYearId = yearId,
                                RemainingNoOfYears = remainingYears,
                                Age = age,
                                ProbabilityOfTurnover = CalculatePercentage(age),
                                SalaryUpOnRetirement = salaryUponRetirement,
                                PVOfSalaryUponRetirement = pVOfSalaryUponRetirement,
                                MaxEntitelementAmount = maximumEntitlemnt,
                                MaxAllowedPeriod = maximumEntitlemnt,
                                EV1 = eV1,
                                EV2 = eV1,
                                TotalEV = eV1+eV2,
                                MortalityRate = mortalityRate,
                                Description = "Severance",
                                DateCreated = DateTime.Now.Date,
                                CreatedBy = User.Identity.Name,
                                LastModified = DateTime.Now.Date,
                                ModifiedBy = User.Identity.Name
                            };
                            db.SeveranceWithProbabilities.Add(entity);
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

        double calculateValue(double K6)
        {
            if (K6 <= 20)
                return 0.00306;
            else if (K6 <= 25)
                return 0.00303;
            else if (K6 <= 30)
                return 0.00355;
            else if (K6 <= 35)
                return 0.00405;
            else if (K6 <= 40)
                return 0.00515;
            else if (K6 <= 45)
                return 0.0045;
            else if (K6 <= 50)
                return 0.00628;
            else if (K6 <= 55)
                return 0.00979;
            else if (K6 <= 60)
                return 0.01536;
            else
                return 0;
        }
        decimal CalculatePercentage(decimal K5)
        {
            if (K5 <= 25)
                return 0.15m;  // 15%
            else if (K5 <= 30)
                return 0.125m; // 12.5%
            else if (K5 <= 35)
                return 0.10m;  // 10%
            else if (K5 <= 40)
                return 0.075m; // 7.5%
            else if (K5 <= 45)
                return 0.05m;  // 5%
            else if (K5 <= 50)
                return 0.025m; // 2.5%
            else if (K5 <= 55)
                return 0.01m;  // 1%
            else if (K5 <= 59.9m)
                return 0.005m; // 0.5%
            else
                return 1.00m;  // 100%
        }


        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Severance data");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee severance");
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
