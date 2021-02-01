using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class OmittedOrAddedWorksController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/OmittedOrAddedWorks
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            //var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            //ViewData["PeriodList"] = periods;

            ////ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            //var branches = db.Branch.ToList();
            //ViewData["Branch"] = branches;

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            //var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            //ViewData["StructureType"] = lis;

            //var workItemCategories = from w in db.WorkCategory
            //                         join p in db.ProjectTypeMappings
            //                             on w.ConfigurationId equals p.ConfigurationId
            //                         select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });

            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.WorkCategoryId, c.Name, c.ConfigurationId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(es => new { es.ItemName, es.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();



            //var internalEquipment = db.InternalMachinaryRent.ToList();
            //var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;

            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projects = db.ProjectInformation.Where(x => x.ProjectStatus != ProjectStatus.CLOSED && ids.Contains(x.ProjectInformationId));            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projects.ToDataSourceResult(request, projectInformation => new
            {
                ProjectType = projectInformation.ProjectType,
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,

                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus

            });

            return Json(result);
        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}