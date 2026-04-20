namespace ExceedERP.Core.Domain.FixedAsset.Setting
{

    public enum FixedAssetSettingCategories
    {
        FIXED_ASSET_CONSIDERATION
    }
    public enum MinimumAssetValue
    {
        Value
    }

    public enum BooleanTypeAttributes
    {
        TRUE,
        FALSE
    }

    public enum AssetTransferDelivererType
    {
        None,
        Internal,
        External
    }

    public enum AssetTransferRecipientType
    {
        None,
        Employee,
        ExternalSupplier
    }
    public enum TransferType
    {
        None,
        PersonToPerson,
        StoreToStore
    }
    public enum TagAbbreviationType
    {
        Company,
        BranchOrSector,
        Location,
        Category,
        SubCategory,
        NonFixedAsset
    }
   
    public enum TransferBy
    {
        Loaded,
        SelfDrive
    }
    //public enum Conditions
    //{
    //    New,
    //    Old,
    //    Maintainable,
    //    NotMaintainable
    //}

    public enum PPEAmountType
    {
        Amount,
        Percentage
    }

    //public enum DepreciationMethod
    //{
    //    StraightLine,
    //    Pooling,

    //}

    public enum DepreciationMethod
    {
        Diminishing,
        StraightLine

    }

    public enum FixedAssetClass
    {
        Tangible,
        Intangible,
        Financial

    }
}
