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
            DataSourceResult result = ProformaTeamMember.ToDataSourceResult(request, winningSupplier => new {
                ProformaTeamMemberId = winningSupplier.ProformaTeamMemberId,
                ProformaTeamId = winningSupplier.ProformaTeamId,
                EmployeeId = winningSupplier.EmployeeId,
                OrgBranchName = winningSupplier.OrgBranchName,
                DateCreated = winningSupplier.DateCreated,
                LastModified = winningSupplier.LastModified,
                CreatedBy = winningSupplier.CreatedBy,
                ModifiedBy = winningSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Create([DataSourceRequest]DataSourceRequest request, ProformaTeamMember member , int id )
            {

            if (ModelState.IsValid)
            {
                    var entity = new ProformaTeamMember
                    {
                        ProformaTeamId =id,
                        EmployeeId = member.EmployeeId,
                        OrgBranchName = member.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = member.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = member.ModifiedBy
                    };

                    _db.ProformaTeamMembers.Add(entity);
                    _db.SaveChanges();
                    member.ProformaTeamMemberId = entity.ProformaTeamMemberId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"created winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Update([DataSourceRequest]DataSourceRequest request, ProformaTeamMember winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new ProformaTeamMember
                        {
                            EmployeeId= winningSupplier.EmployeeId,
                            ProformaTeamMemberId = winningSupplier.ProformaTeamMemberId,
                            ProformaTeamId = winningSupplier.ProformaTeamId,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.ProformaTeamMembers.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"updated winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamMembers_Destroy([DataSourceRequest]DataSourceRequest request, ProformaTeamMember winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeamMember
                {
                    ProformaTeamMemberId = winningSupplier.ProformaTeamMemberId,
                    ProformaTeamId = winningSupplier.ProformaTeamId,
                    EmployeeId = winningSupplier.EmployeeId,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.ProformaTeamMembers.Attach(entity);
                _db.ProformaTeamMembers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
