using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
    public class StructureController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private IQueryable<Core.Domain.Common.HRM.Structure> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Core.Domain.Common.HRM.Structure> structures)
        {
            IQueryable<Core.Domain.Common.HRM.Structure> lookupsList = (new Core.Domain.Common.HRM.Structure[] { }).AsQueryable(); ;
            if (filters.Any())
            {
                string searchedValue = "";

                dd: if (filters.Any())
                {

                    foreach (var filter in filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "Name")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (lookupsList.Any())
                                {
                                    var person =
                                        structures.ToList().Where(
                                            j =>
                                                j.Name.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    lookupsList = person;
                                }
                                else
                                {
                                    var person =
                                           structures.ToList().Where(
                                               j =>
                                                   j.Name.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    lookupsList = lookupsList.Union(person).AsQueryable();
                                }

                            }



                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }

                    }

                }
            }
            return lookupsList.AsQueryable();
        }
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin, HrmConstants.HRCompanyUser)]
        public ActionResult Structure_Read([DataSourceRequest]DataSourceRequest request)
        {
            var structures = db.Structure.ToList();

            #region Filteration Comment
            //if (request.Filters != null)
            //{
            //    dd: if (request.Filters.Any())
            //    {
            //        foreach (var filter in request.Filters)
            //        {
            //            var descriptor = filter as FilterDescriptor;
            //            if (descriptor != null)
            //            {
            //                if (descriptor.Member == "Name")
            //                {
            //                    structures = ModifyFilters(request.Filters, structures).AsQueryable();

            //                }

            //            }
            //            else if (filter is CompositeFilterDescriptor)
            //            {

            //                request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
            //                goto dd;
            //                //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

            //            }
            //        }
            //        //request.Filters.Clear();
            //    }
            //}

            #endregion

            DataSourceResult result = structures.ToDataSourceResult(request, structure => new
            {
                StructureId = structure.StructureId,
                Name = structure.Name,

                Remark = structure.Remark,
                DateCreated = structure.DateCreated,
                LastModified = structure.LastModified,
                CreatedBy = structure.CreatedBy,
                ModifiedBy = structure.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Structure_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Common.HRM.Structure structure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExstance = db.Structure.ToList();
                var jobName = structure.Name;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Structure Name already exists in the database!!");
                }
                else
                {
                    var entity = new Core.Domain.Common.HRM.Structure
                    {
                        Name = structure.Name,
                        Remark = structure.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = structure.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = structure.ModifiedBy
                    };

                    db.Structure.Add(entity);
                    db.SaveChanges();
                    structure.StructureId = entity.StructureId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Structure ", "Success", User.Identity.Name, User.Identity.Name + " Create new Structure called " + entity.Name, Modules.HRM);


                    #endregion
                }
            }

            return Json(new[] { structure }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Structure_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Common.HRM.Structure structure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Common.HRM.Structure
                {
                    StructureId = structure.StructureId,
                    Name = structure.Name,
                    Remark = structure.Remark,
                    DateCreated = structure.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = structure.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Structure.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "Structure ", "Success", User.Identity.Name, User.Identity.Name + " Updated Structure called " + entity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { structure }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Structure_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Common.HRM.Structure structure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Common.HRM.Structure
                {
                    StructureId = structure.StructureId,
                    Name = structure.Name,

                    Remark = structure.Remark,
                    DateCreated = structure.DateCreated,
                    LastModified = structure.LastModified,
                    CreatedBy = structure.CreatedBy,
                    ModifiedBy = structure.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Structure " + structure.Name);
                #endregion
                db.Structure.Attach(entity);
                db.Structure.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { structure }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Company structure");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Company structure");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
