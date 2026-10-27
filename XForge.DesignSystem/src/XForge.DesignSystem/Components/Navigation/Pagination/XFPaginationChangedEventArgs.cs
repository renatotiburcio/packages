namespace XForge.DesignSystem.Components.Navigation.Pagination
{
    public sealed class XFPaginationChangedEventArgs
    {
        public XFPaginationChangedEventArgs(int page, int pageSize, int totalItems, int totalPages)
        {
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int Page { get; }

        public int PageIndex => Page - 1;

        public int PageSize { get; }

        public int TotalItems { get; }

        public int TotalPages { get; }

        public int Skip => PageIndex * PageSize;

        public int Top => PageSize;
    }
}
