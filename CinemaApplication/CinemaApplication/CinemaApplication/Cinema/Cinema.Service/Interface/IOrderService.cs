using Cinema.Domain;
using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Service.Interface
{
    public interface IOrderService
    {
        List<Order> getAllOrders(CinemaUser user);
        Order getOrderDetails(Guid? id);
    }
}
