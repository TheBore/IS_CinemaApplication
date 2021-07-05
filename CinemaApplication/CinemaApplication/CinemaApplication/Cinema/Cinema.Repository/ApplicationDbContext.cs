
using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinema.Repository
{
    public class ApplicationDbContext : IdentityDbContext<CinemaUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual DbSet<TicketInOrder> TicketInOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //generating ids when creating new item in the database
            builder.Entity<Ticket>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInShoppingCarts)
                .HasForeignKey(z => z.TicketId);


            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.ShoppingCart)
                .WithMany(z => z.TicketInShoppingCarts)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<ShoppingCart>()
                .HasOne<CinemaUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.OrderedTicket)
                .WithMany(z => z.TicketInOrders)
                .HasForeignKey(z => z.TicketId);


            builder.Entity<TicketInOrder>()
                    .HasOne(z => z.UserOrder)
                    .WithMany(z => z.TicketInOrders)
                    .HasForeignKey(z => z.OrderId);

        }
    }
}
