using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository.Interface;
using Cinema.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;

        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository,
             IUserRepository userRepository, IRepository<Order> orderRepository,
             IRepository<TicketInOrder> ticketInOrderRepository, IRepository<EmailMessage> mailRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _mailRepository = mailRepository;
        }

        public bool deleteTicketFromShoppingCart(string userId, Guid TicketId)
        {
            if (!string.IsNullOrEmpty(userId) && TicketId != null)
            {

                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(TicketId)).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            var userShoppingCart = loggedInUser.UserCart;

            var AllTickets = userShoppingCart.TicketInShoppingCarts.ToList();

            var allTicketPrice = AllTickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quanitity = z.Quantity
            }).ToList();

            double totalPrice = 0;


            foreach (var item in allTicketPrice)
            {
                totalPrice += item.Quanitity * item.TicketPrice;
            }


            ShoppingCartDto scDto = new ShoppingCartDto
            {
                Tickets = AllTickets,
                TotalPrice = totalPrice
            };
            return scDto;
        }

        public bool orderNow(string userId)
        {

            if (!string.IsNullOrEmpty(userId))
            {
                //Select * from Users Where Id LIKE userId

                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Successfully created order";
                mail.Status = false;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);
 
                List<TicketInOrder> ticketInOrders = new List<TicketInOrder>();

                var result = userShoppingCart.TicketInShoppingCarts.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    OrderedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order,
                    Quantity=z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                double totalPrice = 0;

                sb.AppendLine("Your order is completed. The order contains: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    totalPrice += item.Quantity * item.OrderedTicket.Price;

                    sb.AppendLine(i.ToString() + ". " + item.OrderedTicket.movieTitle + " with price of: " + item.OrderedTicket.Price
                        + " and quantity of: " + item.Quantity);
                }

                sb.AppendLine("Total price: " + totalPrice.ToString());


                mail.Content = sb.ToString();


                ticketInOrders.AddRange(result);

                foreach (var item in ticketInOrders)
                {
                    this._ticketInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.TicketInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);
                this._mailRepository.Insert(mail);

                return true;
            }
            return false;
        }
    }
}

