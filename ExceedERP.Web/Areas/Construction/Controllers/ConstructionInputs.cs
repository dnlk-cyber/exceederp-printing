using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers
{
    public class ConstructionInputs
    {
        private ExceedDbContext db = new ExceedDbContext();
        public List<InternalExternalEquipmentViewModel> GetInternalExternalEquipmentViewModel()
        {
            List<InternalExternalEquipmentViewModel> views = new List<InternalExternalEquipmentViewModel>();
            InternalExternalEquipmentViewModel view = new InternalExternalEquipmentViewModel();
            var internalList = db.Fleets.Where(x => x.Status == "Active").AsQueryable();
            var externalList = db.RentOtherSuppliers.AsQueryable();

            foreach (var i in internalList)
            {
                view = new InternalExternalEquipmentViewModel();
                view.AssetNumber = i.FleetsID;
                view.Name = i.EquipmentName;
                view.Model = i.Model;
                view.Year = i.ManufacturerYear;
                view.Type = "Internal";
                view.EquipmentTypeID = i.EquipmentTypeID;
                view.PurchaseCost = i.PurchasingCost;
                views.Add(view);
            }
            foreach (var i in externalList)
            {
                view = new InternalExternalEquipmentViewModel();
                view.AssetNumber = i.RentOtherSuppliersID;
                view.Name = i.Description;
               view.Type = "External";
               view.EquipmentTypeID = i.EquipmentTypeID;
               // view.Year = i.ManufacturedYear;
                views.Add(view);
            }
            var list = views != null ? views.ToList() : new List<InternalExternalEquipmentViewModel>();
            return list;
        }

        public List<MaterialViewModel> GetMaterialViewModel(int projectId)
        {
            List<MaterialViewModel> views = new List<MaterialViewModel>();
            MaterialViewModel view = new MaterialViewModel();
            db = new ExceedDbContext();
            if (projectId ==0)
            {
                return views;
            }
            var workItems= db.WorkItemPerProject.Where(x=> x.ProjectInformationId == projectId ).ToList();
            if (workItems.Any())
            {
           
            var items = db.InventoryItems.ToList();
                foreach (var w in workItems )
                {
                    
                 var material = db.ProjectMaterial.Where (x=> x.WorkItemPerProjectId== w.WorkItemPerProjectId ).AsQueryable();
                    if (material .Any ())
                    {
            foreach (var i in material)
            {

                view = new MaterialViewModel();
                var check = items.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault();
                if (check != null)
                {
                    view.ProjectMaterialId = i.ProjectMaterialId;
                    view.Name = check.Name;                   
                    views.Add(view);
                }
            }
                    }
                }
            }
            var list = views != null ? views.ToList() : new List<MaterialViewModel>();
            return list;
        }
    }
}