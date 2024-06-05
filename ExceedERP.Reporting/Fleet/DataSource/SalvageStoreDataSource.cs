using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class SalvageStoreDataSource
    {

        
        ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetType()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Issue });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.GoodReceive });
           
            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalvageStoreViewModel> GetStores(string Type,  string Identity, string category,
           DateTime startingDate, DateTime endingDate)
        {
            List<SalvageStoreViewModel> FleetList = new List<SalvageStoreViewModel>();
            FleetViewModel FleetViewModel;
            List<Fleets> Fleets = new List<Fleets>();
         




         
            if (!string.IsNullOrEmpty(Identity))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {

                    Fleets =
                                          db.Fleets.Where(
                                              so =>

                                                  so.EquipmentIdentityID.ToString() == Identity)
                                              .ToList();
                }
           
            }
    
            if (Fleets.Any())
            {
                List<Fleets> List = Fleets;
                foreach (Fleets sov in List)
                {

                    string OperatorName = string.Empty;
                    var operators = db.Operator.Where(x => x.FleetsID == sov.FleetsID).FirstOrDefault();
                    if (operators != null)
                    {
                        OperatorName = operators.FullName;
                    }
                    if (Type == MaintenanceConst.Issue)
                    {
                        List<SalvageStore> SalvageStores = db.SalvageStore.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                        foreach (var SalvageStore in SalvageStores)
                        {

                            
                        string SparePart = string.Empty;

                        SalvageStoreGoodReceiving spare = db.SalvageStoreGoodReceiving.Where(x => x.SalvageStoreGoodReceivingID == SalvageStore.SalvageStoreGoodReceivingId).FirstOrDefault();
                        if(spare != null)
                        {
                            SparePart = spare.SparePartName;

                        }

                            var SalvageStoreViewModel = new SalvageStoreViewModel();


                            SalvageStoreViewModel.Date = SalvageStore.Date;
                            SalvageStoreViewModel.Description = SalvageStore.Description;
                            SalvageStoreViewModel.EquipmentName = sov.PlateNumber + " - " + sov.EquipmentName;
                            SalvageStoreViewModel.SparePart = SparePart;
                            SalvageStoreViewModel.VoucherNo = SalvageStore.VoucherNo;

                            FleetList.Add(SalvageStoreViewModel);
                        }
                    }
                    if (Type == MaintenanceConst.GoodReceive)
                    {
                        List<SalvageStoreGoodReceiving> GoodReceives = db.SalvageStoreGoodReceiving.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                        foreach (var GoodReceive in GoodReceives)
                        {

                       
                            var SalvageStoreViewModel = new SalvageStoreViewModel();


                            SalvageStoreViewModel.Date = GoodReceive.Date;
                            SalvageStoreViewModel.Description = GoodReceive.Description;
                            SalvageStoreViewModel.EquipmentName = sov.PlateNumber + " - " + sov.EquipmentName;
                            SalvageStoreViewModel.SparePart = GoodReceive.SparePartName;
                            SalvageStoreViewModel.VoucherNo = GoodReceive.VoucherNo;

                        
                            FleetList.Add(SalvageStoreViewModel);
                        }
                    }
                }

            }
                return FleetList;
            

               
              
            }
        }

    }




