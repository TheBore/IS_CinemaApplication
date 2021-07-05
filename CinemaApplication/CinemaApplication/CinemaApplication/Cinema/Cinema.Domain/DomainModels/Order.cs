using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public CinemaUser User { get; set; }

        public IEnumerable<TicketInOrder> TicketInOrders { get; set; }
    }
}
