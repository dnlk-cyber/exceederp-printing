using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
   [Authorize(Roles = HR_Admin + "," + HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class EmployeAllowanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        public static DateTime? From { get; set; }
        public static DateTime? To { get; set; }
        public ActionResult Index()
        {
            var alloanceList = db.AllowanceDefinations.AsQueryable();
            ViewData["AllowanceList"] = alloanceList.OrderBy(o => o.AllowanceName);

            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive).Where(x => x.IsActive);
           
            return View();
        }
       [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin + "," + HR_Benefit_User)]
        public ActionResult AllowancePerEmployees_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<AllowancePerEmployee> allowanceperemployees = db.AllowancePerEmployees.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = allowanceperemployees.ToDataSourceResult(request, allowancePerEmployee => new {
                EmployeeId = allowancePerEmployee.EmployeeId,
                AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                AllowanceId = allowancePerEmployee.AllowanceId,
                Remark = allowancePerEmployee.Remark,
                DateCreated = allowancePerEmployee.DateCreated,
                LastModified = allowancePerEmployee.LastModified,
                CreatedBy = allowancePerEmployee.CreatedBy,
                ModifiedBy = allowancePerEmployee.ModifiedBy,
                EmployeeCode = allowancePerEmployee.EmployeeCode,
                OrgBranchName = allowancePerEmployee.OrgBranchName,
                From = allowancePerEmployee.From,
                To = allowancePerEmployee.To,
                Pend = allowancePerEmployee.Pend,
            });

            return Json(result);
        }
        public void PerapreEmployeeAllowance()
        {
            var mappings = db.AllowanceMappings;
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name).Where(x => x.IsActive);
            var placment = db.Placements.Where(x => x.isCurrent).ToList();

            foreach (var l in list)
            {
                db = new ExceedDbContext();
                
                foreach (var x in mappings)
                {

                    

                    var empJobtitle = placment.FirstOrDefault(y => y.EmployeeId == l.PersonId);
                    if (empJobtitle !=null)
                    {
                        if (x.LookupId == empJobtitle.LookupId)
                        {
                            var checkExistance = db.AllowancePerEmployees.FirstOrDefault(g => g.EmployeeId == l.PersonId && x.AllowanceDefinationId == g.AllowanceDefinationId);
                            if (checkExistance==null)
                            {
                                var entity = new AllowancePerEmployee
                                {
                                    EmployeeId = l.PersonId,
                                    AllowanceDefinationId = x.AllowanceDefinationId,
                                    Remark = null,
                                    DateCreated = DateTime.Now,
                                    LastModified = null,
                                    CreatedBy = User.Identity.Name,
                                    ModifiedBy = null,
                                    EmployeeCode = l.EmployeeCode,
                                    From = null,
                                    To = null,
                                    Pend = false,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                                };

                                db.AllowancePerEmployees.Add(entity);
                                db.SaveChanges();

                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"add allowance  {entity.AllowanceDefination.AllowanceName} to employee {entity.EmployeeCode} ", Modules.Payroll);
                                #endregion

                            }
                            else
                            {

                                if (checkExistance.To != null && checkExistance.To.Value.Date <= DateTime.Now.Date)
                                {
                                    if (checkExistance.Pend)
                                    {
                                        checkExistance.Pend  = false;
                                        checkExistance.From = null;
                                        checkExistance.To = null;
                                        db.AllowancePerEmployees.Add(checkExistance);
                                        //db.Entry(checkExistance).State = EntityState.Modified;
                                        db.SaveChanges();
                                        #region operation log
                                        UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"unpend  {checkExistance.AllowanceDefination.AllowanceName} to employee {checkExistance.EmployeeCode} ", Modules.Payroll);
                                        #endregion
                                    }


                                }

                               
                            }
                            
                        }
                       
                    }

                }
            }
            GetDelegeatedEmployee(mappings);
        }

        private void GetDelegeatedEmployee(DbSet<AllowanceMapping> mappings)
        {
            var assignedJobTitles = mappings.Select(x => x.LookupId).Distinct().ToList();
            var lookups = db.Lookups.Where( x=> x.LookupType == LookUpTypes.JOB_TITLE && !assignedJobTitles.Contains( x.LookupId )).ToList();

           
            var delegatedEmployees = from l in lookups
                                     join  m in db.Placements
                                     on l.LookupId  equals m.LookupId
                                     join e in db.Person_Employee 
                                     on m.EmployeeId equals e.EmployeeId
                join p in db.AllowancePerEmployees
                on e.EmployeeId equals p.EmployeeId
                                     join a in db.AllowanceDefinations
                on p.AllowanceDefinationId equals a.AllowanceDefinationId


                select (new
                {
                    a.AllowanceDefinationId,
                    p.EmployeeId , p.Pend ,p.To ,l.LookupId
                });
            var employees = delegatedEmployees.ToList();

            foreach (var e in employees)
            {
                var checkExistance =
                    db.AllowancePerEmployees.FirstOrDefault(x => x.AllowanceDefinationId == e.AllowanceDefinationId &&
                                                        e.EmployeeId == x.EmployeeId);
                if (checkExistance != null)
                {
                    if (e.To != null && e.To.Value.Date <= DateTime.Now.Date)
                    {
                        if (!e.Pend)
                        {
                            checkExistance.Pend = true;
                            db.AllowancePerEmployees.Attach(checkExistance);
                            db.Entry(checkExistance).State = EntityState.Modified;
                            db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"pend delegated allowance  {checkExistance.AllowanceDefination.AllowanceName} of employee {checkExistance.EmployeeCode} ", Modules.Payroll);
                            #endregion
                        }


                    }
                }
            }
           
        }

        [Authorize(Roles = HR_Benefit_User)]
       [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Create([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AllowancePerEmployee
                {
                   EmployeeId = allowancePerEmployee.EmployeeId,
                   AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                    Remark = allowancePerEmployee.Remark,
                   DateCreated = DateTime.Now,
                   LastModified = allowancePerEmployee.LastModified,
                   CreatedBy = User.Identity .Name ,
                   ModifiedBy = allowancePerEmployee.ModifiedBy,
                   EmployeeCode = inputs. GetEmployees().Where(x => x.PersonId == allowancePerEmployee.EmployeeId).FirstOrDefault().EmployeeCode,
                   OrgBranchName = UserInformationHelper .GetUserDefaultBranch (User.Identity .Name ),
                   From = allowancePerEmployee.From,
                   To = allowancePerEmployee.To,
                   Pend = allowancePerEmployee.Pend,
                };

                db.AllowancePerEmployees.Add(entity);
                db.SaveChanges();
                allowancePerEmployee.AllowanceId = entity.AllowanceId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"create allowance  {entity.AllowanceDefination.AllowanceName} of employee {entity.EmployeeCode} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult SetDate(string from, string to)
        {
            if (from != "")
            {
                From = Convert.ToDateTime(from);
            }
            if (to != "")
            {
                To = Convert.ToDateTime(to);
            }
            return null;
        }
        [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AllowancePerEmployee> allowanceesPerEmployees)
        {
            var results = new List<AllowancePerEmployee>();

        
            ExceedDbContext dbs = new ExceedDbContext();
            var allowancePerEmployees = allowanceesPerEmployees as IList<AllowancePerEmployee> ?? allowanceesPerEmployees.ToList();

            if (ModelState.IsValid)
            {

                foreach (var allowancePerEmployee in allowancePerEmployees)
                {
                    if(From !=null)
                    {
                        allowancePerEmployee.From = From;
                    }
                    if (To != null)
                    {
                        allowancePerEmployee.To = To;
                    }
                    dbs = new ExceedDbContext();
                    var list = dbs.AllowancePerEmployees.Where(x => x.AllowanceDefinationId == allowancePerEmployee.AllowanceDefinationId&& x.EmployeeId == allowancePerEmployee .EmployeeId );
                    if (list.Any())
                    {
                        dbs.AllowancePerEmployees.RemoveRange(list);
                        dbs.SaveChanges();
                    }
                    dbs = new ExceedDbContext();
                    var firstOrDefault = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == allowancePerEmployee.EmployeeId);
                    if (firstOrDefault != null)
                    {
                        var entity = new AllowancePerEmployee
                        {
                            EmployeeId = allowancePerEmployee.EmployeeId,
                            AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                            Remark = allowancePerEmployee.Remark,
                            DateCreated = allowancePerEmployee.DateCreated ,
                            LastModified = DateTime.Now,
                            CreatedBy = allowancePerEmployee.CreatedBy ,
                            ModifiedBy = User.Identity.Name,
                            EmployeeCode = firstOrDefault.EmployeeCode,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            From = allowancePerEmployee.From,
                            To = allowancePerEmployee.To,
                            Pend = allowancePerEmployee.Pend,
                        };

                        db.AllowancePerEmployees.Add(entity);
                        db.SaveChanges();
                        allowancePerEmployee.AllowanceId = entity.AllowanceId;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"updated allowance  {entity.AllowanceDefination.AllowanceName} of employee {entity.EmployeeCode} ", Modules.Payroll);
                        #endregion
                    }
                }

            }
          
            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Update([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AllowancePerEmployee
                {
                   EmployeeId = allowancePerEmployee.EmployeeId,
                   AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                    AllowanceId = allowancePerEmployee.AllowanceId,
                    Remark = allowancePerEmployee.Remark,
                   DateCreated = allowancePerEmployee.DateCreated,
                   LastModified = DateTime .Now ,
                   CreatedBy = allowancePerEmployee.CreatedBy,
                   ModifiedBy = User.Identity .Name ,
                   EmployeeCode = inputs. GetEmployees( ).Where(x => x.PersonId == allowancePerEmployee.EmployeeId).FirstOrDefault().EmployeeCode,
                   OrgBranchName = allowancePerEmployee.OrgBranchName,
                   From = allowancePerEmployee.From,
                   To = allowancePerEmployee.To,
                   Pend = allowancePerEmployee.Pend,

                };

                db.AllowancePerEmployees.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"updated allowance  {entity.AllowanceDefination.AllowanceName} of employee {entity.EmployeeCode} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Destroy([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == allowancePerEmployee.EmployeeId).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You can not delete this allowance is already processed!!");
                }
                else
                {
                    var entity = new AllowancePerEmployee
                    {
                        EmployeeId = allowancePerEmployee.EmployeeId,
                        AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                        AllowanceId = allowancePerEmployee.AllowanceId,
                        Remark = allowancePerEmployee.Remark,
                        DateCreated = allowancePerEmployee.DateCreated,
                        LastModified = allowancePerEmployee.LastModified,
                        CreatedBy = allowancePerEmployee.CreatedBy,
                        ModifiedBy = allowancePerEmployee.ModifiedBy,
                        EmployeeCode = allowancePerEmployee.EmployeeCode,
                        OrgBranchName = allowancePerEmployee.OrgBranchName,
                        From = allowancePerEmployee.From,
                        To = allowancePerEmployee.To,
                        Pend = allowancePerEmployee.Pend,
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee allowance information of employee " + allowancePerEmployee.EmployeeCode,Modules.Payroll);
                    #endregion
                    db.AllowancePerEmployees.Attach(entity);
                    db.AllowancePerEmployees.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Allowance");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Allowance");
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
