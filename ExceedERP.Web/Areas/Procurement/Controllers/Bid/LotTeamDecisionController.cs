using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Lot
{
   

    public class LotTeamDecisionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotTeamDecisions_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<LotTeamDecision> LotTeamDecision = _db.LotTeamDecisions.Where(p => p.LotTeamId == id);
            DataSourceResult result = LotTeamDecision.ToDataSourceResult(request, winningSupplier => new {
                LotTeamDecisionId = winningSupplier.LotTeamDecisionId,
                LotTeamId = winningSupplier.LotTeamId,
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
        public ActionResult LotTeamDecisions_Create([DataSourceRequest]DataSourceRequest request, LotTeamDecision member , int id )
            {

            if (ModelState.IsValid)
            {
                    var entity = new LotTeamDecision
                    {
                        LotTeamId =id,
                        Description = member.Description,
                        OrgBranchName = member.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = member.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = member.ModifiedBy
                    };

                    _db.LotTeamDecisions.Add(entity);
                    _db.SaveChanges();
                    member.LotTeamDecisionId = entity.LotTeamDecisionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team member", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeamDecisions_Update([DataSourceRequest]DataSourceRequest request, LotTeamDecision winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new LotTeamDecision
                        {
                            Description= winningSupplier.Description,
                            LotTeamDecisionId = winningSupplier.LotTeamDecisionId,
                            LotTeamId = winningSupplier.LotTeamId,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.LotTeamDecisions.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team member", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeamDecisions_Destroy([DataSourceRequest]DataSourceRequest request, LotTeamDecision winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeamDecision
                {
                    LotTeamDecisionId = winningSupplier.LotTeamDecisionId,
                    LotTeamId = winningSupplier.LotTeamId,
                    Description = winningSupplier.Description,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.LotTeamDecisions.Attach(entity);
                _db.LotTeamDecisions.Remove(entity);
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
