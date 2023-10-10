namespace ExceedERP.Core.Domain.Finance
{
    public enum GlCostCenterTypes
    {
        NotApplicable,
        HeadOffice,
        Project,
    }
    public enum ErrorType
    {
        Wrong_Account,
        Wrong_Period,
        Unsplitted_Journal,
        Unsplitted_Ledger,
        Duplicate_Ledger,
        Duplicate_Ledger_Line,
        Duplicate_Beginning_Ledger_Line,
        Duplicate_Ending_Ledger_Line,
        Unposted_Journal_Line,
        Having_both_debit_credit,
        // Journal_Unbalanced_debit_credit

    }
}
