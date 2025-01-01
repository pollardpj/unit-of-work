namespace Domain.Commands
{
    public class CreateOrder
    {
        public Guid Reference { get; set; }
        public string ProductName { get; set; }

        // Output:

        public decimal Price { get; set; }
    }
}
