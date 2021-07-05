using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; } //for the 1-1
        //relationship between user and shopping cart
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public CinemaUser Owner { get; set; }
    }
}
