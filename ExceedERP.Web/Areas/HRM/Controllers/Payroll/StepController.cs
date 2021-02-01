
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;

using System.Data.OleDb;
using System.Net;
using System.Web;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
    public class StepController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Company_Admin = "HR_Company_Admin";
        private const string HR_Company_User = "HR_Company_User";
        private const string HR_Admin = "HR_Admin";
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }
        private IQueryable<Step> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Step> structures)
        {
            IQueryable<Step> lookupsList = new Step[] { }.AsQueryable(); ;
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
                            if (descriptor.Member == "StepName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (lookupsList.Any())
                                {
                                    var person =
                                        structures.ToList().Where(
                                            j =>
                                                j.StepName.ToUpper(XAPI.Common.exceedculture)
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
                                                   j.StepName.ToUpper(XAPI.Common.exceedculture)
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
          [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
        public ActionResult Steps_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Step> steps = db.Steps;
          
                if (request.Filters != null)
                {
                dd: if (request.Filters.Any())
                    {
                        foreach (var filter in request.Filters)
                        {
                            var descriptor = filter as FilterDescriptor;
                            if (descriptor != null)
                            {
                                if (descriptor.Member == "StepName")
                                {
                                    steps = ModifyFilters(request.Filters, steps).AsQueryable();

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
            DataSourceResult result = steps.ToDataSourceResult(request, step => new {
                StepId = step.StepId,
                StepName = step.StepName,
                Remark = step.Remark,
                DateCreated = step.DateCreated,
                LastModified = step.LastModified,
                CreatedBy = step.CreatedBy,
                ModifiedBy = step.ModifiedBy,
                StepSequence = step.StepSequence
            });

            return Json(result);
        }
          [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Steps_Create([DataSourceRequest]DataSourceRequest request, Step step)
          {
              ModelState.Clear();
            if (ModelState.IsValid)
            {
                var salsries = db.Steps.Where(x => x.StepSequence == step.StepSequence || x.StepName == step.StepName).FirstOrDefault();
                if (salsries != null)
                {
                    ModelState.AddModelError("", "Step sequence or name must be unique!!");
                }
                else
                {
                    var entity = new Step
                    {
                        StepName = step.StepName,
                        Remark = step.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = step.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = step.ModifiedBy,
                        StepSequence = step.StepSequence
                    };

                    db.Steps.Add(entity);
                    db.SaveChanges();
                    step.StepId = entity.StepId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll step", "Success", User.Identity.Name, $"created step {entity.StepName} ", Modules.Payroll);
                    #endregion
                }
            }
            return Json(new[] { step }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Steps_Update([DataSourceRequest]DataSourceRequest request, Step step)
          {
              ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Step
                {
                    StepId = step.StepId,
                    StepName = step.StepName,
                    Remark = step.Remark,
                    DateCreated = step.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy =step .CreatedBy ,
                    ModifiedBy = User.Identity.Name,
                    StepSequence = step.StepSequence
                };

                db.Steps.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll step", "Success", User.Identity.Name, $"updated step {entity.StepName} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { step }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Company_Admin + "," + "," + HR_Admin + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Steps_Destroy([DataSourceRequest]DataSourceRequest request, Step step)
          {
              ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Step
                {
                    StepId = step.StepId,
                    StepName = step.StepName,
                    Remark = step.Remark,
                    DateCreated = step.DateCreated,
                    LastModified = step.LastModified,
                    CreatedBy = step.CreatedBy,
                    ModifiedBy = step.ModifiedBy,
                    StepSequence = step.StepSequence
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted step " + step.StepName,Modules.Payroll);
                #endregion
                db.Steps.Attach(entity);
                db.Steps.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { step }.ToDataSourceResult(request, ModelState));
        }
          public ActionResult Step_Import([DataSourceRequest] DataSourceRequest request,
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
              List<Step> issueImports = new List<Step>();
              if (ds.Tables[0].Rows.Count > 0)
              {
                  for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                  {
                      Step import = new Step
                      {
                          StepName = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                          StepSequence = Convert.ToInt32( ds.Tables[0].Rows[j].ItemArray[1].ToString()),
                         Remark = ds.Tables[0].Rows[j].ItemArray[2].ToString()

                      };

                      issueImports.Add(import);
                     
                  }
              }
              foreach (var step in issueImports)
              {
                  var salsries =
                      db.Steps.Where(x => x.StepSequence == step.StepSequence || x.StepName == step.StepName)
                          .FirstOrDefault();
                  if (salsries != null)
                  {
                      ModelState.AddModelError("", "Step sequence or name must be unique!!");
                  } 
                  else
                  {
                  var entity = new Step
                  {
                      StepName = step.StepName,
                      StepSequence = step.StepSequence,
                      Remark = step.Remark,
                      DateCreated = DateTime.Now,
                      LastModified = step.LastModified,
                      CreatedBy = User.Identity.Name,
                      ModifiedBy = step.ModifiedBy,
                      OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                  };

                  db.Steps.Add(entity);
                  db.SaveChanges();
                  step.StepId = entity.StepId;
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
