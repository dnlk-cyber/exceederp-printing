
//using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
//using ExceedERP.DataAccess.Context;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;

//namespace ExceedERP.Web.Areas.Manufacturing.Models
//{ 
//    public class FurnitureUpdateOverallCost
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public void update(int parentId)

//        {

//            decimal totalMaterialCost = db.FurnitureMaterialCosts.Where(q => q.FurnitureEstimationId == parentId).ToList().Sum(q => q.TotalPrice);
//            decimal totalLaborCost = db.FurnitureLaborCosts.Where(q => q.EstimationFormId == parentId).ToList().Sum(q => q.TotalCost);
//            decimal lab_mate_cost = totalMaterialCost + totalLaborCost;
//            var estimation = db.OverallCosts.Where(q => q.EstimationFormId == parentId).ToList().FirstOrDefault();
//            if (estimation == null)
//            {
//                var entity1 = new FurnitureOverallCost
//                {
//                    FurnitureOverallCostId = parentId,
//                    MaterialCost = totalMaterialCost,
//                    LaborCost = totalLaborCost,
//                    T = lab_mate_cost,
//                    NBT = lab_mate_cost,
//                    GT = lab_mate_cost,
//                    EstimatedNetPrice = lab_mate_cost
//                };
//                db.FurnitureOverallCosts.Add(entity1);
//                db.SaveChanges();
//            }
//            else
//            {
//                decimal Tcost = lab_mate_cost + (lab_mate_cost * estimation.OverHeadCost/100);
//                decimal NBTcost= lab_mate_cost + (lab_mate_cost * estimation.MarginalCost/100);
//                decimal GTcost= lab_mate_cost + (lab_mate_cost * estimation.TInpercent/100);
//                estimation.OverallCostId = estimation.OverallCostId;
//                estimation.EstimationFormId = parentId;
//                estimation.MaterialCost = totalMaterialCost;
//                estimation.LaborCost = totalLaborCost;
//                estimation.T = Tcost ;
//                estimation.NBT = NBTcost;
//                estimation.GT = GTcost;
//                estimation.EstimatedNetPrice = lab_mate_cost;
//                db.OverallCosts.Attach(estimation);
//                db.Entry(estimation).State = EntityState.Modified;
//                db.SaveChanges();
                
//            }
//        }

//    }
//    public class UpdateFurnitureOverallCost
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public void update(int parentId)

//        {

//            decimal totalMaterialCost = db.FurnitureMaterialCosts.Where(q => q.FurnitureEstimationId == parentId).ToList().Sum(q => q.TotalPrice);
//            decimal totalLaborCost = db.DirectLaborCosts.Where(q => q.FurnitureEstimationId == parentId).ToList().Sum(q => q.TotalCost);
//            decimal lab_mate_cost = totalMaterialCost + totalLaborCost;
//            var estimation = db.FurnitureOverallCosts.Where(q => q.FurnitureEstimationId == parentId).ToList().FirstOrDefault();
//            if (estimation == null)
//            {
//                var entity1 = new FurnitureOverallCost
//                {
//                    FurnitureEstimationId = parentId,
//                    MaterialCost = totalMaterialCost,
//                    LaborCost = totalLaborCost,
//                    T = lab_mate_cost,
//                    NBT = lab_mate_cost,
//                    GT = lab_mate_cost,
//                    EstimatedNetPrice = lab_mate_cost
//                };
//                db.FurnitureOverallCosts.Add(entity1);
//                db.SaveChanges();
//            }
//            else
//            {
//                decimal Tcost = lab_mate_cost + (lab_mate_cost * estimation.OverHeadCost/100);
//                decimal NBTcost= lab_mate_cost + (lab_mate_cost * estimation.MarginalCost/100);
//                decimal GTcost= lab_mate_cost + (lab_mate_cost * estimation.TInpercent/100);
//                estimation.FurnitureOverallCostId = estimation.FurnitureOverallCostId;
//                estimation.FurnitureEstimationId = parentId;
//                estimation.MaterialCost = totalMaterialCost;
//                estimation.LaborCost = totalLaborCost;
//                estimation.T = Tcost ;
//                estimation.NBT = NBTcost;
//                estimation.GT = GTcost;
//                estimation.EstimatedNetPrice = lab_mate_cost;
//                db.FurnitureOverallCosts.Attach(estimation);
//                db.Entry(estimation).State = EntityState.Modified;
//                db.SaveChanges();
                
//            }
//        }

//    }
//}