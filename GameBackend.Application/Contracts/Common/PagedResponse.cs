namespace GameBackend.Application.Contracts.Common;

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Limit);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}