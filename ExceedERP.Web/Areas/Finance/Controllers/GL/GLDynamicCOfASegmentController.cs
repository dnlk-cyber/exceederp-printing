using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlDynamicCofASegmentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var glSegmentSetups = _db.GlSegmentSetups;
            ViewData["SetupSegments"] = glSegmentSetups;
        }

        public ActionResult GlChartOfAccountSegments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLChartOfAccountSegment> glchartofaccountsegments = _db.GlChartOfAccountSegments;
            DataSourceResult result = glchartofaccountsegments.ToDataSourceResult(request, gLChartOfAccountSegment => new {
                GLSegmentSetupId = gLChartOfAccountSegment.GLSegmentSetupId,
                GLChartOfAccountSegmentId = gLChartOfAccountSegment.GLChartOfAccountSegmentId,
                Values = gLChartOfAccountSegment.Values,
                Description = gLChartOfAccountSegment.Description,
                HasParent = gLChartOfAccountSegment.HasParent,
                Enable = gLChartOfAccountSegment.Enable,
                Parent = gLChartOfAccountSegment.Parent,
                AllowBudgeting = gLChartOfAccountSegment.AllowBudgeting,
                AllowPosting = gLChartOfAccountSegment.AllowPosting,
                Type = gLChartOfAccountSegment.Type,
                OrgBranchName = gLChartOfAccountSegment.OrgBranchName,
                DateCreated = gLChartOfAccountSegment.DateCreated,
                LastModified = gLChartOfAccountSegment.LastModified,
                CreatedBy = gLChartOfAccountSegment.CreatedBy,
                ModifiedBy = gLChartOfAccountSegment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountSegments_Create([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSegment gLChartOfAccountSegment)
        {
            #region validation
            var rule = _db.GlSegmentSetups.Find(gLChartOfAccountSegment.GLSegmentSetupId);
            if (rule != null && rule.SegmentLength != gLChartOfAccountSegment.Values.Length)
                ModelState.AddModelError("length", $"The account length should be {rule.SegmentLength}");

            var exist = _db.GlChartOfAccountSegments
                .Where(x => x.GLSegmentSetupId == gLChartOfAccountSegment.GLSegmentSetupId)
                .Where(x => x.Values == gLChartOfAccountSegment.Values).ToList();

            if (exist.Any())
                ModelState.AddModelError("exist", $"The account value {gLChartOfAccountSegment.Values} already exists.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountSegment
                {
                    GLSegmentSetupId = gLChartOfAccountSegment.GLSegmentSetupId,
                    Values = gLChartOfAccountSegment.Values,
                    Description = gLChartOfAccountSegment.Description,
                    HasParent = gLChartOfAccountSegment.HasParent,
                    Enable = gLChartOfAccountSegment.Enable,
                    Parent = gLChartOfAccountSegment.Parent,
                    Type = gLChartOfAccountSegment.Type,
                    AllowBudgeting = gLChartOfAccountSegment.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSegment.AllowPosting,
                    OrgBranchName = gLChartOfAccountSegment.OrgBranchName,
                    DateCreated = gLChartOfAccountSegment.DateCreated,
                    LastModified = gLChartOfAccountSegment.LastModified,
                    CreatedBy = gLChartOfAccountSegment.CreatedBy,
                    ModifiedBy = gLChartOfAccountSegment.ModifiedBy
                };

                _db.GlChartOfAccountSegments.Add(entity);
                _db.SaveChanges();
                gLChartOfAccountSegment.GLChartOfAccountSegmentId = entity.GLChartOfAccountSegmentId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL dynamic chart of account segment ", "Success", User.Identity.Name, $"created segment {entity.Values} under {entity.GlSegmentSetup.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountSegment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountSegments_Update([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSegment gLChartOfAccountSegment)
        {
            #region validation
            var rule = _db.GlSegmentSetups.Find(gLChartOfAccountSegment.GLSegmentSetupId);
            if (rule != null && rule.SegmentLength != gLChartOfAccountSegment.Values.Length)
                ModelState.AddModelError("length", $"The account length should be {rule.SegmentLength}");

            var exist = _db.GlChartOfAccountSegments
                .Where(x => x.GLChartOfAccountSegmentId != gLChartOfAccountSegment.GLChartOfAccountSegmentId)
                .Where(x => x.GLSegmentSetupId == gLChartOfAccountSegment.GLSegmentSetupId)
                .Where(x => x.Values == gLChartOfAccountSegment.Values).ToList();

            if(exist.Any())
                ModelState.AddModelError("exist", $"The account value {gLChartOfAccountSegment.Values} already exists.");
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountSegment
                {
                    GLSegmentSetupId = gLChartOfAccountSegment.GLSegmentSetupId,
                    GLChartOfAccountSegmentId = gLChartOfAccountSegment.GLChartOfAccountSegmentId,
                    Values = gLChartOfAccountSegment.Values,
                    Description = gLChartOfAccountSegment.Description,
                    HasParent = gLChartOfAccountSegment.HasParent,
                    Enable = gLChartOfAccountSegment.Enable,
                    Parent = gLChartOfAccountSegment.Parent,
                    AllowBudgeting = gLChartOfAccountSegment.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSegment.AllowPosting,
                    Type = gLChartOfAccountSegment.Type,
                    OrgBranchName = gLChartOfAccountSegment.OrgBranchName,
                    DateCreated = gLChartOfAccountSegment.DateCreated,
                    LastModified = gLChartOfAccountSegment.LastModified,
                    CreatedBy = gLChartOfAccountSegment.CreatedBy,
                    ModifiedBy = gLChartOfAccountSegment.ModifiedBy
                };

                _db.GlChartOfAccountSegments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL dynamic chart of account segment ", "Success", User.Identity.Name, $"updated segment {entity.Values} under {entity.GlSegmentSetup.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountSegment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountSegments_Destroy([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSegment gLChartOfAccountSegment)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountSegment
                {
                    GLSegmentSetupId = gLChartOfAccountSegment.GLSegmentSetupId,
                    GLChartOfAccountSegmentId = gLChartOfAccountSegment.GLChartOfAccountSegmentId,
                    Values = gLChartOfAccountSegment.Values,
                    Description = gLChartOfAccountSegment.Description,
                    HasParent = gLChartOfAccountSegment.HasParent,
                    Enable = gLChartOfAccountSegment.Enable,
                    Parent = gLChartOfAccountSegment.Parent,
                    AllowBudgeting = gLChartOfAccountSegment.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSegment.AllowPosting,
                    OrgBranchName = gLChartOfAccountSegment.OrgBranchName,
                    DateCreated = gLChartOfAccountSegment.DateCreated,
                    LastModified = gLChartOfAccountSegment.LastModified,
                    CreatedBy = gLChartOfAccountSegment.CreatedBy,
                    ModifiedBy = gLChartOfAccountSegment.ModifiedBy
                };

                _db.GlChartOfAccountSegments.Attach(entity);
                _db.GlChartOfAccountSegments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLChartOfAccountSegment }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult Enable_Disable(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var account = _db.GlChartOfAccountSegments.FirstOrDefault(p => p.GLChartOfAccountSegmentId == did);
                if (account != null)
                {
                    account.Enable = !account.Enable;
                    _db.GlChartOfAccountSegments.Attach(account);
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "Successfully Updated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

              
                
            }

            string errMsg = "Record not found";
            var formattedData1 = new
            {
                Message = errMsg,
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
