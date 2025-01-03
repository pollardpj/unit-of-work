namespace Domain.Commands
{
    public class CreateOrder
    {
        public required Guid Reference { get; init; }
        public required string ProductName { get; init; }

        // Output:

        public decimal? Price { get; set; }
    }
}
