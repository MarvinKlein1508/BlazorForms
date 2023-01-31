namespace FormularPortal.Core.Filters
{
    public abstract class FilterBase
    {
        private string _searchPhrase = string.Empty;
        public string SearchPhrase { get => _searchPhrase; set => _searchPhrase = value?.ToUpper() ?? string.Empty; }
    }
}
