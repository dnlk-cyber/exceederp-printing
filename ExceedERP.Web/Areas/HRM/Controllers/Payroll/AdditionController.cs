////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Areas\HRM\Controllers\AdditionController.cs
//
// summary:	Implements the addition controller class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = Payroll_Input+","+Payroll_Admin)]
    public class AdditionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        List<string> mybraches = new List<string>();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            SetValues();
           mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            return View();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.PayrollPeriods.Where( x=> x.Status==FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close );
            var list = from p in periodList
                join g in db.GLPeriods
                    on p.Name equals g.Name
                select (new
                {
                    g
                });
            ViewData["PeriodList"] = list.Select(x=> x.g).ToList(); //!= null ? list.ToList() : new List<PayrollPeriod>();

            return Json(list.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the values. </summary>
        ///
        /// <remarks>   Mekanehiwot, 11/11/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetValues()
        {
            GetPeriod();
            var addtions = db.Lookups.Where(l => l.LookupType ==LookUpTypes.ADDITION_TYPE).ToList();
            ViewData["AdditionTypeList"] = addtions;
          
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }

        public ActionResult Additions_Read([DataSourceRequest]DataSourceRequest request)
        {
          List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
                //UserInformationHelper.GetUserBranchs(User.Identity.Name);
          IQueryable<Addition> additions = db.Additions.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = additions.ToDataSourceResult(request, addition => new {
                AdditionId = addition.AdditionId,
                EmployeeId = addition.EmployeeId,
                GLPeriodId = addition.GLPeriodId,
                Amount = addition.Amount,
                AdditionTaxType = addition.AdditionTaxType,
                LookupId = addition.LookupId, DateCreated = addition.DateCreated,
                LastModified = addition.LastModified,
                CreatedBy = addition.CreatedBy,
                OrgBranchName = addition.OrgBranchName,
                ModifiedBy = addition.ModifiedBy,
               

                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Additions_Create([DataSourceRequest]DataSourceRequest request, Addition addition)
        {
            
            if (ModelState.IsValid)
            {
                addition .OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == addition.GLPeriodId && addition.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistancep != null)
                {

                    ModelState.AddModelError("", "You cannot add this addition. This period is already processed!!");
                }
                else
                {
                    var checkexistance = db.Additions.Where(x => x.EmployeeId == addition.EmployeeId && x.GLPeriodId == addition.GLPeriodId && x.LookupId == addition.LookupId).FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "This addition type on this period is already registered!!");
                    }
                    else
                    {
                        if (addition.Amount <= 0)
                        {
                            ModelState.AddModelError("", "The Amount should be greater than 0!!");
                        }
                        else
                        {
                            var entity = new Addition
                            {
                                EmployeeId = addition.EmployeeId,
                                GLPeriodId = addition.GLPeriodId,
                                Amount = addition.Amount,
                                LookupId = addition.LookupId,
                                AdditionTaxType = addition.AdditionTaxType,
                                DateCreated = DateTime.Now,
                                LastModified = addition.LastModified,
                                CreatedBy = User.Identity.Name,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                ModifiedBy = addition.ModifiedBy
                            };

                            db.Additions.Add(entity);
                            db.SaveChanges();
                            addition.AdditionId = entity.AdditionId;
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"added addition for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.AdditionId} amount {entity.Amount} ", Modules.Payroll);
                            #endregion
                        }
                    }
                }
            }

            return Json(new[] { addition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Additions_Update([DataSourceRequest]DataSourceRequest request, Addition addition)
        {
            if (ModelState.IsValid)
            {

                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == addition.EmployeeId && x.GLPeriodId == addition.GLPeriodId && addition.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    var checkaddition = db.PayrollProcessAdditions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.AdditionId == addition.AdditionId).FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "You cannot update this data . It already processed!!");
                    }
                    else
                    {
                        if (addition.Amount <= 0)
                        {
                            ModelState.AddModelError("", "The Amount should be greater than 0!!");
                        }
                        else
                        {
                            var entity = new Addition
                            {
                                EmployeeId = addition.EmployeeId,
                                AdditionId = addition.AdditionId,
                                GLPeriodId = addition.GLPeriodId,
                                Amount = addition.Amount,
                                LookupId = addition.LookupId,
                                AdditionTaxType = addition.AdditionTaxType,
                                DateCreated = addition.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = addition.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                OrgBranchName = addition.OrgBranchName,

                            };

                            db.Additions.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"edited addition for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.AdditionId} amount {entity.Amount} ", Modules.Payroll);
                            #endregion
                        }
                    }
                }
                else
                {
                    if (addition.Amount <= 0)
                    {
                        ModelState.AddModelError("", "The Amount should be greater than 0!!");
                    }
                    else
                    {
                        var entity = new Addition
                        {
                            EmployeeId = addition.EmployeeId,
                            AdditionId = addition.AdditionId,
                            GLPeriodId = addition.GLPeriodId,
                            Amount = addition.Amount,
                            LookupId = addition.LookupId,
                            AdditionTaxType = addition.AdditionTaxType,
                            DateCreated = addition.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = addition.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = addition.OrgBranchName,


                        };

                        db.Additions.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"edited addition for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.AdditionId} amount {entity.Amount} ", Modules.Payroll);
                        #endregion
                    }
                }
            }

            return Json(new[] { addition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Additions_Destroy([DataSourceRequest]DataSourceRequest request, Addition addition)
        {
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == addition.EmployeeId && x.GLPeriodId == addition.GLPeriodId && addition.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    var checkaddition = db.PayrollProcessAdditions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.AdditionId == addition.AdditionId).FirstOrDefault();
                    if (checkaddition != null)
                    {
                        ModelState.AddModelError("", "You cannot delete this data . It already processed!!");
                    }
                    else
                    {
                        var entity = new Addition
                        {
                            EmployeeId = addition.EmployeeId,
                            AdditionId = addition.AdditionId,
                            GLPeriodId = addition.GLPeriodId,
                            Amount = addition.Amount,
                            LookupId = addition.LookupId,
                            AdditionTaxType = addition.AdditionTaxType,
                            DateCreated = addition.DateCreated,
                            LastModified = addition.LastModified,
                            CreatedBy = addition.CreatedBy,
                            ModifiedBy = addition.ModifiedBy,
                            OrgBranchName = addition.OrgBranchName,

                        };
                      
                        db.Additions.Attach(entity);
                        db.Additions.Remove(entity);
                        db.SaveChanges();

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"deleted addition for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.AdditionId} amount {entity.Amount} ", Modules.Payroll);
                        #endregion
                    }
                }
                else
                {
                    var entity = new Addition
                    {
                        EmployeeId = addition.EmployeeId,
                        AdditionId = addition.AdditionId,
                        GLPeriodId = addition.GLPeriodId,
                        Amount = addition.Amount,
                        LookupId = addition.LookupId,
                        AdditionTaxType = addition.AdditionTaxType,
                        DateCreated = addition.DateCreated,
                        LastModified = addition.LastModified,
                        CreatedBy = addition.CreatedBy,
                        ModifiedBy = addition.ModifiedBy,
                        OrgBranchName = addition.OrgBranchName,

                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Addition  information employee " + addition.Employee.EmployeeCode + " , addition type" + addition.Lookup.Description + " and period " + addition.GLPeriodId);
                    #endregion
                    db.Additions.Attach(entity);
                    db.Additions.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { addition }.ToDataSourceResult(request, ModelState));
        }
     
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Additions");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Additions");
            #endregion
            return File(fileContents, contentType, fileName);
        }
    }
}
