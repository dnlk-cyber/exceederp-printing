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

namespace ExceedERP.Web.Areas.HRM.Controllers.Severance
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRAttendanceUser)]
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
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
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
        public ActionResult GenerateSeverancePayable(int id)
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
                                    TaxWithheldIncomeTax = (decimal)XAPI.IncomeTax.GetIncomeTax((float)placement.Salary),
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
