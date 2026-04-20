using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Lot
{
   

    public class LotTeamController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotTeams_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<LotTeam> LotTeam = _db.LotTeams.Where(p => p.LotID == id);
            DataSourceResult result = LotTeam.ToDataSourceResult(request, winningSupplier => new {
                LotTeamId = winningSupplier.LotTeamId,
                LotID = winningSupplier.LotID,
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
        public ActionResult LotTeams_Create([DataSourceRequest]DataSourceRequest request, LotTeam winningSupplier , int id )
            {
            if (ModelState.IsValid)
            {
                    var entity = new LotTeam
                    {
                        LotID =id,
                        Description = winningSupplier.Description,
                        OrgBranchName = winningSupplier.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = winningSupplier.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = winningSupplier.ModifiedBy
                    };

                    _db.LotTeams.Add(entity);
                    _db.SaveChanges();
                    winningSupplier.LotTeamId = entity.LotTeamId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team", "Success", User.Identity.Name, $"created winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeams_Update([DataSourceRequest]DataSourceRequest request, LotTeam winningSupplier)
        {
                if (ModelState.IsValid)
                {
                        var entity = new LotTeam
                        {
                            Description= winningSupplier.Description,
                            LotTeamId = winningSupplier.LotTeamId,
                            LotID = winningSupplier.LotID,
                            OrgBranchName = winningSupplier.OrgBranchName,
                            DateCreated = winningSupplier.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplier.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.LotTeams.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team", "Success", User.Identity.Name, $"updated winner {entity.Description} under record {entity.Description}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { winningSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeams_Destroy([DataSourceRequest]DataSourceRequest request, LotTeam winningSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeam
                {
                    LotTeamId = winningSupplier.LotTeamId,
                    LotID = winningSupplier.LotID,
                    Description = winningSupplier.Description,
                    OrgBranchName = winningSupplier.OrgBranchName,
                    DateCreated = winningSupplier.DateCreated,
                    LastModified = winningSupplier.LastModified,
                    CreatedBy = winningSupplier.CreatedBy,
                    ModifiedBy = winningSupplier.ModifiedBy
                };

                _db.LotTeams.Attach(entity);
                _db.LotTeams.Remove(entity);
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
