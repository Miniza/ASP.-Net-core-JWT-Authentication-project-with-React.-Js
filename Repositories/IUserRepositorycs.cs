using JWTAuthProject.DataModels;

namespace JWTAuthProject.Repositories
{
    public interface IUserRepository
    {
        User Create(User user);
        User GetByEmail(string email);
        User GetById(int Id);
    }
}
