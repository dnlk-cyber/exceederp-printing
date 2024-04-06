using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class CompanyChartController : BaseController
    {
        // GET: HRM/CompanyChart
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
          
            var Nodes = db.MainCompanyStructures.Where(x=>x.IsActive).ToList();

            this.ViewBag.Tree = GetCHildrens(Nodes);
            return View();
        }
        private List<NodeViewModel> GetCHildrens(List<MainCompanyStructure> Nodes)
        {
            NodeViewModel root = new NodeViewModel();
            List<NodeViewModel> items = new List<NodeViewModel>();
            var lookups = db.MainCompanyStructures.Where(x=>x.IsActive);
            if (Nodes.Any())
            {
                foreach (var nod in Nodes)
                {
                    root = new NodeViewModel { Id = nod.OrgStructureId, Title = nod.Name , ParentId = nod.Parent };
                    items.Add(root);
                }
            }
            return BuildTree(items);
        }
        public List<NodeViewModel> BuildTree(IEnumerable<NodeViewModel> source)
        {
            if (!source.Any()) return new List<NodeViewModel>();
            var groups = source.GroupBy(i => i.ParentId);

            var roots = groups.FirstOrDefault(g => g.Key == 0).ToList();

            if (roots.Any())
            {
                var dict = groups.ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }

            return roots;
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

    }

    public class NodeViewModel
    {
        public NodeViewModel()
        {
            this.Expanded = false;
            this.Children = new List<NodeViewModel>();
            
           
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public bool Expanded { get; set; }

        public bool HasChildren
        {
            get { return Children.Any(); }
        }

        public List<NodeViewModel> Children { get;  set; }
    }
}