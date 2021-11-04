using System;
using System.Web;
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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc;
using WebGrease.Activities;
using Attachment = System.Net.Mail.Attachment;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + Payroll_Admin)]
    public class RankController : BaseController
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
        private IQueryable<Rank> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Rank> structures)
        {
            IQueryable<Rank> lookupsList = new Rank[] { }.AsQueryable(); ;
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
                            if (descriptor.Member == "RankName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (lookupsList.Any())
                                {
                                    var person =
                                        structures.ToList().Where(
                                            j =>
                                                j.RankName.ToUpper(XAPI.Common.exceedculture)
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
                                                   j.RankName.ToUpper(XAPI.Common.exceedculture)
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
        public ActionResult Ranks_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Rank> ranks = db.Ranks;
            if (request.Filters != null)
            {
            dd: if (request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "RankName")
                            {
                                ranks = ModifyFilters(request.Filters, ranks).AsQueryable();

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
            DataSourceResult result = ranks.ToDataSourceResult(request, rank => new {
                RankId = rank.RankId,
                RankName = rank.RankName,
                StartingSalary = rank.StartingSalary,
                MaximumSalary = rank.MaximumSalary,
                Remark = rank.Remark,
                DateCreated = rank.DateCreated,
                LastModified = rank.LastModified,
                CreatedBy = rank.CreatedBy,
                ModifiedBy = rank.ModifiedBy
            });

            return Json(result);
        }
  [Authorize(Roles =  HR_Company_User + "," +  Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Ranks_Create([DataSourceRequest]DataSourceRequest request, Rank rank)
            {
                ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (rank.MaximumSalary < rank.StartingSalary)
                {
                    ModelState.AddModelError("","Maximum Salary should be greater than starting salary!!");
                }
                else
                {
                    var entity = new Rank
                    {
                        RankName = rank.RankName,
                        StartingSalary = rank.StartingSalary,
                        MaximumSalary = rank.MaximumSalary,
                        Remark = rank.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = rank.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = rank.ModifiedBy
                    };

                    db.Ranks.Add(entity);
                    db.SaveChanges();
                    rank.RankId = entity.RankId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll rank", "Success", User.Identity.Name, $"added rank {entity.RankName} starting salary {entity.StartingSalary} maximum salary {entity.MaximumSalary}", Modules.Payroll);
                    #endregion
                }
            }
            return Json(new[] { rank }.ToDataSourceResult(request, ModelState));
        }
           [Authorize(Roles = HR_Company_User + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Ranks_Update([DataSourceRequest]DataSourceRequest request, Rank rank)
  {
      ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (rank.MaximumSalary < rank.StartingSalary)
                {
                    ModelState.AddModelError("", "Maximum Salary should be greater than starting salary!!");
                }
                else
                {
                    var entity = new Rank
                    {
                        RankId = rank.RankId,
                        RankName = rank.RankName,
                        StartingSalary = rank.StartingSalary,
                        MaximumSalary = rank.MaximumSalary,
                        Remark = rank.Remark,
                        DateCreated = rank.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = rank.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.Ranks.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll rank", "Success", User.Identity.Name, $"updated rank {entity.RankName} starting salary {entity.StartingSalary} maximum salary {entity.MaximumSalary}", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { rank }.ToDataSourceResult(request, ModelState));
        }
            [Authorize(Roles = HR_Company_Admin + "," + HR_Admin + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Ranks_Destroy([DataSourceRequest]DataSourceRequest request, Rank rank)
           {
               ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Rank
                {
                    RankId = rank.RankId,
                    RankName = rank.RankName,
                    StartingSalary = rank.StartingSalary,
                    MaximumSalary = rank.MaximumSalary,
                    Remark = rank.Remark,
                    DateCreated = rank.DateCreated,
                    LastModified = rank.LastModified,
                    CreatedBy = rank.CreatedBy,
                    ModifiedBy = rank.ModifiedBy
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted ranks information of rank " + rank.RankName,Modules.Payroll);
                #endregion
                db.Ranks.Attach(entity);
                db.Ranks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { rank }.ToDataSourceResult(request, ModelState));
        }

          public ActionResult Rank_Import([DataSourceRequest] DataSourceRequest request,
              IEnumerable<HttpPostedFileBase> file)
          {
              #region Import

              string addtionalName = DateTime.Now.Year.ToString() +
                  DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                  DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                  DateTime.Now.Second.ToString();
              System.Data.DataSet ds = new System.Data.DataSet();
              Rank issuee = db.Ranks.FirstOrDefault();
              List<Rank> issues = new List<Rank>();
              foreach (HttpPostedFileBase fileBase in file)
              {
                  string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                  if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                  {
                      string fileLocation = Server.MapPath("~/Content/Import/HRM/")+addtionalName + Request.Files["file"].FileName;
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
              List<Rank> issueImports = new List<Rank>();
              if (ds.Tables[0].Rows.Count > 0)
              {
                  for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                  {
                      Rank import = new Rank
                      {
                          RankName = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                          StartingSalary = Convert.ToDecimal(ds.Tables[0].Rows[j].ItemArray[1].ToString()),
                          MaximumSalary = Convert.ToDecimal(ds.Tables[0].Rows[j].ItemArray[2].ToString()),
                          Remark = ds.Tables[0].Rows[j].ItemArray[3].ToString()

                      };

                      issueImports.Add(import);
                      
                  }
              }
              foreach (var rank in issueImports)
                     {
                         if (rank.MaximumSalary < rank.StartingSalary)
                         {
                             ModelState.AddModelError("", "Maximum Salary should be greater than starting salary!!");
                         }
                         else
                         {
                             var entity = new Rank
                             {
                                 RankName = rank.RankName,
                                 StartingSalary = rank.StartingSalary,
                                 MaximumSalary = rank.MaximumSalary,
                                 Remark = rank.Remark,
                                 DateCreated = DateTime.Now,
                                 LastModified = rank.LastModified,
                                 CreatedBy = User.Identity.Name,
                                 ModifiedBy = rank.ModifiedBy,
                                 OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                             };

                             db.Ranks.Add(entity);
                             db.SaveChanges();
                             rank.RankId = entity.RankId;
                         }
                     }

        

                #endregion



                
                return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));

            }
        
          public ActionResult DownloadDoument()
          {


              try
              {
                  //var fileContents = Convert.FromBase64String(base64);
                  //return File(fileContents, contentType, fileName);
                  WebClient req = new WebClient();
                  HttpResponse response = System.Web.HttpContext.Current.Response;
                  response.Clear();
                  response.ClearContent();
                  response.ClearHeaders();
                  response.Buffer = true;
                  response.AddHeader("Content-Disposition",
                      "attachment;filename=\"" + "Rank.xlsx" + "\"");
                  byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/HRM/" + "Rank.xlsx"));
                  response.BinaryWrite(datas);
                  response.End();

                  return File("Rank.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
              }
              catch (System.Exception)
              {
                  
                //  throw;
              }


              return null;

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
                      byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/HRM/" + filename ));
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
