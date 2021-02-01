﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class CompanyEvaluationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            ViewData["Programs"] = db.CMSPrograms.Select(p => new { p.CMSProgramId, p.Name }).ToList();
            ViewData["Courses"] = db.CMSCourses.Select(p => new { p.CMSCourseId, p.Name }).ToList();
            ViewData["Cities"] = db.Cities.Select(c => new { c.Name, c.CityId }).ToList();


            ViewData["Checklists"] = db.ISOChecklists.Select(c => new { c.ISOChecklistId, c.Name }).ToList();

            return View();
        }
        public ActionResult Add(int compI, string company)
        {
            var currentCompany = db.CmsTrainees.Find(compI);
            CompanyEvaluationDataEntryViewModel model = new CompanyEvaluationDataEntryViewModel();

            if(currentCompany != null)
            {
                model.Trainee = currentCompany;
                model.CmsTraineeId = compI;

                var idDefinition = db.TraineeIdNumberComponents.FirstOrDefault(i => i.CmsTraineeId == compI);
                if(idDefinition != null)
                {
                    var isoType = db.ISOTypes.Find(idDefinition.ISOTypeId);
                    if(isoType != null)
                    {
                        var isoViewModel = new CompanyEvaluationDataEntryISOViwModel
                        {
                            Name = isoType.Name,
                            ISOTypeId = isoType.ISOTypeId,

                        };

                        var checklists = db.ISOChecklists.Where(c => c.ISOTypeId == isoType.ISOTypeId).ToList();
                        foreach (var checklistItem in checklists)
                        {
                            var checklistViewModel = new CompanyEvaluationDataEntryChecklistViewModel
                            {
                                Name = checklistItem.Name,
                                Code = checklistItem.Code,
                                ISOChecklistId = checklistItem.ISOChecklistId,
                                ValueType = checklistItem.ValueType
                            };
                            isoViewModel.ISOChecklists.Add(checklistViewModel);
                        }

                        model.ISOTypesList.Add(isoViewModel);
                    }

                    
                }
            }

            return View(model);
        }
        [AuthorizeRoles(CMSRoles.Examiner)]
        [HttpPost]
        public ActionResult Add(CompanyEvaluationDataEntryViewModel model)
        {
            var selectedCompany = db.CmsTrainees.Find(model.CmsTraineeId);
            if(selectedCompany != null)
            {
                foreach (var iso in model.ISOTypesList)
                {
                    foreach (var item in iso.ISOChecklists)
                    {
                        if (!db.CompanyEvaluations.Any(d => d.CmsTraineeId == model.CmsTraineeId && d.ISOChecklistId == item.ISOChecklistId))
                        {
                            var value = "";
                            if (item.ValueType == ISOChecklistValueType.Numeric_Value)
                            {
                                value = item.NumericValue.ToString();
                            }
                            else
                            {
                                if (item.IsSelected)
                                {
                                    value = "YES";
                                }
                                else
                                {
                                    value = "NO";
                                }
                            }
                            var recordToAdd = new CompanyEvaluation
                            {
                                CmsTraineeId = model.CmsTraineeId,

                                ISOChecklistId = item.ISOChecklistId,
                                Remark = item.Remark,
                                Value = value,

                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                LastModified = null,
                                ModifiedBy = null,
                            };
                            db.CompanyEvaluations.Add(recordToAdd);
                            db.SaveChanges();

                        }

                    }
                }

            }
            return Redirect(nameof(Index));
        }
        public ActionResult CompanyEvaluations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CompanyEvaluation> evaluations = db.CompanyEvaluations.Where(e => e.CmsTraineeId == id);
            DataSourceResult result = evaluations.ToDataSourceResult(request, isoType => new CompanyEvaluation {
                CompanyEvaluationId = isoType.CompanyEvaluationId,
                CmsTraineeId = isoType.CmsTraineeId,
                ISOChecklistId = isoType.ISOChecklistId,
                Value = isoType.Value,
                Remark = isoType.Remark,
                
                DateCreated = isoType.DateCreated,
                LastModified = isoType.LastModified,
                CreatedBy = isoType.CreatedBy,
                ModifiedBy = isoType.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEvaluation_Create([DataSourceRequest]DataSourceRequest request, CompanyEvaluation isoType, int id)
        {
            //if (DuplicateDefaultExists(isoType))
            //{
            //    ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            //}
            //if (DuplicateISODefinitionExists(isoType))
            //{
            //    ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new CompanyEvaluation
                {
                    CompanyEvaluationId = isoType.CompanyEvaluationId,
                    CmsTraineeId = id,
                    ISOChecklistId = isoType.ISOChecklistId,
                    Value = isoType.Value,
                    Remark = isoType.Remark,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.CompanyEvaluations.Add(entity);
                db.SaveChanges();
                isoType.CompanyEvaluationId = entity.CompanyEvaluationId;
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEvaluation_Update([DataSourceRequest]DataSourceRequest request, CompanyEvaluation isoType)
        {
            //if (DuplicateDefaultExists(isoType))
            //{
            //    ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            //}
            //if (DuplicateISODefinitionExists(isoType))
            //{
            //    ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new CompanyEvaluation
                {
                    CompanyEvaluationId = isoType.CompanyEvaluationId,
                    CmsTraineeId = isoType.CmsTraineeId,
                    ISOChecklistId = isoType.ISOChecklistId,
                    Value = isoType.Value,
                    Remark = isoType.Remark,

                    DateCreated = isoType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CompanyEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEvaluation_Destroy([DataSourceRequest]DataSourceRequest request, CompanyEvaluation isoType)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyEvaluation
                {
                    CompanyEvaluationId = isoType.CompanyEvaluationId,
                    CmsTraineeId = isoType.CmsTraineeId,
                    ISOChecklistId = isoType.ISOChecklistId,
                    Value = isoType.Value,
                    Remark = isoType.Remark,

                    DateCreated = isoType.DateCreated,
                    LastModified = isoType.LastModified,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = isoType.ModifiedBy
                };

                db.CompanyEvaluations.Attach(entity);
                db.CompanyEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
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
            db.Dispose();
            base.Dispose(disposing);
        }

        #region local methods
        //private bool DuplicateDefaultExists(CompanyEvaluation iSOType)
        //{
        //    return db.CompanyEvaluations.Any(i => i.CompanyEvaluationId != iSOType.CompanyEvaluationId && i.IsDefault && iSOType.IsDefault);
        //}
        //private bool DuplicateISODefinitionExists(CompanyEvaluation iSOType)
        //{
        //    return db.CompanyEvaluations.Any(i => i.Name == iSOType.Name && i.CompanyEvaluationId != iSOType.CompanyEvaluationId);
        //}

        #endregion
    }
}
