namespace BlazorForms.Core.Interfaces
{
    /// <summary>
    /// Provides functionality for tabbable objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasTabs<T> where T : Enum
    {
        /// <summary>
        /// Gets or sets the ActiveTab page
        /// </summary>
        public T ActiveTab { get; set; }
    }
}
