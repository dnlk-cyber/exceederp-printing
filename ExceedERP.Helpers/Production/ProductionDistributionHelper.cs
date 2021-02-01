using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Production;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ExceedERP.Helpers.Production
{
    public  class ProductionDistributionHelper
    {
        private readonly ProductionStaticDistributionHelper _staticDistributionHelper = new ProductionStaticDistributionHelper();
        private readonly ProductionDynamicDistributionHelper _dynamicDistributionHelper = new ProductionDynamicDistributionHelper();
        AccountHelper _accountHelper = new AccountHelper();
        // inventory
        public void UpdateGoodReceiveDistribution(ProductionStoreReceive record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
                _dynamicDistributionHelper.UpdateRawGoodReceiveDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateRawGoodReceiveDistribution(record);
            }

        }
        public void UpdateFinishedGoodReceiveDistribution(ProductionFinishedGoodsStoreReceive record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
                _dynamicDistributionHelper.UpdateFinishedGoodReceiveDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateFinishedGoodReceiveDistribution(record);
            }

        }
        public  void UpdateIssueDistribution(ProductionIssue record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.UpdateIssueDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateIssueDistribution(record);
            }

        }
        //public  void UpdateStoreReturnDistribution(StoreReturn record)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.UpdateStoreReturnDistribution(record);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.UpdateStoreReturnDistribution(record);
        //    }

        //}
        //public  void UpdateTransferSendDistribution(ItemTransfer record)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.UpdateTransferSendDistribution(record);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.UpdateTransferSendDistribution(record);
        //    }
        //}
        //public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.UpdateTransferReceiveDistribution(record);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.UpdateTransferReceiveDistribution(record);
        //    }
        //}

        //generate distribution data
        //public  void GenerateIssueDistribution(int inventoryperiod,int start)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //        _staticDistributionHelper.GenerateIssueDistribution(inventoryperiod,start);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.GenerateIssueDistribution(inventoryperiod,start);
        //    }
        //}
        //public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //        _staticDistributionHelper.GenerateGoodReceiveDistribution(inventoryperiod, start);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.GenerateGoodReceiveDistribution(inventoryperiod, start);
        //    }
        //}
        //public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.GenerateStoreReturnDistribution(inventoryperiod, start);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.GenerateStoreReturnDistribution(inventoryperiod, start);
        //    }
        //}
        //public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.GenerateTransferSendDistribution(inventoryperiod, start);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.GenerateTransferSendDistribution(inventoryperiod, start);
        //    }
        //}
        //public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        //{
        //    if (_accountHelper.IsDynamicCoA())
        //    {
        //       _dynamicDistributionHelper.GenerateTransferReceiveDistribution(inventoryperiod, start);
        //    }
        //    else
        //    {
        //        _staticDistributionHelper.GenerateTransferReceiveDistribution(inventoryperiod, start);
        //    }
        //}
    }
}
