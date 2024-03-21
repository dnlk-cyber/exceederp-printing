using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
   

    public class ProformaTeamMemberController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaTeamMembers_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ProformaTeamMember> ProformaTeamMember = _db.ProformaTeamMembers.Where(p => p.ProformaTeamId == id);
            DataSourceResult result = ProformaTeamMember.ToDataSourceResult(request, team => new {
                ProformaTeamMemberId = team.ProformaTeamMemberId,
                ProformaTeamId = team.ProformaTeamId,
                EmployeeId = team.EmployeeId,
                Title = team.Title,
                OrgBranchName = team.OrgBranchName,
                DateCreated = team.DateCreated,
                LastModified = team.LastModified,
                CreatedBy = team.CreatedBy,
                ModifiedBy = team.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Create([DataSourceRequest]DataSourceRequest request, ProformaTeamMember team , int id )
            {

            if (ModelState.IsValid)
            {
                    var entity = new ProformaTeamMember
                    {
                        ProformaTeamId =id,
                        EmployeeId = team.EmployeeId,
                        Title = team.Title,
                        OrgBranchName = team.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = team.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = team.ModifiedBy
                    };

                    _db.ProformaTeamMembers.Add(entity);
                    _db.SaveChanges();
                    team.ProformaTeamMemberId = entity.ProformaTeamMemberId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"created winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Update([DataSourceRequest]DataSourceRequest request, ProformaTeamMember team)
        {
                if (ModelState.IsValid)
                {
                        var entity = new ProformaTeamMember
                        {
                            EmployeeId= team.EmployeeId,
                            Title = team.Title,
                            ProformaTeamMemberId = team.ProformaTeamMemberId,
                            ProformaTeamId = team.ProformaTeamId,
                            OrgBranchName = team.OrgBranchName,
                            DateCreated = team.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = team.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.ProformaTeamMembers.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"updated winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Destroy([DataSourceRequest]DataSourceRequest request, ProformaTeamMember team)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeamMember
                {
                    ProformaTeamMemberId = team.ProformaTeamMemberId,
                    ProformaTeamId = team.ProformaTeamId,
                    EmployeeId = team.EmployeeId,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = team.LastModified,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = team.ModifiedBy
                };

                _db.ProformaTeamMembers.Attach(entity);
                _db.ProformaTeamMembers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
