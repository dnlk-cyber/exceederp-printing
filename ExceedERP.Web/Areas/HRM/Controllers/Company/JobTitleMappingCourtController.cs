using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using static ExceedERP.Web.Areas.HRM.Controllers.Company.CompanyChartController;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
   
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class JobTitleMappingCourtController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
        public static int JobCode { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var Nodes = db.MainCompanyStructures.ToList();
            AllJobCodes = new List<MainCompanyStructure>();
            AllJobCodes = Nodes;
            this.ViewBag.Tree = GetCHildrens(Nodes);
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();
            ViewData["jobTitles"] = jobTitle;
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
        }
        private List<NodeViewModel> GetCHildrens(List<MainCompanyStructure> Nodes)
        {
            NodeViewModel root = new NodeViewModel();
            List<NodeViewModel> items = new List<NodeViewModel>();
            if (Nodes.Count > 0)
            {
                foreach (var nod in Nodes)
                {
                    root = new NodeViewModel { Id = nod.OrgStructureId, Title = nod.Name, ParentId = nod.Parent };
                    items.Add(root);
                }
            }
            return BuildTree(items);
        }
        public List<NodeViewModel> BuildTree(IEnumerable<NodeViewModel> source)
        {
            if (source.Count() > 0)
            {
                var groups = source.GroupBy(i => i.ParentId);

                var roots = groups.FirstOrDefault(g => g.Key == 0).ToList();

                if (roots.Count > 0)
                {
                    var dict = groups.ToDictionary(g => g.Key.Value, g => g.ToList());
                    for (int i = 0; i < roots.Count; i++)
                        AddChildren(roots[i], dict);
                }
                return roots;
            }
            return null;

        }

        private static void AddChildren(NodeViewModel node, IDictionary<int, List<NodeViewModel>> source)
        {
            if (source.ContainsKey(node.Id))
            {
                node.Children = source[node.Id];
                for (int i = 0; i < node.Children.Count; i++)
                    AddChildren(node.Children[i], source);
            }
            else
            {
                node.Children = new List<NodeViewModel>();
            }
        }
       
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult JobTitleMapping_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobTitleMapping> jobtitlemapping = db.JobTitleMapping;
            DataSourceResult result = jobtitlemapping.ToDataSourceResult(request, jobTitleMapping => new
            {
                JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                OrgStructureId = jobTitleMapping.OrgStructureId,
                LookupId = jobTitleMapping.LookupId,
                //MainFunction = jobTitleMapping.MainFunction,
                Quntity = jobTitleMapping.Quntity,
                Remark = jobTitleMapping.Remark,
                PlanType = jobTitleMapping.PlanType,
                DateCreated = jobTitleMapping.DateCreated,
                LastModified = jobTitleMapping.LastModified,
                CreatedBy = jobTitleMapping.CreatedBy,
                ModifiedBy = jobTitleMapping.ModifiedBy,
                WorkUnitId = jobTitleMapping.WorkUnitId,
                BranchId = jobTitleMapping.BranchId,
            });

            return Json(result);
        }
  
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Create([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var mappings = db.JobTitleMapping.Where(x => x.LookupId == jobTitleMapping.LookupId && x.OrgStructureId == JobCode).FirstOrDefault();
                if (mappings != null)
                {
                    ModelState.AddModelError("Please Check", "This Mapping is already exist");
                }
                else
                {

                    var entity = new JobTitleMapping
                    {
                        OrgStructureId = JobCode,
                        LookupId = jobTitleMapping.LookupId,
                        //MainFunction = jobTitleMapping.MainFunction,
                        Quntity = jobTitleMapping.Quntity,
                        Remark = jobTitleMapping.Remark,
                        PlanType = jobTitleMapping.PlanType,
                        DateCreated = DateTime.Now,
                        LastModified = jobTitleMapping.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = jobTitleMapping.ModifiedBy,
                        WorkUnitId = jobTitleMapping.WorkUnitId,
                        BranchId = jobTitleMapping.BranchId,
                    };

                    db.JobTitleMapping.Add(entity);
                    db.SaveChanges();
                    jobTitleMapping.JobTitleMappingId = entity.JobTitleMappingId;
                }
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Update([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMapping
                {
                    JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                    OrgStructureId = JobCode,
                    LookupId = jobTitleMapping.LookupId,
                    //MainFunction = jobTitleMapping.MainFunction,
                    Quntity = jobTitleMapping.Quntity,
                    Remark = jobTitleMapping.Remark,
                    WorkUnitId = jobTitleMapping.WorkUnitId,
                    BranchId = jobTitleMapping.BranchId,
                    PlanType = jobTitleMapping.PlanType,
                    DateCreated = jobTitleMapping.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = jobTitleMapping.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.JobTitleMapping.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTitleMapping_Destroy([DataSourceRequest]DataSourceRequest request, JobTitleMapping jobTitleMapping)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new JobTitleMapping
                {
                    JobTitleMappingId = jobTitleMapping.JobTitleMappingId,
                    OrgStructureId = jobTitleMapping.OrgStructureId,
                    LookupId = jobTitleMapping.LookupId,
                    //MainFunction = jobTitleMapping.MainFunction,
                    Quntity = jobTitleMapping.Quntity,
                    Remark = jobTitleMapping.Remark,
                    PlanType = jobTitleMapping.PlanType,
                    DateCreated = jobTitleMapping.DateCreated,
                    LastModified = jobTitleMapping.LastModified,
                    CreatedBy = jobTitleMapping.CreatedBy,
                    ModifiedBy = jobTitleMapping.ModifiedBy,
                    WorkUnitId = jobTitleMapping.WorkUnitId,
                    BranchId = jobTitleMapping.BranchId,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted job title mapping of structure " + jobTitleMapping.OrgStructureId + " job title" + jobTitleMapping.LookupId + " branch " + jobTitleMapping.BranchId);
                #endregion
                db.JobTitleMapping.Attach(entity);
                db.JobTitleMapping.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobTitleMapping }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Job title mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Job title mapping");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public void GetJobCode(string jobName)
        {

            if (AllJobCodes == null || AllJobCodes.Count <= 0) return;

            var mainCompanyStructure = AllJobCodes.Where(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture))).FirstOrDefault();

            if (mainCompanyStructure != null)
                JobCode = mainCompanyStructure.OrgStructureId;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
