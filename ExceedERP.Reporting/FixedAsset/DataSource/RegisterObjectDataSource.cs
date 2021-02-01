using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{



    [DataObject]
    public class RegisterObjectDataSource : List<RegisterViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RegisterViewModel> GetRegister(string fiscalYear)
        {
            List<RegisterViewModel> registerLists = new List<RegisterViewModel>();
            var assets = _db.AssetRegistrations.AsQueryable();
            var disposals = _db.DisposalItems.AsQueryable();


            if (!String.IsNullOrWhiteSpace(fiscalYear))
            {
                var year = _db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == fiscalYear);
                if (year!=null)
                {
                    DateTime prevDate=year.DateFrom.AddDays(-1);
                    var yearFrom = prevDate;
                    var yearTo = year.DateTo;

                    var catagories = _db.FixedAssetCategories.ToList();
                    var groups = _db.FixedAssetGroups.ToList();

                    foreach (var group in groups)
                    {
                        decimal before = 0;
                        decimal addition = 0;
                        decimal disposal = 0;
                       

                        var groupCategories = catagories.Where(x => x.FixedAssetGroupID == group.FixedAssetGroupID).ToList();

                        foreach (var catagory in groupCategories)
                        {
                            var befores = assets.Where(x =>
                                x.GRNDate < yearFrom &&
                                x.FixedAssetCategoryID == catagory.FixedAssetCategoryID).ToList();

                            if(befores.Any())
                                 before = before +befores.Sum(x => x.BookValue);

                            var additions = assets
                               .Where(x => x.GRNDate >= yearFrom)
                               .Where(x => x.GRNDate <= yearTo)
                               .Where(x=>x.FixedAssetCategoryID == catagory.FixedAssetCategoryID).ToList();

                            if(additions.Any())
                                    addition = addition + additions.Sum(x => x.PurchasePrice*x.Quanity);

                            var removeds= disposals.Where(x =>
                                    x.AssetDisposal.Date >= yearFrom && x.AssetDisposal.Date <= yearTo)
                                .Where(x => x.FixedAssetCategoryID == catagory.FixedAssetCategoryID).ToList();
                            if(removeds.Any())
                                 disposal = disposal + removeds.Sum(x => x.BookValue);
                        }

                        var regist = new RegisterViewModel
                        {
                            DateBefore = yearFrom,
                            DateNow = yearTo,
                            Description = group.Name,
                            Before = before,
                            Addition = addition,
                            Desposal = disposal
                        };
                        regist.After = regist.Addition + regist.Before - regist.Desposal;

                        registerLists.Add(regist);
                    }

                }
            }
            return registerLists;
        }
    }
}