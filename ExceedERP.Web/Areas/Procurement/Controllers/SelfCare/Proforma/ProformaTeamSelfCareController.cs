using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class ProformaTeamSelfCareController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            return View();
        }

        public ActionResult ProformaTeams_Read([DataSourceRequest] DataSourceRequest request)
        {
            int userEmpId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);

            var employeeMembers = _db.ProformaTeamMembers

                .Where(p => p.EmployeeId == userEmpId)
                .Select(p => p.ProformaTeamId);

            IQueryable<ProformaTeam> proformaTeam = _db.ProformaTeams.Where(p => employeeMembers.Contains(p.ProformaTeamId));
            DataSourceResult result = proformaTeam.ToDataSourceResult(request, team => new
            {
                ProformaTeamId = team.ProformaTeamId,
                ProformaPurchaseID = team.ProformaPurchaseID,
                Description = team.Description,
                OrgBranchName = team.OrgBranchName,
                DateCreated = team.DateCreated,
                LastModified = team.LastModified,
                CreatedBy = team.CreatedBy,
                ModifiedBy = team.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeams_Create([DataSourceRequest] DataSourceRequest request, ProformaTeam team, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeam
                {
                    ProformaPurchaseID = id,
                    Description = team.Description,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = DateTime.Today,
                    LastModified = team.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = team.ModifiedBy
                };

                _db.ProformaTeams.Add(entity);
                _db.SaveChanges();
                team.ProformaTeamId = entity.ProformaTeamId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeams_Update([DataSourceRequest] DataSourceRequest request, ProformaTeam team)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeam
                {
                    Description = team.Description,
                    ProformaTeamId = team.ProformaTeamId,
                    ProformaPurchaseID = team.ProformaPurchaseID,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ProformaTeams.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;

                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeams_Destroy([DataSourceRequest] DataSourceRequest request, ProformaTeam team)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeam
                {
                    ProformaTeamId = team.ProformaTeamId,
                    ProformaPurchaseID = team.ProformaPurchaseID,
                    Description = team.Description,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = team.LastModified,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = team.ModifiedBy
                };

                _db.ProformaTeams.Attach(entity);
                _db.ProformaTeams.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult Sign(int id)
        {
            int userEmpId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            var member = _db.ProformaTeamMembers.FirstOrDefault(x => x.ProformaTeamId == id && x.EmployeeId == userEmpId);
            if (member == null)
            {
                var formattedData = new
                {
                    Message = "You are not member of the proforma team.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (member.Signed)
            {
                var formattedData = new
                {
                    Message = "You have already signed before.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                member.Signed = true;
                member.SignedBy = User.Identity.Name;
                member.SginTime = DateTime.Now;
                _db.ProformaTeamMembers.Attach(member);
                _db.Entry(member).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Sign Complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
