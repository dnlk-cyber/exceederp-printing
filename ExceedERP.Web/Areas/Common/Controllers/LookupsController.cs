﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Data.OleDb;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.SalaryStructure;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Areas.HRM.Controllers;
﻿using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles =  HR_Admin)]
    public class LookupsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            SetValues();
            return View();
        }
        private void SetValues()
        {
            var LookUpTypes = Enum.GetValues(typeof(LookUpTypes))
                      .Cast<LookUpTypes>()
                      .ToList();
            var LookUpList = LookUpTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LookUpTypes"] = LookUpList;
        }
        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.OrderBy(l=> l.LookupType );
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {

            if (ModelState.IsValid)
            {
                var checkExstance = db.Lookups.Where( x=> x.LookupType == lookup.LookupType ).ToList();
                var jobName = lookup.Description;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Lookup Name is already exists in the database!!");
                }
                else
                {
                    var entity = new Lookup
                    {
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = lookup.LookupType,
                        Description = lookup.Description,
                        IsDefault = true,
                        IsActive = true,
                        Remark = lookup.Remark
                    };

                    db.Lookups.Add(entity);
                    db.SaveChanges();
                    lookup.LookupId = entity.LookupId;
                }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType  = lookup.LookupType ,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark
                };

                db.Lookups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            var placement = db.Placements.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var recruitmentPlan = db.RecruitmentPlans.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            var recruitmentRequest = db.RecruitmentRequest.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var manPower = db.JobTitleMapping.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var jobtitlesalary = db.SalariesJobTitles.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var applicantRegistration = db.ApplicantRegistration.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var applicantResult = db.ApplicantionFormes.FirstOrDefault(x=>x.LookupId == lookup.LookupId);

            if(placement !=null || recruitmentPlan != null || recruitmentRequest != null || manPower != null || jobtitlesalary != null || applicantRegistration != null || applicantResult != null)
            {
                ModelState.AddModelError("","You can't delete this data it is used in other forms");
            }

            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType  = lookup.LookupType ,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Lookup " + lookup.LookupType.ToString() + " " + lookup.Description );
                #endregion
                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadTemplate(string filename)
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

        public ActionResult Lookup_Import([DataSourceRequest] DataSourceRequest request,
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
            var lookUpTypes = Enum.GetValues(typeof(LookUpTypes))
               .Cast<LookUpTypes>()
               .ToList();
            List<Lookup> issueImports = new List<Lookup>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    Lookup import = new Lookup
                    {
                        Description = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        LookupType = lookUpTypes.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[1].ToString()).Trim()).FirstOrDefault(),
                        Remark = ds.Tables[0].Rows[j].ItemArray[2].ToString(),

                    };

                    issueImports.Add(import);

                }
            }
            foreach (var lookup in issueImports)
            {
                var checkExstance = db.Lookups.Where(x => x.LookupType == lookup.LookupType).ToList();
                var jobName = lookup.Description;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Lookup Name is already exists in the database!!");
                }
                    else if (lookup .Description ==null|| lookup .Description=="" )
                { }
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
                        LookupType = lookup.LookupType,
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
