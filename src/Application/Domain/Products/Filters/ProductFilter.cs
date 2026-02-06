namespace Domain.Products.Filters;

public sealed record ProductFilter(
        long[]? Ids,
        decimal? MinPrice,
        decimal? MaxPrice,
        string? SubStringName,
        long Cursor,
        int PageSize);
