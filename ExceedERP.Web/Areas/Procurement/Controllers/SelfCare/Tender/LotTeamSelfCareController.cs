using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class LotTeamSelfCareController : Controller
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

        public ActionResult LotTeams_Read([DataSourceRequest] DataSourceRequest request)
        {
            int userEmpId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);

            var employeeMembers = _db.LotTeamMembers

                .Where(p => p.EmployeeId == userEmpId)
                .Select(p => p.LotTeamId);

            IQueryable<LotTeam> LotTeam = _db.LotTeams.Where(p => employeeMembers.Contains(p.LotTeamId));
            DataSourceResult result = LotTeam.ToDataSourceResult(request, team => new
            {
                LotTeamId = team.LotTeamId,
                LotID = team.LotID,
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
        public ActionResult LotTeams_Create([DataSourceRequest] DataSourceRequest request, LotTeam team, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeam
                {
                    LotID = id,
                    Description = team.Description,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = DateTime.Today,
                    LastModified = team.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = team.ModifiedBy
                };

                _db.LotTeams.Add(entity);
                _db.SaveChanges();
                team.LotTeamId = entity.LotTeamId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeams_Update([DataSourceRequest] DataSourceRequest request, LotTeam team)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeam
                {
                    Description = team.Description,
                    LotTeamId = team.LotTeamId,
                    LotID = team.LotID,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.LotTeams.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;

                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeams_Destroy([DataSourceRequest] DataSourceRequest request, LotTeam team)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeam
                {
                    LotTeamId = team.LotTeamId,
                    LotID = team.LotID,
                    Description = team.Description,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = team.LastModified,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = team.ModifiedBy
                };

                _db.LotTeams.Attach(entity);
                _db.LotTeams.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult Sign(int id)
        {
            int userEmpId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            var member = _db.LotTeamMembers.FirstOrDefault(x => x.LotTeamId == id && x.EmployeeId == userEmpId);
            if (member == null)
            {
                var formattedData = new
                {
                    Message = "You are not member of the Lot team.",
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
                _db.LotTeamMembers.Attach(member);
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
