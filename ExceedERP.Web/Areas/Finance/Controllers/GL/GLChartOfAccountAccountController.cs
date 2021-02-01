using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlChartOfAccountAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Index()
        {
            var catagories = _db.GLChartOfAccountAccounts.Where(x => x.Parent == true);

            ViewData["parents"] = catagories.Select(e => new 
            {
                HasParent = e.Values + "-" + e.Description
            }).AsQueryable();

            return View();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult GLChartOfAccountAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLChartOfAccountAccount> glchartofaccountaccounts = _db.GLChartOfAccountAccounts;
            DataSourceResult result = glchartofaccountaccounts.ToDataSourceResult(request, gLChartOfAccountAccount => new
            {

                Values = gLChartOfAccountAccount.Values,
                Description = gLChartOfAccountAccount.Description,
                HasParent = gLChartOfAccountAccount.HasParent,
                Enable = gLChartOfAccountAccount.Enable,
                Parent = gLChartOfAccountAccount.Parent,
                AllowBudgeting = gLChartOfAccountAccount.AllowBudgeting,
                AllowPosting = gLChartOfAccountAccount.AllowPosting,
                AccountType = gLChartOfAccountAccount.AccountType,
                AccountSubType = gLChartOfAccountAccount.AccountSubType,
                OrgBranchName = gLChartOfAccountAccount.OrgBranchName,
                DateCreated = gLChartOfAccountAccount.DateCreated,
                LastModified = gLChartOfAccountAccount.LastModified,
                CreatedBy = gLChartOfAccountAccount.CreatedBy,
                ModifiedBy = gLChartOfAccountAccount.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountAccounts_Create([DataSourceRequest]DataSourceRequest request, GLChartOfAccountAccount gLChartOfAccountAccount, string hasParent)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.Account);
            bool valid = (gLChartOfAccountAccount.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountAccount
                {
                    Values = gLChartOfAccountAccount.Values,
                    Description = gLChartOfAccountAccount.Description,
                    HasParent = hasParent,
                    Enable = gLChartOfAccountAccount.Enable,
                    Parent = gLChartOfAccountAccount.Parent,
                    AllowBudgeting = gLChartOfAccountAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountAccount.AllowPosting,
                    AccountType = gLChartOfAccountAccount.AccountType,
                    AccountSubType = gLChartOfAccountAccount.AccountSubType,
                    OrgBranchName = gLChartOfAccountAccount.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLChartOfAccountAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLChartOfAccountAccount.ModifiedBy
                };
                
                    _db.GLChartOfAccountAccounts.Add(entity);
                    _db.SaveChanges();
                    gLChartOfAccountAccount.Values = entity.Values;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account", "Success", User.Identity.Name, $"created chart of account natural account {entity.Values} {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountAccount }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountAccounts_Update([DataSourceRequest]DataSourceRequest request, GLChartOfAccountAccount gLChartOfAccountAccount)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.Account);
            bool valid = (gLChartOfAccountAccount.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountAccount
                {
                    Values = gLChartOfAccountAccount.Values,
                    Description = gLChartOfAccountAccount.Description,
                    HasParent = gLChartOfAccountAccount.HasParent,
                    Enable = gLChartOfAccountAccount.Enable,
                    Parent = gLChartOfAccountAccount.Parent,
                    AllowBudgeting = gLChartOfAccountAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountAccount.AllowPosting,
                    AccountType = gLChartOfAccountAccount.AccountType,
                    AccountSubType = gLChartOfAccountAccount.AccountSubType,
                    OrgBranchName = gLChartOfAccountAccount.OrgBranchName,
                    DateCreated = gLChartOfAccountAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLChartOfAccountAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                _db.GLChartOfAccountAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account", "Success", User.Identity.Name, $"updated chart of account natural account {entity.Values} {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountAccount }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountAccounts_Destroy([DataSourceRequest]DataSourceRequest request, GLChartOfAccountAccount gLChartOfAccountAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountAccount
                {
                    Values = gLChartOfAccountAccount.Values,
                    Description = gLChartOfAccountAccount.Description,
                    HasParent = gLChartOfAccountAccount.HasParent,
                    Enable = gLChartOfAccountAccount.Enable,
                    Parent = gLChartOfAccountAccount.Parent,
                    AllowBudgeting = gLChartOfAccountAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountAccount.AllowPosting,
                    AccountType = gLChartOfAccountAccount.AccountType,
                    OrgBranchName = gLChartOfAccountAccount.OrgBranchName,
                    DateCreated = gLChartOfAccountAccount.DateCreated,
                    LastModified = gLChartOfAccountAccount.LastModified,
                    CreatedBy = gLChartOfAccountAccount.CreatedBy,
                    ModifiedBy = gLChartOfAccountAccount.ModifiedBy
                };

                _db.GLChartOfAccountAccounts.Attach(entity);
                _db.GLChartOfAccountAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLChartOfAccountAccount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var account = _db.GLChartOfAccountAccounts.FirstOrDefault(p => p.Values == id);
                if (account != null && account.Enable)
                {
                    account.Enable = false;
                }
                else
                {
                    if (account != null) account.Enable = true;
                }
                _db.GLChartOfAccountAccounts.Attach(account);
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");

        }
        public ActionResult GLChartOfAccountAccounts_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            

            DataSet ds = new DataSet();
            GLChartOfAccountAccount glChartOfAccountAccount = _db.GLChartOfAccountAccounts.FirstOrDefault();
            List<Issue> issues = new List<Issue>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                var requestFile = Request.Files["file"];
                if (requestFile != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(requestFile.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                    {
                        string fileLocation = Server.MapPath("~/Content/Import/Finance/") + DateTime.Now.Year.ToString() +
                                              DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                              DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                              DateTime.Now.Second.ToString() + requestFile.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        requestFile.SaveAs(fileLocation);
                        var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsm")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
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
            }
            List<GLChartOfAccountAccount> importRecords = new List<GLChartOfAccountAccount>();
            bool valid = true;
            if (ds.Tables[0].Rows.Count > 0)
            {
               
                #region validate

                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    if (String.IsNullOrEmpty(ds.Tables[0].Rows[j].ItemArray[0].ToString()))
                    {
                        ModelState.AddModelError("values", "The values field is required");
                        valid = false;
                    }
                    if (String.IsNullOrEmpty(ds.Tables[0].Rows[j].ItemArray[1].ToString()))
                    {
                        ModelState.AddModelError("desc", "The description field is required");
                        valid = false;
                    }
                    var enable = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                    if (String.IsNullOrEmpty(enable) || enable.ToUpper() != "TRUE" || enable.ToUpper() != "FALSE")
                    {
                        ModelState.AddModelError("enable", "The enable field should be either true or false");
                        valid = false;
                    }
                    var parent = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                    if (String.IsNullOrEmpty(parent) || parent.ToUpper() != "TRUE" || parent.ToUpper() != "FALSE")
                    {
                        ModelState.AddModelError("parent", "The parent field should be either true or false");
                        valid = false;
                    }
                    var allowBudgeting = ds.Tables[0].Rows[j].ItemArray[5].ToString();
                    if (String.IsNullOrEmpty(allowBudgeting) || allowBudgeting.ToUpper() != "TRUE" || allowBudgeting.ToUpper() != "FALSE")
                    {
                        ModelState.AddModelError("budget", "The allow budgeting field should be either true or false");
                        valid = false;
                    }
                    var allowPosting = ds.Tables[0].Rows[j].ItemArray[6].ToString();
                    if (String.IsNullOrEmpty(allowPosting) || allowPosting.ToUpper() != "TRUE" || allowPosting.ToUpper() != "FALSE")
                    {
                        ModelState.AddModelError("budget", "The allow posting field should be either true or false");
                        valid = false;
                    }
                    var type = ds.Tables[0].Rows[j].ItemArray[7].ToString();
                    if (String.IsNullOrEmpty(type))
                    {
                        ModelState.AddModelError("type", "The account type field should be from 0 to 4 [Asset,Liability, Capital,Revenue,Expense]");
                        valid = false;
                    }

                    var category = ds.Tables[0].Rows[j].ItemArray[8].ToString();
                    if (String.IsNullOrEmpty(category))
                    {
                        ModelState.AddModelError("type", "The account category field is required");
                        valid = false;
                    }
                }

                #endregion
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    var importRow = new GLChartOfAccountAccount
                    {
                        Values = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        Description = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        HasParent = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
                        Enable = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[3].ToString()),
                        Parent = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[4].ToString()),
                        AllowBudgeting = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[5].ToString()),
                        AllowPosting = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[6].ToString()),
                        AccountType = (GlAccountTypes)int.Parse(ds.Tables[0].Rows[j].ItemArray[7].ToString()),
                        AccountSubType = ds.Tables[0].Rows[j].ItemArray[8].ToString()
                    };

                    importRecords.Add(importRow);
                }
            }
           
            if (ModelState.IsValid)
            {
                if(valid)
                {
                    foreach (var row in importRecords)
                    {
                        var exist = _db.GLChartOfAccountAccounts.Find(row.Values);
                        if (exist != null)
                        {
                            _db.GLChartOfAccountAccounts.Add(row);
                            _db.SaveChanges();
                        }

                    }
                }

            }
            return Json(new[] { glChartOfAccountAccount }.ToDataSourceResult(request, ModelState));

        }
        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/Downloads/Templates/Finance/ChartOfAccount-Accounts.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
