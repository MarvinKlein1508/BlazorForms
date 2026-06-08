namespace BlazorForms.Core.Filters.Abstract
{
    public abstract record PageFilterBase : FilterBase
    {
        private int _pageNumber = 1;
        
        public int PageNumber { get => _pageNumber; set => _pageNumber = value < 1 ? 1 : value; }
        public int Limit { get; set; } = 30;
    }
}
