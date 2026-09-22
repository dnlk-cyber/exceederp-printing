using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace ExceedERP.Helpers.Common
{
    public class CacheData
    {
        private readonly ExceedDbContext _db;
        private static readonly object _chartSegmentCacheLock = new object();
        private static readonly object _chartAccountCacheLock = new object();
        private static readonly object _segmentSetupCacheLock = new object();
        private static readonly object _branchCacheLock = new object();
        private static readonly object _businessUnitCacheLock = new object();
        private static readonly object _workUnitCacheLock = new object();
        private static readonly object _employeeCacheLock = new object();
        private static readonly object _taxCacheLock = new object();
        private static readonly object _catCacheLock = new object();
        private static readonly object _itemCacheLock = new object();
        private static readonly object _inventoryPeriodCacheLock = new object();
        private static readonly object _storeCacheLock = new object();
        private static readonly object _supplierCacheLock = new object();
        private static readonly object _receiveTypesCacheLock = new object();
        //finance
        private static readonly object _periodsCacheLock = new object();
        private static readonly object _jcategoriesCacheLock = new object();
        private static readonly object _jcategoriesRecordCacheLock = new object();
        private static readonly object _jusersCacheLock = new object();
        private static readonly object _paymentMethodsCacheLock = new object();
        private static readonly object _apSetupItemCacheLock = new object();
        //ppe
        private static readonly object _ppePeriodCacheLock = new object();
        private static readonly object _ppeCategoryCacheLock = new object();
        private static readonly object _ppeSubCategoryCacheLock = new object();
        private static readonly object _ppeSubCategoryNameCacheLock = new object();
        private static readonly object _ppeAssetCacheLock = new object();
        private static readonly object _ppeConditionCacheLock = new object();

        // CRM
        private static readonly object _crmProductsCacheLock = new object();
        private static readonly object _salesStoreCacheLock = new object();
        private static readonly object _crmCustomersCacheLock = new object();
        private static readonly object _crmPeriodsCacheLock = new object();
        private static readonly object _bankAccountsCacheLock = new object();
        private static readonly object _repairTypesCacheLock = new object();
        // HRM
        private static readonly object _jobTitlesCacheLock = new object();
        private static readonly object _ranksCacheLock = new object();
        private static readonly object _stepsCacheLock = new object();
        //fleet 
        private static readonly object _EquipmentIdentity = new object();
        private static readonly object _EquipmentType = new object();
        private static readonly object _EquipmentName = new object();
        private static readonly object _Fleets = new object();
        private static readonly object _FleetsSecond = new object();
        //maintenance
        private static readonly object _MaintenanceCenter = new object();
        public CacheData()
        {
            _db = new ExceedDbContext();

        }

        //finance
        public List<ForeginKeySelectorVm> GetJournalRecordUsersCaches()
        {
            const string cacheKey = CacheKeys.JournalUsers;

            var jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (jcategories == null)
            {
                lock (_jusersCacheLock)
                {
                    jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (jcategories == null)
                    {
                        jcategories = _db.GLRecordJournals
                               .Where(x => x.IsOnlineApproved && !x.isVoid && !x.isPosted)
                               .AsNoTracking()
                               .Select(x => x.CreatedBy)
                               .Distinct()
                               .OrderBy(x => x)
                           .Select(userName => new ForeginKeySelectorVm
                           {
                               Id = 0,
                               Name = userName
                           })
                           .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            jcategories,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return jcategories;
        }
        public List<ForeginKeySelectorVm> GetJournalRecordCategoriesCaches()
        {
            const string cacheKey = CacheKeys.JournalRecordCategories;

            var jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (jcategories == null)
            {
                lock (_jcategoriesRecordCacheLock)
                {
                    jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (jcategories == null)
                    {
                         jcategories = _db.GLRecordJournals
                                .Where(x => x.IsOnlineApproved && !x.isVoid && !x.isPosted)
                                .AsNoTracking()
                                .Select(x => x.CategoryName)
                                .Distinct()
                                .OrderBy(x => x)
                            .Select(name => new ForeginKeySelectorVm
                            {
                                Id = 0,
                                Name = name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            jcategories,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return jcategories;
        }
        public List<ForeginKeySelectorVm> GetJournalCategoriesCaches()
        {
            const string cacheKey = CacheKeys.JournalCategories;

            var jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (jcategories == null)
            {
                lock (_jcategoriesCacheLock)
                {
                    jcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (jcategories == null)
                    {
                        jcategories = _db.GLJournalCategoriers
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.GLJournalCategoriersID,
                                Name = x.Category
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            jcategories,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return jcategories;
        }

        public List<GlSegmentSetupCache> GetSegmentSetupCaches()
        {
            const string cacheKey = CacheKeys.GlSegmentSetups;

            var segmentSequence = MemoryCache.Default[cacheKey] as List<GlSegmentSetupCache>;
            if (segmentSequence == null)
            {
                lock (_segmentSetupCacheLock)
                {
                    segmentSequence = MemoryCache.Default[cacheKey] as List<GlSegmentSetupCache>;
                    if (segmentSequence == null)
                    {
                        segmentSequence = _db.GlSegmentSetups
                            .Where(x => x.Enabled)
                            .AsNoTracking()  // Good for read-only queries; avoids tracking overhead
                            .Select(x => new GlSegmentSetupCache
                            {
                                Sequence = x.Sequence,
                                SegmentType = x.SegmentType,
                                GLSegmentSetupId = x.GLSegmentSetupId,
                                Name = x.Name,
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            segmentSequence,
                            ObjectCache.InfiniteAbsoluteExpiration  // Change to time-based expiration if needed
                        );
                    }
                }
            }

            return segmentSequence;
        }
        public List<GLChartOfAccountAccountCache> GetOfAccountAccountCaches()
        {
            const string cacheKey = CacheKeys.GLChartOfAccountAccounts;

            var accounts = MemoryCache.Default[cacheKey] as List<GLChartOfAccountAccountCache>;
            if (accounts == null)
            {
                lock (_chartAccountCacheLock)
                {
                    accounts = MemoryCache.Default[cacheKey] as List<GLChartOfAccountAccountCache>;
                    if (accounts == null)
                    {
                        accounts = _db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.Parent == false)
                            .OrderBy(x => x.Values)
                            .Select(x => new GLChartOfAccountAccountCache
                            {
                                Values = x.Values,
                                Description = x.Description
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            accounts,
                            ObjectCache.InfiniteAbsoluteExpiration
                        );
                    }
                }
            }

            return accounts;
        }
        public List<GlChartOfAccountSegmentCache> GetChartOfAccountSegmentCaches()
        {
            const string cacheKey = CacheKeys.GlChartOfAccountSegments;

            var segments = MemoryCache.Default[cacheKey] as List<GlChartOfAccountSegmentCache>;
            if (segments == null)
            {
                lock (_chartSegmentCacheLock)
                {
                    segments = MemoryCache.Default[cacheKey] as List<GlChartOfAccountSegmentCache>;
                    if (segments == null)
                    {
                        segments = _db.GlChartOfAccountSegments
                            .Where(x => x.Enable && x.Parent == false)
                            .OrderBy(x => x.Values)
                            .Select(x => new GlChartOfAccountSegmentCache
                            {
                                GLSegmentSetupId = x.GLSegmentSetupId,
                                Values = x.Values,
                                Description = x.Description
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            segments,
                            ObjectCache.InfiniteAbsoluteExpiration
                        );
                    }
                }
            }

            return segments;
        }

        public List<ForeginKeySelectorVm> GetBranchCaches()
        {
            const string cacheKey = CacheKeys.Branchs;

            var branches = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (branches == null)
            {
                lock (_branchCacheLock)
                {
                    branches = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (branches == null)
                    {
                        branches = _db.Branch
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.BranchId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            branches,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return branches;
        }
        public List<ForeginKeySelectorVm> GetPeriodsCaches()
        {
            const string cacheKey = CacheKeys.Periods;

            var periods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (periods == null)
            {
                lock (_periodsCacheLock)
                {
                    periods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (periods == null)
                    {
                        periods = _db.GLPeriods
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.GLPeriodId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            periods,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return periods;
        }

        public List<ForeginKeySelectorVm> GetBusinessUnitCaches()
        {
            const string cacheKey = CacheKeys.BusinessUnits;

            var businessUnits = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (businessUnits == null)
            {
                lock (_businessUnitCacheLock)
                {
                    businessUnits = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (businessUnits == null)
                    {
                        businessUnits = _db.BusinessUnits
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.BusinessUnitId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            businessUnits,
                            DateTimeOffset.Now.AddHours(8) // Change to Infinite if truly static
                        );
                    }
                }
            }

            return businessUnits;
        }

        public List<ForeginKeySelectorVm> GetWorkUnitCaches()
        {
            const string cacheKey = CacheKeys.WorkUnits;

            var workUnits = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (workUnits == null)
            {
                lock (_workUnitCacheLock)
                {
                    workUnits = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (workUnits == null)
                    {
                        workUnits = _db.MainCompanyStructures
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.OrgStructureId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            workUnits,
                            DateTimeOffset.Now.AddHours(8) // You can adjust or use InfiniteAbsoluteExpiration
                        );
                    }
                }
            }

            return workUnits;
        }

        public List<ForeginKeyAmharicSelectorVm> GetEmployeeCaches()
        {
            const string cacheKey = CacheKeys.Employees;
            var employees = MemoryCache.Default[cacheKey] as List<ForeginKeyAmharicSelectorVm>;
            if (employees == null)
            {
                lock (_employeeCacheLock)
                {
                    employees = MemoryCache.Default[cacheKey] as List<ForeginKeyAmharicSelectorVm>;
                    if (employees == null)
                    {
                        employees = _db.Person_Employee
                            .Select(x => new ForeginKeyAmharicSelectorVm
                            {
                                Id = x.EmployeeId,
                                Code = x.EmployeeCode,
                                Name = (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).Trim(),
                                AmharicName = (x.FirstName + " " + x.MiddleName + " " + x.LastName).Trim()
                            })
                            .ToList();
                        MemoryCache.Default.Add(
                            cacheKey,
                            employees,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }
            return employees;
        }
        public List<ForeginKeySelectorVm> GetApItemsCaches()
        {
            const string cacheKey = CacheKeys.APItemsSetups;

            var taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (taxes == null)
            {
                lock (_apSetupItemCacheLock)
                {
                    taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (taxes == null)
                    {
                        taxes = _db.ApSetupItems
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.ApSetupItemId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            taxes,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return taxes;
        }
        public List<ForeginKeySelectorVm> GetTaxCaches()
        {
            const string cacheKey = CacheKeys.Taxes;

            var taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (taxes == null)
            {
                lock (_taxCacheLock)
                {
                    taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (taxes == null)
                    {
                        taxes = _db.GLTaxRates
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.GLTaxRateID,
                                Name = x.TaxName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            taxes,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return taxes;
        }
        public List<ForeginKeySelectorVm> GetPaymentMethodCaches()
        {
            const string cacheKey = CacheKeys.PaymentMethods;

            var taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (taxes == null)
            {
                lock (_paymentMethodsCacheLock)
                {
                    taxes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (taxes == null)
                    {
                        taxes = _db.PaymentMethods
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.PaymentMethodID,
                                Name = x.Method
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            taxes,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return taxes;
        }
        public List<ForeginKeySelectorVm> GetReceiveTypesCaches()
        {
            const string cacheKey = CacheKeys.ReceiveTypes;

            var receiveTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (receiveTypes == null)
            {
                lock (_receiveTypesCacheLock)
                {
                    receiveTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (receiveTypes == null)
                    {
                        receiveTypes = _db.ReceiveTypes
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.ReceiveTypeId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            receiveTypes,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return receiveTypes;
        }

        public List<ForeginKeyStringSelectorVm> GetInventoryCategoryCaches()
        {
            const string cacheKey = CacheKeys.InventoryCategory;

            var categories = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
            if (categories == null)
            {
                lock (_catCacheLock)
                {
                    categories = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
                    if (categories == null)
                    {
                        categories = _db.ItemCategorys
                            .Select(x => new ForeginKeyStringSelectorVm
                            {
                                Id = x.ItemCategoryCode,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            categories,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return categories;
        }
        public List<ForeginKeyStringSelectorVm> GetInventoryItemCaches()
        {
            const string cacheKey = CacheKeys.InventoryItems;

            var items = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
            if (items == null)
            {
                lock (_itemCacheLock)
                {
                    items = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
                    if (items == null)
                    {
                        items = _db.InventoryItems
                            .Select(x => new ForeginKeyStringSelectorVm
                            {
                                Id = x.ItemCode,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            items,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return items;
        }

        public List<ForeginKeyStringSelectorVm> GetStoreCaches()
        {
            const string cacheKey = CacheKeys.Stores;

            var stores = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
            if (stores == null)
            {
                lock (_storeCacheLock)
                {
                    stores = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
                    if (stores == null)
                    {
                        stores = _db.InventoryStores
                            .Select(x => new ForeginKeyStringSelectorVm
                            {
                                Id = x.StoreCode,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            stores,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return stores;
        }

        public List<ForeginKeySelectorVm> GetInventoryPeriodCaches()
        {
            const string cacheKey = CacheKeys.InventoryPeriods;

            var periods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (periods == null)
            {
                lock (_inventoryPeriodCacheLock)
                {
                    periods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (periods == null)
                    {
                        periods = _db.InventoryPeriods
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.InventoryPeriodId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            periods,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return periods;
        }

        public List<ForeginKeyStringSelectorVm> GetSupplierCaches()
        {
            const string cacheKey = CacheKeys.Suppliers;

            var suppliers = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
            if (suppliers == null)
            {
                lock (_supplierCacheLock)
                {
                    suppliers = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
                    if (suppliers == null)
                    {
                        suppliers = _db.ProcurementSuppliers
                            .Select(x => new ForeginKeyStringSelectorVm
                            {
                                Id = x.SupplierId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            suppliers,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return suppliers;
        }

        public List<ForeginKeySelectorVm> GetFiscalYearCaches()
        {
            const string cacheKey = CacheKeys.FiscalYears;

            var fiscalYears = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (fiscalYears == null)
            {
                lock (_supplierCacheLock)
                {
                    fiscalYears = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (fiscalYears == null)
                    {
                        fiscalYears = _db.GLFiscalYears
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.GlFiscalYearId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            fiscalYears,
                            DateTimeOffset.Now.AddHours(8) // or InfiniteAbsoluteExpiration if truly static
                        );
                    }
                }
            }

            return fiscalYears;
        }
        //ppe
        public List<ForeginKeySelectorVm> GetPPEPeriodCaches()
        {
            const string cacheKey = CacheKeys.PPEPeriods;

            var ppePeriods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (ppePeriods == null)
            {
                lock (_ppePeriodCacheLock)
                {
                    ppePeriods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (ppePeriods == null)
                    {
                        ppePeriods = _db.FixedAssetPeriods
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.FixedAssetPeriodId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            ppePeriods,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return ppePeriods;
        }
        public List<ForeginKeySelectorVm> GetPPECategoryCaches()
        {
            const string cacheKey = CacheKeys.PPECategories;

            var ppecategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (ppecategories == null)
            {
                lock (_ppeCategoryCacheLock)
                {
                    ppecategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (ppecategories == null)
                    {
                        ppecategories = _db.FixedAssetCategories
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.FixedAssetCategoryID,
                                Name = x.CategoryName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            ppecategories,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return ppecategories;
        }
        public List<ForeginKeySelectorVm> GetPPESubCategoryCaches()
        {
            const string cacheKey = CacheKeys.PPESubCategories;

            var ppesubcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (ppesubcategories == null)
            {
                lock (_ppeSubCategoryCacheLock)
                {
                    ppesubcategories = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (ppesubcategories == null)
                    {
                        ppesubcategories = _db.FixedAssetSubCategories
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.FixedAssetSubCategoryID,
                                Name = x.SubCategoryName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            ppesubcategories,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return ppesubcategories;
        }
        public List<ForeginKeySelectorVm> GetPPESubCategoryNameCaches()
        {
            const string cacheKey = CacheKeys.PPESubCategoryNames;

            var ppesubcategoryNames = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (ppesubcategoryNames == null)
            {
                lock (_ppeSubCategoryNameCacheLock)
                {
                    ppesubcategoryNames = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (ppesubcategoryNames == null)
                    {
                        ppesubcategoryNames = _db.FixedAssetSubCategoryNames
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.FixedAssetSubCategoryNameId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            ppesubcategoryNames,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return ppesubcategoryNames;
        }
        public List<ForeginKeySelectorVm> GetPPEAssetCaches()
        {
            const string cacheKey = CacheKeys.PPEAsstes;

            var assets = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (assets == null)
            {
                lock (_ppeAssetCacheLock)
                {
                    assets = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (assets == null)
                    {
                        assets = _db.AssetRegistrations
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.AssetRegistrationId,
                                Name = x.Tag + "-" + x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            assets,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return assets;
        }
        public List<ForeginKeySelectorVm> GetPPEAssetConditionsCaches()
        {
            const string cacheKey = CacheKeys.PPEAsstConditions;

            var conditions = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (conditions == null)
            {
                lock (_ppeConditionCacheLock)
                {
                    conditions = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (conditions == null)
                    {
                        conditions = _db.FixedAssetConditions
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.FixedAssetConditionId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            conditions,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if employees are static
                        );
                    }
                }
            }

            return conditions;
        }
        // CRM
        public List<ForeginKeySelectorVm> GetCrmProductsCache()
        {
            const string cacheKey = CacheKeys.CrmProducts;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> products))
            {
                lock (_crmProductsCacheLock)
                {
                    products = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (products == null)
                    {
                        products = _db.Products
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.ProductID,
                                Name = x.ItemCode + " " + x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            products,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return products;
        }
        public List<ForeginKeySelectorVm> GetSalesStoresCache()
        {
            const string cacheKey = CacheKeys.SalesStores;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> stores))
            {
                lock (_salesStoreCacheLock)
                {
                    stores = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (stores == null)
                    {
                        stores = _db.SalesStores
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.SalesStoreId,
                                Name = x.Description
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            stores,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return stores;
        }
        public List<ForeginKeySelectorVm> GetCrmCustomersCache()
        {
            const string cacheKey = CacheKeys.CrmCustomers;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> customers))
            {
                lock (_crmCustomersCacheLock)
                {
                    customers = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (customers == null)
                    {
                        customers = _db.OrganizationCustomers
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.OrganizationCustomerID,
                                Name = x.TradeName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            customers,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return customers;
        }
        public List<ForeginKeySelectorVm> GetCrmActiveCustomersCache()
        {
            const string cacheKey = CacheKeys.CrmActiveCustomers;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> customers))
            {
                lock (_crmCustomersCacheLock)
                {
                    customers = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (customers == null)
                    {
                        customers = _db.OrganizationCustomers
                            .Where(c => !c.IsTemporary && c.IsActive)
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.OrganizationCustomerID,
                                Name = x.TradeName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            customers,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return customers;
        }
        public void UpdateCrmCustomersCache()
        {
            const string cacheKey = CacheKeys.CrmCustomers;
            lock (_crmCustomersCacheLock)
            {
                var customers = _db.OrganizationCustomers
                    .Select(x => new ForeginKeySelectorVm
                    {
                        Id = x.OrganizationCustomerID,
                        Name = x.TradeName
                    })
                    .ToList();
                MemoryCache.Default.Set(
                    cacheKey,
                    customers,
                    DateTimeOffset.Now.AddHours(12)
                );
            }
        }
        public List<ForeginKeySelectorVm> GetCrmPeriodsCache()
        {
            const string cacheKey = CacheKeys.CrmPeriods;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> periods))
            {
                lock (_crmPeriodsCacheLock)
                {
                    periods = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (periods == null)
                    {
                        periods = _db.CRMPeriods
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.CRMPeriodId,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            periods,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return periods;
        }
        public List<ForeginKeySelectorVm> GetBankAccountsCache()
        {
            const string cacheKey = CacheKeys.BankAccounts;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorVm> accounts))
            {
                lock (_bankAccountsCacheLock)
                {
                    accounts = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (accounts == null)
                    {
                        accounts = _db.BankAccounts
                            .Select(p => new ForeginKeySelectorVm
                            {
                                Id = p.BankAccountId,
                                Name = p.AccountNumber + " " + p.AccountName + " " + p.BankBranch + " Branch"
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            accounts,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return accounts;
        }
        public List<ForeginKeyStringSelectorVm> GetRepairTypesCache()
        {
            const string cacheKey = CacheKeys.RepairTypes;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeyStringSelectorVm> repairTypes))
            {
                lock (_repairTypesCacheLock)
                {
                    repairTypes = MemoryCache.Default[cacheKey] as List<ForeginKeyStringSelectorVm>;
                    if (repairTypes == null)
                    {
                        repairTypes = _db.RepairType
                            .Select(x => new ForeginKeyStringSelectorVm
                            {
                                Id = x.RepairTypeID.ToString(),
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            repairTypes,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return repairTypes;
        }

        //hrm 
        public List<ForeginKeySelectorVm> GetJobTitlesCaches()
        {
            const string cacheKey = CacheKeys.JobTitles;
            var jobTitles = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (jobTitles == null)
            {
                lock (_jobTitlesCacheLock)
                {
                    jobTitles = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (jobTitles == null)
                    {
                        jobTitles = _db.Lookups
                                    .Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                             || x.LookupType == LookUpTypes.JOB_TITLE
                                             || x.LookupType == LookUpTypes.Jobtitle_External_Experience)
                            .Select(x => new ForeginKeySelectorVm
                            {
                                Id = x.LookupId,
                                Name = x.Description.Trim(),
                            }).ToList();
                        MemoryCache.Default.Add(
                            cacheKey,
                            jobTitles,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if jobTitles are static
                        );
                    }
                }
            }
            return jobTitles;
        }
        public List<ForeginKeySelectorVm> GetRankCaches()
        {
            const string cacheKey = CacheKeys.Ranks;
            var ranks = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (ranks == null)
            {
                lock (_ranksCacheLock)
                {
                    ranks = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (ranks == null)
                    {
                        ranks = _db.Ranks.Select(x => new ForeginKeySelectorVm
                        {
                            Id = x.RankId,
                            Name = x.RankName,
                        }).ToList();
                        MemoryCache.Default.Add(
                            cacheKey,
                            ranks,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if jobTitles are static
                        );
                    }
                }
            }
            return ranks;
        }
        public List<ForeginKeySelectorVm> GetStepCaches()
        {
            const string cacheKey = CacheKeys.Steps;
            var steps = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
            if (steps == null)
            {
                lock (_stepsCacheLock)
                {
                    steps = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorVm>;
                    if (steps == null)
                    {
                        steps = _db.Steps.Select(x => new ForeginKeySelectorVm
                        {
                            Id = x.StepId,
                            Name = x.StepName.Trim(),
                        }).ToList();
                        MemoryCache.Default.Add(
                            cacheKey,
                            steps,
                            DateTimeOffset.Now.AddHours(8) // or use InfiniteAbsoluteExpiration if steps are static
                        );
                    }
                }
            }
            return steps;
        }
        //fleet
        public List<ForeginKeySelectorGuidVm> GetEquipmentIdentityCaches()
        {
            const string cacheKey = CacheKeys.EquipmentIdentity;

            var EquipmentIdentities = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
            if (EquipmentIdentities == null)
            {
                lock (_EquipmentIdentity)
                {
                    EquipmentIdentities = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
                    if (EquipmentIdentities == null)
                    {
                        EquipmentIdentities = _db.EquipmentIdentity
                            .Select(x => new ForeginKeySelectorGuidVm
                            {
                                Id = x.EquipmentIdentityID,
                                Name = x.EquipmentCategoryName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            EquipmentIdentities,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return EquipmentIdentities;
        }
        public List<ForeginKeySelectorGuidVm> GetEquipmentTypeCaches()
        {
            const string cacheKey = CacheKeys.EquipmentType;

            var EquipmentTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
            if (EquipmentTypes == null)
            {
                lock (_EquipmentType)
                {
                    EquipmentTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
                    if (EquipmentTypes == null)
                    {
                        EquipmentTypes = _db.EquipmentType
                            .Select(x => new ForeginKeySelectorGuidVm
                            {
                                Id = x.EquipmentTypeID,
                                Name = x.EquipmentTypeName
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            EquipmentTypes,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return EquipmentTypes;
        }
        public List<ForeginKeySelectorFleetVm> GetFleetsCache()
        {
            const string cacheKey = CacheKeys.Fleets;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorFleetVm> fleets))
            {
                lock (_Fleets)
                {
                    fleets = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorFleetVm>;
                    if (fleets == null)
                    {
                        fleets = _db.Fleets
                            .Select(x => new ForeginKeySelectorFleetVm
                            {
                                Id = x.FleetsID,
                                OrgBranchName = x.OrgBranchName,
                                Name = x.EquipmentName + "-" + x.PlateNumber + "-" + x.ChasisNumber

                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            fleets,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return fleets;
        }
        public List<ForeginKeySelectorFleetSecondVm> GetFleetsSecondCache()
        {
            const string cacheKey = CacheKeys.FleetsSecond;

            if (!(MemoryCache.Default[cacheKey] is List<ForeginKeySelectorFleetSecondVm> fleets))
            {
                lock (_FleetsSecond)
                {
                    fleets = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorFleetSecondVm>;
                    if (fleets == null)
                    {
                        fleets = _db.Fleets
                            .Select(x => new ForeginKeySelectorFleetSecondVm
                            {
                                PlateNumber = x.PlateNumber + "--" + x.SerialNo + "--" + x.UnitNum,
                                FleetsID = x.FleetsID,
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            fleets,
                            DateTimeOffset.Now.AddHours(12)
                        );
                    }
                }
            }
            return fleets;
        }
        public List<ForeginKeySelectorGuidVm> GetEquipmentNameCaches()
        {
            const string cacheKey = CacheKeys.EquipmentName;

            var EquipmentTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
            if (EquipmentTypes == null)
            {
                lock (_EquipmentName)
                {
                    EquipmentTypes = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
                    if (EquipmentTypes == null)
                    {
                        EquipmentTypes = _db.EquipmentName
                            .Select(x => new ForeginKeySelectorGuidVm
                            {
                                Id = x.EquipmentNameID,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            EquipmentTypes,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return EquipmentTypes;
        }
        //maintenance
        public List<ForeginKeySelectorGuidVm> GetMaintenanceCenterCaches()
        {
            const string cacheKey = CacheKeys.MaintenanceCenters;

            var MaintenanceCenters = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
            if (MaintenanceCenters == null)
            {
                lock (_MaintenanceCenter)
                {
                    MaintenanceCenters = MemoryCache.Default[cacheKey] as List<ForeginKeySelectorGuidVm>;
                    if (MaintenanceCenters == null)
                    {
                        MaintenanceCenters = _db.MaintenanceCenter
                            .Select(x => new ForeginKeySelectorGuidVm
                            {
                                Id = x.MaintenanceCenterID,
                                Name = x.Name
                            })
                            .ToList();

                        MemoryCache.Default.Add(
                            cacheKey,
                            MaintenanceCenters,
                            DateTimeOffset.Now.AddHours(8) // Or use Infinite if the data rarely changes
                        );
                    }
                }
            }

            return MaintenanceCenters;
        }
    }
}
