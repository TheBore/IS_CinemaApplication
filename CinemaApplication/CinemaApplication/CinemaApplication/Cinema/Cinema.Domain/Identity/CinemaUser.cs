using Cinema.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Domain
{
    public class CinemaUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
    }
}
