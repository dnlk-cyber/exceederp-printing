using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class EmployeeAccountController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        // GET: HRM/EmployeeAccount
        public ActionResult Index()
        {
            SetInitialDatas();
         
      
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            var branchs = db.Branch;
            ViewData["branches"] = branchs;


            return View();
        }
        public JsonResult GetCostCenter()
        {
            var glchartofaccountaccount = db.GLChartOfAccountCostCenters.AsQueryable();
            ViewData["CostAcc"] = glchartofaccountaccount;
            return Json(glchartofaccountaccount.Select(i => new { GlchartAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountCode()
        {
            var glchartofaccountaccount = db.GLChartOfAccountAccounts.AsQueryable();
            ViewData["AccCode"] = glchartofaccountaccount;
            return Json(glchartofaccountaccount.Select(i => new { GlchartAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssignment()
        {
            List<string> list = new List<string>( ) { "ASSIGNED", "NOT_ASSIGNED" };

            return Json(list.Select(i => new { Name = i.ToString(), Value = i.ToString() }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranch()
        {
            var branches = db.Branch;
           

            return Json(branches.Select(i => new { Name = i.Name, Value=i.Name  }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubAccountCode()
        {
            var glchartofaccountaccount = db.GLChartOfAccountSubAccounts.AsQueryable();
            ViewData["SubAccCode"] = glchartofaccountaccount;
            return Json(glchartofaccountaccount.Select(i => new { GlchartSubAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompany()
        {
            var glCompany = db.GLChartOfAccountLocations.AsQueryable();
            return Json(glCompany.Select(i => new { Name = i.Values + "_" + i.Description, Value = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public void SetInitialDatas()
        {
            //var categoryList = db.PersonCategories;
          //  var identificationType = db.Lookups.Where(lup => lup.Category == "" && lup.LookupType == "");
            var nationalityList = db.Countries;
        
            var periodList = db.Periods;
           // var accountList = db.Accounts;
            var bankList = db.Banks;
            ViewData["BankList"] = bankList != null ? bankList.ToList() : new List<Bank >();

          

            ViewData["NationalityList"] = nationalityList != null ? nationalityList.ToList() : new List<Country>();

          

            
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<Period>();

         

        }
        public ActionResult Person_Employee_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            var person_employe = db.Person_Employee.GroupJoin(db.BankReferences,
                u => u.EmployeeId.ToString(),
                p => p.Reference,
                (u, p) =>
                    new { employee = u, bank = p.DefaultIfEmpty() })
                    .Select(a => new { EmployeeId = a.employee.EmployeeId,
                                        EmployeeCode = a.employee.EmployeeCode,
                                       AccountNumber= a.bank.FirstOrDefault(x => x.Reference == a.employee .EmployeeId.ToString( ) && x.Type == BankAccountType.PAYROLL).AccountNumber ,
                                       MothersFullName= a.employee .MothersFullName ,
                                       BirthPlace= a.employee .BirthPlace ,
                                       Gender= a.employee .Gender ,
                                       FirstNameEnglish=a.employee .FirstNameEnglish ,
                                       MiddleNameEnglish= a.employee .MiddleNameEnglish ,
                                       LastNameEnglish= a.employee .LastNameEnglish ,
                                       DateOfBirthAm= a.employee .DateOfBirthAm ,
                                       HiredDateAmharic= a.employee .HiredDateAmharic ,
                                       //Branch = a.bank.FirstOrDefault(x => x.Reference == a.employee.EmployeeId.ToString()).AccountNumber,

                    });
            var person_employee = person_employe.GroupJoin(db.Placements,
                     u => u.EmployeeId,
                     p => p.EmployeeId,
                     (u, p) =>
                         new { employee = u, bank = p.DefaultIfEmpty() })
                         .Select(a => new
                         {
                             EmployeeId = a.employee.EmployeeId,
                             EmployeeCode = a.employee.EmployeeCode,
                             AccountNumber = a.employee.AccountNumber,
                             MothersFullName = a.employee.MothersFullName,
                             BirthPlace = a.employee.BirthPlace,
                             Gender = a.employee.Gender,
                             FirstNameEnglish = a.employee.FirstNameEnglish,
                             MiddleNameEnglish = a.employee.MiddleNameEnglish,
                             LastNameEnglish = a.employee.LastNameEnglish,
                             DateOfBirthAm = a.employee.DateOfBirthAm,
                             HiredDateAmharic = a.employee.HiredDateAmharic,
                             Branch = a.bank.FirstOrDefault(x => x.EmployeeId == a.employee.EmployeeId && x.isCurrent ).BranchId,

                         });

            var filterd = person_employee.Where(x => employees.Contains(x.EmployeeId.ToString())).OrderBy(x => x.AccountNumber);
            DataSourceResult result =filterd .ToDataSourceResult(request, person_Employee => new
            {
                EmployeeId = person_Employee.EmployeeId,
                EmployeeCode = person_Employee.EmployeeCode,
                MothersFullName = person_Employee.MothersFullName,
                BirthPlace = person_Employee.BirthPlace,
                Gender = person_Employee.Gender,
                FirstNameEnglish = person_Employee.FirstNameEnglish,
                MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                LastNameEnglish = person_Employee.LastNameEnglish,
                DateOfBirthAm = person_Employee.DateOfBirthAm,
                HiredDateAmharic = person_Employee.HiredDateAmharic,
                AccountNumber = person_Employee.AccountNumber,
                Branch = person_Employee.Branch,
             

            });

            return Json(result);
        }
        private string GetBranchName(int id)
        {
            var placement = db.Placements.Where(br => br.isCurrent && br.EmployeeId == id).FirstOrDefault();
            if (placement != null)
            {
                var branch = db.Branch.Find(placement.BranchId);
                if(branch != null)
                {
                    return branch.Name;
                }
            }
            return "";
        }
        private int GetBranchId(int id)
        {
            var branch = db.Placements.Where(br => br.isCurrent && br.EmployeeId == id).FirstOrDefault();
            if (branch != null)
            {
                return branch.BranchId;
            }
            return 0;
        }
        private string GetAssignment(int id)
        {
            var bankreferences = db.BankReferences.Where(br => br.ReferenceType == "Employee" && br.Reference == id.ToString() && br.Type == BankAccountType.PAYROLL).FirstOrDefault();
            if (bankreferences != null)
            {
                return "ASSIGNED";
            }
            return "NOT_ASSIGNED";
        }
        private string GetAccountNumber(int id)
        {
            var bankreferences = db.BankReferences.Where(br => br.ReferenceType == "Employee" && br.Reference == id.ToString() && br.Type == BankAccountType.PAYROLL).FirstOrDefault();
            if (bankreferences != null)
            {
                return bankreferences.AccountNumber;
            }
            return "";
        }
        private int GetReferenceId(int id)
        {
            var bankreferences = db.BankReferences.Where(br => br.ReferenceType == "Employee" && br.Reference == id.ToString() && br.Type == BankAccountType.PAYROLL).FirstOrDefault();
            if (bankreferences != null)
            {
                return bankreferences.BankReferenceId;
            }
            return 0;
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Bank Account");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Bank Account");
            #endregion
            return File(fileContents, contentType, fileName);
        }
    }


}