using Domain.Orders.Enums;

namespace Domain.Orders.Filters;

public sealed record OrderFilter(
    long[] Ids,
    OrderState? State,
    string? CreatedBy,
    long Cursor,
    int PageSize)
{
    public static OrderFilter ByIds(IEnumerable<long> ids)
    {
        long[] idsArray = ids as long[] ?? ids.ToArray();

        return new OrderFilter(
            Ids: idsArray,
            State: null,
            CreatedBy: null,
            Cursor: 0,
            PageSize: idsArray.Length);
    }
}
