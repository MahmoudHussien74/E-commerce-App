namespace E_commerce.Application.Common;

public class PaginatedList<T>
{
    public PaginatedList(IReadOnlyList<T> items, int pageNumber, int totalCount, int pageSize)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalCount = totalCount;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int TotalCount { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
