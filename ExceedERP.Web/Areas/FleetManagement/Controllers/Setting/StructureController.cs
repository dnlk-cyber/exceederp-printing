using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class StructureController : Controller
        {

        private IStructureService _StructureService;

        ExceedDbContext db = new ExceedDbContext();

        public StructureController()
            {

            _StructureService=new StructureService();
            }

        // GET: FleetManagement/Structure
        public ActionResult Index()
        {
            return View();
        }
   
    
      public ActionResult StructureDDBind()
        {
            List<MainCompanyStructure> structures = new List<MainCompanyStructure>();
            structures = db.MainCompanyStructures.ToList();
            
      
            return Json(structures, JsonRequestBehavior.AllowGet);
        }
    }
    
    
    }
