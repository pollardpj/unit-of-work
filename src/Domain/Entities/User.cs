namespace Domain.Entities;

public class User
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Order> Orders { get; } = new List<Order>();
}