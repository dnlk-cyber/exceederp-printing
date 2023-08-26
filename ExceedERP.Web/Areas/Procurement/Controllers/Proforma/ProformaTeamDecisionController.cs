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
   

    public class ProformaTeamDecisionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaTeamDecisions_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ProformaTeamDecision> ProformaTeamDecision = _db.ProformaTeamDecisions.Where(p => p.ProformaTeamId == id);
            DataSourceResult result = ProformaTeamDecision.ToDataSourceResult(request, winningSupplier => new {
                ProformaTeamDecisionId = winningSupplier.ProformaTeamDecisionId,
                ProformaTeamId = winningSupplier.ProformaTeamId,
                Description = winningSupplier.Description,
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
        public ActionResult ProformaTeamDecisions_Create([DataSourceRequest]DataSourceRequest request, ProformaTeamDecision member , int id )
            {

            if (ModelState.IsValid)
            {
                    var entity = new ProformaTeamDecision
                    {
                        ProformaTeamId =id,
                        Description = member.Description,
                        OrgBranchName = member.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = member.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = member.ModifiedBy
                    };

                    _db.ProformaTeamDecisions.Add(entity);
                    _db.SaveChanges();
                    member.ProformaTeamDecisionId = entity.ProformaTeamDecisionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamDecisions_Update([DataSourceRequest]DataSourceRequest request, ProformaTeamDecision winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new ProformaTeamDecision
                        {
                            Description= winningSupplier.Description,
                            ProformaTeamDecisionId = winningSupplier.ProformaTeamDecisionId,
                            ProformaTeamId = winningSupplier.ProformaTeamId,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.ProformaTeamDecisions.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team member", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeamDecisions_Destroy([DataSourceRequest]DataSourceRequest request, ProformaTeamDecision winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeamDecision
                {
                    ProformaTeamDecisionId = winningSupplier.ProformaTeamDecisionId,
                    ProformaTeamId = winningSupplier.ProformaTeamId,
                    Description = winningSupplier.Description,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.ProformaTeamDecisions.Attach(entity);
                _db.ProformaTeamDecisions.Remove(entity);
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
