using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class CommitteeMemberController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        HRMInputs inputs = new HRMInputs();

        // GET: HRM/CommitteeMember
        public ActionResult Index()
        {
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = db.Person_Employee.ToList();//inputs.GetEmployees(User.Identity.Name);
            var commitee = db.Committees.ToList();
            ViewData["committee"] = commitee;
        }


        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult CommitteeMember_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            //IQueryable<CommitteeMember> committeeMember = db.CommitteeMembers.Where(x => employees.Contains(x.EmployeeId.ToString())); 
            //IQueryable<CommitteeMember> committeeMember=db.CommitteeMembers.AsQueryable();
            IQueryable<CommitteeMember> committeeMember = db.CommitteeMembers.Where(x => x.CommitteeId == id);

            DataSourceResult result = committeeMember.ToDataSourceResult(request, committeeMembers => new
            {
                CommitteeMemberId = committeeMembers.CommitteeMemberId,
                CommitteeId = committeeMembers.CommitteeId,
                EmployeeId = committeeMembers.EmployeeId,
                Role = committeeMembers.Role,
                IsActive = committeeMembers.IsActive,
                Remark = committeeMembers.Remark,
                //Date = DateTime.Now,
                CreatedBy = committeeMembers.CreatedBy,
                //DateAmharic = committeeMembers.DateAmharic
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteeMember_Create([DataSourceRequest]DataSourceRequest request, CommitteeMember committeeMember, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var members = db.CommitteeMembers.FirstOrDefault(x => x.CommitteeId == id && x.EmployeeId == committeeMember.EmployeeId);
                if (members != null)
                {
                    ModelState.AddModelError("", "you are already assigned to other role");
                }
                else
                {
                    var entity = new CommitteeMember
                    {
                        CommitteeId = id,
                        EmployeeId = committeeMember.EmployeeId,
                        Role = committeeMember.Role,
                        IsActive = committeeMember.IsActive,
                        Remark = committeeMember.Remark,
                       // Date = DateTime.Now,
                        CreatedBy = committeeMember.CreatedBy,
                        //DateAmharic = committeeMember.DateAmharic
                    };

                    db.CommitteeMembers.Add(entity);
                    db.SaveChanges();
                    committeeMember.CommitteeMemberId = entity.CommitteeMemberId;

                    #region operation log

                    var commitee = db.Committees.FirstOrDefault(x=>x.CommitteeId == entity.CommitteeId);
                    if(commitee != null)
                    {
                       
                                UserHelper.OperationLog(Request.UserHostAddress, "Committee Member ", "Success", User.Identity.Name, User.Identity.Name + " Create Committee Member for "  + " added Employee Name is "+ User.Identity.Name,Modules.HRM);

                            

                    }

                    #endregion
                }
            }

            return Json(new[] { committeeMember }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteeMember_Update([DataSourceRequest]DataSourceRequest request, CommitteeMember committeeMember)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new CommitteeMember
                {
                    CommitteeMemberId = committeeMember.CommitteeMemberId,
                    CommitteeId = committeeMember.CommitteeId,
                    EmployeeId = committeeMember.EmployeeId,
                    Role = committeeMember.Role,
                    IsActive = committeeMember.IsActive,
                    Remark = committeeMember.Remark,
                    //Date = DateTime.Now,
                    CreatedBy = committeeMember.CreatedBy,
                    // DateAmharic = committeeMember.DateAmharic
                };

                db.CommitteeMembers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var commitee = db.Committees.FirstOrDefault(x => x.CommitteeId == entity.CommitteeId);
                if (commitee != null)
                {
                    
                            UserHelper.OperationLog(Request.UserHostAddress, "Committee Member ", "Success", User.Identity.Name, User.Identity.Name + " Updated Committee Member Info of " + " added Employee Name is " + User.Identity.Name,Modules.HRM);

                       
                }

                #endregion
            }

            return Json(new[] { committeeMember }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteeMember_Destroy([DataSourceRequest]DataSourceRequest request, CommitteeMember committeeMember)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new CommitteeMember
                {
                    CommitteeMemberId = committeeMember.CommitteeMemberId,
                    CommitteeId = committeeMember.CommitteeId,
                    EmployeeId = committeeMember.EmployeeId,
                    Role = committeeMember.Role,
                    IsActive = committeeMember.IsActive,
                    Remark = committeeMember.Remark,
                    // Date = committeeMember.Date,
                    CreatedBy = committeeMember.CreatedBy,
                    //DateAmharic = committeeMember.DateAmharic
                };
                #region delete log
                var name = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == committeeMember.EmployeeId).FullNameaEnglish;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Commitee Member   " + name);
                #endregion
                db.CommitteeMembers.Attach(entity);
                db.CommitteeMembers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { committeeMember }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetCommittee()
        {


            var list = db.VaccancyPlans.ToList();


            return Json(list.Select(p => new { VaccancyCode = p.VaccancyCode }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


