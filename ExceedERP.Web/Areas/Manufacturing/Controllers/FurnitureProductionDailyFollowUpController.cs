using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureProductionDailyFollowUpController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            ViewData["Employee"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName,
                Value = s.EmployeeId
            });
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return View();
        }

        public ActionResult FurnitureJobOrderProductions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderProduction> furniturejoborderproductions = db.FurnitureJobOrderProductions.Where(x => x.IsOnlineApproved);
            DataSourceResult result = furniturejoborderproductions.ToDataSourceResult(request, furnitureJobOrderProduction => new FurnitureJobOrderProduction
            {
                FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                CustomerId = furnitureJobOrderProduction.CustomerId,
                JobTypeId = furnitureJobOrderProduction.JobTypeId,
                JobNo = furnitureJobOrderProduction.JobNo,
                Unit = furnitureJobOrderProduction.Unit,
                Quantity = furnitureJobOrderProduction.Quantity,
                DeliveryDate = furnitureJobOrderProduction.DeliveryDate,
                FurnitureJobOrderFormId = furnitureJobOrderProduction.FurnitureJobOrderFormId,
                IsClosed = furnitureJobOrderProduction.IsClosed,
                IsOnlineApproved = furnitureJobOrderProduction.IsOnlineApproved,
                
            });

            return Json(result);
        }

        #region Approval
        public void ActivateAssembly(int id)
        {           
            var production = db.FurnitureJobOrderProductions.Find(id);

            if (production != null)
            {
                if (!production.IsSendForFinishing)
                {
                    production.IsSendForFinishing = true;
                    production.SendForFinishingBy = User.Identity.Name;
                    production.SendForFinishingDate = DateTime.Today;

                    db.FurnitureJobOrderProductions.Attach(production);
                    db.Entry(production).State = EntityState.Modified;
                    db.SaveChanges();
                }
                         
            }
            
        }

        public JsonResult SendUnfinishedGoodsForAssembly(int id)
        {
            object response = null;
            var unFinishedGood = db.FurnitureUnfinishedMaterialTransferForms.Find(id);

            if (unFinishedGood != null)
            {

                unFinishedGood.IsSent = true;
                unFinishedGood.SentBy = User.Identity.Name;
                unFinishedGood.SentDate = DateTime.Today;

                db.FurnitureUnfinishedMaterialTransferForms.Attach(unFinishedGood);
                db.Entry(unFinishedGood).State = EntityState.Modified;
                db.SaveChanges();
                // Activates Assembly/Finshing
                ActivateAssembly(unFinishedGood.FurnitureJobOrderProductionId);

                response = new { Success = true, Message = "Sent for Assembly/Finishing Success!" };

            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
