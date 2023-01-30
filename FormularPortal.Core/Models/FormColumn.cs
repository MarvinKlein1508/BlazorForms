namespace FormularPortal.Core.Models
{
    /// <summary>
    /// Represents a column for a <see cref="FormRow"/>
    /// </summary>
    public class FormColumn
    {
        /// <summary>
        /// Gets or sets the elements for this column
        /// </summary>
        public List<FormElement> Elements { get; set; } = new();
    }
}
