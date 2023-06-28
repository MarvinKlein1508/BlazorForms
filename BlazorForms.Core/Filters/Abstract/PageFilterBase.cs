namespace BlazorForms.Core.Filters.Abstract
{
    public abstract class PageFilterBase : FilterBase
    {
        private int _pageNumber = 1;
        private int _limit = 30;

        public int PageNumber { get => _pageNumber; set => _pageNumber = value < 1 ? 1 : value; }
        public int Limit { get => _limit; set => _limit = value < 1 ? 1 : value; }
    }
}
