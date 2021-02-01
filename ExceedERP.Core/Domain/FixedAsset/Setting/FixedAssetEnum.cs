namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public enum AssetTransferDelivererType
    {
        Internal, External
    }

    public enum AssetTransferRecipientType
    {
        Employee, ExternalSupplier
    }
    public enum TransferType
    {
        PersonToPerson,
        StoreToStore
    }
    public enum TagAbbreviationType
    {
        Company,
        BranchOrSector,
        Location,
        Category,
        SubCategory
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

    public enum DepreciationMethod
    {
        StraightLine,
        Pooling,

    }

    public enum FixedAssetClass
    {
        Tangible,
        Intangible,
        Financial

    }
}
