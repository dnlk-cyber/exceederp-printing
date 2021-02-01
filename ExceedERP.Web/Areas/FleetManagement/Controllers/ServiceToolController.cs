using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      [AuthorizeRoles(FleetManagementRoles.ServiceUser)] 
    public class ServiceToolController : Controller
    {
         IServiceToolService _ServiceToolService;

        public ServiceToolController()
        {
            _ServiceToolService = new ServiceToolService();

        }

        //
        // GET: /FleetManagement/ServiceTool/
 

        public ActionResult ServiceTool_Read([DataSourceRequest] DataSourceRequest request)
        {
            var ServiceTool = _ServiceToolService.GetAll().Select(i => new ServiceTool

            {
                ServiceToolID = i.ServiceToolID,
                ServiceID = i.ServiceID,
                Date = i.Date,
                ServiceName = i.MaintenanceTools.ToolsName

            }

            );
            return Json(ServiceTool.ToDataSourceResult(request));
        }


      
        public ActionResult ServiceTool_Create(string tserviceID, [DataSourceRequest] DataSourceRequest request, ServiceTool ServiceTool)
        {
            ModelState.Remove("ServiceToolID");
            ModelState.Remove("ServiceID");
            ModelState.Remove("MaintenanceToolID");
            IMaintenanceToolService ToolService=new MaintenanceToolService();
            Guid IdentityID = ToolService.GetAll().Where(x => x.ToolsName == ServiceTool.ServiceName).Select(x => x.MaintenanceToolID).FirstOrDefault();

            List<ServiceTool> Services = _ServiceToolService.GetAll().Where(f => f.MaintenanceToolID == IdentityID && f.ServiceID == Guid.Parse(tserviceID)).ToList();
            if (Services.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingName", "  Tool Already Exists in Service ");
                }
            else
                {
                    ServiceTool.ServiceToolID = Guid.NewGuid();
                         ServiceTool.MaintenanceToolID = IdentityID;
                    ServiceTool.ServiceID = Guid.Parse(tserviceID);
                    ServiceTool.CreatedBy = User.Identity.Name;
                    ServiceTool.DateCreated = DateTime.Now;
                    ServiceTool.ApprovalStatus = ApprovalStatuses.Pending;
                    if (ServiceTool != null)
                        {
                            _ServiceToolService.Insert(ServiceTool);
                        }
                }
            return Json(new[] { ServiceTool }.ToDataSourceResult(request,ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceTool_Approve([DataSourceRequest] DataSourceRequest request, ServiceTool ServiceTool)
        {
            if (ServiceTool != null)
            {
                if (ServiceTool.ApprovalStatus == ApprovalStatuses.Approve)
                {
                    ServiceTool.IsOnlineApproved = true;
                    ServiceTool.OnlineApprovedTime = DateTime.Now;
                    ServiceTool.OnlineApprovedBy = User.Identity.Name;
                }
                _ServiceToolService.Update(ServiceTool);
            }

            return Json(new[] { ServiceTool }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceTool_Update([DataSourceRequest] DataSourceRequest request, ServiceTool ServiceTool)
        {
            if (ServiceTool != null)
            {
                ServiceTool.ModifiedBy = User.Identity.Name;
                ServiceTool.LastModified = DateTime.Now;
                _ServiceToolService.Update(ServiceTool);
            }

            return Json(new[] { ServiceTool }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceTool_Destroy([DataSourceRequest] DataSourceRequest request, ServiceTool serviceTool)
        {
            var ServiceTool = _ServiceToolService.GetById(serviceTool.ServiceToolID);
            if (ServiceTool != null)
            {
                _ServiceToolService.Delete(ServiceTool);
            }

            return Json(new[] { ServiceTool }.ToDataSourceResult(request));
        }

        public JsonResult ServiceToolByService(string tserviceID, [DataSourceRequest] DataSourceRequest request)
        {
            _ServiceToolService = new ServiceToolService();
            return Json(_ServiceToolService.GetAll().Where(s => s.ServiceID == Guid.Parse(tserviceID.ToString())).Select(f => 
                new ServiceTool { ServiceToolID = f.ServiceToolID,ServiceID=f.ServiceID,MaintenanceToolID = f.MaintenanceToolID, ServiceName = f.ServiceName, Date = f.Date, ApprovalStatus =f.ApprovalStatus , Remark = f.Remark , DateCreated = f.DateCreated , CreatedBy = f.CreatedBy }).ToDataSourceResult(request));
        }

    }
}
