using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class OrderRepository: IOrderRepository
    {
        private readonly ApplicationDbContext context;

        private DbSet<Order> entities; 

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> GetAllOrders()
        {
            // return entities.ToList();
            return entities
                .Include(z => z.TicketInOrders)
                .Include(z => z.User)
                .Include("TicketInOrders.OrderedTicket")
                .ToListAsync().Result;
        }

        public Order GetOrderDetails(Guid? id)
        {
            return entities
              .Include(z => z.TicketInOrders)
              .Include(z => z.User)
              .Include("TicketInOrders.OrderedTicket")
              .SingleOrDefaultAsync(z => z.Id == id)
              .Result;
        }
    }
}

