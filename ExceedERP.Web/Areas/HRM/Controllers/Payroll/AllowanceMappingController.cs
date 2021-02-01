
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class AllowanceMappingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            ViewData["jobTitles"] = lookups;
            return View();
        }

        public ActionResult AllowanceMappings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AllowanceMapping> allowancedefinations = db.AllowanceMappings.Where (x=> x.AllowanceDefinationId ==id);
            DataSourceResult result = allowancedefinations.ToDataSourceResult(request, allowanceDefination => new {
                AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,               
                LookupId = allowanceDefination.LookupId,
                Remark = allowanceDefination.Remark,
                DateCreated = allowanceDefination.DateCreated,
                LastModified = allowanceDefination.LastModified,
                CreatedBy = allowanceDefination.CreatedBy,
                ModifiedBy = allowanceDefination.ModifiedBy,
                AllowanceMappingId = allowanceDefination.AllowanceMappingId,            
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Create([DataSourceRequest]DataSourceRequest request, AllowanceMapping allowanceDefination, int idTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var checkExistance = db.AllowanceMappings.Where(x => x.AllowanceDefinationId == idTwo && x.LookupId == allowanceDefination.LookupId).FirstOrDefault();
               if (checkExistance != null)
                {
                    ModelState.AddModelError("", "This mapping is already exist!!");
                }
               
                else
                {
                    var entity = new AllowanceMapping
                    {
                        AllowanceDefinationId = idTwo,                       
                        LookupId = allowanceDefination.LookupId,
                        DateCreated = DateTime.Now,
                        LastModified = allowanceDefination.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = allowanceDefination.ModifiedBy
                       
                    };

                    db.AllowanceMappings.Add(entity);
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success", User.Identity.Name, $"added allowance  {entity.AllowanceDefination.AllowanceName} and lookup {entity.Lookup.Description}" , Modules.Payroll);
                    #endregion
                    allowanceDefination.AllowanceMappingId = entity.AllowanceMappingId;
                    if (idTwo != 0)
                    {
                        List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
                        var placmemnts = db.Placements.Where(x => x.LookupId == allowanceDefination.LookupId && x.isCurrent && mybraches.Contains(x.BranchId.ToString())).ToList();
                        IQueryable<Person_Employee> person_employee = db.Person_Employee;
                        List<Person_Employee> employees = new List<Person_Employee>();
                        foreach (var emp in person_employee)
                        {
                            var check = placmemnts.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                            if (check != null)
                            {
                                employees.Add(emp);
                            }
                        }
                        foreach (var e in employees)
                        {
                            var entityy = new AllowancePerEmployee
                            {
                                EmployeeId = e.EmployeeId,
                                AllowanceDefinationId = idTwo,
                                Remark = allowanceDefination.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = allowanceDefination.LastModified,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = allowanceDefination.ModifiedBy,
                                From = null,
                                To = null,
                                Pend = false,
                                EmployeeCode = e.EmployeeCode,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                            };

                            db.AllowancePerEmployees.Add(entityy);
                            db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"added allowance  {entity.AllowanceDefination.AllowanceName} and employee {e.EmployeeCode}", Modules.Payroll);
                            #endregion

                        }
                    }
                }
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Update([DataSourceRequest]DataSourceRequest request, AllowanceMapping allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var checkExistance = db.AllowanceMappings.Where(x => x.AllowanceDefinationId == allowanceDefination.AllowanceDefinationId && x.LookupId == allowanceDefination.LookupId).FirstOrDefault();
                // if (checkExistance != null)
                // {
                //     ModelState.AddModelError("", "This mapping is already exist!!");
                // }

                // else
                // {
                     var entity = new AllowanceMapping
                         {
                             AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                             AllowanceMappingId = allowanceDefination.AllowanceMappingId,
                             Remark = allowanceDefination.Remark,
                             LookupId = allowanceDefination.LookupId,
                             DateCreated = allowanceDefination.DateCreated,
                             LastModified = DateTime.Now,
                             CreatedBy = allowanceDefination.CreatedBy,
                             ModifiedBy = User.Identity.Name,

                         };

                     db.AllowanceMappings.Attach(entity);
                     db.Entry(entity).State = EntityState.Modified;
                     db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success", User.Identity.Name, $"updated allowance  {entity.AllowanceDefination.AllowanceName} and lookup {entity.Lookup.Description}", Modules.Payroll);
                #endregion

                if (allowanceDefination.LookupId != 0)
                     {
                         List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
                         var placmemnts = db.Placements.Where(x => x.LookupId == allowanceDefination.LookupId && x.isCurrent && mybraches.Contains(x.BranchId.ToString())).ToList();
                         IQueryable<Person_Employee> person_employee = db.Person_Employee;
                         List<Person_Employee> employees = new List<Person_Employee>();
                         foreach (var emp in person_employee)
                         {
                             var check = placmemnts.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                             if (check != null)
                             {
                                 employees.Add(emp);
                             }
                         }
                         foreach (var e in employees)
                         {
                             var entityy = new AllowancePerEmployee
                             {
                                 EmployeeId = e.EmployeeId,
                                 AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                                 Remark = allowanceDefination.Remark,
                                 DateCreated = DateTime.Now,
                                 LastModified = allowanceDefination.LastModified,
                                 CreatedBy = User.Identity.Name,
                                 ModifiedBy = allowanceDefination.ModifiedBy,
                                 From = null,
                                 To = null,
                                 Pend = false,
                                 EmployeeCode = e.EmployeeCode,
                                 OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                             };

                             db.AllowancePerEmployees.Add(entityy);
                             db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"updated allowance  {entity.AllowanceDefination.AllowanceName} and employee {e.EmployeeCode}", Modules.Payroll);
                            #endregion

                        }
                }
                 //}
                
               
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Destroy([DataSourceRequest]DataSourceRequest request, AllowanceMapping allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var checkexistance = db.PayrollProcessAllowances.Where(x => x.AllowanceType == allowanceDefination.AllowanceType).FirstOrDefault();
                //if (checkexistance != null)
                //{
                //    ModelState.AddModelError("", "You can not delete this type of allowance is already processed!!");
                //}
                //else
                //{
                var entity = new AllowanceMapping
                    {
                        AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                        AllowanceMappingId = allowanceDefination.AllowanceMappingId,            
                                              Remark = allowanceDefination.Remark,
                        LookupId = allowanceDefination.LookupId,
                        DateCreated = allowanceDefination.DateCreated,
                        LastModified = allowanceDefination.LastModified,
                        CreatedBy = allowanceDefination.CreatedBy,
                        ModifiedBy = allowanceDefination.ModifiedBy,
                        
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted allowance mapping information of allowance  " + allowanceDefination.AllowanceDefinationId+"and "+allowanceDefination .LookupId,Modules.Payroll );
                    #endregion
                    db.AllowanceMappings.Attach(entity);
                    db.AllowanceMappings.Remove(entity);
                    db.SaveChanges();
                //}
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
