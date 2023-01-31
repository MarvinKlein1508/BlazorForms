namespace FormularPortal.Core.Models
{
    /// <summary>
    /// Represents a column for a <see cref="FormRow"/>
    /// </summary>
    public class FormColumn : IDbModel
    {
        [CompareField("column_id")]
        public int ColumnId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
        /// <summary>
        /// Gets or sets the elements for this column
        /// </summary>
        public List<FormElement> Elements { get; set; } = new();

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "COLUMN_ID", ColumnId },
                { "FORM_ID", FormId },
                { "ROW_ID", RowId },
                { "IS_ACTIVE", IsActive},
                { "SORT_ORDER", SortOrder }
            };
        }
    }
}
