using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class CreateStructureController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            GetOrganizations();
            GetStructureType();
            GetStructures();
            return View();
        }
        private IQueryable<MainCompanyStructure> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<MainCompanyStructure> maincompanystructures)
        {
            IQueryable<MainCompanyStructure> lookupsList = new MainCompanyStructure[] { }.AsQueryable(); ;
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
                                        maincompanystructures.ToList().Where(
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
                                           maincompanystructures.ToList().Where(
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

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult MainCompanyStructures_Read([DataSourceRequest]DataSourceRequest request)
        {
            var maincompanystructures = db.MainCompanyStructures.ToList();

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
            //                    maincompanystructures = ModifyFilters(request.Filters, maincompanystructures).AsQueryable();

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

            DataSourceResult result = maincompanystructures.ToDataSourceResult(request, mainCompanyStructure => new
            {
                OrgStructureId = mainCompanyStructure.OrgStructureId,
                Parent = mainCompanyStructure.Parent,
                Name = mainCompanyStructure.Name,
                StructureId = mainCompanyStructure.StructureId,
                IsActive = mainCompanyStructure.IsActive,
                Remark = mainCompanyStructure.Remark,
                DateCreated = mainCompanyStructure.DateCreated,
                LastModified = mainCompanyStructure.LastModified,
                CreatedBy = mainCompanyStructure.CreatedBy,
                ModifiedBy = mainCompanyStructure.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyStructures_Create([DataSourceRequest]DataSourceRequest request, MainCompanyStructure mainCompanyStructure)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var checkExstance = db.MainCompanyStructures.ToList();
                var jobName = mainCompanyStructure.Name;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Name == jobName);
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This structure name already exists in the database!!");
                }
                else
                {
                    var entity = new MainCompanyStructure
                    {
                        Name = mainCompanyStructure.Name,
                        Parent = mainCompanyStructure.Parent,
                        StructureId = mainCompanyStructure.StructureId,
                        IsActive = true,
                        Remark = mainCompanyStructure.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = mainCompanyStructure.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = mainCompanyStructure.ModifiedBy
                    };
                    //#region delete log
                    //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Company Structure " + mainCompanyStructure.Name);
                    //#endregion
                    db.MainCompanyStructures.Add(entity);
                    db.SaveChanges();
                    mainCompanyStructure.OrgStructureId = entity.OrgStructureId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Company Structure ", "Success", User.Identity.Name, User.Identity.Name + " Add new comopany Sturucture " + entity.Name, Modules.HRM);


                    #endregion
                }
            }

            return Json(new[] { mainCompanyStructure }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyStructures_Update([DataSourceRequest]DataSourceRequest request, MainCompanyStructure mainCompanyStructure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new MainCompanyStructure
                {
                    OrgStructureId = mainCompanyStructure.OrgStructureId,
                    Name = mainCompanyStructure.Name,
                    Parent = mainCompanyStructure.Parent,
                    StructureId = mainCompanyStructure.StructureId,
                    IsActive = mainCompanyStructure.IsActive,
                    Remark = mainCompanyStructure.Remark,
                    DateCreated = mainCompanyStructure.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = mainCompanyStructure.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MainCompanyStructures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mainCompanyStructure }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyStructures_Destroy([DataSourceRequest]DataSourceRequest request, MainCompanyStructure mainCompanyStructure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new MainCompanyStructure
                {
                    OrgStructureId = mainCompanyStructure.OrgStructureId,
                    Name = mainCompanyStructure.Name,
                    Parent = mainCompanyStructure.Parent,
                    StructureId = mainCompanyStructure.StructureId,
                    IsActive = mainCompanyStructure.IsActive,
                    Remark = mainCompanyStructure.Remark,
                    DateCreated = mainCompanyStructure.DateCreated,
                    LastModified = mainCompanyStructure.LastModified,
                    CreatedBy = mainCompanyStructure.CreatedBy,
                    ModifiedBy = mainCompanyStructure.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Company Sturucture " + mainCompanyStructure.Name);
                #endregion
                db.MainCompanyStructures.Attach(entity);
                db.MainCompanyStructures.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mainCompanyStructure }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported company structure mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported company structure mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }



        public JsonResult GetStructureType()
        {
            var Types = Enum.GetValues(typeof(OrganizationUnitDefinitionType))
                         .Cast<OrganizationUnitDefinitionType>()

                         .ToList();
            var TypesList = Types.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["Type"] = TypesList;
            return Json(Types.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrganizations()
        {
            db = new ExceedDbContext();
            var organizationUnits = db.MainCompanyStructures.ToList();
            ViewData["Parent"] = organizationUnits;
            return Json(organizationUnits.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructures()
        {
            db = new ExceedDbContext();
            var structures = db.Structure;

            ViewData["structures"] = structures;
            return Json(structures.Select(s => new { StructureId = s.StructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnableOrDisable(int id)
        {
            var structure = db.MainCompanyStructures.Find( id);

            if (structure != null && structure.IsActive)
            {

                structure.IsActive = false;

                db.MainCompanyStructures.Attach(structure);
                db.Entry(structure).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "Company Structure ", "Success", User.Identity.Name, User.Identity.Name + " Add new comopany Sturucture " + structure.Name+"IsActive"+structure.IsActive, Modules.HRM);


                #endregion
                var formattedData = new
                {
                    Message = "You successfully Disabled this Work unit.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (structure != null && !structure.IsActive)
            {

                structure.IsActive = true;

                db.MainCompanyStructures.Attach(structure);
                db.Entry(structure).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "Company Structure ", "Success", User.Identity.Name, User.Identity.Name + " Add new comopany Sturucture " + structure.Name + "IsActive" + structure.IsActive, Modules.HRM);


                #endregion

                var formattedData = new
                {
                    Message = "You successfully enabled this Work unit.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "this WorkU nit Not Found .",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
