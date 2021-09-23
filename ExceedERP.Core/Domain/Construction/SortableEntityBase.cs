namespace ExceedERP.Core.Domain.Construction
{
    public class SortableEntityBase : UndoableUserOperationBase
    {
        public int OrderIndex { get; set; }
    }
}
