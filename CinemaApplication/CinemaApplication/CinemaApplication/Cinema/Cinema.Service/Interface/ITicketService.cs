using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Service.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        void CreateNewTicket(Ticket t);
        Ticket GetDetailsForTicket(Guid? id);
        void UpdateExistingTicket(Ticket t);
        void DeleteTicket(Guid id);
        AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        bool AddToShoppingCart(AddToShoppingCartDto item, string userID);
        List<Ticket> FilterTickets(DateTime date);


    }
}

