
using OrderAndPaymentAPI.Entities;

public interface IUserService
{
    Task<User> GetByEmailAsync(string email);
}

public class UserService: IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _userRepo.GetByEmailAsync(email);
    }
}