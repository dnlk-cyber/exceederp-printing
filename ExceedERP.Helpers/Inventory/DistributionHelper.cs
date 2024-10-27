using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;

namespace ExceedERP.Helpers.Inventory
{
    public  class DistributionHelper
    {
        private readonly DynamicDistributionHelper _dynamicDistributionHelper;
        public DistributionHelper()
        {
            _dynamicDistributionHelper = new DynamicDistributionHelper();
        }
        // inventory
        public void UpdateServiceReceiveDistribution(ServiceReceive record)
        {
            _dynamicDistributionHelper.UpdateServiceReceiveDistribution(record);

        }
        public  void UpdateGoodReceiveDistribution(GoodReceive record)
        {
            _dynamicDistributionHelper.UpdateGoodReceiveDistribution(record);

        }
        public  void UpdateIssueDistribution(Issue record)
        {
            _dynamicDistributionHelper.UpdateIssueDistribution(record);

        }
        public  void UpdateStoreReturnDistribution(StoreReturn record)
        {
            _dynamicDistributionHelper.UpdateStoreReturnDistribution(record);

        }
        public  void UpdateTransferSendDistribution(ItemTransfer record)
        {
            _dynamicDistributionHelper.UpdateTransferSendDistribution(record);
        }
        public  void UpdateTransferReceiveDistribution(ItemTransferReceive record)
        {
            _dynamicDistributionHelper.UpdateTransferReceiveDistribution(record);
        }
        public void UpdateGoodSellingDistribution(GoodSellingAndLending record)
        {
            _dynamicDistributionHelper.UpdateGoodSellingDistribution(record);
        }

        //generate distribution data
        public  void GenerateIssueDistribution(int inventoryperiod,int start)
        {
            _dynamicDistributionHelper.GenerateIssueDistribution(inventoryperiod, start);
        }
        public  void GenerateGoodReceiveDistribution(int inventoryperiod, int start)
        {
            _dynamicDistributionHelper.GenerateGoodReceiveDistribution(inventoryperiod, start);
        }
        public  void GenerateStoreReturnDistribution(int inventoryperiod, int start)
        {
            _dynamicDistributionHelper.GenerateStoreReturnDistribution(inventoryperiod, start);
        }
        public  void GenerateTransferSendDistribution(int inventoryperiod, int start)
        {
            _dynamicDistributionHelper.GenerateTransferSendDistribution(inventoryperiod, start);
        }
        public  void GenerateTransferReceiveDistribution(int inventoryperiod, int start)
        {
            _dynamicDistributionHelper.GenerateTransferReceiveDistribution(inventoryperiod, start);
        }

       
    }
}
