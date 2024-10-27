using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Procurements.DataSource;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize]
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

        public JsonResult GetAllUsers()
        {
            ApplicationDbContext appdb = new ApplicationDbContext();
            var unitOfm = appdb.Users.AsQueryable();
            return Json(unitOfm.Select(i => new
            {
                Name = i.UserName + "_" + i.FirstName + " " + i.MiddleName + " " + i.LastName,
                UserName=i.UserName,
                Code = i.Id
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserFullName(string id)
        {
            ApplicationDbContext appdb = new ApplicationDbContext();
            var user = appdb.Users.Where(u => u.UserName == id).FirstOrDefault();
            
            var formattedData = new
            {
                Name = user != null? user.FirstName+"-"+user.MiddleName+"-" +user.LastName : "Name not found",
                Status = user != null ? "OK" : "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetServiceRequisitionSecondApproverSelector()
        {
            return View();
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

        public JsonResult GetPlacedJobTitles()
        {
            var list = from l in db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE)
                       join p in db.Placements
                               on l.LookupId equals p.LookupId
                       where p.isCurrent
                       select new
                       {
                           l.LookupId,
                           l.Description
                       };
            var uniquelist = list.DistinctByField(x => x.LookupId);

            return Json(uniquelist.Select(p => new
            {
                Code = p.LookupId,
                Name = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentApprovalGroup(string id,string text)
        {
            var list = db.ApprovalGroups.AsQueryable();

            if (!string.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var record = db.AssignApprovalGroups.Find(did);

                if (GlobalHelper.IsMultipleBusinessUnit())
                {
                    if (record.BusinessUnit != null && record.BusinessUnit != 0)
                    {
                        list = list
                        .Where(x => x.BusinessUnit == record.BusinessUnit)
                        .AsQueryable();
                    }
                    else
                    {
                        list = list
                       .Where(x => x.BusinessUnit == null ||x.BusinessUnit==0)
                       .AsQueryable();
                    }
                }
                //branch
                if (record.OpertatingUnit != null && record.OpertatingUnit != 0)
                {
                    list = list
                    .Where(x => x.OpertatingUnit == record.OpertatingUnit)
                    .AsQueryable();
                }
                else
                {
                    list = list
                   .Where(x => x.OpertatingUnit == null || x.OpertatingUnit == 0)
                   .AsQueryable();
                }
            }

            if (!string.IsNullOrEmpty(text))
                list = list.Where(x => x.Name.Contains(text) || x.Description.Contains(text)).AsQueryable();

            return Json(list.Select(p => new
            {
                Code = p.ApprovalGroupId,
                Name = p.Name + "_" +p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProcurementPurchasersSelector()
        {
            return View();
        }

        public ActionResult GetNextApprover(string id, string t)
        {
            ViewBag.id = id;
            ViewBag.type = t;
            return View();
        }

        public JsonResult GetNextApproverList(int id,string type)
        {
            List<UserVM> list = new List<UserVM>();

            switch(type)
            {
                case "PO":
                    list = UserInformationHelper.GetUsersInRole(ProcurementRoles.ProcurementTenderApprover);
                    break;
                case "Proforma":
                    list = UserInformationHelper.GetUsersInRole(ProcurementRoles.ProcurementTenderApprover);
                    break;
                case "POChange":
                    list = UserInformationHelper.GetUsersInRole(ProcurementRoles.ProcurementTenderApprover);
                    break;
                case "PaymentRequestSelfCare":
                    list = UserInformationHelper.GetUsersInRole(HrmConstants.HRDepartmentHead);
                    break;
                case "PurchasePayment":
                    list = UserInformationHelper.GetUsersInRole(ProcurementRoles.ProcurementPaymentApprover);
                    break;
                case "TravelOrderSelfCare":
                    list = UserInformationHelper.GetUsersInRole(HrmConstants.HRDepartmentHead);
                    break;
                case "TravelExpenseClaimSelfCare":
                    list = UserInformationHelper.GetUsersInRole(HrmConstants.HRDepartmentHead);
                    break;
                case "FundTransferSelfCare":
                    list = UserInformationHelper.GetUsersInRole(HrmConstants.HRDepartmentHead);
                    break;
                default:
                    break;
            }
          
           
            return Json(list.Select(p => new
            {
                UserName = p.UserName,
                FullName = p.UserName+"- ("+p.FullName +")"
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateNextApprover(int id, string type,string user)
        {
            int empid = UserInformationHelper.GetUserEmployeeId(user);
            int branchId = DocApprovalHelper.GetEmployeeBranch(empid);
            decimal docamount = 0;
            switch (type)
            {
                case "PO":                   
                    var po = db.PurchaseOrders.Find(id);
                    var polines = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id);
                    docamount = polines.Any()? polines.Sum(x => x.Total):0;
                    po.ForwardedTo = user;
                    po.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user,ApprovalDocumentType.PurchaseOrder,docamount, branchId);
                    db.PurchaseOrders.Attach(po);
                    db.Entry(po).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "Proforma":
                    var proforma = db.ProformaPurchases.Find(id);
                    var awardIds = db.WinningSuppliers.Where(x => x.AwardGiven).Where(x => x.ProformaPurchaseID == id).Select(x => x.WinningSupplierID).ToList();
                    var awardItems = db.WinningSupplierItems.Where(x => awardIds.Contains(x.WinningSupplierID)).ToList();
             
                    docamount = awardItems.Any() ? awardItems.Sum(x => x.TPrice) : 0;
                    proforma.ForwardedTo = user;
                    proforma.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.ProformaPurchase, docamount, branchId);
                    db.ProformaPurchases.Attach(proforma);
                    db.Entry(proforma).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "POChange":
                    var pochange = db.PoChangeOforders.Find(id);
                    var pochangelines = db.PoChangeOfOrderItems.Where(x => x.PoChangeOforderId == id);
                    docamount = pochangelines.Any() ? pochangelines.Sum(x => x.Total) : 0;
                    pochange.ForwardedTo = user;
                    pochange.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.POChangeOrder, docamount, branchId);
                    db.PoChangeOforders.Attach(pochange);
                    db.Entry(pochange).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "PaymentRequestSelfCare":
                    var paymentRequest = db.PaymentRequests.Find(id);
                    var prlines = db.PaymentRequestLines.Where(x => x.PaymentRequestId == id);
                    docamount = prlines.Any() ? prlines.Sum(x => x.TotalAmount):0;
                    paymentRequest.ForwardedTo = user;
                    paymentRequest.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.ApPaymentRequestSelfCare, docamount, branchId);
                    db.PaymentRequests.Attach(paymentRequest);
                    db.Entry(paymentRequest).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "PurchasePayment":
                    var purchasePayment = db.PurchasePayments.Find(id);
                    docamount = purchasePayment.NetPayable;
                    purchasePayment.ForwardedTo = user;
                    purchasePayment.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.PurchasePaymentRequest, docamount, branchId);
                    db.PurchasePayments.Attach(purchasePayment);
                    db.Entry(purchasePayment).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "TravelOrderSelfCare":
                    var travelorder = db.ApTravelOrders.Find(id);
                    docamount = travelorder.AdvanceAmount;
                    travelorder.ForwardedTo = user;
                    travelorder.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.ApTravelOrderSelfCare, docamount, branchId);
                    db.ApTravelOrders.Attach(travelorder);
                    db.Entry(travelorder).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "TravelExpenseClaimSelfCare":
                    var claim = db.ApTravelExpenseClaims.Find(id);
                    var exclines = db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);
                    docamount = exclines.Any() ? exclines.Sum(x => x.Total) : 0;
                    claim.ForwardedTo = user;
                    claim.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.ApTravelExpenseClaimSelfCare, docamount, branchId);
                    db.ApTravelExpenseClaims.Attach(claim);
                    db.Entry(claim).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case "FundTransferSelfCare":
                    var fund = db.FundTransfers.Find(id);
                    var fundLines = db.FundTransferDetails.Where(x => x.FundTransferId == id);
                    docamount = fundLines.Any() ? fundLines.Sum(x => x.Approved) : 0;
                    fund.ForwardedTo = user;
                    fund.IsFinalApproval = DocApprovalWorkflowHelper.CanUserGiveFinalApproval(user, ApprovalDocumentType.ApFundTransferSelfCare, docamount, branchId);
                    db.FundTransfers.Attach(fund);
                    db.Entry(fund).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                default:
                    break;
            }
            
            var formattedData = new
            {
                Message = "Update successfully",
                Status ="OK" 
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetYears()
        {
            var years = db.GLFiscalYears.OrderByDescending(x => x.DateTo);
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
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId)).AsNoTracking().ToList();
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Code = x.BranchId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllEmployees(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
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