using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{

    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private IQueryable<Lookup> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Lookup> lookups)
        {
            IQueryable<Lookup> lookupsList = new Lookup[] { }.AsQueryable(); ;
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
                            if (descriptor.Member == "Description")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (lookupsList.Any())
                                {
                                    var person =
                                        lookupsList.ToList().Where(
                                            j =>
                                                j.Description.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    lookupsList = person;
                                }
                                else
                                {
                                    var person =
                                           lookups.ToList().Where(
                                               j =>
                                                   j.Description.ToUpper(XAPI.Common.exceedculture)
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
        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            if (request.Filters != null)
            {
                dd: if (request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "Description")
                            {
                                lookups = ModifyFilters(request.Filters, lookups).AsQueryable();

                            }

                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }
                    }
                    //request.Filters.Clear();
                }
            }
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new
            {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                CreatedBy = lookup.CreatedBy,
                ModifiedBy = lookup.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExstance = db.Lookups.ToList();
                var jobName = lookup.Description;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.Trim().ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.Trim().ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This job title name already exists in the database!!");
                }
                else
                {
                    var entity = new Lookup
                    {
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = LookUpTypes.JOB_TITLE,
                        Description = lookup.Description,
                        IsDefault = lookup.IsDefault,
                        IsActive = true,
                        Remark = lookup.Remark,
                        DateCreated = System.DateTime.Now,
                        LastModified = lookup.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = lookup.ModifiedBy
                    };

                    db.Lookups.Add(entity);
                    db.SaveChanges();
                    lookup.LookupId = entity.LookupId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "JobTitle ", "Success", User.Identity.Name, User.Identity.Name + " Maintain new Jobtitle " + entity.Description, Modules.HRM);


                    #endregion
                }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.JOB_TITLE,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = System.DateTime.Now,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Lookups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "Jobtitle ", "Success", User.Identity.Name, User.Identity.Name + " Update Jobtile info of " + entity.Description, Modules.HRM);


                #endregion
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.JOB_TITLE,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = lookup.LastModified,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = lookup.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title name" + lookup.Description);
                #endregion
                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported job titles");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported job titles");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public ActionResult JobTitle_Import([DataSourceRequest] DataSourceRequest request,
           IEnumerable<HttpPostedFileBase> file)
        {
            #region Import

            string addtionalName = DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();
            System.Data.DataSet ds = new System.Data.DataSet();
            Step issuee = db.Steps.FirstOrDefault();
            List<Step> issues = new List<Step>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/HRM/") + addtionalName + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            List<Lookup> issueImports = new List<Lookup>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    Lookup import = new Lookup
                    {
                        Description = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        Remark = ds.Tables[0].Rows[j].ItemArray[1].ToString(),

                    };

                    issueImports.Add(import);

                }
            }
            foreach (var lookup in issueImports)
            {
                var checkExstance = db.Lookups.ToList();
                var jobName = lookup.Description;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This job title name already exists in the database!!");
                }
                else
                {
                    var entity = new Lookup
                    {
                        Description = lookup.Description,
                        Remark = lookup.Remark,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        IsDefault = lookup.IsDefault,
                        IsActive = true,
                        LookupType = LookUpTypes.JOB_TITLE,
                        DateCreated = DateTime.Now,
                        LastModified = lookup.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = lookup.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Lookups.Add(entity);
                    db.SaveChanges();
                    lookup.LookupId = entity.LookupId;
                }
            }



            #endregion




            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult DownloadFile(string filename)
        {
            db = new ExceedDbContext();
            try
            {

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/HRM/" + filename));
                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, "application/vnd.ms-excel");// attachment.FileType);
            }

            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult GetDescription()
        {
            var jobTitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            return Json(jobTitles.Select(p => new
            {
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
