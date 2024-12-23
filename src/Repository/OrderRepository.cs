using Domain.Entities;

namespace Repository;

public class OrderRepository(MyAppContext context) 
    : Shared.Repository<Order>(context), IOrderRepository;