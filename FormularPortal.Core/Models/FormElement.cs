using DatabaseControllerProvider;
using FluentValidation;
using System.Configuration;

namespace FormularPortal.Core.Models
{
    public abstract class FormElement : IDbModel
    {
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("guid")]
        public Guid Guid { get; set; }
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
        public int Id => ElementId;

        public override string ToString() => Name;
        public abstract ElementType GetElementType();
        public virtual Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ELEMENT_ID", ElementId },
                { "FORM_ID", FormId },
                { "ROW_ID", RowId },
                { "GUID", Guid.ToString() },
                { "COLUMN_ID", ColumnId },
                { "NAME", Name },
                { "IS_ACTIVE", IsActive },
                { "IS_REQUIRED", IsRequired },
                { "SORT_ORDER", SortOrder },
                { "TYPE", GetElementType() },
            };
        }

        public FormElement()
        {
            GenerateGuid();
        }

        public void GenerateGuid()
        {
            Guid = Guid.NewGuid();
        }
        public abstract string GetDefaultName();
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
