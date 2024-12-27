namespace Domain.DTOs;

public class UserDto
{
    public Guid Reference { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IEnumerable<OrderDto> Orders { get; set; }
}