using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ExceedERP.Helpers.Inventory
{
    public  class DistributionHelper
    {
        private readonly StaticDistributionHelper _staticDistributionHelper = new StaticDistributionHelper();
        private readonly DynamicDistributionHelper _dynamicDistributionHelper = new DynamicDistributionHelper();
        AccountHelper _accountHelper = new AccountHelper();
        // inventory
        public  void UpdateGoodReceiveDistribution(GoodReceive record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.UpdateGoodReceiveDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateGoodReceiveDistribution(record);
            }
           
        }
        public  void UpdateIssueDistribution(Issue record)
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
        public  void UpdateStoreReturnDistribution(StoreReturn record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.UpdateStoreReturnDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateStoreReturnDistribution(record);
            }

        }
        public  void UpdateTransferSendDistribution(ItemTransfer record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.UpdateTransferSendDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateTransferSendDistribution(record);
            }
        }
        public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.UpdateTransferReceiveDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateTransferReceiveDistribution(record);
            }
        }
        public void UpdateGoodSellingDistribution(GoodSellingAndLending record)
        {
            if (_accountHelper.IsDynamicCoA())
            {
                _dynamicDistributionHelper.UpdateGoodSellingDistribution(record);
            }
            else
            {
                _staticDistributionHelper.UpdateGoodSellingDistribution(record);
            }
        }

        //generate distribution data
        public  void GenerateIssueDistribution(int inventoryperiod,int start)
        {
            if (_accountHelper.IsDynamicCoA())
            {
                _staticDistributionHelper.GenerateIssueDistribution(inventoryperiod,start);
            }
            else
            {
                _staticDistributionHelper.GenerateIssueDistribution(inventoryperiod,start);
            }
        }
        public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        {
            if (_accountHelper.IsDynamicCoA())
            {
                _staticDistributionHelper.GenerateGoodReceiveDistribution(inventoryperiod, start);
            }
            else
            {
                _staticDistributionHelper.GenerateGoodReceiveDistribution(inventoryperiod, start);
            }
        }
        public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.GenerateStoreReturnDistribution(inventoryperiod, start);
            }
            else
            {
                _staticDistributionHelper.GenerateStoreReturnDistribution(inventoryperiod, start);
            }
        }
        public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.GenerateTransferSendDistribution(inventoryperiod, start);
            }
            else
            {
                _staticDistributionHelper.GenerateTransferSendDistribution(inventoryperiod, start);
            }
        }
        public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        {
            if (_accountHelper.IsDynamicCoA())
            {
               _dynamicDistributionHelper.GenerateTransferReceiveDistribution(inventoryperiod, start);
            }
            else
            {
                _staticDistributionHelper.GenerateTransferReceiveDistribution(inventoryperiod, start);
            }
        }

       
    }
}
