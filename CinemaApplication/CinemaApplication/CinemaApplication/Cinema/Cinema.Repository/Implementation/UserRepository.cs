using Cinema.Domain;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext context;

        private DbSet<CinemaUser> entities; 

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CinemaUser>();
        }
        public IEnumerable<CinemaUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public CinemaUser Get(string id)
        {
            //to load the relations - because of the virtual properties
            return entities
                .Include(z => z.UserCart)
                .Include("UserCart.TicketInShoppingCarts")
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(CinemaUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(CinemaUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(CinemaUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
