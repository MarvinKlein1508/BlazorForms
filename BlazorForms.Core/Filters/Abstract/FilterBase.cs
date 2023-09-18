using BlazorForms.Core.Models;
using System.Text.Json;

namespace BlazorForms.Core.Filters.Abstract
{
    public abstract record FilterBase
    {
        private string _searchPhrase = string.Empty;
        public string SearchPhrase { get => _searchPhrase; set => _searchPhrase = value?.ToUpper() ?? string.Empty; }

        public abstract FilterTypes FilterType { get; }

        public virtual SavedFilter ToSavedFilter<T>(int userId, string page) where T : FilterBase, new()
        {
            var json = JsonSerializer.Serialize((T)this);

            return new SavedFilter
            {
                FilterType = FilterType,
                Page = page,
                Json = json,
                UserId = userId
            };
        }
    }
}
