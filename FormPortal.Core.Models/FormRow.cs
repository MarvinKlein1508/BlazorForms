﻿using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    /// <summary>
    /// Represents a row within the Form.
    /// </summary>
    public class FormRow : IDbModel
    {
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int Id => RowId;
        /// <summary>
        /// Gets or sets all columns for this row.
        /// </summary>
        public List<FormColumn> Columns { get; set; } = new();

        public Form? Parent { get; set; }
        /// <summary>
        /// Creates an empty row.
        /// </summary>
        public FormRow()
        {

        }
        /// <summary>
        /// Creates a new row with a specified amount of columns.
        /// </summary>
        /// <param name="columns"></param>
        public FormRow(int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                Columns.Add(new FormColumn());
            }
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ROW_ID", RowId },
                { "FORM_ID", FormId },
                { "IS_ACTIVE", IsActive },
                { "SORT_ORDER", SortOrder }
            };
        }

        public void SetColumnSortOrder()
        {
            int columnCount = 1;
            foreach (var column in Columns)
            {
                column.SortOrder = columnCount++;
            }
        }

        public IEnumerable<FormElement> GetElements()
        {
            foreach (var column in Columns)
            {
                foreach (var element in column.Elements)
                {
                    yield return element;
                }
            }
        }

        public string GetColumnClass()
        {
            if (Columns.Count is 1)
            {
                return "col-12";
            }

            if (Columns.Count is 2)
            {
                return "col-md-6";
            }

            if (Columns.Count is 3)
            {
                return "col-md-4";
            }

            if (Columns.Count is 4)
            {
                return "col-md-3";
            }

            return "col";
        }
    }
}