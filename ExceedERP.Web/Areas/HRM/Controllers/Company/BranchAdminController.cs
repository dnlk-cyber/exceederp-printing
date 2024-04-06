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
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class BranchAdminController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string AmharicName { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            ViewData["types"] = types;

            var branches = db.Branch;
            ViewData["branches"] = branches;
        }
        public void GetAmharicName(string car)
        {
            AmharicName = car;
        }

        public JsonResult GetBranches()
        {

            var branchs = db.Branch.AsQueryable();

            return Json(branchs.Select(p => new
            {
                BranchId = p.BranchId,
                Name =  p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult Branch_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Branch> branchs = db.Branch;
            DataSourceResult result = branchs.ToDataSourceResult(request, branch => new
            {
                BranchId = branch.BranchId,
                Name = branch.Name,
                ParentBranchId = branch.ParentBranchId,
                Remark = branch.Remark,
                DateCreated = branch.DateCreated,
                LastModified = branch.LastModified,
                CreatedBy = branch.CreatedBy,
                ModifiedBy = branch.ModifiedBy,
                AmharicName = branch.AmharicName,
                Code = branch.Code,
                BranchTypes = branch.BranchTypes,
                Abbreviation = branch.Abbreviation,
                IsActive = branch.IsActive,
                // LookupId = branch.LookupId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branch_Create([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            var branches = db.Branch.ToList();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string jobName = branch.Name;
                var checkExistance = branches.FirstOrDefault(x => x.Code == branch.Code || x.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (checkExistance != null)
                {
                    ModelState.AddModelError("", "The branch name or code must be unique!!");
                }
                else
                {
                    var entity = new Branch
                    {
                        Name = branch.Name,
                        ParentBranchId = branch.ParentBranchId,
                        Remark = branch.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = branch.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = branch.ModifiedBy,
                        AmharicName = branch.AmharicName,
                        Code = branch.Code,
                        BranchTypes = branch.BranchTypes,
                        Abbreviation = branch.Abbreviation,
                        IsActive = branch.IsActive,
                        // LookupId = branch.LookupId
                    };

                    db.Branch.Add(entity);
                    db.SaveChanges();
                    branch.BranchId = entity.BranchId;

                }
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branch_Update([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicName == null) AmharicName = branch.AmharicName;
                var entity = new Branch
                {
                    BranchId = branch.BranchId,
                    ParentBranchId = branch.ParentBranchId,
                    Name = branch.Name,
                    Remark = branch.Remark,
                    DateCreated = branch.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = branch.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    AmharicName = branch.AmharicName,
                    Code = branch.Code,
                    BranchTypes = branch.BranchTypes,
                    Abbreviation = branch.Abbreviation,
                    IsActive = branch.IsActive,
                    //LookupId = branch.LookupId

                };

                db.Branch.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }
        //   [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Branch_Destroy([DataSourceRequest]DataSourceRequest request, Branch branch)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Branch
        //        {
        //            BranchId = branch.BranchId,
        //            Name = branch.Name,
        //            Remark = branch.Remark,
        //            DateCreated = branch.DateCreated,
        //            LastModified = branch.LastModified,
        //            CreatedBy = branch.CreatedBy,
        //            ModifiedBy = branch.ModifiedBy,
        //            AmharicName = branch.AmharicName,
        //            Code = branch.Code,
        //            BranchTypes = branch.BranchTypes,
        //            Abbreviation = branch.Abbreviation,
        //            LookupId = branch.LookupId
        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Branch" + branch.Name +"("+branch.AmharicName +")");
        //        #endregion
        //        db.Branch.Attach(entity);
        //        db.Branch.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        //}
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Branches");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Branches");
            #endregion
            return File(fileContents, contentType, fileName);
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
        public ActionResult Branch_Import([DataSourceRequest] DataSourceRequest request,
         IEnumerable<HttpPostedFileBase> file)
        {
            #region Import
            var branches = db.Branch.ToList();
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
            var branchTypes = Enum.GetValues(typeof(BranchTypes))
             .Cast<BranchTypes>()
             .ToList();
            List<Branch> issueImports = new List<Branch>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    Branch import = new Branch
                    {
                        Name = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        Code = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        BranchTypes = branchTypes.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[2].ToString()).Trim()).FirstOrDefault(),
                        Abbreviation = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
                        AmharicName = ds.Tables[0].Rows[j].ItemArray[4].ToString(),
                        Remark = ds.Tables[0].Rows[j].ItemArray[5].ToString(),

                    };

                    issueImports.Add(import);

                }
            }
            foreach (var branch in issueImports)
            {
                string jobName = branch.Name;
                var checkExistance = branches.FirstOrDefault(x => x.Code == branch.Code || x.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (checkExistance != null)
                {
                    ModelState.AddModelError("", "The branch name or code must be unique!!");
                }
                else
                {
                    var entity = new Branch
                    {
                        Name = branch.Name,
                        AmharicName = branch.AmharicName,
                        Code = branch.Code,
                        BranchTypes = branch.BranchTypes,
                        Abbreviation = branch.Abbreviation,
                        Remark = branch.Remark,
                        Lookup = branch.Lookup,
                        SubAccount = branch.SubAccount,
                        DateCreated = DateTime.Now,
                        LastModified = branch.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = branch.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };
                    db = new ExceedDbContext();
                    db.Branch.Add(entity);
                    db.SaveChanges();
                    branch.BranchId = entity.BranchId;
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
