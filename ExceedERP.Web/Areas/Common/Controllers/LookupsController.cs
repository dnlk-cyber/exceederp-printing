using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles = HR_Admin)]
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
            IQueryable<Lookup> lookups = db.Lookups.OrderBy(l => l.LookupType);
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new
            {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                EvalutionType = lookup.EvalutionType,
                Percentage = lookup.Percentage,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                DescriptionEng = lookup.DescriptionEng,
                MainFunction = lookup.MainFunction,
                InformationOne = lookup.InformationOne,
                InformationTwo = lookup.InformationTwo,
                TaxTresholdAmount = lookup.TaxTresholdAmount,
                Remark = lookup.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {

           
            var checkExstance = db.Lookups.Where(x => x.LookupType == lookup.LookupType).ToList();
            var jobName = lookup.Description;
            if (string.IsNullOrEmpty(jobName))
            {
                ModelState.AddModelError("", "Decription file is required!!");
                return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));

            }
            var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
            if (mainCompanyStructures != null)
            {
                ModelState.AddModelError("", "This Lookup Name is already exists in the database!!");
                return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));

            }
            if (lookup.LookupType == LookUpTypes.EDUCATION_LEVEL || lookup.LookupType == LookUpTypes.EDUCATION_FIELD)
            {
                var conflictingLookup = db.Lookups
                        .Where(x => x.LookupType == lookup.LookupType && x.Index == lookup.Index);

                if (conflictingLookup.Count()>2)
                {
                    ModelState.AddModelError("", lookup.LookupType.ToString()+" can't have the same index value! Note: Higher index means a higher level/field.");
                    return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
                }
            }
            
            if (ModelState.IsValid)
            {
                
                lookup.DateCreated = DateTime.Now;
                lookup.CreatedBy = User.Identity.Name;
                lookup.IsDefault = true;
                lookup.IsActive = true;
                var entity = GetMappedObject(lookup);
                db.Lookups.Add(entity);
                db.SaveChanges();
                lookup.LookupId = entity.LookupId;
                CacheHelper.InvalidateJobTitlesCache();

            }
            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            var checkExstance = db.Lookups.Where(x => x.LookupType == lookup.LookupType).ToList();
            var jobName = lookup.Description;
            if (string.IsNullOrEmpty(jobName))
            {
                ModelState.AddModelError("", "Decription file is required!!");
                return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));

            }
            var mainCompanyStructures = checkExstance.FirstOrDefault(j => (j.LookupId != lookup.LookupId) &&j.Description.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
            if (mainCompanyStructures != null)
            {
                ModelState.AddModelError("", "This Lookup Name is already exists in the database!!");
                return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));

            }
            if (lookup.LookupType == LookUpTypes.EDUCATION_LEVEL || lookup.LookupType == LookUpTypes.EDUCATION_FIELD)
            {
                var conflictingLookup = db.Lookups
                        .Where(x => x.LookupType == lookup.LookupType && x.Index == lookup.Index);

                if (conflictingLookup.Count()>2)
                {
                    ModelState.AddModelError("", lookup.LookupType.ToString() + " can't have the same index value! Note: Higher index means a higher level/field.");
                    return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
                }
            }

           

            if (ModelState.IsValid)
            {
                
                lookup.LastModified = DateTime.Now;
                lookup.ModifiedBy = User.Identity.Name;
                var trackedEntity = db.Lookups.Local.FirstOrDefault(e => e.LookupId == lookup.LookupId);
                if (trackedEntity != null)
                {
                    db.Entry(trackedEntity).State = EntityState.Detached;
                }

                var entity = GetMappedObject(lookup);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            var placement = db.Placements.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            var recruitmentPlan = db.RecruitmentPlans.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            var recruitmentRequest = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            var jobtitlesalary = db.SalariesJobTitles.FirstOrDefault(x => x.LookupId == lookup.LookupId);
            //  var applicantRegistration = db.ApplicantRegistration.FirstOrDefault(x=>x.LookupId == lookup.LookupId);
            var applicantResult = db.ApplicantionFormes.FirstOrDefault(x => x.LookupId == lookup.LookupId);

            if (placement != null || recruitmentPlan != null || recruitmentRequest != null || manPower != null || jobtitlesalary != null || applicantResult != null)
            {
                ModelState.AddModelError("", "You can't delete this data it is used in other forms");
            }

            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(lookup);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Lookup " + lookup.LookupType.ToString() + " " + lookup.Description);
                #endregion
                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
                CacheHelper.InvalidateJobTitlesCache();

            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }
        private Lookup GetMappedObject(Lookup lookup)
        {
            var entity = new Lookup
            {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                EvalutionType = lookup.EvalutionType,
                Description = lookup.Description,
                Percentage = lookup.Percentage,
                DescriptionEng = lookup.DescriptionEng,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                MainFunction = lookup.MainFunction,
                InformationOne = lookup.InformationOne,
                InformationTwo = lookup.InformationTwo,
                TaxTresholdAmount = lookup.TaxTresholdAmount,
                CreatedBy = lookup.CreatedBy,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                ModifiedBy = lookup.ModifiedBy,
                OrgBranchName = lookup.OrgBranchName,
                Remark = lookup.Remark
            };
            return entity;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EnableOrDisable(int id)
        {
            var lookup = db.Lookups.Find(id);

            if (lookup == null)
            {
                // Record not found response
                return Json(new
                {
                    Message = "Record Not Found.",
                    Status = "not_ok"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive status
            bool isCurrentlyActive = lookup.IsActive;
            lookup.IsActive = !isCurrentlyActive;
            db.Entry(lookup).State = EntityState.Modified;
            db.SaveChanges();

            #region Operation Log

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            string logMessage = $"{User.Identity.Name} {(isCurrentlyActive ? "Disabled" : "Enabled")} lookup '{lookup.Description}' (IsActive: {lookup.IsActive}).";

            if (empId != 0)
            {
                var employee = db.Person_Employee.Find(empId);
                if (employee != null)
                {
                    logMessage = $"{employee.FullNameaEnglish} {logMessage}";
                }
            }

            UserHelper.OperationLog(Request.UserHostAddress, "Lookup Management", "Success", User.Identity.Name, logMessage, Modules.Common);

            #endregion

            // Success response
            return Json(new
            {
                Message = $"You successfully {(isCurrentlyActive ? "Disabled" : "Enabled")} this Lookup.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult EnableOrDisable(int id)
        //{
        //    var lookup = db.Lookups.Find(id);

        //    if (lookup != null && lookup.IsActive)
        //    {

        //        lookup.IsActive = false;

        //        db.Entry(lookup).State = EntityState.Modified;
        //        db.SaveChanges();

        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        if (empId != 0)
        //        {
        //            var employee = db.Person_Employee.Find(empId);
        //            if (employee != null)
        //            {
        //                UserHelper.OperationLog(Request.UserHostAddress, "Company Structure ", "Success", User.Identity.Name, employee.FullNameaEnglish + " Add new comopany Sturucture " + jobTitle.Lookup.Description + "IsActive" + jobTitle.IsActive, Modules.HRM);

        //            }
        //        }
        //        else
        //        {
        //            UserHelper.OperationLog(Request.UserHostAddress, "Company Structure ", "Success", User.Identity.Name, User.Identity.Name + " Add new comopany Sturucture " + jobTitle.Lookup.Description + "IsActive" + jobTitle.IsActive, Modules.HRM);

        //        }


        //        #endregion
        //        var formattedData = new
        //        {
        //            Message = "You successfully Disabled this Job Title.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (lookup != null && lookup.IsActive == false)
        //    {

        //        lookup.IsActive = true;

        //        //db.JobTitleMapping.Attach(jobTitle);
        //        db.Entry(lookup).State = EntityState.Modified;
        //        db.SaveChanges();
        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //        if (empId != 0)
        //        {
        //            var employee = db.Person_Employee.Find(empId);
        //            if (employee != null)
        //            {
        //                UserHelper.OperationLog(Request.UserHostAddress, "Kook Up ", "Success", User.Identity.Name, employee.FullNameaEnglish + " Add new Look " + jobTitle.Lookup.Description + "IsActive" + jobTitle.IsActive, Modules.Common);

        //            }
        //        }
        //        else
        //        {
        //            UserHelper.OperationLog(Request.UserHostAddress, "Look Up ", "Success", User.Identity.Name, User.Identity.Name + " Add new Look " + jobTitle.Lookup.Description + "IsActive" + jobTitle.IsActive, Modules.Common);

        //        }

        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "You successfully enabled .",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    var formattedData1 = new
        //    {
        //        Message = "Record Not Found .",
        //        Status = "FAIL"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);

        //}

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
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Lookup");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Lookup");
            #endregion
            return File(fileContents, contentType, fileName);
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
                else if (lookup.Description == null || lookup.Description == "")
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
