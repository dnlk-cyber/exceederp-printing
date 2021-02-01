using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using static ExceedERP.Web.Areas.HRM.Controllers.Company.CompanyChartController;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin, HrmConstants.PayrollAdmin)]
    public class BranchCostCenterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
        public static List<Branch> branches { get; set; }
        public static int JobCode { get; set; }
        public static int branchId { get; set; }
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
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var costcenters = db.GLChartOfAccountCostCenters;
            ViewData["costCenter"] = costcenters;
            var orgs = db.MainCompanyStructures;
            ViewData["Orgs"] = orgs;
        }
        public ActionResult GetCostCenter(int BranchId)
        {
            GlCostCenterTypes type;
            List<GLChartOfAccountCostCenter> costcenterList = new List<GLChartOfAccountCostCenter>();
            var branch = db.Branch.Where(x => x.BranchId == BranchId).FirstOrDefault();
            if (branch != null)
            {
                if (branch.BranchTypes == BranchTypes.HEAD_OFFICE)
                {
                    type = GlCostCenterTypes.HeadOffice;
                }
                else
                {
                    type = GlCostCenterTypes.Project;
                }
                costcenterList = db.GLChartOfAccountCostCenters.Where(x => x.Type == type && x.Enable && !x.Parent).ToList();
            }

            return Json(costcenterList.Select(p => new { Values = p.Values, Description = p.Description }), JsonRequestBehavior.AllowGet);
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
        public JsonResult GetJobTitle()
        {
            db = new ExceedDbContext();
            List<Lookup> titleList = new List<Lookup>();

            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == JobCode).ToList();

            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    titleList.Add(lookup);

                }
            }
            ViewData["Titles"] = titleList;

            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public int GetJobCode(string jobName)
        {

            if (AllJobCodes == null || AllJobCodes.Count <= 0)
            {
                AllJobCodes = db.MainCompanyStructures.ToList();
            }
            var mainCompanyStructure = AllJobCodes.FirstOrDefault(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
            if (mainCompanyStructure != null)
            {
                JobCode = mainCompanyStructure.OrgStructureId;
                GetJobTitle();
            }

            return JobCode;
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
        public ActionResult BranchCostCenter_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BranchCostCenter> branchcostcenter = db.BranchCostCenter;//.Where(x => x.Remark != "System");
            DataSourceResult result = branchcostcenter.ToDataSourceResult(request, branchCostCenter => new
            {
                BranchCostCenterId = branchCostCenter.BranchCostCenterId,
                BranchId = branchCostCenter.BranchId,
                Values = branchCostCenter.Values,
                OrgStructureId = branchCostCenter.OrgStructureId,
                Remark = branchCostCenter.Remark,
                Name = branchCostCenter.MainCompanyStructure.Name
            });
            SetForAllChild();
            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser, HrmConstants.PayrollAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchCostCenter_Create([DataSourceRequest]DataSourceRequest request, BranchCostCenter branchCostCenter)
        {

            var checkExistance = db.BranchCostCenter.FirstOrDefault(x => x.OrgStructureId == JobCode && x.BranchId == branchCostCenter.BranchId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "It already exist in the database!!");
                branchCostCenter.OrgStructureId = JobCode;
                SetForChild(branchCostCenter);
            }
            if (ModelState.IsValid)
            {
                var entity = new BranchCostCenter
                {
                    BranchId = branchCostCenter.BranchId,
                    Values = branchCostCenter.Values,
                    OrgStructureId = JobCode,
                    Remark = branchCostCenter.Remark
                };

                db.BranchCostCenter.Add(entity);
                db.SaveChanges();
                branchCostCenter.BranchCostCenterId = entity.BranchCostCenterId;
                branchCostCenter.OrgStructureId = JobCode;
                SetForChild(branchCostCenter);
            }


            return Json(new[] { branchCostCenter }.ToDataSourceResult(request, ModelState));
        }
        private void SetForAllChild()
        {
            var branchCostCenters = db.BranchCostCenter.ToList();
            foreach (var branchCostCenter in branchCostCenters)
            {

                var mainStructures =
                    db.MainCompanyStructures.Where(x => x.Parent == branchCostCenter.OrgStructureId).ToList();
                foreach (var m in mainStructures)
                {
                    var check =
                        branchCostCenters.Where(
                            x => x.OrgStructureId == m.OrgStructureId && x.BranchId == branchCostCenter.BranchId)
                            .FirstOrDefault();
                    if (check == null)
                    {

                        var entity = new BranchCostCenter
                        {
                            BranchId = branchCostCenter.BranchId,
                            Values = branchCostCenter.Values,
                            OrgStructureId = m.OrgStructureId,
                            Remark = "System"
                        };
                        db = new ExceedDbContext();
                        db.BranchCostCenter.Add(entity);
                        db.SaveChanges();
                        branchCostCenter.BranchCostCenterId = entity.BranchCostCenterId;
                    }
                }
            }

        }
        private void SetForChild(BranchCostCenter branchCostCenter)
        {
            var allCostCenter = db.BranchCostCenter.ToList();
            var mainStructures = db.MainCompanyStructures.Where(x => x.Parent == branchCostCenter.OrgStructureId).ToList();
            foreach (var m in mainStructures)
            {
                var check =
                    allCostCenter
                        .FirstOrDefault(x => x.OrgStructureId == m.OrgStructureId && x.BranchId == branchCostCenter.BranchId);
                if (check == null)
                {

                    var entity = new BranchCostCenter
                    {
                        BranchId = branchCostCenter.BranchId,
                        Values = branchCostCenter.Values,
                        OrgStructureId = m.OrgStructureId,
                        Remark = "System"
                    };

                    db.BranchCostCenter.Add(entity);
                    db.SaveChanges();
                    branchCostCenter.BranchCostCenterId = entity.BranchCostCenterId;
                }
            }

        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser, HrmConstants.PayrollAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchCostCenter_Update([DataSourceRequest]DataSourceRequest request, BranchCostCenter branchCostCenter)
        {


            if (JobCode == 0)
            {
                JobCode = branchCostCenter.OrgStructureId;
            }
            //var checkExistance = db.BranchCostCenter.Where(x => x.OrgStructureId == JobCode && x.BranchId == branchCostCenter.BranchId && x.Values == branchCostCenter.Values).FirstOrDefault();
            //if (checkExistance != null)
            //{
            //    ModelState.AddModelError("", "It already exist in the database!!");
            //}
            if (ModelState.IsValid)
            {

                var entity = new BranchCostCenter
                {
                    BranchId = branchCostCenter.BranchId,
                    Values = branchCostCenter.Values,
                    OrgStructureId = JobCode,
                    BranchCostCenterId = branchCostCenter.BranchCostCenterId,
                    Remark = branchCostCenter.Remark
                };

                db.BranchCostCenter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                // }
            }

            JobCode = 0;
            return Json(new[] { branchCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchCostCenter_Destroy([DataSourceRequest]DataSourceRequest request, BranchCostCenter branchCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new BranchCostCenter
                {
                    BranchCostCenterId = branchCostCenter.BranchCostCenterId,
                    BranchId = branchCostCenter.BranchId,
                    Values = branchCostCenter.Values,
                    OrgStructureId = branchCostCenter.OrgStructureId,
                    Remark = branchCostCenter.Remark
                };
                #region delete log 
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Branch Cost center of branch " + branchCostCenter.BranchId + "Cost center " + branchCostCenter.Values);
                #endregion
                db.BranchCostCenter.Attach(entity);
                db.BranchCostCenter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { branchCostCenter }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Branch cost center");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Branch cost center");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
