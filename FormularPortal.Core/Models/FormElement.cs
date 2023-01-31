namespace FormularPortal.Core.Models
{
    public abstract class FormElement : ICloneable
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

        public abstract object GetParameters();
    }
}
