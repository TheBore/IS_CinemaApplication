using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository;
using Cinema.Repository.Interface;
using Cinema.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Service.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext context;
        public TicketService(IRepository<Ticket> ticketRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository, IUserRepository userRepository,
            ApplicationDbContext context)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            this.context = context;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
        }

        public void CreateNewTicket(Ticket t)
        {
         
            this._ticketRepository.Insert(t);
        }

        public void DeleteTicket(Guid id)
        {
            var ticket = this.GetDetailsForTicket(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.GetAll().ToList();
        }
        public List<Ticket> FilterTickets(DateTime date)
        {
            DbSet<Ticket> entities= context.Set<Ticket>();
            var tickets = entities.Where(x => x.Date == date).ToList();
            return tickets;
        }
        public Ticket GetDetailsForTicket(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public void UpdateExistingTicket(Ticket t)
        {
            this._ticketRepository.Update(t);
        }
        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedTicket = ticket,
                TicketId = ticket.Id,
                Quantity = 1
                //default value koga ke se dodava vo shopping cart           
            };
            return model;
        }
        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {

            var user = this._userRepository.Get(userID);

            var userShoppingCard = user.UserCart;

            if (item.TicketId != null && userShoppingCard != null)
            {
                var ticket = this.GetDetailsForTicket(item.TicketId);

                if (ticket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        ShoppingCart = userShoppingCard,
                        ShoppingCartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };

                    this._ticketInShoppingCartRepository.Insert(itemToAdd);
                    
                    return true;
                }
                return false;
            }
            return false;
        }


    }
}
