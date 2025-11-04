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

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
   

    public class LotTeamMemberController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotTeamMembers_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<LotTeamMember> LotTeamMember = _db.LotTeamMembers.Where(p => p.LotTeamId == id);
            DataSourceResult result = LotTeamMember.ToDataSourceResult(request, team => new {
                LotTeamMemberId = team.LotTeamMemberId,
                LotTeamId = team.LotTeamId,
                EmployeeId = team.EmployeeId,
                Title = team.Title,
                SignedBy = team.SignedBy,
                SginTime = team.SginTime,
                Signed = team.Signed,
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
        public ActionResult LotTeamMembers_Create([DataSourceRequest]DataSourceRequest request, LotTeamMember team , int id )
            {

            if (ModelState.IsValid)
            {
                    var entity = new LotTeamMember
                    {
                        LotTeamId =id,
                        EmployeeId = team.EmployeeId,
                        Title = team.Title,
                        SignedBy = team.SignedBy,
                        SginTime = team.SginTime,
                        Signed = team.Signed,
                        OrgBranchName = team.OrgBranchName,
                        DateCreated = DateTime.Today,
                        LastModified = team.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = team.ModifiedBy
                    };

                    _db.LotTeamMembers.Add(entity);
                    _db.SaveChanges();
                    team.LotTeamMemberId = entity.LotTeamMemberId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team member", "Success", User.Identity.Name, $"created winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion

            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeamMembers_Update([DataSourceRequest]DataSourceRequest request, LotTeamMember team)
        {
                if (ModelState.IsValid)
                {
                        var entity = new LotTeamMember
                        {
                            EmployeeId= team.EmployeeId,
                            Title = team.Title,
                            SignedBy = team.SignedBy,
                            SginTime = team.SginTime,
                            Signed = team.Signed,
                            LotTeamMemberId = team.LotTeamMemberId,
                            LotTeamId = team.LotTeamId,
                            OrgBranchName = team.OrgBranchName,
                            DateCreated = team.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = team.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.LotTeamMembers.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                    
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Lot team member", "Success", User.Identity.Name, $"updated winner {entity.EmployeeId} under record {entity.EmployeeId}", Modules.Procurement);
                #endregion
            }


            return Json(new[] { team }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotTeamMembers_Destroy([DataSourceRequest]DataSourceRequest request, LotTeamMember team)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotTeamMember
                {
                    LotTeamMemberId = team.LotTeamMemberId,
                    LotTeamId = team.LotTeamId,
                    EmployeeId = team.EmployeeId,
                    Title = team.Title,
                    SignedBy = team.SignedBy,
                    SginTime = team.SginTime,
                    Signed = team.Signed,
                    OrgBranchName = team.OrgBranchName,
                    DateCreated = team.DateCreated,
                    LastModified = team.LastModified,
                    CreatedBy = team.CreatedBy,
                    ModifiedBy = team.ModifiedBy
                };

                _db.LotTeamMembers.Attach(entity);
                _db.LotTeamMembers.Remove(entity);
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
