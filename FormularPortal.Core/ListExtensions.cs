using FormPortal.Core.Interfaces;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void SetSortOrder<T>(this List<T> list) where T : IHasSortableElement
        {
            int i = 1;
            foreach (var item in list)
            {
                item.SortOrder = i++;
            }
        }
    }
}
