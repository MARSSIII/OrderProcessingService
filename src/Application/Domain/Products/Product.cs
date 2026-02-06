namespace Domain.Products;

public record Product(
    long Id,
    string Name,
    decimal Price)
{
    public static Product Create(string name, decimal price, long id = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid product name", nameof(name));

        if (price < 0)
            throw new ArgumentException($"Invalid product price for '{name}', it should be >= 0", nameof(price));

        return new Product(id, name, price);
    }
}
