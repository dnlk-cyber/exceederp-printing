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
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlBalanceImportController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        readonly AccountHelper _accountHelper = new AccountHelper();
        // GET: Finance/GLBalanceImport
        public ActionResult Index()
        {
            return View();
        }

        public FileResult DownloadTemplate()
        {
            //DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/Downloads//Finance/GLBalanceImport.xlsx"));
            string physicalPath = Server.MapPath("~/Content/Downloads/Templates/Finance/GLBalanceImport.xlsx");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(physicalPath, contentType, Path.GetFileName(physicalPath));

            //return File(Path.GetFileName(dirInfo.Name), contentType "GLBalanceImport.xlsx");
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Import([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            int periodId = 0;
            int yearId = id;
            DateTime effectivedate = new DateTime();
            var year = _db.GLFiscalYears.Find(yearId);
            if (year != null)
            {
                effectivedate = year.DateFrom.Date;
                periodId = year.GLPeriod.Min(x => x.GLPeriodId);
            }
            else
            {
                ModelState.AddModelError("Year", "The Fiscal Year is not found.");
            }

            var period = _db.GLPeriods.Find(periodId);

            string fileNamePrefix = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

            #region Import

            DataSet ds = new DataSet();
            GlRecordJournal recordJournal = new GlRecordJournal();

            foreach (HttpPostedFileBase fileBase in file)
            {
                var postedFileBase = Request.Files["file"];
                if (postedFileBase != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(postedFileBase.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                    {
                        string fileLocation = Server.MapPath("~/App_Data/Import/Finance/Beginning/") + fileNamePrefix +
                                              DateTime.Now.Second.ToString() + postedFileBase.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        postedFileBase.SaveAs(fileLocation);
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
            List<GlBalanceImport> glBalanceImports = new List<GlBalanceImport>();
           
            string errors = "";
            decimal debit = 0;
            decimal credit = 0;

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    GlBalanceImport import = new GlBalanceImport
                    {
                        AccountCode = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        AccountDesc = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        Debit = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
                        Credit = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
                    };

                    glBalanceImports.Add(import);
                }
            }

            foreach (var glBalanceImport in glBalanceImports)
            {
                
                try
                {
                    debit = decimal.Parse(glBalanceImport.Debit);
                }
                catch (Exception e)
                {
                    errors = errors + " the debit amount should be in number: " + glBalanceImport.Debit + "\n";
                }
                try
                {
                    credit = decimal.Parse(glBalanceImport.Credit);
                }
                catch (Exception e)
                {
                    errors = errors + " the credit amount should be in number: " + glBalanceImport.Credit + "\n";
                }
                if (debit >= 0 && credit >= 0)
                {
                    errors = errors + " An account should have either debit or credit balance,not both at the same time.: " + glBalanceImport.AccountCode + "\n";
                }


            }
            if (!String.IsNullOrWhiteSpace(errors))
            {
                ModelState.AddModelError("errors", errors);
                return Content(errors);
            }

            if (ModelState.IsValid)
                {
                    #region journal

                    if (year != null)
                    {
                        if (period != null)
                        {
                            var journal = new GlRecordJournal
                            {
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                JournalName = "GL Beginning",
                                Description = "Beginning for the year",
                                FiscalYear = year.Name,
                                PeriodName = period.Name,
                                CategoryName = "JV",
                                SourcesName = "GL",
                                JournalNumber = "Beginning" + year.Name,
                                EffectiveDate = effectivedate,
                                Status = "Editing",
                                IsOnlineApproved = false
                            };
                            _db.GLRecordJournals.Add(journal);
                            _db.SaveChanges();
                            #region create lines

                            foreach (var glBalanceImport in glBalanceImports)
                            {
                                var line = new GLRecordJournalDetail
                                {
                                    GlRecordJournalId = journal.GlRecordJournalId,
                                    AccountCode = glBalanceImport.AccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(glBalanceImport.AccountCode),
                                    Debit = decimal.Parse(glBalanceImport.Debit),
                                    Credit = decimal.Parse(glBalanceImport.Credit),
                                    DateCreated = DateTime.Now,
                                    CreatedBy = User.Identity.Name,
                                };

                                _db.GLRecordJournalDetails.Add(line);
                                _db.SaveChanges();
                            }


                            #endregion
                        }
                    }


                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL beggining import", "Success", User.Identity.Name, $"imported beginning ", Modules.Finance);
                #endregion
            }

            #endregion
            return Json(new[] { recordJournal }.ToDataSourceResult(request, ModelState));

        }

       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}