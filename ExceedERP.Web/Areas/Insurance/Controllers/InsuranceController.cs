using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
   
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
    public class InsuranceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            GetInsuranceGroup();
            GetBusinessClass();
            PopulateViewDate();
            return View();
        }
        public void PopulateViewDate()
        {
            var insurance = db.InsuranceCategory.ToList();
            ViewData["InsuranceCatagory"] = insurance;
            ViewData["EmployeeList"] = db.Person_Employee.Select(x => new {
                EmployeeId=x.EmployeeId,
                FullNameaEnglish = x.EmployeeCode + "-" + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
            }).ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

        }
        public JsonResult GetBusinessClass()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var insuranceTypes = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Employee).ToList();
            foreach (var insuranceType in insuranceTypes)
            {
                InsuranceType insurance = new InsuranceType();
                var businessClass = db.Lookups.FirstOrDefault(x => x.LookupId == insuranceType.LookupId && x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);

                insurance.LookupId = businessClass.LookupId;
                insurance.Name = businessClass.Description;

                insuranceTypeList.Add(insurance);
                ViewData["classList"] = insuranceTypeList;
            }


            return Json(insuranceTypeList.Select(s => new
            {
                LookupId = s.LookupId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceType()
        {

            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var insuranceTypes = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Employee).ToList();

            insuranceTypeList.AddRange(insuranceTypes);


            return Json(insuranceTypeList.Select(s => new
            {

                InsuranceTypeId = s.InsuranceTypeId,
                Name = s.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceGroup()
        {

            var insurace = db.InsuranceGroup.ToList();

            ViewData["insuraceGroup"] = insurace;
            return Json(insurace.Select(s => new { InsuranceGroupId = s.InsuranceGroupId, GroupName = s.GroupName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceCategory(int InsuranceTypeId)
        {

            var insurace = db.InsuranceCategory.Where(x=>x.InsuranceTypeId == InsuranceTypeId).ToList();

            ViewData["insuraceGroup"] = insurace;
            return Json(insurace.Select(s => new { InsuranceCategoryId = s.InsuranceCategoryId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public void UpdateAllEmployeeInsurance()
        {
            var insurances = db.Insurance.ToList();

            foreach (var insurance in insurances)
            {


                var placement = db.Placements.Where(x => x.EmployeeId == insurance.EmployeeId).FirstOrDefault();
                if(placement !=null &&placement.Salary !=0)
                {
                    insurance.Salary = placement.Salary;
                    db.Entry(insurance).State = EntityState.Modified;
                }

               

            }

            db.SaveChanges(); // Save all changes to the database at once
        }
        public string GetEmployeeBirthDate(int id)
        {
            string birthDate = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                birthDate = person.DateOfBirthAm;

            }
            return birthDate;

        }
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
        public ActionResult Insurance_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Core.Domain.Insurance.Insurance> insurances = db.Insurance;
            DataSourceResult result = insurances.ToDataSourceResult(request, insurance => new
            {
                InsuranceId = insurance.InsuranceId,
                EmployeeId = insurance.EmployeeId,
                InsuranceCategoryId = insurance.InsuranceCategoryId,
                InsuranceTypeId = insurance.InsuranceTypeId,
                From = insurance.From,
                To = insurance.To,
                LookupId = insurance.LookupId,
                InsuranceCorporation = insurance.InsuranceCorporation,
                Number = insurance.Number,
                Salary = insurance.Salary,
                BirthDate = GetEmployeeBirthDate(insurance.EmployeeId),
                SumofInsured = insurance.SumofInsured,
                Premium = insurance.Premium,
                Remark = insurance.Remark,
                FromAmharic = insurance.FromAmharic,
                ToAmharic = insurance.ToAmharic,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult Insurance_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Insurance.Insurance insurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date values are mandatory!!");
                }
                else
                {


                    if (FromAmharic == null)
                    {
                        var splitDate = insurance.FromAmharic.Split('/');
                        From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    }
                    if (ToAmharic == null)
                    {
                        var splitDate = insurance.ToAmharic.Split('/');
                        To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    }
                    insurance.From = From;
                    insurance.To = To;
                    //if (insurance.Salary <= 0)
                    //{
                    //    insurance.Salary = GetInsuranceSalaryInside(insurance.InsuranceCategoryId);
                    //}
                    //if (insurance.Number <= 0)
                    //{
                    //    insurance.Number = GetInsuranceNumber(insurance.InsuranceCategoryId);
                    //}
                    var employee = db.Placements.FirstOrDefault(x => x.EmployeeId == insurance.EmployeeId);
                    insurance.Salary = employee != null ? employee.Salary : 0;
                    var entity = GetMappedObject(insurance);
                    db.Insurance.Add(entity);
                    db.SaveChanges();
                    insurance.InsuranceId = entity.InsuranceId;
                }
            }

            return Json(new[] { insurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
       
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult Insurance_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Insurance.Insurance insurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    var splitDate = insurance.FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    var splitDate = insurance.ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                insurance.From = From;
                insurance.To = To;
                //if(insurance.Salary <= 0)
                //{
                //    insurance.Salary = GetInsuranceSalaryInside(insurance.InsuranceCategoryId);
                //}
                //if (insurance.Number <= 0)
                //{
                //    insurance.Number = GetInsuranceNumber(insurance.InsuranceCategoryId);
                //}
                var employee = db.Placements.FirstOrDefault(x => x.EmployeeId == insurance.EmployeeId);
                insurance.Salary = employee != null ? employee.Salary : 0;
                var entity = GetMappedObject(insurance);
                db.Insurance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance", "Success", User.Identity.Name, User.Identity.Name + " update Insurance at Date " + DateTime.Today + " insurance Corporation " + entity.InsuranceCorporation, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
        public ActionResult Insurance_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Insurance.Insurance insurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insurance);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted group insurance information of catagory" + insurance.InsuranceCategoryId);
                #endregion
                db.Insurance.Attach(entity);
                db.Insurance.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { insurance }.ToDataSourceResult(request, ModelState));
        }
        private Core.Domain.Insurance.Insurance GetMappedObject(Core.Domain.Insurance.Insurance insurance)
        {
            var entity = new Core.Domain.Insurance.Insurance
            {
                InsuranceId = insurance.InsuranceId,
                EmployeeId = insurance.EmployeeId,
                InsuranceCategoryId = insurance.InsuranceCategoryId,
                InsuranceTypeId = insurance.InsuranceTypeId,
                From = insurance.From,
                To = insurance.To,
                LookupId = insurance.LookupId,
                InsuranceCorporation = insurance.InsuranceCorporation,
                Number = insurance.Number,
                Salary = insurance.Salary,
                SumofInsured = insurance.SumofInsured,
                Premium = insurance.Premium,
                Remark = insurance.Remark,
                FromAmharic = insurance.FromAmharic,
                ToAmharic = insurance.ToAmharic,
            };
            return entity;
        }
      
        static decimal ClassofBussiness
        {
            get;
            set;
        }
        static decimal Salary = 0;
        static int number = 0;
        static decimal sumOfInsured = 0;
        static decimal premium = 0;
        public string GetInsuranceDetail(int group)
        {
            Salary = 0;
            number = 0;
            sumOfInsured = 0;
            premium = 0;
            var grop = db.InsuranceGroup.FirstOrDefault(x => x.InsuranceGroupId == group);


            string value = "";

            if (grop != null)
            {
                var classdesc = db.Lookups.Where(x => x.LookupId == grop.LookupId).FirstOrDefault().Description;


                var setting = db.Configurations.Where(x => x.Category == "Insurance" && x.Reference == classdesc).FirstOrDefault();
                if (setting != null)
                {
                    ClassofBussiness = Convert.ToDecimal(setting.CurrentValue);
                }

                var placment = db.Placements.Where(x => x.LookupId == grop.LookupId && x.isCurrent).ToList();
                number = placment.Count;
                var salary = db.SalariesJobTitles.Where(x => x.LookupId == grop.LookupId).FirstOrDefault();
                if (salary != null)
                {
                    var jk = salary.Salary;
                    var salaryAmount = db.Salaries.Where(x => x.RankId == salary.RankId && x.StepId == salary.StepId).FirstOrDefault();
                    if (salaryAmount != null)
                    {

                        var amt = Convert.ToDecimal(salaryAmount.SalaryAmount);
                        Salary = amt;
                        value += amt.ToString();
                        sumOfInsured = Math.Round(amt * number * 12 * 5, 2);
                        value += "-" + sumOfInsured;
                        premium = Math.Round(sumOfInsured * ClassofBussiness, 2);
                        value += "-" + premium;
                    }
                }

                value += "-" + number.ToString();
            }
            return value;
        }

        public string GetInsuranceSalary(int group)
        {


            decimal salaryAmount = 0;
            number = 0;
            var times = 1;
            sumOfInsured = 0;
            premium = 0;
            string value;
            var insuranceGourps = db.InsuranceGroup.Where(x => x.InsuranceCategoryId == group).ToList();
            foreach (var insuranceGourp in insuranceGourps)
            {
                var placment = db.Placements.Where(x => x.LookupId == insuranceGourp.LookupId && x.isCurrent).ToList();
                number = number + placment.Count;
                times = placment.Count();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == insuranceGourp.LookupId);
                if (lookup != null)
                {
                    var salary = db.SalariesJobTitles.FirstOrDefault(x => x.LookupId == lookup.LookupId);
                    if (salary != null)
                    {
                        salaryAmount +=  (salary.Salary * times);
                       // sumOfInsured = salaryAmount * number * 12 * 5;
                    }
                }
               

            }
            value = salaryAmount + "-" + number ;

            return value;
        }
        public int GetInsuranceNumber(int group)
        {


            //decimal salaryAmount = 0;
            number = 0;
            //var times = 1;
            sumOfInsured = 0;
            premium = 0;
            var insuranceGourps = db.InsuranceGroup.Where(x => x.InsuranceCategoryId == group).ToList();
            foreach (var insuranceGourp in insuranceGourps)
            {
                var placment = db.Placements.Where(x => x.LookupId == insuranceGourp.LookupId && x.isCurrent).ToList();
                number = number + placment.Count;
                
                
               

            }

            return number;
        }
        public decimal GetInsuranceSalaryInside(int group)
        {


            decimal salaryAmount = 0;
            number = 0;
            var times = 1;
            sumOfInsured = 0;
            premium = 0;
            var insuranceGourps = db.InsuranceGroup.Where(x => x.InsuranceCategoryId == group).ToList();
            foreach (var insuranceGourp in insuranceGourps)
            {
                var placment = db.Placements.Where(x => x.LookupId == insuranceGourp.LookupId && x.isCurrent).ToList();
                number = number + placment.Count;
                times = placment.Count();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == insuranceGourp.LookupId);
                if (lookup != null)
                {
                    var salary = db.SalariesJobTitles.FirstOrDefault(x => x.LookupId == lookup.LookupId);
                    if (salary != null)
                    {
                        salaryAmount +=  (salary.Salary * times);
                       // sumOfInsured = salaryAmount * number * 12 * 5;
                    }
                }
               

            }
           

            return salaryAmount;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
