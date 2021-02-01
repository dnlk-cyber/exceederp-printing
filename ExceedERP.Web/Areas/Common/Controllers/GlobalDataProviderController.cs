using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    public class GlobalDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext userdb = new ApplicationDbContext();


        public JsonResult GetUoM()
        {
            var unitOfm = db.UoMs.AsQueryable();
            return Json(unitOfm.Select(i => new
                {
                    Name = i.UoMName,
                    Code = i.UoMName
                })
                , JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsers()
        {
            ApplicationDbContext appdb = new ApplicationDbContext();
            var unitOfm = appdb.Users.AsQueryable();
            return Json(unitOfm.Select(i => new
                {
                    Name = i.UserName,
                    Code = i.Id
                }),
                JsonRequestBehavior.AllowGet);
        }
      

        public JsonResult GetServiceRequisitionSecondApprovers()
        {
            List<UserVM> list = UserInformationHelper.GetUsersInRole("Service_Requisition_Second_Approver");
            return Json(list.Select(p => new
            {
                UserName = p.UserName
            }), JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetAllJobTitles()
        {
            var list = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            return Json(list.Select(p => new
            {
                Code = p.LookupId,
                Name=p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentApprovalGroup(string operatingUnit,string text)
        {
            var list = db.ApprovalGroups.ToList();
            if (!string.IsNullOrEmpty(operatingUnit))
                list = list.Where(x => x.OpertatingUnit.ToString() == operatingUnit).ToList();

            if (!string.IsNullOrEmpty(text))
                list = list.Where(x => x.Name.Contains(text) ||x.Description.Contains(text)).ToList();

            return Json(list.Select(p => new
            {
                Code = p.ApprovalGroupId,
                Name = p.Description+"_" + db.Branch.Find(p.OpertatingUnit)?.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProcurementPurchasersSelector()
        {
            return View();
        }

        public JsonResult GetProcurementCheckers()
        {
            List<UserVM> list = new List<UserVM>();

            var purchasers = db.Purchasers.ToList();
            foreach (var purchaser in purchasers)
            {
                string username = UserInformationHelper.GetEmployeeUserId(purchaser.EmployeeId);
                if(!String.IsNullOrWhiteSpace(username))
                {
                    UserVM user = new UserVM
                    {
                        UserId = username,
                        UserName = username
                    };
                    list.Add(user);
                }
            }
            return Json(list.Select(p => new
            {
                UserName = p.UserName
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFixedAssetSecondApproverSelector()
        {
            return View();
        }
        public JsonResult GetFixedAssetSecondApprovers()
        {
            List<UserVM> list = UserInformationHelper.GetUsersInRole("FixedAsset_Requisition_Second_Approver");

            return Json(list.Select(p => new
            {
                UserName = p.UserName,
                FullName = p.FullName
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInventorySecondApproverSelector()
        {
            return View();
        }

        public JsonResult GetInventorySecondApprovers()
        {
            List<UserVM> list = UserInformationHelper.GetUsersInRole("Property_Requisition_Second_Approver");

            return Json(list.Select(p => new
            {
                UserName = p.UserName,
                FullName = p.FullName
            }), JsonRequestBehavior.AllowGet);
        }

       

        public ActionResult GetServiceRequisitionSecondApproverSelector()
        {
            return View();
        }
        public JsonResult GetCountry()
        {
            var countries = db.GLCountryTypes.AsQueryable();
            return Json(countries.Select(p => new
            {
                CountryName = p.CountryName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUoMs()
        {
            var countries = db.UoMs.AsQueryable();
            return Json(countries.Select(p => new
            {
                Name = p.UoMName,
                Code = p.UoMName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesByBranch(string branchid)
        {
            var employee = db.Person_Employee;
            return
                Json(
                    employee.Select(
                        i => new { Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish, Code = i.EmployeeId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmployeesEnglish()
        {
            var employee = db.Person_Employee;
            return
                Json(
                    employee.Select(
                        i => new
                        {
                            Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                            Code = i.EmployeeId
                        }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeCustomers()
        {
            var customerProfiles = db.ARCustomerProfiles;
            return Json(customerProfiles.Select(x => new
            {
                Name = x.CompanyName,
                CustomerId = x.CustomerId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYearsOpenYearSinceBeginning()
        {
            var begginning = db.GlBeginningPeriods.ToList();

            if (begginning.Any() && begginning.Count == 1)
            {
                var firstYearId = begginning.FirstOrDefault()?.GlFiscalYearId;

                var firstYear = db.GLFiscalYears.Find(firstYearId);

                if (firstYear != null)
                {
                    var years = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open);
                    years = years.Where(x => x.DateFrom >= firstYear.DateFrom).OrderByDescending(x => x.DateTo);
                    return Json(years.Select(x => new
                    {
                        Name = x.Name,
                        GlFiscalYearId = x.GlFiscalYearId
                    }), JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            return null;
        
        }

        public JsonResult GetPreviosYears()
        {
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            int currentYearId = fiscalYearHelper.GetCurrentFYearId();
            var currentYear = db.GLFiscalYears.Find(currentYearId);
            var years = db.GLFiscalYears.Where(x=>x.DateTo<currentYear.DateFrom).OrderByDescending(x => x.DateTo);
            return Json(years.Select(x => new
            {
                Name = x.Name,
                GlFiscalYearId = x.GlFiscalYearId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenYears()
        {
            var years = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open).OrderByDescending(x => x.DateTo);
            return Json(years.Select(x => new
            {
                Name = x.Name,
                GlFiscalYearId = x.GlFiscalYearId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriods(string year)
        {
            int did = int.Parse(year);
            var periods = db.GLPeriods.OrderByDescending(x=>x.DateTo).Where(x => x.GlFiscalYearId == did);
            return Json(periods.Select(x => new
            {
                Name = x.Name,
                GLPeriodId = x.GLPeriodId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOpenPeriods(string year)
        {
            int did = int.Parse(year);
            var periods = db.GLPeriods.Where(x=>x.Status==FiscalYearStatus.Open).OrderByDescending(x => x.DateTo).Where(x => x.GlFiscalYearId == did);
            return Json(periods.Select(x => new
            {
                Name = x.Name,
                GLPeriodId = x.GLPeriodId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirstPeriods(string year)
        {
            int did = int.Parse(year);

            var begPeriod = db.GlBeginningPeriods.FirstOrDefault();
            var period = db.GLPeriods.Find(begPeriod.GLPeriodId);

            var periods = db.GLPeriods
                .Where(x => x.Index == 1)//beginning period
                .Where(x=>x.DateFrom>= period.DateFrom)
                .Where(x => x.GlFiscalYearId == did);
            return Json(periods.Select(x => new
            {
                Name = x.Name,
                GLPeriodId = x.GLPeriodId
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAllYearsOpenYearSinceBeginning()
        {
            var begginning = db.GlBeginningPeriods.ToList();

            if (begginning.Any() && begginning.Count == 1)
            {
                var firstYearId = begginning.FirstOrDefault()?.GlFiscalYearId;

                var firstYear = db.GLFiscalYears.Find(firstYearId);

                if (firstYear != null)
                {
                    var years = db.GLFiscalYears.Where(x => x.DateFrom >= firstYear.DateFrom).OrderByDescending(x => x.DateTo);
                    return Json(years.Select(x => new
                    {
                        Name = x.Name,
                        GlFiscalYearId = x.GlFiscalYearId
                    }), JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            return null;

        }
        public JsonResult GetBranches()
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId.ToString())).ToList();
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Code = x.BranchId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBranches()
        {

            var branches = db.Branch;
            return Json(branches.Select(x => new
            {
                Name = x.Code+"-"+x.Name,
                Code = x.BranchId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBusinessUnits()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits;
                foreach(var bu in businessUnits)
                {
                    var record = new SelectorVm();
                    record.Name = bu.Name;
                    record.Code = bu.BusinessUnitId.ToString();
                    list.Add(record);
                }
            }
            else
            {
                var record = new SelectorVm();
                record.Name = "No Business Unit";
                record.Code = "0";
                list.Add(record);
            }

            
            return Json(list.Select(x => new
            {
                Name = x.Name,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountries()
        {

            var branches = db.Countries.AsQueryable();
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Code = x.CountryId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrency()
        {

            var branches = db.FinSetupCurrencies.Where(x=>x.Enable).AsQueryable();
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurentUser()
        {
                var formattedData = new
                {
                    User =User.Identity.Name
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            
        }
    }
}