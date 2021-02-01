using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid                    
{
   
    public class TenderEvaluationCriteriaController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()                                   
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult TenderEvaluationCriterias_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TenderEvaluationCriteria> tenderevaluationcriterias = _db.TenderEvaluationCriterias.Where(l => l.LotID == id);
            DataSourceResult result = tenderevaluationcriterias.ToDataSourceResult(request, tenderEvaluationCriteria => new {
                TenderEvaluationCriteriaID = tenderEvaluationCriteria.TenderEvaluationCriteriaID,
                LotID = tenderEvaluationCriteria.LotID,
               
                Type = tenderEvaluationCriteria.Type,
                Weight = tenderEvaluationCriteria.Weight,
                Description = tenderEvaluationCriteria.Description,
                OrgBranchName = tenderEvaluationCriteria.OrgBranchName,
                DateCreated = tenderEvaluationCriteria.DateCreated,
                LastModified = tenderEvaluationCriteria.LastModified,
                CreatedBy = tenderEvaluationCriteria.CreatedBy,
                ModifiedBy = tenderEvaluationCriteria.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationCriterias_Create([DataSourceRequest]DataSourceRequest request, TenderEvaluationCriteria tenderEvaluationCriteria , int id)
        {
            if (_db.TenderEvaluationCriterias.Any(x => x.Type == tenderEvaluationCriteria.Type && x.LotID == id))
            {
              ModelState.AddModelError("alreardyexist", "You have already Created Criteria with this Type");
            }
            var prev = _db.TenderEvaluationCriterias.Where(x => x.LotID == id).ToList();
            float prevPoints = 0;
            if (prev != null)
            {
                prevPoints = prev.Sum(x => x.Weight);
            }

            var total = prevPoints + tenderEvaluationCriteria.Weight;
            if (total > 100)
            {
                ModelState.AddModelError("above100", "The total Weight will be above 100");
            }


            if (tenderEvaluationCriteria.Type == CriteriaType.Technical ||
                tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {

                if (tenderEvaluationCriteria.Type == CriteriaType.Technical)
                {
                          bool checkExistance = _db.TenderEvaluationCriterias.Any(
                          x =>  x.Type == CriteriaType.Pass_Fail   && x.LotID == id);
                        if (checkExistance)
                        {
                            ModelState.AddModelError("conflict","You can not have both Pass/Fail and Technical Evaluations in the same Lot.");

                        }
                }
                else if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    bool checkExistance = _db.TenderEvaluationCriterias.Any(
                           x => x.Type == CriteriaType.Technical && x.LotID == id);
                    if (checkExistance)
                    {
                        ModelState.AddModelError("conflict","You can not have both Pass/Fail and Technical Evaluations in the same Lot.");

                    }  
                }

                if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    tenderEvaluationCriteria.Weight = 0;
                }

         
            }
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationCriteria
                {
                    LotID = id,
                    
                    Weight = tenderEvaluationCriteria.Weight,
                    Type = tenderEvaluationCriteria.Type,
                    Description = tenderEvaluationCriteria.Description,
                    OrgBranchName = tenderEvaluationCriteria.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = tenderEvaluationCriteria.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = tenderEvaluationCriteria.ModifiedBy
                };

                _db.TenderEvaluationCriterias.Add(entity);
                _db.SaveChanges();
                tenderEvaluationCriteria.TenderEvaluationCriteriaID = entity.TenderEvaluationCriteriaID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation criteria", "Success", User.Identity.Name, $"created criteria type {entity.Type} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationCriterias_Update([DataSourceRequest]DataSourceRequest request, TenderEvaluationCriteria tenderEvaluationCriteria)
        {
            
            if (_db.TenderEvaluationCriterias.Any(x => x.TenderEvaluationCriteriaID != tenderEvaluationCriteria.TenderEvaluationCriteriaID && x.Type == tenderEvaluationCriteria.Type && x.LotID == tenderEvaluationCriteria.LotID))
            {
                ModelState.AddModelError("alreardyexist", "You have already Created Criteria with this Type");
               
            }

            var prev = _db.TenderEvaluationCriterias.Where(x => x.TenderEvaluationCriteriaID != tenderEvaluationCriteria.TenderEvaluationCriteriaID)
                                                   .Where(x => x.LotID == tenderEvaluationCriteria.LotID).ToList();
            float prevPoints = 0;
            if (prev != null)
            {
                prevPoints = prev.Sum(x => x.Weight);
            }

            var total = prevPoints + tenderEvaluationCriteria.Weight;
            if (total > 100)
            {
                ModelState.AddModelError("above100", "The total Weight will be above 100");
            }

            if (tenderEvaluationCriteria.Type == CriteriaType.Technical || tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {
                bool chkType = false;
                if (tenderEvaluationCriteria.Type == CriteriaType.Technical)
                {
                  chkType =   _db.TenderEvaluationCriterias.Any(
                       x =>
                          x.Type == CriteriaType.Pass_Fail  && x.LotID == tenderEvaluationCriteria.LotID);
                }
                else if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    chkType =   _db.TenderEvaluationCriterias.Any(
                       x =>
                          x.Type == CriteriaType.Technical  && x.LotID == tenderEvaluationCriteria.LotID);
              
                }
                   

                bool checkExistance = _db.TenderEvaluationCriterias.Any(
                    x =>
                        x.TenderEvaluationCriteriaID != tenderEvaluationCriteria.TenderEvaluationCriteriaID &&
                        x.Type == tenderEvaluationCriteria.Type && x.LotID == tenderEvaluationCriteria.LotID);

                if ( chkType)
                {
                    ModelState.AddModelError("conflict", "You can not have both Pass/Fail and Technical Evaluations Per Lot.");
                    
                }
            }

            if (tenderEvaluationCriteria.Type == CriteriaType.Technical ||
                tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {

                if (tenderEvaluationCriteria.Type == CriteriaType.Technical)
                {
                    bool checkExistance = _db.TenderEvaluationCriterias.Any(
                        x=>x.TenderEvaluationCriteriaID != tenderEvaluationCriteria.TenderEvaluationCriteriaID&&
                         x.Type == CriteriaType.Pass_Fail && x.LotID == tenderEvaluationCriteria.LotID);
                    if (checkExistance)
                    {
                        ModelState.AddModelError("conflict", "You can not have both Pass/Fail and Technical Evaluations in the same Lot.");

                    }
                }
                else if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    bool checkExistance = _db.TenderEvaluationCriterias.Any(
                        x => x.TenderEvaluationCriteriaID != tenderEvaluationCriteria.TenderEvaluationCriteriaID &&
                        x.Type == CriteriaType.Technical && x.LotID == tenderEvaluationCriteria.LotID);
                    if (checkExistance)
                    {
                        ModelState.AddModelError("conflict", "You can not have both Pass/Fail and Technical Evaluations in the same Lot.");

                    }
                }

                if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
                {
                    tenderEvaluationCriteria.Weight = 0;
                }


            }
            if (tenderEvaluationCriteria.Type == CriteriaType.Pass_Fail)
            {
                tenderEvaluationCriteria.Weight = 0;
            }
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationCriteria
                {
                    TenderEvaluationCriteriaID = tenderEvaluationCriteria.TenderEvaluationCriteriaID,
                    LotID = tenderEvaluationCriteria.LotID,
                    Weight = tenderEvaluationCriteria.Weight,
                    Type = tenderEvaluationCriteria.Type,
                    Description = tenderEvaluationCriteria.Description,
                    OrgBranchName = tenderEvaluationCriteria.OrgBranchName,
                    DateCreated = tenderEvaluationCriteria.DateCreated,
                    LastModified = tenderEvaluationCriteria.LastModified,
                    CreatedBy = tenderEvaluationCriteria.CreatedBy,
                    ModifiedBy = tenderEvaluationCriteria.ModifiedBy
                };

                _db.TenderEvaluationCriterias.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation criteria", "Success", User.Identity.Name, $"updated criteria type {entity.Type} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationCriterias_Destroy([DataSourceRequest]DataSourceRequest request, TenderEvaluationCriteria tenderEvaluationCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationCriteria
                {
                    TenderEvaluationCriteriaID = tenderEvaluationCriteria.TenderEvaluationCriteriaID,
                   
                    LotID = tenderEvaluationCriteria.LotID,
                    Weight = tenderEvaluationCriteria.Weight,
                    Type = tenderEvaluationCriteria.Type,
                    Description = tenderEvaluationCriteria.Description,
                    OrgBranchName = tenderEvaluationCriteria.OrgBranchName,
                    DateCreated = tenderEvaluationCriteria.DateCreated,
                    LastModified = tenderEvaluationCriteria.LastModified,
                    CreatedBy = tenderEvaluationCriteria.CreatedBy,
                    ModifiedBy = tenderEvaluationCriteria.ModifiedBy
                };

                _db.TenderEvaluationCriterias.Attach(entity);
                _db.TenderEvaluationCriterias.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetCriteria(string id)
            {
                int lotid = int.Parse(id);
                var evaluation = _db.TenderEvaluationCriterias.Where(x => x.LotID == lotid).AsQueryable();
                return Json(evaluation.Select(i => new { CriteriaName = i.Type }), JsonRequestBehavior.AllowGet);

            }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
