using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class OrganizationChartController : Controller
    {
        // GET: HRM/OrganizationChart
        private static ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
           
            return View();
        }

        public JsonResult GetData()
        {
            List<OrgChartViewModel> structure = new List<OrgChartViewModel>();
            structure = GetAllStructure();
            var root = structure.FirstOrDefault(x => x.parent == 0);

            setChildren(root, structure);
            return Json(root, JsonRequestBehavior.AllowGet);
        }
        public void setChildren(OrgChartViewModel orgModel,List<OrgChartViewModel> workunitList)
        {
            var childs = workunitList.Where(x => x.parent == orgModel.relationship).ToList();
            if(childs.Any())
            {
                foreach(var child in childs)
                {
                    setChildren(child,workunitList);
                    orgModel.children.Add(child);
                }
            }
           
        }
        public static List<OrgChartViewModel> GetAllStructure()
        {
            var structureList = new List<OrgChartViewModel>();
            var workunits = db.MainCompanyStructures.ToList();
            if(workunits.Any())
            {
                foreach(var workunit in workunits)
                {
                    var structure = new OrgChartViewModel();
                    structure.relationship = workunit.OrgStructureId;
                    structure.name = workunit.Name;
                    structure.parent = workunit.Parent;
                    var depohead = db.Placements.FirstOrDefault(x=>x.OrgStructureId == workunit.OrgStructureId && x.isCurrent && x.IsHead);
                    if(depohead != null)
                    {
                        var headName = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId  == depohead.EmployeeId);
                        if(headName != null)
                        {
                            structure.head = headName.FullNameAmharic;
                        }
                   
                    }

                         structureList.Add(structure);
                }
               
            }

            return structureList;
        }
    }

    public class OrgChartViewModel
    {
        public int relationship { get; set; }
        public string name { get; set; }
        public int? parent { get; set; }
        public IList<OrgChartViewModel> children { get; set; }
        public int id { get; set; }
        public string head { get; set; }
        public string headPhoto { get; set; }
        public OrgChartViewModel()
        {
            children = new List<OrgChartViewModel>();
        }
    }
    
}