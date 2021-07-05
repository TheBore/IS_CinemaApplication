using Cinema.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class Ticket:BaseEntity
    {
        public DateTime Date { get; set; }
        public Genre MovieGenre { get; set; }
        public string movieTitle { get; set; }
        public double Price { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public IEnumerable<TicketInOrder> TicketInOrders { get; set; }
    }
}
