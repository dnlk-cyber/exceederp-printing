using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class MaintainStructureController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/MaintainStructure
        public ActionResult Index()
        {
            GetOrganizations();
          //  GetStructureType();
            GetStructures();

            return View();
        }
        //public JsonResult GetStructureType()
        //{
        //    var Types = Enum.GetValues(typeof(OrganizationUnitDefinitionType))
        //                 .Cast<OrganizationUnitDefinitionType>().ToList();

        //    var TypesList = Types.Select(s => new { Text = s.ToString(), Value = s.ToString() });
        //    ViewData["Type"] = TypesList;
        //    return Json(Types.Select(s => new
        //    {
        //        Text = s.ToString(),
        //        Value = s.ToString()
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetOrganizations()
        {
            db = new ExceedDbContext();
            var organizationUnits = db.MainCompanyStructures.ToList();

            ViewData["Parent"] = organizationUnits;

            return Json(organizationUnits.Select(s => new
            {
                OrgStructureId = s.OrgStructureId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructures()
        {
            db = new ExceedDbContext();
            var structures = db.Structure;

            ViewData["structures"] = structures;

            return Json(structures.Select(s => new
            {
                StructureId = s.StructureId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
    }
}