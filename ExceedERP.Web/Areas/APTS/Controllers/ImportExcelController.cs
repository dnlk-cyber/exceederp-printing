using ExceedERP.Core;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class ImportExcelController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: APTS/ImportExcel
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DrugUnit_Read([DataSourceRequest]DataSourceRequest request)
        {
            var medisince = db.ImportExcels.ToList();
            DataSourceResult result = medisince.ToDataSourceResult(request, unit => new
            {
                ImportExcelId = unit.ImportExcelId,
                Name = unit.Name,
                Age = unit.Age


            });

            return Json(result);
        }
        public ActionResult DrugUnit_Create([DataSourceRequest]DataSourceRequest request, ImportExcel unit)
        {
            if (ModelState.IsValid)
            {
                var entity = new ImportExcel
                {
                    ImportExcelId = unit.ImportExcelId,
                    Name = unit.Name,
                    Age = unit.Age
                };
                db.ImportExcels.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugUnit_Update([DataSourceRequest]DataSourceRequest request, ImportExcel unit)
        {
            if (ModelState.IsValid)
            {
                var entity = new ImportExcel
                {
                    ImportExcelId = unit.ImportExcelId,
                    Name = unit.Name,
                    Age = unit.Age
                };

                db.ImportExcels.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugUnit_Destroy([DataSourceRequest]DataSourceRequest request, ImportExcel unit)
        {
            if (ModelState.IsValid)
            {

                var entity = new ImportExcel
                {
                    ImportExcelId = unit.ImportExcelId,
                    Name = unit.Name,
                    Age = unit.Age
                };
                db.ImportExcels.Attach(entity);
                db.ImportExcels.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ImportQuantitiesFromExcel(IEnumerable<HttpPostedFileBase> file)
        {
            if (file != null)
            {
                string fileNamePrefix = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

                //return new JavaScriptResult() { Script = "alert('" + "Data Copied Successfully!!" + "');" };
                DataSet ds = new DataSet();
                #region import

                foreach (HttpPostedFileBase fileBase in file)
                {
                    var postedFileBase = Request.Files["file"];
                    if (postedFileBase != null)
                    {
                        string fileExtension = System.IO.Path.GetExtension(postedFileBase.FileName);
                        if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                        {
                            string fileLocation = Server.MapPath("~/App_Data/Images/") + fileNamePrefix +
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
                #endregion

                string errors = "";
                //decimal debit = 0;
                //decimal credit = 0;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                    {
                        ImportExcel import = new ImportExcel
                        {
                            Name = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                            Age = Convert.ToInt32(ds.Tables[0].Rows[j].ItemArray[1]),
                        };

                        db.ImportExcels.Add(import);
                        db.SaveChanges();
                        //var workItem = new GenericWorkItemPerProject();
                        //var id = 0;
                        //var quantity = new decimal();
                        //var idInString = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                        //var qtyInString = ds.Tables[0].Rows[j].ItemArray[6].ToString();
                        //try
                        //{
                        //    id = int.Parse(idInString);
                        //    //workItem = db.GenericWorkItemPerProject.Find(id);
                        //}
                        //catch (Exception e)
                        //{
                        //    errors = errors + " Id should be a number : " + idInString + "\n";
                        //}
                        //try
                        //{
                        //    quantity = decimal.Parse(qtyInString);
                        //}
                        //catch (Exception e)
                        //{
                        //    errors = errors + " Quantity of Work should be decimal: " + qtyInString + "\n";
                        //}
                        //var workItem = db.ImportExcels.Find(id);
                        //if (workItem != null)
                        //{
                        //    workItem.TotalQuantityOfWork = quantity;

                        //    db.ImportExcels.Attach(workItem);
                        //    db.Entry(workItem).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                    }
                }
            }

            return RedirectToAction("Index");
        }

    }
}