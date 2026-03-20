namespace E_commerce.Core.Common;

public class PaginatedList<T>
{
    public List<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int TotalCount { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPage { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPage;

    public PaginatedList(List<T> items, int pageNumber, int count, int pageSize)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalCount = count;
        PageSize = pageSize;
        TotalPage = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default!)
    {
        var count = source.Count();

        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PaginatedList<T>(items, pageNumber, count, pageSize));
    }
}
