namespace ExceedERP.Core.Domain.Production
{
    public enum CropGeneration
    {
        PreBasic,
        Basic,
        Certified
    }

    public enum CropType
    {
        OpenPollinated,
        Hybrid
    }
    //
    //  Summary :
    //      Applied for budget year and periods
    public enum TimeRangeStatus
    {
        Open,
        Close,
        PermanetlyClosed
    }
    public enum ProductType
    {
        Seed = 0,
        NaturalGum = 1
    }

    public enum ProductionSettingType
    {
        SeedProductionStage = 0,
        SeedProcessingStage = 1,
        NaturalGumProductionStage = 2,
        NaturalGumProcessingStage = 3
    }

    public enum ProcessingCenterSource
    {
        Extenrnal = 0,
        Internal = 1
    }
    public enum ProcessingCenterType
    {
        SeedProcessing = 0,
        NaturalGumProcessing = 1
    }

    //
    // Summary 
    //      Used to categorize the operation types in 
    //      cost by month, crop and generation tracking 
    public enum OperationType
    {
        MachineryOperation,
        LaborOperation,
        Mixed
    }
    public enum ControllerFunctionType
    {
        Read,
        Create,
        Update,
        Delete
    }
    public enum ProductionOrderType
    {
        SeedProduction = 0,
        NaturalGumProduction = 1
    }
    public enum SeedProductionType
    {
        OwnFarmProduction,
        ContractualProduction
    }
    public enum GumProductionType
    {
        DirectProduction,
        ProductionByConcession
    }
    public enum ProcessingPlantType
    {
        SeedProcessing,
        NaturalGumProcessing
    }
    public enum ProductionDefaultLaborOperations
    {
        Trash_Disposal,
        Hand_Planting,
        Assist_Planting,
        Assist_Pesticide_Application,
        Insecticide_Application,
        DAP_Application,
        Urea_Application,
        Hoeing_1,
        Hoeing_2,
        Final_Weeding,
        Detasseling,
        Harvesting_and_Picking,
        Pickingup_after_Combine,
        Assist_Herbicide,
        Urea_Application_2,
        Rogueing,
        Sheller_Feeding,
        Assist_Combining,
        Guarding,
        Loading_and_Unloading,
        Cob_Sorting,
        Male_Removing,
        Bagging_and_Weighing,
        Uprooting,
        Combining_Uprooted_Product,
    }
    public enum ProductionDefaultMachineryOperations
    {
        Ploughing,
        Minimum_Tillage,
        Disking_1,
        Disking_2,
        Disking_3,
        Ridging,
        Planting,
        Herbicide_Application,
        Insecticide_Application,
        Input_Transport,
        Treshing,
        Combining,
        Shelling,
        Product_Transport,
        General_Transport
    }
    public enum InputMaterialApplicationPlace
    {
        Field,
        Store,
        Other
    }
    public enum ProductStatus
    {
        Production,
        RawProduct,
        Processing,
        FinishedGood
    }
}
