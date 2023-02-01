using DatabaseControllerProvider;

namespace FormularPortal.Core.Models
{
    public abstract class FormElement : ICloneable, IDbModel
    {
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("column_id")]
        public int ColumnId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("is_active")]
        public bool IsActive { get; set; }
        [CompareField("is_required")]
        public bool IsRequired { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public object Clone()
        {
            object tmp = this.MemberwiseClone();

            return tmp;
        }

        public override string ToString() => Name;
        public abstract ElementType GetElementType();
        public virtual Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ELEMENT_ID", ElementId },
                { "FORM_ID", FormId },
                { "ROW_ID", RowId },
                { "COLUMN_ID", ColumnId },
                { "NAME", Name },
                { "IS_ACTIVE", IsActive },
                { "IS_REQUIRED", IsRequired },
                { "SORT_ORDER", SortOrder },
                { "TYPE", GetElementType() },
            };
        }

    }
    public enum ElementType
    {
        Checkbox = 0,
        Date = 1,
        File = 2,
        Label = 3,
        Number = 4,
        Radio = 5,
        Select = 6,
        Table = 7,
        Text = 8,
        Textarea = 9
    }
}
