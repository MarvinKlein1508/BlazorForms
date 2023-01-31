namespace DatabaseControllerProvider
{
    /// <summary>
    /// This property is being used for extended mapping. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CompareFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets the corresponding field name of the database column.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Create a new CompareFieldAttribut
        /// </summary>
        /// <param name="fieldName">The name of the field as it is read from the database.</param>
        public CompareFieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
