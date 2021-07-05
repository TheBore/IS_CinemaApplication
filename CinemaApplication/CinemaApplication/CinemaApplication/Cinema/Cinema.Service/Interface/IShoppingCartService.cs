using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDto getShoppingCartInfo(string userId);
        bool deleteTicketFromShoppingCart(string userId, Guid ticketId);
        bool orderNow(string userId);
    }

}
