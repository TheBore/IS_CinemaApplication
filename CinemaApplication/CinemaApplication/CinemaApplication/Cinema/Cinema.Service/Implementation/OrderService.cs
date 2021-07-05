using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Cinema.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Service.Implementation
{
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;

        }
        public List<Order> getAllOrders(CinemaUser user)
        {
            return this._orderRepository.GetAllOrders();
        }

        public Order getOrderDetails(Guid? id)
        {
            return this._orderRepository.GetOrderDetails(id);
        }
    }
}
