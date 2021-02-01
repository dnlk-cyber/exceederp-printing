﻿using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.IO;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;


using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Budget.Models;

using ExceedERP.Core.Domain.Budget;

using System.Text;
using ExceedERP.Helpers.Budget;
using System.Diagnostics;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.Finance;
using OfficeOpenXml;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class AttachmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();




        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {

                var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/0-DOCUMENT"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            int id = 0;
            try
            {
                int i = 0;

                if (files != null)
                {
                    foreach (var file in files)
                    {

                        byte[] imgData;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            imgData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                        file.SaveAs(physicalPath);
                        var entity = new Attachment
                        {

                            Reference = id.ToString(),
                            Description = file.FileName,
                            FileType = fileName.Split('.')[1],
                            ReferenceType = (AttachmentType)Enum.Parse(typeof(AttachmentType), fileName.Split('.')[1]),
                            Index = i,
                            BinaryFile = imgData,
                            URL = physicalPath

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        i++;
                    }

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.

                }

            }
            catch (Exception)
            {

                // throw;
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DownloadCustomTemplate(AttachmentViewModel Attachment)
        {

            System.Data.DataTable table = new System.Data.DataTable();
            table = new System.Data.DataTable();

            table.Columns.Add("Id", typeof(int)).ReadOnly = true;
            table.Columns.Add("Branch", typeof(string)).ReadOnly = true;
            table.Columns.Add("Work Unit", typeof(string)).ReadOnly = true;
            //table.Columns.Add("Category Code", typeof(int)).ReadOnly = true;
            table.Columns.Add("Account Code", typeof(string)).ReadOnly = true;
            table.Columns.Add("Description", typeof(string)).ReadOnly = true;

            table.Columns.Add("Month", typeof(string)).ReadOnly = true;
            table.Columns.Add("  Unit", typeof(string)).ReadOnly = true;
            table.Columns.Add("Amount", typeof(int));




            var Currencies = db.BudgetCurrencies.ToList();
            PlanCategory Category = db.PlanCategorys.Where(x => x.GlFiscalYear.Name == Attachment.FiscalYear && x.Name == Attachment.CategoryName).FirstOrDefault();
            if (Category != null)
            {
                BudgetCategory budgetCategory = db.BudgetCategorys.Where(x => x.Name == Attachment.CategoryName).FirstOrDefault();

                if (budgetCategory != null)
                {
                    var BudgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == Category.PlanCategoryID).ToList();

                    if (Attachment.Branch != null)
                    {
                        BudgetPlans = BudgetPlans.Where(x => x.BranchId.ToString() == Attachment.Branch).ToList();
                    }

                    if (Attachment.WorkUnitName != null)
                    {
                        BudgetPlans = BudgetPlans.Where(x => x.WorkUnitName.ToString() == Attachment.WorkUnitName).ToList();
                    }

                    foreach (var BudgetPlan in BudgetPlans)
                    {


                       
                        var Months = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == BudgetPlan.BudgetPlanID).ToList();
                        foreach (var Month in Months)
                        {
                            table.Rows.Add(Month.BudgetPlanMonthlyID, BudgetPlan.Branch.Name, BudgetPlan.WorkUnitName, BudgetPlan.AccountCode, BudgetPlan.AccountDesc, Month.GLPeriod.Name, BudgetPlan.BudgetCurrency.Name, Month.CurrentBudget);
                        }
                    }
                }
                if (table.Rows.Count == 0)
                {

                    table.Rows.Add(0, 0, "", "", "", "", 0);
                }




                using (ExcelPackage excelPackage = new ExcelPackage())
                {


                    //create a WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");


                    var ws = excelPackage.Workbook.Worksheets.Add("BudgetPlan");



                    //add all the content from the DataTable, starting at cell A1
                    worksheet.Cells["A1"].LoadFromDataTable(table, true);

                    Response.BinaryWrite(excelPackage.GetAsByteArray());
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=BudgetPlan.xlsx");

                }



                return null;

            }
            return null;

        }


        public ActionResult DownloadTemplate()
        {


            db = new ExceedDbContext();
            try
            {
                string filename = "BudgetPlanTemplate.xlsx";
                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Import/Budget/" + filename));
                response.BinaryWrite(datas);
                response.End();

                return File(filename, "application/vnd.ms-excel");
            }

            catch (Exception)
            {

                // throw;
            }

            return null;
        }

        public ActionResult Remove(string[] fileNames, int id)
        {
            // The parameter of the Remove action must be called "fileNames"
            var attachments = db.Attachments.Where(a => a.Reference == id.ToString());
            db = new ExceedDbContext();
            if (attachments.Count() > 0)
            {
                if (fileNames != null)
                {
                    foreach (var a in attachments)
                    {
                        foreach (var fullName in fileNames)
                        {

                            var fileName = Path.GetFileName(fullName);
                            if (a.Description == fileName)
                            {

                                if (System.IO.File.Exists(a.URL))
                                {
                                    // The files are not actually removed in this demo
                                    System.IO.File.Delete(a.URL);
                                }

                                db.Attachments.Attach(a);
                                db.Attachments.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }
                }

            }
            // Return an empty string to signify success
            return Content("");
        }




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
