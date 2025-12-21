namespace Validations.Benchmarking.Models;

public class UserModelDeep : UserModel
{
    public string? Email { get; set; }

    public OrderModel[] Orders { get; set; } = [];
}

public class OrderModel
{
    public string? OrderNumber { get; set; }
    public OrderItemModel[] Items { get; set; } = [];
}

public class OrderItemModel
{
    public ProductModel? Product { get; set; }
    public int Quantity { get; set; }
}

public class ProductModel
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
}
