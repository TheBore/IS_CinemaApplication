using Cinema.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<CinemaUser> GetAll();
        CinemaUser Get(string id);
        void Insert(CinemaUser entity);
        void Update(CinemaUser entity);
        void Delete(CinemaUser entity);
    }
}
