using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
   
    public class ProformaEvaluationCriteriaController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ProformaEvaluationCriterias_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ProformaEvaluationCriteria> proformaevaluationcriterias = _db.ProformaEvaluationCriterias.Where(p => p.ProformaPurchaseID == id );
            DataSourceResult result = proformaevaluationcriterias.ToDataSourceResult(request, proformaEvaluationCriteria => new {
                ProformaEvaluationCriteriaID = proformaEvaluationCriteria.ProformaEvaluationCriteriaID,
                ProformaPurchaseID = proformaEvaluationCriteria.ProformaPurchaseID,
                Type = proformaEvaluationCriteria.Type,
                Weight = proformaEvaluationCriteria.Weight,
                Description = proformaEvaluationCriteria.Description,
                OrgBranchName = proformaEvaluationCriteria.OrgBranchName,
                DateCreated = proformaEvaluationCriteria.DateCreated,
                LastModified = proformaEvaluationCriteria.LastModified,
                CreatedBy = proformaEvaluationCriteria.CreatedBy,
                ModifiedBy = proformaEvaluationCriteria.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationCriterias_Create([DataSourceRequest]DataSourceRequest request, ProformaEvaluationCriteria proformaEvaluationCriteria , int id)
        {
            if (_db.ProformaEvaluationCriterias.Any(x => x.Type == proformaEvaluationCriteria.Type && x.ProformaPurchaseID == id))
            {
                ModelState.AddModelError("alreardyexist", "You have already Created Criteria with this Type");
            }

            var prev = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID==id).ToList();
            float prevPoints = 0;
            if (prev != null)
            {
               prevPoints= prev.Sum(x => x.Weight);
            }
                
            var total = prevPoints + proformaEvaluationCriteria.Weight;
                if(total>100)
                {
                    ModelState.AddModelError("above100", "The total Weight will be above 100");
                }


            if (proformaEvaluationCriteria.Type == CriteriaType.Technical ||
                proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {

                if (proformaEvaluationCriteria.Type == CriteriaType.Technical)
                {
                    bool checkExistance = _db.ProformaEvaluationCriterias.Any(
                    x => x.Type == CriteriaType.Pass_Fail && x.ProformaPurchaseID == id);
                    if (checkExistance)
                    {
                        ModelState.AddModelError("conflict","You can not have both Pass/Fail and Technical Evaluations Per Lot.");

                    }
                }
                else if (proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    bool checkExistance = _db.ProformaEvaluationCriterias.Any(
                           x => x.Type == CriteriaType.Technical && x.ProformaPurchaseID == id);
                    if (checkExistance)
                    {
                        ModelState.AddModelError("conflict","You can not have both Pass/Fail and Technical Evaluations Per Lot.");

                    }
                }


            }

            if (proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {
                proformaEvaluationCriteria.Weight = 0;
            }
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationCriteria
                {
                    ProformaPurchaseID = id,
                  
                    Type = proformaEvaluationCriteria.Type,
                    Weight = proformaEvaluationCriteria.Weight,
                    Description = proformaEvaluationCriteria.Description,
                    OrgBranchName = proformaEvaluationCriteria.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = proformaEvaluationCriteria.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = proformaEvaluationCriteria.ModifiedBy
                };

                _db.ProformaEvaluationCriterias.Add(entity);
                _db.SaveChanges();
                proformaEvaluationCriteria.ProformaEvaluationCriteriaID = entity.ProformaEvaluationCriteriaID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation criteria", "Success", User.Identity.Name, $"created criteria {entity.Description} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationCriterias_Update([DataSourceRequest]DataSourceRequest request, ProformaEvaluationCriteria proformaEvaluationCriteria)
        {
            if (_db.ProformaEvaluationCriterias.Any(x => x.ProformaEvaluationCriteriaID != proformaEvaluationCriteria.ProformaEvaluationCriteriaID && x.Type == proformaEvaluationCriteria.Type && x.ProformaPurchaseID == proformaEvaluationCriteria.ProformaPurchaseID))
            {
                ModelState.AddModelError("alreardyexist", "You have already Created Criteria with this Type");

            }
            var prev = _db.ProformaEvaluationCriterias.Where(x => x.ProformaEvaluationCriteriaID != proformaEvaluationCriteria.ProformaEvaluationCriteriaID)
                                                     .Where(x => x.ProformaPurchaseID == proformaEvaluationCriteria.ProformaPurchaseID).ToList();
            float prevPoints = 0;
            if (prev != null)
            {
                prevPoints = prev.Sum(x => x.Weight);
            }

            var total = prevPoints + proformaEvaluationCriteria.Weight;
            if (total > 100)
            {
                ModelState.AddModelError("above100", "The total Weight will be above 100");
            }
            if (proformaEvaluationCriteria.Type == CriteriaType.Technical || proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {
                bool chkType = false;
                if (proformaEvaluationCriteria.Type == CriteriaType.Technical)
                {
                    chkType = _db.ProformaEvaluationCriterias.Any(
                         x =>
                            x.Type == CriteriaType.Pass_Fail && x.ProformaPurchaseID == proformaEvaluationCriteria.ProformaPurchaseID);
                }
                else if (proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    chkType = _db.ProformaEvaluationCriterias.Any(
                       x =>
                          x.Type == CriteriaType.Technical && x.ProformaPurchaseID == proformaEvaluationCriteria.ProformaPurchaseID);

                }


                bool checkExistance = _db.ProformaEvaluationCriterias.Any(
                    x =>
                        x.ProformaEvaluationCriteriaID != proformaEvaluationCriteria.ProformaEvaluationCriteriaID &&
                        x.Type == proformaEvaluationCriteria.Type && x.ProformaPurchaseID == proformaEvaluationCriteria.ProformaPurchaseID);

                if (chkType)
                {
                    ModelState.AddModelError("conflict", "You can not have both Pass/Fail and Technical Evaluations Per Lot.");

                }
            }

            if (proformaEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {
                proformaEvaluationCriteria.Weight = 0;
            }
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationCriteria
                {
                    ProformaEvaluationCriteriaID = proformaEvaluationCriteria.ProformaEvaluationCriteriaID,
                    ProformaPurchaseID = proformaEvaluationCriteria.ProformaPurchaseID,
                  
                    Weight = proformaEvaluationCriteria.Weight,
                    Type = proformaEvaluationCriteria.Type,
                    Description = proformaEvaluationCriteria.Description,
                    OrgBranchName = proformaEvaluationCriteria.OrgBranchName,
                    DateCreated = proformaEvaluationCriteria.DateCreated,
                    LastModified = proformaEvaluationCriteria.LastModified,
                    CreatedBy = proformaEvaluationCriteria.CreatedBy,
                    ModifiedBy = proformaEvaluationCriteria.ModifiedBy
                };

                _db.ProformaEvaluationCriterias.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation criteria", "Success", User.Identity.Name, $"updated criteria {entity.Description} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationCriterias_Destroy([DataSourceRequest]DataSourceRequest request, ProformaEvaluationCriteria proformaEvaluationCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationCriteria
                {
                    ProformaEvaluationCriteriaID = proformaEvaluationCriteria.ProformaEvaluationCriteriaID,
                    ProformaPurchaseID = proformaEvaluationCriteria.ProformaPurchaseID,
                  
                    Weight = proformaEvaluationCriteria.Weight,
                    Type = proformaEvaluationCriteria.Type,
                    Description = proformaEvaluationCriteria.Description,
                    OrgBranchName = proformaEvaluationCriteria.OrgBranchName,
                    DateCreated = proformaEvaluationCriteria.DateCreated,
                    LastModified = proformaEvaluationCriteria.LastModified,
                    CreatedBy = proformaEvaluationCriteria.CreatedBy,
                    ModifiedBy = proformaEvaluationCriteria.ModifiedBy
                };

                _db.ProformaEvaluationCriterias.Attach(entity);
                _db.ProformaEvaluationCriterias.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformaEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetEvaluationCriteria(string id)
        {
            int cid = int.Parse(id);
            var criteria = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == cid);
            return Json(criteria.Select(x => new {Name = x.Type , Value = x.ProformaEvaluationCriteriaID}), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
