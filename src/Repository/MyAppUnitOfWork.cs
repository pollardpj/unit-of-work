﻿using Domain.UnitOfWork;
using Shared.Repository;

namespace Repository;

public class MyAppUnitOfWork(
    MyAppContext context, 
    IOrderRepository orderRepository,
    IOrderEventRepository orderEventRepository) 
    : UnitOfWork(context), IMyAppUnitOfWork
{
    public IOrderRepository OrderRepository => orderRepository;
    public IOrderEventRepository OrderEventRepository => orderEventRepository;
}