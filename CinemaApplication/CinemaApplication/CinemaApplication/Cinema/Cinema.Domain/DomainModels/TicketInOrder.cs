using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class TicketInOrder : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Ticket OrderedTicket { get; set; }
        public Guid OrderId { get; set; }
        public Order UserOrder { get; set; }
        public int Quantity { get; set; }
    }
}
