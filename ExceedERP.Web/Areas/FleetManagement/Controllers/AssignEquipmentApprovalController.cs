using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
       [AuthorizeRoles(FleetManagementRoles.ResourceApprover)] 
      
    public class AssignEquipmentApprovalController : Controller
    {
        //

        // GET: /FleetManagement/AssignEquipmentApproval/
     ExceedDbContext db = new  ExceedDbContext();
     IAssignedEquipmentService _EquipmentPlanandEvaluationsService;

        public AssignEquipmentApprovalController()
        {
            _EquipmentPlanandEvaluationsService = new AssignedEquipmentService();
        }
        public ActionResult Index()
        {
           
            return View();
        }
         
	
	}
}