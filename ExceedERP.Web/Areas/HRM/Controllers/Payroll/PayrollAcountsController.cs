﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
 using ExceedERP.Core.Domain.Finance.GL.Dynamic;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
 using ExceedERP.Core.Domain.HRM.Payroll.Setting;
 using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin)]
    public class PayrollAcountsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        AccountHelper accountHelper = new AccountHelper();
        public ActionResult Index()
        {
         
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            SetPayRollAcounts();
            return View();
        }

        public void SetPayRollAcounts()
        {

            var accounts = db.PayrollAccounts.ToList();
            if (accounts.Count == 0)
            {
                PayrollAccounts payrollAccounts = new PayrollAccounts();
                List<PayrollAccounts> payrollAccountses = new List<PayrollAccounts>();

                var payrollAccountTypes = Enum.GetValues(typeof (PayrollAccountType))
                    .Cast<PayrollAccountType>()

                    .ToList();
                foreach (var p in payrollAccountTypes)
                {
                    payrollAccounts = new PayrollAccounts
                    {
                        Description = p.ToString(),
                        ReferenceType = "Normal",
                        IsPerEmployee = false,
                        IsPerBranch = false,
                        DateCreated = DateTime.Now,
                        LastModified = null,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = null
                    };
                    payrollAccountses.Add(payrollAccounts);
                }
                #region Contributions

                var contrubutions = db.ContributionTypes.ToList();
                if (contrubutions.Any())
                {
                    foreach (var p in contrubutions)
                    {
                        payrollAccounts = new PayrollAccounts();
                        payrollAccounts.Description = p.Description;
                        payrollAccounts.ReferenceType = "Cont";
                        payrollAccounts.DateCreated = DateTime .Now ;
                       payrollAccounts. LastModified = null;
                       payrollAccounts. CreatedBy = User.Identity .Name ;
                       payrollAccounts. ModifiedBy = null;
               var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
               if (checkTwo == null)
               {
                   payrollAccountses.Add(payrollAccounts);
               }
                    }
                }
            #endregion
                #region allowances
                var allowances = Enum.GetValues(typeof(AllowanceType))
                    .Cast<AllowanceType>()
                    .ToList();
                if (allowances.Any())
                {
                    foreach (var p in allowances)
                    {
                        var check = accounts.Where(x => x.Description == p.ToString()).FirstOrDefault();
                        if (check == null)
                        {
                            payrollAccounts = new PayrollAccounts();
                            payrollAccounts.Description = p.ToString();
                            payrollAccounts.ReferenceType = "Allw";
                            payrollAccounts.DateCreated = DateTime.Now;
                            payrollAccounts.LastModified = null;
                            payrollAccounts.CreatedBy = User.Identity.Name;
                            payrollAccounts.ModifiedBy = null;
                            var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
                            if (checkTwo == null)
                            {
                                payrollAccountses.Add(payrollAccounts);
                            }
                        }
                    }
                }
                db = new ExceedDbContext();
                db.PayrollAccounts.AddRange(payrollAccountses);
                db.SaveChanges();
                #endregion
            }
            else
            {
                PayrollAccounts payrollAccounts = new PayrollAccounts();
                List<PayrollAccounts> payrollAccountses = new List<PayrollAccounts>();
              
                var payrollAccountTypes = Enum.GetValues(typeof(PayrollAccountType))
                    .Cast<PayrollAccountType>()
                    .ToList();
                foreach (var p in payrollAccountTypes)
                {
                    var check = accounts.Where(x => x.Description == p.ToString()).FirstOrDefault();
                    if (check == null)
                    {
                        payrollAccounts = new PayrollAccounts();
                        payrollAccounts.Description = p.ToString();
                        payrollAccounts.IsPerEmployee = false;
                        payrollAccounts.IsPerBranch = false;
                        payrollAccounts.DateCreated = DateTime .Now ;
               payrollAccounts. LastModified = null;
               payrollAccounts. CreatedBy = User.Identity .Name;
               payrollAccounts. ModifiedBy = null;
               var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
               if (checkTwo == null)
               {
                   payrollAccountses.Add(payrollAccounts);
               }
                    }
                }
                #region Contribution
                var contrubutions = db.ContributionTypes.ToList();
                if (contrubutions.Any())
                {
                    foreach (var p in contrubutions)
                    {
                        var check = accounts.Where(x => x.Description == p.Description).FirstOrDefault();
                        if (check == null)
                        {
                            payrollAccounts = new PayrollAccounts();
                            payrollAccounts.Description = p.Description;
                            payrollAccounts.DateCreated = DateTime .Now ;
               payrollAccounts. LastModified = null;
               payrollAccounts. CreatedBy = User.Identity .Name ;
               payrollAccounts. ModifiedBy = null;
               var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
               if (checkTwo==null)
               {
                   payrollAccountses.Add(payrollAccounts);
               }
                        }
                    }
                }
                #endregion
                #region Allowance
                var allowances = Enum.GetValues(typeof(AllowanceType))
                    .Cast<AllowanceType>()
                    .ToList();
                if (allowances.Any())
                {
                    foreach (var p in allowances)
                    {
                        var check = accounts.Where(x => x.Description == p.ToString()).FirstOrDefault();
                        if (check == null)
                        {
                            payrollAccounts = new PayrollAccounts();
                            payrollAccounts.Description = p.ToString();
                            payrollAccounts.ReferenceType = "Allw";
                            payrollAccounts.DateCreated = DateTime .Now ;
               payrollAccounts. LastModified = null;
               payrollAccounts. CreatedBy = User.Identity .Name ;
               payrollAccounts.ModifiedBy = null;
               var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
               if (checkTwo == null)
               {
                   payrollAccountses.Add(payrollAccounts);
               }
                        }
                    }
                }
                #endregion


                db = new ExceedDbContext();
                db.PayrollAccounts.AddRange(payrollAccountses);
                db.SaveChanges();
                #region Penality
                var penalities = Enum.GetValues(typeof(PenalityType ))
                   .Cast<PenalityType>()
                   .ToList();
                if (penalities.Any())
                {
                    payrollAccountses = new List<PayrollAccounts>();
                    foreach (var p in penalities)
                    {
                        var check = accounts.Where(x => x.Description == p.ToString()).FirstOrDefault();
                        if (check == null)
                        {
                            payrollAccounts = new PayrollAccounts();
                            payrollAccounts.Description = p.ToString();
                            payrollAccounts.ReferenceType = "Penality";
                            payrollAccounts.DateCreated = DateTime.Now;
                            payrollAccounts.LastModified = null;
                            payrollAccounts.CreatedBy = User.Identity.Name;
                            payrollAccounts.ModifiedBy = null;
                            var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
                            if (checkTwo == null)
                            {
                                payrollAccountses.Add(payrollAccounts);
                            }
                        }
                    }
                }

               
                #endregion
                #region Addition
                var additionTypes = db.Lookups .Where (x=> x.LookupType == LookUpTypes.ADDITION_TYPE ).ToList();
                if (additionTypes.Any())
                {
                    foreach (var p in additionTypes)
                    {
                        var check = accounts.Where(x => x.Description == p.Description).FirstOrDefault();
                        if (check == null)
                        {
                            payrollAccounts = new PayrollAccounts();
                            payrollAccounts.Description = p.Description;
                            payrollAccounts.DateCreated = DateTime.Now;
                            payrollAccounts.LastModified = null;
                            payrollAccounts.CreatedBy = User.Identity.Name;
                            payrollAccounts.ModifiedBy = null;
                            var checkTwo = payrollAccountses.Where(x => x.Description == payrollAccounts.Description).FirstOrDefault();
                            if (checkTwo == null)
                            {
                                payrollAccountses.Add(payrollAccounts);
                            }
                        }
                    }
                }
                #endregion
                db = new ExceedDbContext();
                db.PayrollAccounts.AddRange(payrollAccountses);
                db.SaveChanges();
            }
        }

        public JsonResult GetCompany()
        {
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);

            return Json(locations.Select(i => new { Name = i.Values + "_" + i.Description, Value = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCostCenter()
        {
            if (accountHelper.IsDynamicCoA())
            {
                var glsegment = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glchartofaccountaccount = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glsegment.GLSegmentSetupId).AsQueryable();
                ViewData["CostAcc"] = glchartofaccountaccount;

                return Json(glchartofaccountaccount.Select(i => new
                {
                    GlchartAccunt = i.Values + "_" + i.Description,
                    GlValue = i.Values

                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var glchartofaccountaccount = db.GLChartOfAccountCostCenters.AsQueryable();
                //ViewData["CostAcc"] = glchartofaccountaccount;

                //return Json(glchartofaccountaccount.Select(i => new
                //{
                //    GlchartAccunt = i.Values + "_" + i.Description,
                //    GlValue = i.Values

                //}), JsonRequestBehavior.AllowGet);
                return null;
            }
            
        }
        public JsonResult GetAccountCode()
        {
            var glchartofaccountaccount = db.GLChartOfAccountAccounts.AsQueryable();
            ViewData["AccCode"] = glchartofaccountaccount;
            return Json(glchartofaccountaccount.Select(i => new { GlchartAccunt = i.Values + "_" + i.Description, GlValue = i.Values }),
                 JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubAccountCode()
        {
            if (accountHelper.IsDynamicCoA())
            {
                var glsegment = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                var glchartofaccountaccount = db.GlChartOfAccountSegments.Where(x=>x.GLSegmentSetupId==glsegment.GLSegmentSetupId).AsQueryable();
                ViewData["SubAccCode"] = glchartofaccountaccount;
                return Json(glchartofaccountaccount.Select(i => new
                {
                    GlchartSubAccunt = i.Values + "_" + i.Description,
                    GlValue = i.Values
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var glchartofaccountaccount = db.GLChartOfAccountSubAccounts.AsQueryable();
                //ViewData["SubAccCode"] = glchartofaccountaccount;
                //return Json(glchartofaccountaccount.Select(i => new
                //{
                //    GlchartSubAccunt = i.Values + "_" + i.Description,
                //    GlValue = i.Values
                //}), JsonRequestBehavior.AllowGet);
                return null;
            }

           

        }
        public ActionResult PayrollAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<PayrollAccounts> payrollaccounts = db.PayrollAccounts;
            // payrollaccounts=   SetAccountTypesValue(payrollaccounts);
            //.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = payrollaccounts.ToDataSourceResult(request, payrollAccounts => new {
                PayrollAccountsId = payrollAccounts.PayrollAccountsId,
                Description = payrollAccounts.Description,
                IsPerCostCenter = payrollAccounts.IsPerCostCenter,
                IsPerBranch = payrollAccounts.IsPerBranch,
                IsPerEmployee = payrollAccounts.IsPerEmployee,
                EmployeeId = payrollAccounts.EmployeeId,
                Account = payrollAccounts.Account,
                MainAccount = payrollAccounts.MainAccount,
                SubAccount = payrollAccounts.SubAccount,
                BranchAccount = payrollAccounts.BranchAccount,
                CostCenter = payrollAccounts.CostCenter,
                ReferenceType = payrollAccounts.ReferenceType,
                Remark = payrollAccounts.Remark,
                DateCreated = payrollAccounts.DateCreated,
                LastModified = payrollAccounts.LastModified,
                CreatedBy = payrollAccounts.CreatedBy,
                ModifiedBy = payrollAccounts.ModifiedBy
            });
            
            return Json(result);
        }

        public IQueryable<PayrollAccounts> SetAccountTypesValue(IQueryable<PayrollAccounts> payrollaccounts)
        {
            List<PayrollAccounts> allAccounts = new List<PayrollAccounts>();
            foreach (var p in payrollaccounts)
            {
                db = new ExceedDbContext();
                if (p.Account != null)
                {
                    if (p.MainAccount == null)
                    {
                        p.MainAccount = p.Account.Split('-')[0];
                    }
                    if (p.BranchAccount == null)
                    {
                        p.BranchAccount = p.Account.Split('-')[1];
                    }
                    if (p.CostCenter == null)
                    {
                        p.CostCenter = p.Account.Split('-')[2];
                    }
                    if (p.SubAccount == null)
                    {
                        p.SubAccount = p.Account.Split('-')[3];
                    }
                    db.PayrollAccounts.Attach(p);
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();

                }
                //allAccounts.Add(p);
            }
            return db.PayrollAccounts;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollAccounts_Create([DataSourceRequest]DataSourceRequest request, PayrollAccounts payrollAccounts)
        {
            AccountHelper myAccountHelper = new AccountHelper();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (payrollAccounts.Account == null)
                {
                    ModelState.AddModelError("","Please set a value for account.");
                }
                else
                {
                var entity = new PayrollAccounts
                {
                    Description = payrollAccounts.Description,
                    IsPerEmployee = payrollAccounts.IsPerEmployee,
                    IsPerCostCenter = payrollAccounts.IsPerCostCenter,
                    IsPerBranch = payrollAccounts.IsPerBranch,
                    EmployeeId = payrollAccounts.EmployeeId,
                    Account = payrollAccounts.Account,
                    ReferenceType = payrollAccounts.ReferenceType,
                    Remark = payrollAccounts.Remark,
                    DateCreated = DateTime .Now ,
                    LastModified = payrollAccounts.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = payrollAccounts.ModifiedBy,
                    MainAccount = accountHelper.GetNaturalAccountSegment(payrollAccounts.Account),
                    SubAccount = accountHelper.GetSubAccountSegment(payrollAccounts.Account),
                    BranchAccount = accountHelper.GetBranchSegment(payrollAccounts.Account),
                    CostCenter = accountHelper.GetCostCenterSegment(payrollAccounts.Account),
                };
                    entity.Account = payrollAccounts.Account;
                db.PayrollAccounts.Add(entity);
                db.SaveChanges();
                payrollAccounts.PayrollAccountsId = entity.PayrollAccountsId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll accounts", "Success", User.Identity.Name, $"added payroll account for employee  {entity.EmployeeId} description {entity.Description} account {entity.Account}", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { payrollAccounts }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollAccounts_Update([DataSourceRequest]DataSourceRequest request, PayrollAccounts payrollAccounts)
        {
            ModelState.Clear();
            AccountHelper myAccountHelper = new AccountHelper();
            if (ModelState.IsValid)
            {
                if (payrollAccounts.Account == null)
                {
                    ModelState.AddModelError("","Please set a value for account.");
                }
                else
                {
                    var entity = new PayrollAccounts
                    {
                        PayrollAccountsId = payrollAccounts.PayrollAccountsId,
                        Description = payrollAccounts.Description,
                        IsPerEmployee = payrollAccounts.IsPerEmployee,
                        IsPerCostCenter = payrollAccounts.IsPerCostCenter,
                        IsPerBranch = payrollAccounts.IsPerBranch,
                        EmployeeId = payrollAccounts.EmployeeId,
                        Account = payrollAccounts.Account,
                        ReferenceType = payrollAccounts.ReferenceType,
                        Remark = payrollAccounts.Remark,
                        DateCreated = payrollAccounts.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = payrollAccounts.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        MainAccount =accountHelper.GetNaturalAccountSegment(payrollAccounts.Account),
                        SubAccount = accountHelper.GetSubAccountSegment(payrollAccounts.Account),
                        BranchAccount = accountHelper.GetBranchSegment(payrollAccounts.Account),
                        CostCenter = accountHelper.GetCostCenterSegment(payrollAccounts.Account),
                    };
                    entity.Account = payrollAccounts.Account;
                    db.PayrollAccounts.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll accounts", "Success", User.Identity.Name, $"updated payroll account for employee  {entity.EmployeeId} description {entity.Description} account {entity.Account}", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { payrollAccounts }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollAccounts_Destroy([DataSourceRequest]DataSourceRequest request, PayrollAccounts payrollAccounts)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PayrollAccounts
                {
                    PayrollAccountsId = payrollAccounts.PayrollAccountsId,
                    Description = payrollAccounts.Description,
                    IsPerEmployee = payrollAccounts.IsPerEmployee,
                    IsPerBranch = payrollAccounts.IsPerBranch,
                    EmployeeId = payrollAccounts.EmployeeId,
                    IsPerCostCenter = payrollAccounts.IsPerCostCenter,
                    Account = payrollAccounts.Account,
                    ReferenceType = payrollAccounts.ReferenceType,
                    Remark = payrollAccounts.Remark,
                    DateCreated = payrollAccounts.DateCreated,
                    LastModified = payrollAccounts.LastModified,
                    CreatedBy = payrollAccounts.CreatedBy,
                    ModifiedBy = payrollAccounts.ModifiedBy,
                    MainAccount = payrollAccounts.MainAccount,
                    SubAccount = payrollAccounts.SubAccount,
                    BranchAccount = payrollAccounts.BranchAccount,
                    CostCenter = payrollAccounts.CostCenter,
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payroll accounts information of account " + payrollAccounts.Account,Modules.Payroll);
                #endregion
                db.PayrollAccounts.Attach(entity);
                db.PayrollAccounts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { payrollAccounts }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Payroll Acount");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Payroll Acount");
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
