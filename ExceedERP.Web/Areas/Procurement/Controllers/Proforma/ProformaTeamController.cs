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
   

    public class ProformaTeamController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaTeams_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ProformaTeam> proformaTeam = _db.ProformaTeams.Where(p => p.ProformaPurchaseID == id);
            DataSourceResult result = proformaTeam.ToDataSourceResult(request, winningSupplier => new {
                ProformaTeamId = winningSupplier.ProformaTeamId,
                ProformaPurchaseID = winningSupplier.ProformaPurchaseID,
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
        public ActionResult ProformaTeams_Create([DataSourceRequest]DataSourceRequest request, ProformaTeam winningSupplier , int id )
            {
            if (ModelState.IsValid)
            {
                    var entity = new ProformaTeam
                    {
                        ProformaPurchaseID =id,
                        Description = winningSupplier.Description,
                        OrgBranchName = winningSupplier.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = winningSupplier.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = winningSupplier.ModifiedBy
                    };

                    _db.ProformaTeams.Add(entity);
                    _db.SaveChanges();
                    winningSupplier.ProformaTeamId = entity.ProformaTeamId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeams_Update([DataSourceRequest]DataSourceRequest request, ProformaTeam winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new ProformaTeam
                        {
                            Description= winningSupplier.Description,
                            ProformaTeamId = winningSupplier.ProformaTeamId,
                            ProformaPurchaseID = winningSupplier.ProformaPurchaseID,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.ProformaTeams.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma team", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaTeams_Destroy([DataSourceRequest]DataSourceRequest request, ProformaTeam winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaTeam
                {
                    ProformaTeamId = winningSupplier.ProformaTeamId,
                    ProformaPurchaseID = winningSupplier.ProformaPurchaseID,
                    Description = winningSupplier.Description,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.ProformaTeams.Attach(entity);
                _db.ProformaTeams.Remove(entity);
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
