namespace Gacfox.Wealthome.Models.Base;

public class Pagination<T>
{
    public List<T> List { get; set; } = new();
    public int Total { get; set; } = 0;
    public int? TotalPage { get; set; }
    public int Current { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public static Pagination<T> FromList(List<T> list, int currentPage, int pageSize)
    {
        int totalRows = list.Count;
        int pageStart = currentPage == 1 ? 0 : (currentPage - 1) * pageSize;
        int pageEnd = Math.Min(totalRows, currentPage * pageSize);
        List<T> listPage = totalRows > pageStart ? list.GetRange(pageStart, pageEnd - pageStart) : new List<T>();
        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

        return new Pagination<T>
        {
            List = listPage,
            Total = totalRows,
            TotalPage = totalPages,
            Current = currentPage,
            PageSize = pageSize
        };
    }
}